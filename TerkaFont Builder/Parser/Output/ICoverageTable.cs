namespace Terka.FontBuilder.Parser.Output
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a coverage table.
    /// </summary>
    public interface ICoverageTable 
    {
        /// <summary>
        /// Gets the covered glyph IDs.
        /// </summary>
        /// <value>
        /// The covered glyph IDs. Key is the coverage index, value is the glyph ID.
        /// </value>
        IDictionary<ushort, ushort> CoveredGlyphIds { get; }
    }
}
