namespace Terka.FontBuilder.Parser.Output
{    
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Corresponds to OT "Coverage format 1".
    /// </summary>
    public class ListCoverageTable : ICoverageTable
    {
        /// <summary>
        /// Gets or sets the covered glyph ID list.
        /// </summary>
        /// <value>
        /// The covered glyph ID list.
        /// </value>
        public IEnumerable<ushort> CoveredGlyphIdList { get; set; }
       
        /// <inheritdoc/>
        public IDictionary<ushort, ushort> CoveredGlyphIds
        {
            get
            {
                return this.CoveredGlyphIdList.Select((p, i) => new { Index = (ushort)i, GlyphId = p }).ToDictionary(p => p.Index, p => p.GlyphId);
            }
        }
    }
}
