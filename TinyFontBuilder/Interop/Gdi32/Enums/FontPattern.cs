namespace Win32
{
    using System;

    partial class Gdi32
    {
        [Flags]
        public enum FontPattern : uint
        {
            Italic = 0,
            Underscore = 1 << 1,
            Negative = 1 << 2,
            Outline = 1 << 3,
            Strikeout = 1 << 4,
            Bold = 1 << 5
        }
    }
}