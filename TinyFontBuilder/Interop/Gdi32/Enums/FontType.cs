namespace Win32
{
    using System;

    partial class Gdi32
    {
        [Flags]
        public enum FontType
        {
            Raster = 0x01,
            Device = 0x02,
            TrueType = 0x04
        }
    }
}