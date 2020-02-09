namespace Win32
{
    using System;

    partial class Gdi32
    {
        [Flags]
        public enum FontLicensing : uint
        {
            NotLicensed = 0,
            NoEmbedding = 1,
            ReadOnlyEmbedding = 2
        }
    }
}