namespace Win32
{
    using System;

    partial class Gdi32
    {
        [Flags]
        public enum FontPitchAndFamily : byte
        {
            PitchDefault = 0,
            PitchFixed = 1,
            PitchVariable = 2,
            FamilyDontCare = (0 << 4),
            FamilyRoman = (1 << 4),
            FamilySwiss = (2 << 4),
            FamilyModern = (3 << 4),
            FamilyScript = (4 << 4),
            FamilyDecorative = (5 << 4),
        }
    }
}