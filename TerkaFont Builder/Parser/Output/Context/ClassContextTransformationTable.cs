namespace Terka.FontBuilder.Parser.Output.Context
{
    /// <summary>
    /// Corresponds to OT "Context substitution format 2".
    /// </summary>
    public class ClassContextTransformationTable : ContextRuleTransformationTableBase
    {
        /// <summary>
        /// Gets or sets the class definitions.
        /// </summary>
        /// <value>
        /// The class definitions.
        /// </value>
        public virtual IGlyphClassDefinition ClassDefinitions
        {
            get;
            set;
        }
    }
}