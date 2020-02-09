namespace Terka.FontBuilder.Parser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.TextFormatting;
    using Terka.FontBuilder.Parser.Reflection;

    /// <summary>
    /// Retrieves information from the CMAP table.
    /// </summary>
    public class CmapParser
    {
        /// <summary>
        /// Gets the character to glyph ID mapping.
        /// </summary>
        /// <param name="typeface">The typeface.</param>
        /// <returns>Dictionary with pairs character - glyphId.</returns>
        public IDictionary<char, ushort> GetCharacterToGlyphIdMapping(GlyphTypeface typeface)
        {
            /* Generate sequence of all characters representable in .Net and feed it into WPF conversion function
             * - it will process the characters through its internal representation of CMAP and return glyph IDs. */
            var conversionChars = Enumerable.Range(0, ushort.MaxValue).Select(p => (char)p).ToArray();
            var characterBuffer = new CharacterBufferRange(conversionChars, 0, conversionChars.Length);

            dynamic typefaceDynamic = new AccessPrivateWrapper(typeface);

            var glyphIdArray = new ushort[conversionChars.Length];
            typefaceDynamic.GetGlyphIndicesOptimized(characterBuffer, glyphIdArray);

            return conversionChars
                .Zip(glyphIdArray, (charId, glyphId) => new { charId, glyphId })
                .Where(p => p.glyphId != 0)
                .ToDictionary(p => p.charId, p => p.glyphId);
        }
    }
}
