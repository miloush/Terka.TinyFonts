namespace Terka.FontBuilder.Parser.Output
{
    /// <summary>
    /// Glyph transformation table, which is only applied on glyphs in its coverage table.
    /// </summary>
    public abstract class CoveredGlyphTransformationTableBase : IGlyphTransformationTable
    {
        /// <summary>
        /// Gets or sets the coverage.
        /// </summary>
        /// <value>
        /// The coverage.
        /// </value>
        public virtual ICoverageTable Coverage
        {
            get;
            set;
        }

        /// <inheritdoc />
        public LookupFlags LookupFlags { get; set; }
    }
}