namespace Terka.FontBuilder.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    using Terka.FontBuilder.Parser.Reflection;
    using Terka.FontBuilder.Parser.Reflection.Extensions;

    public class GdefParser : TableParserBase
    {
        public IEnumerable<ushort> GetGlyphIdsByLookupFlags(GlyphTypeface typeface, LookupFlags lookupFlags)
        {
            var glyphClasses = this.GetGlyphClasses(typeface);
            var markAttachClasses = this.GetMarkAttachClassIds(typeface);

            return 
                from glyphWithClass in glyphClasses 
                let currentGlyphId = glyphWithClass.Key 
                let currentGlyphClass = glyphWithClass.Value 
                where 
                    !((lookupFlags & LookupFlags.IgnoreBaseGlyphs) != 0 && currentGlyphClass == GlyphClass.Base) && 
                    !((lookupFlags & LookupFlags.IgnoreLigatures) != 0 && currentGlyphClass == GlyphClass.Ligature) && 
                    !((lookupFlags & LookupFlags.IgnoreMarks) != 0 && currentGlyphClass == GlyphClass.Mark) &&
                    !((lookupFlags & LookupFlags.MarkAttachmentTypeMask) != 0 && (ushort)(lookupFlags & LookupFlags.MarkAttachmentTypeMask) == markAttachClasses[currentGlyphId]) 
                select currentGlyphId; 
        }

        public Dictionary<ushort, GlyphClass> GetGlyphClasses(GlyphTypeface typeface)
        {
            dynamic header = this.GetFontTableHeader();
            dynamic fontTable = this.GetFontTable(typeface);

            if (!fontTable.IsPresent)
            {
                return new Dictionary<ushort, GlyphClass>();
            }

            dynamic classDef = new AccessPrivateWrapper(header.GetGlyphClassDef(fontTable.Wrapped));

            if (classDef.IsInvalid)
            {
                return new Dictionary<ushort, GlyphClass>();
            }

            return Enumerable.Range(0, ushort.MaxValue)
                .ToDictionary(
                    glyphId => (ushort)glyphId,
                    glyphId => (GlyphClass)classDef.GetClass(fontTable.Wrapped, (ushort)glyphId));
        }

        public Dictionary<ushort, ushort> GetMarkAttachClassIds(GlyphTypeface typeface)
        {
            dynamic header = this.GetFontTableHeader();
            dynamic fontTable = this.GetFontTable(typeface);

            if (!fontTable.IsPresent)
            {
                return new Dictionary<ushort, ushort>();
            }

            dynamic classDef = new AccessPrivateWrapper(header.GetMarkAttachClassDef(fontTable.Wrapped));

            if (classDef.IsInvalid)
            {
                return new Dictionary<ushort, ushort>();
            }

            return Enumerable.Range(0, ushort.MaxValue)
                .ToDictionary(
                    glyphId => (ushort)glyphId,
                    glyphId => (ushort)classDef.GetClass(fontTable.Wrapped, (ushort)glyphId));
        }

        protected override dynamic GetFontTableHeader()
        {
            var headerType = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.GDEFHeader");

            return Activator.CreateInstance(headerType).AccessNonPublic();
        }

        protected override dynamic GetFontTable(GlyphTypeface typeface)
        {
            var fontTableType = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.FontTable");

            dynamic font = this.GetOpenTypeFont(typeface);
            dynamic layoutInfo = new AccessPrivateWrapper(font._layout);
            byte[] fontTableBytes = layoutInfo.Gdef();

            return Activator.CreateInstance(fontTableType, fontTableBytes).AccessNonPublic();
        }
    }
}
