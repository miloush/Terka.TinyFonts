namespace Terka.FontBuilder.Parser.Output.Context
{
    using System.Collections.Generic;

    /// <summary>
    /// Corresponds to OT "SubRule table".
    /// </summary>
    public class ContextTransformationRule
    {
        /// <summary>
        /// Gets or sets the context glyph/class IDs.
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
        /// Gets or sets the collection of substitutions to execute on a matched range.
        /// </summary>
        /// <value>
        /// The substitutions.
        /// </value>
        public virtual IEnumerable<ContextTransformationSet> TransformationSets
        {
            get;
            set;
        }
    }
}