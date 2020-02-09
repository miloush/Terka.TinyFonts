namespace Terka.TinyFonts
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// A helper class that groups data about a single character in <see cref="TinyFont"/>.
    /// </summary>
    public class CharacterInfo
    {
        private TinyFont _font;
        private char _c;
        private int _rangeIndex;
        private int _characterIndex;
        private CharacterRangeDescription _range;
        private CharacterDescription _character;
        private int _offset;
        private int _antialiasOffset;
        private byte[] _characterBitmapData;
        private byte[] _characterAntialiasingData;
        private int _planeNumber;

        /// <summary>
        /// Gets Tiny Font which contains this character.
        /// </summary>
        public TinyFont Font { get { return _font; } }
        /// <summary>
        /// Gets range of characters in which this character belongs.
        /// </summary>
        public CharacterRangeDescription CharacterRange { get { return _range; } }
        /// <summary>
        /// Gets <see cref="CharacterDescription"/> metrics.
        /// </summary>
        public CharacterDescription Character { get { return _character; } }
        /// <summary>
        /// Gets character.
        /// </summary>
        public char CharacterValue { get { return _c; } }
        /// <summary>
        /// Gets index to character range containing this character.
        /// </summary>
        public int CharacterRangeIndex { get { return _rangeIndex; } }
        /// <summary>
        /// Gets character index widthin its plane.
        /// </summary>
        public int CharacterIndex { get { return _characterIndex; } }
        /// <summary>
        /// Gets raw character bitmap data.
        /// </summary>
        public byte[] CharacterBitmapData { get { return _characterBitmapData; } }
        /// <summary>
        /// Gets raw character anti-aliasing data.
        /// </summary>
        public byte[] CharacterAntialiasingData { get { return _characterAntialiasingData; } }
        /// <summary>
        /// Gets number of plane to which this character belongs.
        /// </summary>
        public int PlaneNumber { get { return _planeNumber; } }
        /// <summary>
        /// Gets full character code.
        /// </summary>
        public int Codepoint { get { return (_planeNumber << 16) + _c; } }
        /// <summary>
        /// Gets or sets character's left margin.
        /// </summary>
        public sbyte MarginLeft
        {
            get { return _character.LeftMargin; }
            set { _character.LeftMargin = value; }
        }
        /// <summary>
        /// Gets or sets character's right margin.
        /// </summary>
        public sbyte MarginRight
        {
            get { return _character.RightMargin; }
            set { _character.RightMargin = value; }
        }
        /// <summary>
        /// Gets or sets character's inner width.
        /// </summary>
        public short InnerWidth
        {
            get
            {
                checked
                {
                    if (_c == _range.LastCharacter)
                        return (short)((_font.CharacterRanges[_rangeIndex + 1].Offset + _font.Characters[_characterIndex + 1].Offset) - _offset);

                    else
                        return (short)(_font.Characters[_characterIndex + 1].Offset - _character.Offset);
                }
            }
        }

        /// <summary>
        /// Gets or sets character's width in bitmap.
        /// </summary>
        public short Width
        {
            get { checked { return (short)(MarginLeft + MarginRight + InnerWidth); } }
        }
        /// <summary>
        /// Gets or sets character's height in bitmap.
        /// </summary>
        public short Height
        {
            get { checked { return (short)_font.CharacterBitmap.Height; } }
        }

        /// <summary>
        /// Gets offset to character's position in <see cref="CharacterBitmapData"/>.
        /// </summary>
        public int Offset { get { return _offset; } }
        /// <summary>
        /// Gets offset to character's position in <see cref="CharacterAntialiasingData"/>.
        /// </summary>
        public int AntialiasOffset { get { return _antialiasOffset; } }

        /// <summary>
        /// Creates new instance of Character Info.
        /// </summary>
        /// <param name="font">Tiny Font containing character.</param>
        /// <param name="rangeIndex">Index to character range containing this character.</param>
        /// <param name="c">Characeter.</param>
        /// <param name="plane">Font plane describing plane to which this character belongs.</param>
        /// <param name="planeNumber">Number of <paramref name="plane"/> containg this character.</param>
        internal CharacterInfo(TinyFont font, int rangeIndex, char c, FontPlane plane, int planeNumber)
        {
            Contract.Assert(plane != null, "Plane for specified character does not exists.");

            _font = font;
            _characterAntialiasingData = plane.CharacterAntialiasingData;
            _characterBitmapData = plane.CharacterBitmapData;
            _c = (char)c;
            _planeNumber = planeNumber;

            _rangeIndex = rangeIndex;
            _range = plane.CharacterRanges[_rangeIndex];

            checked
            {
                _characterIndex = (int)_range.IndexOfFirstCharacter + _c - _range.FirstCharacter;
                _character = plane.Characters[_characterIndex];

                _offset = (int)(_range.Offset + _character.Offset);

                if (_font.Description.IsExtended)
                    _antialiasOffset = (int)(plane.CharacterRangesAntialiasing[_rangeIndex].Offset + plane.CharactersAntialiasing[_characterIndex].Offset);
                else
                    _antialiasOffset = -1;
            }
        }
        
        /// <summary>
        /// Returns if on specified position is any visible pixel of character. If true, pixel is visible.
        /// </summary>
        /// <param name="x">x position.</param>
        /// <param name="y">y position.</param>
        /// <returns>If true pixel is visible.</returns>
        public bool GetPixel(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x");

            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("y");

            if (y < _font.Metrics.Offset)
                return false;

            y -= _font.Metrics.Offset;

            int wordWidth = (int)((_font.CharacterBitmap.Width * _font.CharacterBitmap.BitsPerPixel + 31) / 32) * 32;
            return Helper.ReadBit(_font.CharacterBitmapData, _offset + wordWidth * y + x);
        }

        /// <summary>
        /// Returns intensity of character's anti-aliasing on specified position.
        /// </summary>
        /// <param name="x">x position.</param>
        /// <param name="y">y position.</param>
        /// <returns>Anti-aliasing intensity.</returns>
        public double GetAntialias(int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}