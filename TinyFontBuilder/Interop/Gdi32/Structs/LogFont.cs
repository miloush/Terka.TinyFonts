namespace Win32
{
    using System;
    using System.Runtime.InteropServices;

    partial class Gdi32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public struct LogFont
        {
            public int Height;
            public int Width;
            public int Escapement;
            public int Orientation;
            public FontWeight Weight;
            [MarshalAs(UnmanagedType.U1)]
            public bool Italic;
            [MarshalAs(UnmanagedType.U1)]
            public bool Underline;
            [MarshalAs(UnmanagedType.U1)]
            public bool StrikeOut;
            public FontCharSet CharSet;
            public FontPrecision OutPrecision;
            public FontClipPrecision ClipPrecision;
            public FontQuality Quality;
            public FontPitchAndFamily PitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string FaceName;

            public LogFont(FontCharSet charSet) : this(string.Empty, charSet) { }
            public LogFont(string faceName) : this(faceName, FontCharSet.Default) { }
            public LogFont(string faceName, FontCharSet charSet) : this()
            {
                CharSet = charSet;
                FaceName = faceName;
                PitchAndFamily = FontPitchAndFamily.PitchDefault;
            }
        }
    }
}