namespace Terka.FontBuilder.Parser.Output.Substitution
{
    /// <summary>
    /// Corresponds to OT "Single substitution format 1".
    /// </summary>
    public class DeltaSubstitutionTable : CoveredGlyphTransformationTableBase
    {
        /// <summary>
        /// Gets or sets the glyph ID delta by which each covered glyph ID is modified.
        /// </summary>
        /// <value>
        /// The glyph ID delta.
        /// </value>
        public virtual short GlyphIdDelta
        {
            get;
            set;
        }
    }
}