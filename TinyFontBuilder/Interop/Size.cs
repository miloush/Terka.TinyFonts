namespace Win32
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("Width = {Width}, Height = {Height}")]
    internal struct Size
    {
        public int Width;
        public int Height;

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}