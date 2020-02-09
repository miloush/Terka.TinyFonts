namespace Terka.TinyFonts
{
    using System;

    /// <summary>
    /// Flags describing the font.
    /// </summary>
    [Flags]
    public enum FontDescriptionFlags : ushort
    {
        /// <summary>
        /// No special properties of font file.
        /// </summary>
        None,
        /// <summary>
        /// The font is bold.
        /// </summary>
        Bold = 1,
        /// <summary>
        /// The font is italic.
        /// </summary>
        Italic = 2,
        /// <summary>
        /// The font is underlined.
        /// </summary>
        Underlined = 4,
        /// <summary>
        /// The Tiny Font contains the extended structure.
        /// </summary>
        Extended = 8,
    }
}
