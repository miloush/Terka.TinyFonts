namespace Win32
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class Gdi32
    {
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, uint nWidth, uint nHeight);

        [DllImport("gdi32.dll")]
        public static extern IntPtr GetCurrentObject(IntPtr hdc, ObjectType objectType);
    }
}