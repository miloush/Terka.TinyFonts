namespace Terka.FontBuilder.Parser.Output.Substitution
{
    using System.Collections.Generic;

    /// <summary>
    /// Corresponds to OT "Ligature table".
    /// </summary>
    public class Ligature
    {
        /// <summary>
        /// Gets or sets the component glyph IDs.
        /// </summary>
        /// <value>
        /// The component glyph IDs.
        /// </value>
        public IEnumerable<ushort> ComponentGlyphIds { get; set; }

        /// <summary>
        /// Gets or sets the ligature glyph ID.
        /// </summary>
        /// <value>
        /// The ligature glyph ID.
        /// </value>
        public ushort LigatureGlyphId { get; set; }
    }
}