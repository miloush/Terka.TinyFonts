namespace Win32
{
    using System;

    partial class Gdi32
    {
        public enum FontPrecision : byte
        {
            Default = 0,
            String = 1,
            Character = 2,
            Stroke = 3,
            TrueTypePrefer = 4,
            DevicePrefer = 5,
            RasterPrefer = 6,
            TrueTypeOnly = 7,
            Outline = 8,
            ScreenOutline = 9,
            PostScriptOnly = 10,
        }
    }
}