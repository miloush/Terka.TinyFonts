namespace Terka.FontBuilder.Parser.Output
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Corresponds to OT "Class definition table format 1".
    /// </summary>
    public class ListGlyphClassDefinition : IGlyphClassDefinition
    {
        /// <summary>
        /// Gets or sets the first glyph ID. Length of the classified segment is defined by length of <see cref="ClassIdList"/>.
        /// </summary>
        /// <value>
        /// The first glyph ID.
        /// </value>
        public virtual ushort FirstGlyphId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the class ID list.
        /// </summary>
        /// <value>
        /// The class ID list. Each class ID belongs to corresponding glyph in the classified segment.
        /// </value>
        public virtual IEnumerable<ushort> ClassIdList
        {
            get;
            set;
        }

        /// <inheritdoc />
        public ILookup<ushort, ushort> ClassAssignments
        {
            get
            {
                return
                    this.ClassIdList
                        .Zip(Enumerable.Range(this.FirstGlyphId, this.ClassIdList.Count()), (classId, glyphId) => new { GlyphId = glyphId, ClassId = classId })
                        .ToLookup(p => p.ClassId, p => (ushort)p.GlyphId);
            }
        }
    }
}
