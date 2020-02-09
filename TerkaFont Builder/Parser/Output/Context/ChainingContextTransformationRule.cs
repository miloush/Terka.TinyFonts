namespace Terka.FontBuilder.Parser.Output.Context
{
    using System.Collections.Generic;

    /// <summary>
    /// Corresponds to OT "ChainSubRule".
    /// </summary>
    public class ChainingContextTransformationRule
    {
        /// <summary>
        /// Gets or sets the context (on which the transformations are applied).
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public virtual IEnumerable<ushort> Context
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the lookahead context.
        /// </summary>
        /// <value>
        /// The lookahead.
        /// </value>
        public virtual IEnumerable<ushort> Lookahead
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the lookback context.
        /// </summary>
        /// <value>
        /// The lookback.
        /// </value>
        public virtual IEnumerable<ushort> Lookback
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the transformations applied on the matched context.
        /// </summary>
        /// <value>
        /// The transformations.
        /// </value>
        public virtual IEnumerable<ContextTransformationSet> TransformationSets
        {
            get;
            set;
        }
    }
}