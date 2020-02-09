namespace Win32
{
    using System;

    partial class Gdi32
    {
        public enum ObjectType : uint
        {
            Pen = 1,
            Brush = 2,
            DeviceContext = 3,
            MetaDeviceContext = 4,
            Palete = 5,
            Font = 6,
            Bitmap = 7,
            Region = 8,
            Metafile = 9,
            MemoryDeviceContext = 10,
            ExtendedPen = 11,
            EnhancedMetafileDeviceContext = 12,
            EnhancedMetafile = 13,
            ColorSpace = 14,
        }
    }
}