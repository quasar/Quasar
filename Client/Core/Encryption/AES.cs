using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Core.Encryption
{
    internal class AES
    {
        public static string Encrypt(string input, string keyy)
        {
            RijndaelManaged rd = new RijndaelManaged();

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] key = md5.ComputeHash(Encoding.UTF8.GetBytes(keyy));

            md5.Clear();
            rd.Key = key;
            rd.GenerateIV();

            byte[] iv = rd.IV;
            MemoryStream ms = new MemoryStream();

            ms.Write(iv, 0, iv.Length);

            CryptoStream cs = new CryptoStream(ms, rd.CreateEncryptor(), CryptoStreamMode.Write);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(input);

            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            byte[] encdata = ms.ToArray();

            cs.Close();
            rd.Clear();
            ms.Close();

            return Convert.ToBase64String(encdata);
        }

        public static string Decrypt(string input, string keyy)
        {
            RijndaelManaged rd = new RijndaelManaged();
            int rijndaelIvLength = 16;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] key = md5.ComputeHash(Encoding.UTF8.GetBytes(keyy));

            md5.Clear();

            byte[] encdata = Convert.FromBase64String(input);
            MemoryStream ms = new MemoryStream(encdata);
            byte[] iv = new byte[16];

            ms.Read(iv, 0, rijndaelIvLength);
            rd.IV = iv;
            rd.Key = key;

            CryptoStream cs = new CryptoStream(ms, rd.CreateDecryptor(), CryptoStreamMode.Read);

            byte[] data = new byte[ms.Length - rijndaelIvLength + 1];
            int i = cs.Read(data, 0, data.Length);

            cs.Close();
            rd.Clear();
            ms.Close();

            return System.Text.Encoding.UTF8.GetString(data, 0, i);
        }
    }
}