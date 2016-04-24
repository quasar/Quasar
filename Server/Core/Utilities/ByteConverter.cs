using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xServer.Core.Utilities
{
    public class ByteConverter
    {
        private static byte NULL_BYTE = byte.MinValue;

        public static byte[] GetBytes(int value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(long value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(uint value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(ulong value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(string value)
        {
            return StringToBytes(value);
        }

        public static byte[] GetBytes(string[] value)
        {
            return StringArrayToBytes(value);
        }

        public static int ToInt32(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }

        public static long ToInt64(byte[] bytes)
        {
            return BitConverter.ToInt64(bytes, 0);
        }

        public static uint ToUInt32(byte[] bytes)
        {
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static ulong ToUInt64(byte[] bytes)
        {
            return BitConverter.ToUInt64(bytes, 0);
        }

        public static string ToString(byte[] bytes)
        {
            return BytesToString(bytes);
        }

        public static string[] ToStringArray(byte[] bytes)
        {
            return BytesToStringArray(bytes);
        }

        private static byte[] GetNullBytes()
        {
            //Null bytes: 00 00
            return new byte[] { NULL_BYTE, NULL_BYTE };
        }

        private static byte[] StringToBytes(string value)
        {
            byte[] bytes = new byte[value.Length * sizeof(char)];
            Buffer.BlockCopy(value.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static byte[] StringArrayToBytes(string[] strings)
        {
            List<byte> bytes = new List<byte>();

            foreach(string str in strings)
            {
                bytes.AddRange(StringToBytes(str));
                bytes.AddRange(GetNullBytes());
            }

            return bytes.ToArray();
        }

        private static string BytesToString(byte[] bytes)
        {
            int nrChars = (int)Math.Ceiling((float)bytes.Length / (float)sizeof(char));
            char[] chars = new char[nrChars];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        private static string[] BytesToStringArray(byte[] bytes)
        {
            List<string> strings = new List<string>();

            int i = 0;
            StringBuilder strBuilder = new StringBuilder(bytes.Length);
            while (i < bytes.Length)
            {
                //Holds the number of nulls (3 nulls indicated end of a string)
                int nullcount = 0;
                while (i < bytes.Length && nullcount < 3)
                {
                    if (bytes[i] == NULL_BYTE)
                    {
                        nullcount++;
                    }
                    else
                    {
                        strBuilder.Append(Convert.ToChar(bytes[i]));
                        nullcount = 0;
                    }
                    i++;
                }
                strings.Add(strBuilder.ToString());
                strBuilder.Clear();
            }

            return strings.ToArray();
        }
    }
}
