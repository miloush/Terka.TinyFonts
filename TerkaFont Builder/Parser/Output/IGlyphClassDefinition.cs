namespace Terka.FontBuilder.Parser.Output
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Corresponds to OT "ClassDef".
    /// </summary>
    public interface IGlyphClassDefinition
    {
        /// <summary>
        /// Gets the class assignments.
        /// </summary>
        /// <value>
        /// The class assignments. Indexed by class ID, values are glyph IDs. Class IDs are 1-based.
        /// </value>
        ILookup<ushort, ushort> ClassAssignments { get; }
    }
}

