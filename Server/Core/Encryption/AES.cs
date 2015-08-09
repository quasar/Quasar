using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace xServer.Core.Encryption
{
    public static class AES
    {
        private const int IVLENGTH = 16;
        private static byte[] _key;

        public static void PreHashKey(string key)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                _key = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
            }
        }

        public static string Encrypt(string input)
        {
            if (_key == null) throw new Exception("The key can not be null.");

            byte[] data = Encoding.UTF8.GetBytes(input), encdata;

            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var rd = new RijndaelManaged())
                    {
                        rd.Key = _key;
                        rd.GenerateIV();
                        byte[] iv = rd.IV;

                        using (var cs = new CryptoStream(ms, rd.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            ms.Write(iv, 0, iv.Length); // write first 16 bytes IV, followed by encrypted message
                            cs.Write(data, 0, data.Length);
                            cs.FlushFinalBlock();
                        }

                        iv = null;
                    }

                    encdata = ms.ToArray();
                }

                return Convert.ToBase64String(encdata);
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                encdata = null;
                data = null;
            }
        }

        public static string Encrypt(string input, string keyy)
        {
            byte[] key, data = Encoding.UTF8.GetBytes(input), encdata;

            try
            {
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    key = md5.ComputeHash(Encoding.UTF8.GetBytes(keyy));
                }

                using (var ms = new MemoryStream())
                {
                    using (var rd = new RijndaelManaged())
                    {
                        rd.Key = key;
                        rd.GenerateIV();
                        byte[] iv = rd.IV;

                        using (var cs = new CryptoStream(ms, rd.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            ms.Write(iv, 0, iv.Length); // write first 16 bytes IV, followed by encrypted message
                            cs.Write(data, 0, data.Length);
                            cs.FlushFinalBlock();
                        }

                        iv = null;
                    }

                    encdata = ms.ToArray();
                }

                return Convert.ToBase64String(encdata);
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                encdata = null;
                data = null;
            }
        }

        public static byte[] Encrypt(byte[] input)
        {
            if (_key == null) throw new Exception("The key can not be null.");

            byte[] data = input, encdata;

            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var rd = new RijndaelManaged())
                    {
                        rd.Key = _key;
                        rd.GenerateIV();
                        byte[] iv = rd.IV;

                        using (var cs = new CryptoStream(ms, rd.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            ms.Write(iv, 0, iv.Length); // write first 16 bytes IV, followed by encrypted message
                            cs.Write(data, 0, data.Length);
                            cs.FlushFinalBlock();
                        }

                        iv = null;
                    }

                    encdata = ms.ToArray();
                }

                return encdata;
            }
            catch
            {
                return new byte[0];
            }
            finally
            {
                encdata = null;
                data = null;
            }
        }

        public static string Decrypt(string input)
        {
            if (_key == null) throw new Exception("The key can not be null.");

            byte[] data;
            int i;

            try
            {
                using (var ms = new MemoryStream(Convert.FromBase64String(input)))
                {
                    using (var rd = new RijndaelManaged())
                    {
                        byte[] iv = new byte[IVLENGTH];
                        ms.Read(iv, 0, IVLENGTH); // read first 16 bytes for IV, followed by encrypted message
                        rd.IV = iv;
                        rd.Key = _key;

                        using (var cs = new CryptoStream(ms, rd.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            data = new byte[ms.Length - IVLENGTH + 1];
                            i = cs.Read(data, 0, data.Length);
                        }

                        iv = null;
                    }
                }

                return Encoding.UTF8.GetString(data, 0, i);
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                data = null;
            }
        }

        public static byte[] Decrypt(byte[] input)
        {
            if (_key == null) throw new Exception("The key can not be null.");

            byte[] temp, data;

            try
            {
                using (var ms = new MemoryStream(input))
                {
                    using (var rd = new RijndaelManaged())
                    {
                        byte[] iv = new byte[IVLENGTH];
                        ms.Read(iv, 0, IVLENGTH); // read first 16 bytes for IV, followed by encrypted message
                        rd.IV = iv;
                        rd.Key = _key;

                        using (var cs = new CryptoStream(ms, rd.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            temp = new byte[ms.Length - IVLENGTH + 1];
                            int read = cs.Read(temp, 0, temp.Length);
                            data = new byte[read];
                            Buffer.BlockCopy(temp, 0, data, 0, read);
                        }

                        iv = null;
                    }
                }

                return data;
            }
            catch
            {
                return new byte[0];
            }
            finally
            {
                temp = null;
                data = null;
            }
        }
    }
}