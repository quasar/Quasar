using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using xServer.Core.Cryptography;

namespace xServer.Core.Helper
{
    public static class CryptographyHelper
    {
        /// <summary>
        /// Compares two byte arrays for equality.
        /// </summary>
        /// <param name="a1">Byte array to compare</param>
        /// <param name="a2">Byte array to compare</param>
        /// <returns>True if equal, else false</returns>
        /// <remarks>
        /// Assumes that the byte arrays have the same length.
        /// This method is safe against timing attacks.
        /// </remarks>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static bool AreEqual(byte[] a1, byte[] a2)
        {
            bool result = true;
            for (int i = 0; i < a1.Length; ++i)
            {
                if (a1[i] != a2[i])
                    result = false;
            }
            return result;
        }

        public static void DeriveKeys(string password, out string key, out string authKey)
        {
            using (Rfc2898DeriveBytes derive = new Rfc2898DeriveBytes(password, AES.Salt, 50000))
            {
                key = Convert.ToBase64String(derive.GetBytes(16));
                authKey = Convert.ToBase64String(derive.GetBytes(64));
            }
        }
    }
}
