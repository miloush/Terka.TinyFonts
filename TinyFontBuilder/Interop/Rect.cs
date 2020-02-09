namespace Win32
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("Left = {Left}, Top = {Top}, Right = {Right}, Bottom = {Bottom}")]
    internal struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public Rect(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static Rect FromXYWH(int x, int y, int width, int height)
        {
            return new Rect(x, y, x + width, y + height);
        }

        public Point Location
        {
            get { return new Point(Left, Top); }
        }

        public Size Size
        {
            get { return new Size(Bottom - Top, Right - Top); }
        }
    }
}