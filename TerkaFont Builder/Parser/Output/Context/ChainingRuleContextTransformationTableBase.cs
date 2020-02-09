namespace Terka.FontBuilder.Parser.Output.Context
{
    using System.Collections.Generic;

    /// <summary>
    /// Base for rule-based chaining context transformation tables.
    /// </summary>
    public abstract class ChainingRuleContextTransformationTableBase : CoveredGlyphTransformationTableBase
    {
        /// <summary>
        /// Gets or sets the transformation rules.
        /// </summary>
        /// <value>
        /// The substitution rules.
        /// </value>
        public virtual IEnumerable<IEnumerable<ChainingContextTransformationRule>> TransformationRules
        {
            get;
            set;
        }
    }
}