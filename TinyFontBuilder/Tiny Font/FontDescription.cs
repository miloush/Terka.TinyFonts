namespace Terka.TinyFonts
{
    using System;
    using System.ComponentModel;
    using System.IO;

    /// <summary>
    /// Contains basic information TinyFont.
    /// </summary>
    public class FontDescription
    {
        internal const int SizeOf = sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(FontDescriptionFlags);

        private const ushort AntiAliasingLevelMask = 0xF0;
        private const ushort AntiAliasingLevelShift = 4;

        private ushort _ranges;
        private ushort _characters;
        private FontDescriptionFlags _flags;
        private ushort _appendices;

        /// <summary>
        /// Gets or sets the number of continuous ranges of characters in the font.
        /// </summary>
        public ushort Ranges
        {
            get { return _ranges; }
            set { _ranges = value; }
        }
        /// <summary>
        /// Gets ot sets total number of characters included in the font.
        /// </summary>
        public ushort Characters
        {
            get { return _characters; }
            set { _characters = value; }
        }
        /// <summary>
        /// Gets or sets various flags of the Tiny Font.
        /// </summary>
        public FontDescriptionFlags Flags
        {
            get { return _flags; }
            set { _flags = value; }
        }
        /// <summary>
        /// Gets of sets the number of appendices present in the Tiny Font file.
        /// </summary>
        public ushort Appendices
        {
            get { return _appendices; }
            set { _appendices = value; }
        }

        #region Flags

        /// <summary>
        /// Gets or sets the level of anti-aliasing. 
        /// </summary>
        public AntialiasingLevel AntialiasingLevel
        {
            get { return (AntialiasingLevel)Helper.UnpackFlags(AntiAliasingLevelMask, AntiAliasingLevelShift, (int)_flags); }
            set { _flags = (FontDescriptionFlags)Helper.PackFlags(AntiAliasingLevelMask, AntiAliasingLevelShift, (int)_flags, (int)value); }
        }

        /// <summary>
        /// Gets or sets if Tiny Font is bold.
        /// </summary>
        public bool IsBold
        {
            get { return _flags.HasFlag(FontDescriptionFlags.Bold); }
            set { _flags = (FontDescriptionFlags)Helper.SetFlag((int)_flags, (int)FontDescriptionFlags.Bold, value); }
        }
        /// <summary>
        /// Gets or sets if Tiny Font is italic.
        /// </summary>
        public bool IsItalic
        {
            get { return _flags.HasFlag(FontDescriptionFlags.Italic); }
            set { _flags = (FontDescriptionFlags)Helper.SetFlag((int)_flags, (int)FontDescriptionFlags.Italic, value); }
        }
        /// <summary>
        /// Gets or sets if Tiny Font is underlined.
        /// </summary>
        public bool IsUnderlined
        {
            get { return _flags.HasFlag(FontDescriptionFlags.Underlined); }
            set { _flags = (FontDescriptionFlags)Helper.SetFlag((int)_flags, (int)FontDescriptionFlags.Underlined, value); }
        }
        /// <summary>
        /// Gets or sets if Tiny Font contains extended data (eg. antialiasing data).
        /// </summary>
        public bool IsExtended
        {
            get { return _flags.HasFlag(FontDescriptionFlags.Extended); }
            set { _flags = (FontDescriptionFlags)Helper.SetFlag((int)_flags, (int)FontDescriptionFlags.Extended, value); }
        }

        #endregion

        /// <summary>
        /// Deserializes this structure from binary data.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
        public void ReadFrom(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException();

            _ranges = reader.ReadUInt16();
            _characters = reader.ReadUInt16();
            _flags = (FontDescriptionFlags)reader.ReadUInt16();
            _appendices = reader.ReadUInt16();
        }
        /// <summary>
        /// Serializes this structure into binary data.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> is null.</exception>
        public void WriteTo(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException();

            writer.Write(_ranges);
            writer.Write(_characters);
            writer.Write((ushort)_flags);
            writer.Write(_appendices);
        }
    }
}
