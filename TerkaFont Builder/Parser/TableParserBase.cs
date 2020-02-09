namespace Terka.FontBuilder.Parser
{
    using System.Windows.Media;

    using Terka.FontBuilder.Parser.Reflection;
    using Terka.FontBuilder.Parser.Reflection.Extensions;

    public abstract class TableParserBase
    {
        protected abstract dynamic GetFontTableHeader();

        protected abstract dynamic GetFontTable(GlyphTypeface typeface);

        protected dynamic GetOpenTypeFont(GlyphTypeface typeface)
        {
            var gsubGposTablesType = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.FontCache.GsubGposTables");

            return new AccessPrivateWrapper(TypeExtensions.InstantiateNonPublic(gsubGposTablesType, this.GetFontFaceLayoutInfo(typeface).Wrapped));
        }

        private dynamic GetFontFaceLayoutInfo(GlyphTypeface typeface)
        {
            return new AccessPrivateWrapper(typeface.AccessNonPublic().FontFaceLayoutInfo);
        }
    }
}