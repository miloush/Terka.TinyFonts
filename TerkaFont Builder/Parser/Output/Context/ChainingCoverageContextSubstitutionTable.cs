namespace Terka.FontBuilder.Parser.Output.Context
{
    using System.Collections.Generic;

    /// <summary>
    /// Corresponds to OT "Chaining context substitution format 3".
    /// </summary>
    public class ChainingCoverageContextSubstitutionTable : IGlyphTransformationTable
    {
        /// <summary>
        /// Gets or sets the lookback coverages.
        /// </summary>
        /// <value>
        /// The lookback coverages.
        /// </value>
        public virtual IEnumerable<ICoverageTable> LookbackCoverages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the context coverages.
        /// </summary>
        /// <value>
        /// The context coverages.
        /// </value>
        public virtual IEnumerable<ICoverageTable> ContextCoverages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the lookahead coverages.
        /// </summary>
        /// <value>
        /// The lookahead coverages.
        /// </value>
        public virtual IEnumerable<ICoverageTable> LookaheadCoverages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the substitutions.
        /// </summary>
        /// <value>
        /// The substitutions.
        /// </value>
        public virtual IEnumerable<ContextTransformationSet> TransformationSets
        {
            get;
            set;
        }

        /// <inheritdoc />
        public LookupFlags LookupFlags { get; set; }
    }
}