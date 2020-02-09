namespace Terka.FontBuilder.Parser.Output.Context
{
    using System.Collections.Generic;

    /// <summary>
    /// Base for rule-based context transformation tables.
    /// </summary>
    public abstract class ContextRuleTransformationTableBase : CoveredGlyphTransformationTableBase
    {
        /// <summary>
        /// Gets or sets the collection of transformation rules. The first level collection is ordered corresponding to the coverage table, 
        /// the second level is a collection of rules which begin with the glyph matched by the coverage table.
        /// </summary>
        /// <value>
        /// The transformation rules.
        /// </value>
        public virtual IEnumerable<IEnumerable<ContextTransformationRule>> TransformationRules
        {
            get;
            set;
        }
    }
}