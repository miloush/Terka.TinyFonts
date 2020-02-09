namespace Terka.FontBuilder.Parser.Output.Context
{
    using System.Collections.Generic;

    /// <summary>
    /// Corresponds to "Context substitution format 3".
    /// </summary>
    public class CoverageContextTransformationTable : IGlyphTransformationTable
    {
        /// <summary>
        /// Gets or sets the collection of coverages which define the context.
        /// </summary>
        /// <value>
        /// The coverages.
        /// </value>
        public virtual IEnumerable<ICoverageTable> Coverages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection of substitutions to perform on the matched context.
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