using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Terka.TinyFonts
{
    partial class TinyFontBuilder
    {
        private class BuilderState
        {
            public readonly GlyphTypeface GlyphTypeface;
            public readonly double EmSize;
            
            public readonly AntialiasingLevel AntialiasingLevel;
            public readonly Transform GlyphTransform;
            public readonly Point GlyphTransformOrigin;

            public readonly IDictionary<ushort, int> GlyphToCharacterMap;

            public BuilderState(TinyFontBuilder current)
            {
                GlyphTypeface = current.GlyphTypeface;
                EmSize = current.EmSize;

                AntialiasingLevel = current.AntialiasingLevel;
                GlyphTransform = current.GlyphTransform;
                GlyphTransformOrigin = current.GlyphTransformOrigin;

                GlyphToCharacterMap = current.GlyphToCharacterMap;
            }
        }
    }
}
