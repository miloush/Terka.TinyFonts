namespace Terka.FontBuilder.Parser.Output.Substitution
{
    using System.Collections.Generic;

    /// <summary>
    /// Corresponds to OT "Ligature substitution subtable".
    /// </summary>
    public class LigatureSubstitutionTable : CoveredGlyphTransformationTableBase
    {
        /// <summary>
        /// Gets or sets the ligature collection. The first level collection is indexed by coverage index of the first
        /// glyph of the ligature. Second level collection contains all ligatures starting with that glyph.
        /// </summary>
        /// <value>
        /// The ligatures.
        /// </value>
        public virtual IEnumerable<IEnumerable<Ligature>> LigatureSets
        {
            get;
            set;
        }
    }
}