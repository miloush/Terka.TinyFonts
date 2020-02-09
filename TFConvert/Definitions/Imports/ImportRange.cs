namespace Terka.TinyFonts.TFConvert
{
    // [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Fields do not corrupt state and distingush computed values.")]

    /// <summary>
    /// Specifies a range of Unicode characters to import.
    /// </summary>
    /// <remarks>
    /// A <see cref="SelectFont" /> command that specifies the font from which to import glyphs 
    /// must precede the ImportRange command in definition, otherwise TFConvert will fail.
    /// If there are multiple <see cref="SelectFont" /> commands,
    /// the one that more closely precedes the ImportRange statement will be used.
    /// The ImportRange command may be applied multiple times per definition,
    /// to select non-contiguous ranges of characters.
    /// </remarks>
    [TinyCommand]
    [RequiresCommand(typeof(SelectFont), Before = true)]
    public class ImportRange : TinyCommandBase
    {
        /// <summary>
        /// An integer that indicates the beginning of the range of Unicode characters to import.
        /// </summary>
        [TinyParameter(0)]
        public int Start;

        /// <summary>
        /// An integer that indicates the end of the range of Unicode characters to import.
        /// </summary>
        [TinyParameter(1)]
        public int End;

        /// <summary>
        /// Initializes a new instance of <see cref="ImportRange"/> command.
        /// </summary>
        public ImportRange()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ImportRange"/> command with specified start and end bounds.
        /// </summary>
        /// <param name="start">An integer that indicates the beginning of the range of Unicode characters to import.</param>
        /// <param name="end">An integer that indicates the end of the range of Unicode characters to import.</param>
        public ImportRange(int start, int end)
        {
            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ImportRange"/> command that imports single character.
        /// </summary>
        /// <param name="charCode">The Unicode code point for the character to import.</param>
        public ImportRange(int charCode)
        {
            this.Start = this.End = charCode;
        }

        /// <summary>
        /// Creates a new instance of ImportRange command which imports single character.
        /// </summary>
        /// <param name="character">The character to import.</param>
        public ImportRange(char character) : this((int)character) 
        {
        }

        /// <summary>
        /// Creates a new instance of ImportRange command which imports continous range of characters.
        /// </summary>
        /// <param name="character">The character to start import at.</param>
        /// <param name="count">The number of successive characters to import starting with the one specified in character parameter.</param>
        public ImportRange(char character, int count) : this((int)character)
        {
            this.End += count - 1;
        }

        /// <summary>
        /// Converts an relative character range import to an absolute one.
        /// </summary>
        /// <param name="command">The command with relative definitions.</param>
        /// <returns>A command with the same absolute definitions.</returns>
        public static explicit operator ImportRange(ImportRangeAndMap command)
        {
            return new ImportRange(command.AbsoluteStart, command.AbsoluteEnd);
        }
    }
}
