namespace Terka.FontBuilder.Parser.Output
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Corresponds to "Class definition table format 2".
    /// </summary>
    public class RangeGlyphClassDefinition : IGlyphClassDefinition
    {
        /// <summary>
        /// Gets or sets the class ranges.
        /// </summary>
        /// <value>
        /// The class ranges. Key is the glyph ID range (tuple of min and max glyph ID) and value is the class ID.
        /// </value>
        public Dictionary<Tuple<ushort, ushort>, ushort> ClassRanges { get; set; }

        /// <inheritdoc />
        public ILookup<ushort, ushort> ClassAssignments
        {
            get
            {
                return (
                    from classRange in this.ClassRanges
                    from i in Enumerable.Range(classRange.Key.Item1, classRange.Key.Item2 - classRange.Key.Item1 + 1)
                    select new { GlyphId = (ushort)i, ClassId = classRange.Value }).ToLookup(p => p.ClassId, p => p.GlyphId);
                    // TODO: Pouzit query expression "group by"
            }
        }

        public static RangeGlyphClassDefinition CreateEmptyClassDef()
        {
            return new RangeGlyphClassDefinition { ClassRanges = new Dictionary<Tuple<ushort, ushort>, ushort>() };
        }
    }
}