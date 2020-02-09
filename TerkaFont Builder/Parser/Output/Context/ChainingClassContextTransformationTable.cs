namespace Terka.FontBuilder.Parser.Output.Context
{
    /// <summary>
    /// Corresponds to OT "Chaining context substitution format 2".
    /// </summary>
    public class ChainingClassContextTransformationTable : ChainingRuleContextTransformationTableBase
    {
        /// <summary>
        /// Gets or sets the class definitions for the lookahead context.
        /// </summary>
        /// <value>
        /// The lookahead class definition.
        /// </value>
        public virtual IGlyphClassDefinition LookaheadClassDefinition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the class definitions for the main context.
        /// </summary>
        /// <value>
        /// The context class definition.
        /// </value>
        public virtual IGlyphClassDefinition ContextClassDefinitions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the class definitions for the lookback context.
        /// </summary>
        /// <value>
        /// The lookback class definition.
        /// </value>
        public virtual IGlyphClassDefinition LookbackClassDefinition
        {
            get;
            set;
        }
    }
}