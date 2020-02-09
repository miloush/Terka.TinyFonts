namespace Win32
{
    using System;

    partial class Gdi32
    {
        public enum FontCharSet : byte
        {
            ANSI = 0,
            Default = 1,
            Symbol = 2,
            ShiftJIS = 128,
            Hangeul = 129,
            Hangul = 129,
            GB2312 = 134,
            ChineseBig5 = 136,
            OEM = 255,
            Johab = 130,
            Hebrew = 177,
            Arabic = 178,
            Greek = 161,
            Turkish = 162,
            Vitenamesse = 163,
            Thai = 222,
            EasternEurope = 238,
            Russian = 204,
            MAC = 77,
            Baltic = 186,
        }
    }
}