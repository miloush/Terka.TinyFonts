namespace Win32
{
    using System;

    partial class Gdi32
    {
        public enum FontQuality : byte
        {
            Default = 0,
            Draft = 1,
            Proof = 2,
            Nonantialiased = 3,
            Antialiased = 4,
            ClearType = 5,
            ClearTypeNatural = 6,
        }
    }
}