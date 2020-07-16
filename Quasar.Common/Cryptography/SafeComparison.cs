using System.Runtime.CompilerServices;

namespace Quasar.Common.Cryptography
{
    public class SafeComparison
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
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
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
    }
}
