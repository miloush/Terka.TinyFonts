namespace Terka.FontBuilder.Parser.Output
{
    /// <summary>
    /// Common base for all glyph transformation tables.
    /// </summary>
    public interface IGlyphTransformationTable
    {
        /// <summary>
        /// Gets or sets the lookup flags.
        /// </summary>
        /// <value>
        /// The lookup flags.
        /// </value>
        LookupFlags LookupFlags { get; set; }
    }
}

