namespace Terka.TinyFonts.TFConvert
{
    // [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Fields do not corrupt state and distingush computed values.")]

    /// <summary>
    /// Specifies the degree to which to anti-alias the font.
    /// </summary>
    /// <remarks>
    /// A <see cref="SelectFont" /> command that specifies the font from which to import glyphs must precede the AntiAlias statement in the definition.
    /// The AntiAlias option may be applied to a range of characters defined by a ImportRange option.
    /// </remarks>
    [TinyCommand]
    public class AntiAlias : TinyCommandBase
    {
        /// <summary>
        /// An integer, that may be either 1, 2, 4, or 8, that indicates the level of anti-aliasing to support. The font bitmaps contain n^2+1 (n squared plus one) levels of gray.
        /// </summary>
        [TinyParameter]
        public int BitsPerPixel;

        /// <summary>
        /// Creates a new instance of the <see cref="AntiAlias"/> command.
        /// </summary>
        public AntiAlias()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AntiAlias"/> command with specified anti-alising level.
        /// </summary>
        /// <param name="bitsPerPixel">An integer, that may be either 1, 2, 4, or 8, that indicates the level of anti-aliasing to support. The font bitmaps contain n^2+1 (n squared plus one) levels of gray.</param>
        public AntiAlias(int bitsPerPixel)
        {
            this.BitsPerPixel = bitsPerPixel;
        }
    }
}
