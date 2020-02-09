namespace Win32
{
    using System;

    partial class Gdi32
    {
        [Flags]
        public enum FontClipPrecision : byte
        {
            Default = 0,
            Character = 1,
            Stroke = 2,
            Mask = 0xF,
            LefHandedAngles = (1 << 4),
            TrueTypeAlways = (2 << 4),
            DisableFontAssociation = (4 << 4),
            Embedded = (8 << 4),
        }
    }
}