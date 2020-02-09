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
    public class ImportRangeAndMap : TinyCommandBase
    {
        /// <summary>
        /// An integer, that when added to offset, indicates the beginning of the range of Unicode characters to import.
        /// </summary>
        [TinyParameter(0)]
        public int Start;

        /// <summary>
        /// An integer, when added to offset, indicates the end of the range of Unicode characters to import.
        /// </summary>
        [TinyParameter(1)]
        public int End;

        /// <summary>
        /// An integer that indicates the offset from which start and end are calculated.
        /// </summary>
        [TinyParameter(2)]
        public int Offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportRangeAndMap"/> command.
        /// </summary>
        public ImportRangeAndMap()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportRangeAndMap"/> command.
        /// </summary>
        /// <param name="start">An integer, that when added to offset, indicates the beginning of the range of Unicode characters to import.</param>
        /// <param name="end">An integer, when added to offset, indicates the end of the range of Unicode characters to import.</param>
        /// <param name="offset">An integer that indicates the offset from which start and end are calculated.</param>
        public ImportRangeAndMap(int start, int end, int offset)
        {
            this.Start = start;
            this.End = end;
            this.Offset = offset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportRangeAndMap"/> command that imports continous range of characters.
        /// </summary>
        /// <param name="character">The character to start import at.</param>
        /// <param name="count">The number of successive characters to import starting with the one specified in character parameter.</param>
        public ImportRangeAndMap(char character, int count) : this(character, count - 1, 0)
        {
        }

        /// <summary>
        /// Gets or sets an integer that indicates the beginning of the range of Unicode characters to import.
        /// </summary>
        /// <returns>
        /// An integer that indicates the beginning of the range of Unicode characters to import.
        /// </returns>
        public int AbsoluteStart
        {
            get { return this.Start + this.Offset; }
            set { this.Start = value - this.Offset; }
        }

        /// <summary>
        /// Gets or sets an integer that indicates the end of the range of Unicode characters to import.
        /// </summary>
        /// <returns>
        /// An integer that indicates the end of the range of Unicode characters to import.
        /// </returns>
        public int AbsoluteEnd
        {
            get { return this.End + this.Offset; }
            set { this.End = value - this.Offset; }
        }

        /// <summary>
        /// Converts an absolute character range import to a relative one.
        /// </summary>
        /// <param name="command">The command with absolute definitions.</param>
        /// <returns>A command with the same relative definitions.</returns>
        public static explicit operator ImportRangeAndMap(ImportRange command)
        {
            return new ImportRangeAndMap(0, command.End - command.Start, command.Start);
        }
    }
}
