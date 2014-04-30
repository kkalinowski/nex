using System;

namespace nex.FileSystem.Windows
{
    internal static class ShellHelper
    {
        #region Low/High Word
        /// <summary>
        /// Retrieves the High Word of a WParam of a WindowMessage
        /// </summary>
        /// <param name="ptr">The pointer to the WParam</param>
        /// <returns>The unsigned integer for the High Word</returns>
        public static uint HiWord32(IntPtr ptr)
        {
            if (((uint)ptr & 0x80000000) == 0x80000000)
                return ((uint)ptr >> 16);
            else
                return ((uint)ptr >> 16) & 0xffff;
        }

        public static ulong HiWord64(IntPtr ptr)
        {
            if (((ulong)ptr & 0x80000000) == 0x80000000)
                return ((ulong)ptr >> 16);
            else
                return ((ulong)ptr >> 16) & 0xffff;
        }

        /// <summary>
        /// Retrieves the Low Word of a WParam of a WindowMessage
        /// </summary>
        /// <param name="ptr">The pointer to the WParam</param>
        /// <returns>The unsigned integer for the Low Word</returns>
        public static uint LoWord32(IntPtr ptr)
        {
            return (uint)ptr & 0xffff;
        }

        public static ulong LoWord64(IntPtr ptr)
        {
            return (ulong)ptr & 0xffff;
        }
        #endregion
    }
}