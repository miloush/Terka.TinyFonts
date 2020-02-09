namespace Terka.TinyFonts
{
    using System;
    using System.IO;

    /// <summary>
    /// Font appendix storage base class.
    /// </summary>
    public abstract class FontAppendix
    {
        /// <summary>
        /// ID of glyph classes appendix.
        /// </summary>
        public const byte GlyphClasses = (byte)'C';
        /// <summary>
        /// ID of glyphs appendix.
        /// </summary>
        public const byte Glyphs = (byte)'G';
        /// <summary>
        /// ID of glyphs metadata appendix.
        /// </summary>
        public const byte GlyphMetadata = (byte)'M';
        /// <summary>
        /// ID of positioning machine appendix.
        /// </summary>
        public const byte PositioningMachine = (byte)'P';
        /// <summary>
        /// ID of substitution machine appendix.
        /// </summary>
        public const byte SubstitutionMachine = (byte)'S';
        /// <summary>
        /// ID of font strings appendix.
        /// </summary>
        public const byte FontStrings = (byte)'T';
        /// <summary>
        /// ID of unicode plane appendix.
        /// </summary>
        public const byte UnicodePlane = (byte)'U';
        /// <summary>
        /// ID of attachment points appendix.
        /// </summary>
        public const byte AttachmentPoints = (byte)'A';

        private byte _id;

        /// <summary>
        /// Gets ID of this font appendix.
        /// </summary>
        public byte ID { get { return _id; } }
        /// <summary>
        /// Gets size of this appendix.
        /// </summary>
        public abstract int GetSize(TinyFont font);
        /// <summary>
        /// Gets if this font appendix has any valid content.
        /// </summary>
        public virtual bool HasContent { get { return true; } }

        /// <summary>
        /// Creates new instance of font appendix specified by its ID.
        /// </summary>
        /// <param name="id">Font appendix ID.</param>
        public FontAppendix(byte id)
        {
            _id = id;
        }

        /// <summary>
        /// Deserializes this structure from binary data.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
        /// <param name="font">The <see cref="TinyFont"/> containing this appendix.</param>
        public abstract void ReadFrom(BinaryReader reader, TinyFont font);
        /// <summary>
        /// Serializes this structure into binary data.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
        /// <param name="font">The <see cref="TinyFont"/> containing this appendix.</param>
        public abstract void WriteTo(BinaryWriter writer, TinyFont font);

        /// <summary>
        /// Ensures this structure contains valid data.
        /// </summary>
        /// <param name="font">The <see cref="TinyFont"/> containing this appendix.</param>
        public abstract void Update(TinyFont font);
    }
}
