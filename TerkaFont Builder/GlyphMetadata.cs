namespace Terka.FontBuilder
{
    using System;

    /// <summary>
    /// Corresponds to OT "GlyphClassDef".
    /// </summary>
    public enum GlyphClass
    {
        /// <summary>
        /// The glyph doesn't have its class defined in the GDEF table.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Corresponds to OT "Base glyph".
        /// </summary>
        Base = 1,

        /// <summary>
        /// Corresponds to OT "Ligature glyph".
        /// </summary>
        Ligature = 2,

        /// <summary>
        /// Corresponds to OT "Mark glyph".
        /// </summary>
        Mark = Base | Ligature,

        /// <summary>
        /// Corresponds to OT "Component glyph".
        /// </summary>
        Component = 4
    }

    /// <summary>
    /// Contains additional information about glyphs necessary during state machine run-time.
    /// </summary>
    public class GlyphMetadata
    {
        public GlyphMetadata()
        {
            this.CharacterToGlyphIdMapping = p => (ushort)p;
            this.GlyphIdToGlyphClassMapping = p => GlyphClass.Unknown;
            this.GlyphIdToMarkAttachClassIdMapping = p => 0;
        }

        public Func<char, ushort> CharacterToGlyphIdMapping { get; set; }

        public Func<ushort, GlyphClass> GlyphIdToGlyphClassMapping { get; set; }

        public Func<ushort, ushort> GlyphIdToMarkAttachClassIdMapping { get; set; }

        public bool IsGlyphIgnored(ushort glyphId, LookupFlags lookupFlags)
        {
            var glyphClass = this.GlyphIdToGlyphClassMapping(glyphId);

            // UseMarkFilteringSet is not supported
            return
                ((lookupFlags & LookupFlags.IgnoreBaseGlyphs) > 0 && glyphClass == GlyphClass.Base) ||
                ((lookupFlags & LookupFlags.IgnoreMarks) > 0 && glyphClass == GlyphClass.Mark) ||
                ((lookupFlags & LookupFlags.IgnoreLigatures) > 0 && glyphClass == GlyphClass.Ligature) ||
                ((lookupFlags & LookupFlags.MarkAttachmentTypeMask) > 0 && this.GlyphIdToMarkAttachClassIdMapping(glyphId) != (uint)(lookupFlags & LookupFlags.MarkAttachmentTypeMask) >> 16);
        }
    }
}
