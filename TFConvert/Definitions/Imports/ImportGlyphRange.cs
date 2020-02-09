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
    public class ImportGlyphRange : TinyCommandBase
    {
        /// <summary>
        /// An unsigned short integer that indicates the beginning of the range of glyphs to import.
        /// </summary>
        [TinyParameter(0)]
        public ushort Start;

        /// <summary>
        /// An unsigned short integer that indicates the end of the range of glyphs to import.
        /// </summary>
        [TinyParameter(1)]
        public ushort End;

        /// <summary>
        /// Initializes a new instance of <see cref="ImportGlyphRange"/> command.
        /// </summary>
        public ImportGlyphRange()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ImportGlyphRange"/> command with specified start and end bounds.
        /// </summary>
        /// <param name="start">An unsigned short integer that indicates the beginning of the range of glyphs to import.</param>
        /// <param name="end">An unsigned short integer that indicates the end of the range of glyphs to import.</param>
        public ImportGlyphRange(ushort start, ushort end)
        {
            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ImportGlyphRange"/> command that imports single glyph.
        /// </summary>
        /// <param name="glyphId">The glyph ID for the glyph to import.</param>
        public ImportGlyphRange(ushort glyphId)
        {
            this.Start = this.End = glyphId;
        }

        /// <summary>
        /// Converts an relative glyph range import to an absolute one.
        /// </summary>
        /// <param name="command">The command with relative definitions.</param>
        /// <returns>A command with the same absolute definitions.</returns>
        public static explicit operator ImportGlyphRange(ImportGlyphRangeAndMap command)
        {
            return new ImportGlyphRange(command.AbsoluteStart, command.AbsoluteEnd);
        }
    }
}
