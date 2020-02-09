using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terka.TinyFonts
{
    partial class TinyFontBuilder
    {
        private class CharacterGlyphPair
        {
            private int? _character;
            private ushort? _glyph;

            public int? Character { get { return _character; } set { _character = value; } }
            public ushort? Glyph { get { return _glyph; } set { _glyph = value; } }

            public bool HasBasicPlaneCharacter
            {
                get { return _character <= BasicPlaneLastCharacter; }
            }

            public CharacterGlyphPair(int? character, ushort? glyph)
            {
                _character = character;
                _glyph = glyph;
            }

            public override string ToString()
            {
                object character = "null";
                if (_character.HasValue)
                    character = (char)(int)_character;

                object glyph = "null";
                if (_glyph.HasValue)
                    glyph = (ushort)_glyph;

                return string.Format("Character: {1} '{0}', Glyph: {2}", character, (int)_character, glyph);
            }
        }
    }
}
