namespace Win32
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class User32
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetDC(IntPtr hWnd); 

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        public static IntPtr GetScreenDC()
        {
            return GetDC(IntPtr.Zero);
        }

        public static void ReleaseScreenDC(IntPtr screenDC)
        {
            ReleaseDC(IntPtr.Zero, screenDC);
        }
    }
}
