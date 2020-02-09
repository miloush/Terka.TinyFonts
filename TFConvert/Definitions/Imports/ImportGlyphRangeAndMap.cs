using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terka.TinyFonts.TFConvert
{
    // [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Fields do not corrupt state and distingush computed values.")]

    /// <summary>
    /// Specifies a range of glyphs to import.
    /// </summary>
    /// <remarks>
    /// A <see cref="SelectFont" /> command that specifies the font from which to import glyphs 
    /// must precede the ImportGlyphRange command in definition, otherwise TFConvert will fail.
    /// If there are multiple <see cref="SelectFont" /> commands,
    /// the one that more closely precedes the ImportGlyphRange statement will be used.
    /// The ImportGlyphRange command may be applied multiple times per definition,
    /// to select non-contiguous ranges of glyphs.
    /// </remarks>
    [TinyCommand]
    [RequiresCommand(typeof(SelectFont), Before = true)]
    public class ImportGlyphRangeAndMap : TinyCommandBase
    {
        /// <summary>
        /// An unsigned short integer, that when added to offset, indicates the beginning of the range of glyphs to import.
        /// </summary>
        [TinyParameter(0)]
        public ushort Start;

        /// <summary>
        /// An unsigned short integer, when added to offset, indicates the end of the range of glyphs to import.
        /// </summary>
        [TinyParameter(1)]
        public ushort End;

        /// <summary>
        /// An unsigned short integer that indicates the offset from which start and end are calculated.
        /// </summary>
        [TinyParameter(2)]
        public ushort Offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportRangeAndMap"/> command.
        /// </summary>
        public ImportGlyphRangeAndMap()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportRangeAndMap"/> command.
        /// </summary>
        /// <param name="start">An unsigned short integer, that when added to offset, indicates the beginning of the range of glyphs to import.</param>
        /// <param name="end">An unsigned short integer, when added to offset, indicates the end of the range of glyphs to import.</param>
        /// <param name="offset">An unsigned short integer that indicates the offset from which start and end are calculated.</param>
        public ImportGlyphRangeAndMap(ushort start, ushort end, ushort offset)
        {
            this.Start = start;
            this.End = end;
            this.Offset = offset;
        }

        /// <summary>
        /// Gets or sets an unsigned short integer that indicates the beginning of the range of glyphs to import.
        /// </summary>
        /// <returns>
        /// An unsigned short integer that indicates the beginning of the range of glyphs to import.
        /// </returns>
        public ushort AbsoluteStart
        {
            get { return (ushort)(this.Start + this.Offset); }
            set { this.Start = (ushort)(value - this.Offset); }
        }

        /// <summary>
        /// Gets or sets an unsigned short integer that indicates the end of the range of glyphs to import.
        /// </summary>
        /// <returns>
        /// An unsigned short integer that indicates the end of the range of glyphs to import.
        /// </returns>
        public ushort AbsoluteEnd
        {
            get { return (ushort)(this.End + this.Offset); }
            set { this.End = (ushort)(value - this.Offset); }
        }

        /// <summary>
        /// Converts an absolute glyph range import to a relative one.
        /// </summary>
        /// <param name="command">The command with absolute definitions.</param>
        /// <returns>A command with the same relative definitions.</returns>
        public static explicit operator ImportGlyphRangeAndMap(ImportGlyphRange command)
        {
            return new ImportGlyphRangeAndMap(0, (ushort)(command.End - command.Start), command.Start);
        }
    }
}
