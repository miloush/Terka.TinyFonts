using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Terka.TinyFonts
{
    /// <summary>
    /// OpenType Compiler.
    /// </summary>
    public interface IOpenTypeCompiler
    {
        /// <summary>
        /// Checks if substitution feature is present in glyph <paramref name="typeface"/>.
        /// </summary>
        /// <param name="typeface">Glyph typeface in which look for feature.</param>
        /// <param name="scriptId">ID of script in which look for feature.</param>
        /// <param name="languageId">ID of language in which look for feature.</param>
        /// <param name="featureId">ID of feature to look up.</param>
        /// <returns>True if feature is present.</returns>
        bool IsSubstitutionFeaturePresent(GlyphTypeface typeface, uint scriptId, uint languageId, uint featureId);
        /// <summary>
        /// Checks if positioning feature is present in glyph <paramref name="typeface"/>.
        /// </summary>
        /// <param name="typeface">Glyph typeface in which look for feature.</param>
        /// <param name="scriptId">ID of script in which look for feature.</param>
        /// <param name="languageId">ID of language in which look feature up.</param>
        /// <param name="featureId">ID of feature to look up.</param>
        /// <returns>True if feature is present.</returns>
        bool IsPositioningFeaturePresent(GlyphTypeface typeface, uint scriptId, uint languageId, uint featureId);

        /// <summary>
        /// Compiles state machine for substitution feature and saves state machine to <paramref name="substitution"/> appendix.
        /// </summary>
        /// <param name="typeface">Glyph typeface in which look for feature.</param>
        /// <param name="script">ID of script in which look for feature.</param>
        /// <param name="language">ID of language in which look for feature.</param>
        /// <param name="feature">ID of feature to look up.</param>
        /// <param name="substitution">Substitution appendix in which will be compiled state machine stored.</param>
        /// <param name="glyphClasses">Glyph classes appendix for use by substitution appendix.</param>
        /// <param name="availableGlyphs">Which glyphs should be restricted in state machine.</param>
        /// <returns>All used glyphs during compilation. Can add additional glyphs to availableGlyphs.</returns>
        IEnumerable<ushort> CompileFeature(GlyphTypeface typeface, uint script, uint language, uint feature, SubstitutionAppendix substitution, GlyphClassesAppendix glyphClasses, IEnumerable<ushort> availableGlyphs);
        /// <summary>
        /// Compiles state machine for positioning feature and saves state machine to <paramref name="positioning"/> appendix.
        /// </summary>
        /// <param name="typeface">Glyph typeface in which look for feature.</param>
        /// <param name="script">ID of script in which look for feature.</param>
        /// <param name="language">ID of language in which look for feature.</param>
        /// <param name="feature">ID of feature to look up.</param>
        /// <param name="positioning">Positioning appendix in which will be compiled state machine stored.</param>
        /// <param name="glyphClasses">Glyph classes appendix for use by positioning appendix.</param>
        /// <param name="availableGlyphs">Which glyphs should be restricted in state machine.</param>
        /// <param name="emSize">Requested em size.</param>
        /// <returns>All used glyphs during compilation. Can add additional glyphs to availableGlyphs.</returns>
        IEnumerable<ushort> CompileFeature(GlyphTypeface typeface, uint script, uint language, uint feature, PositioningAppendix positioning, GlyphClassesAppendix glyphClasses, IEnumerable<ushort> availableGlyphs, double emSize);

        /// <summary>
        /// Gets generated glyphs during compilation of state machine.
        /// </summary>
        /// <param name="typeface">Glyph typeface in which look for feature.</param>
        /// <param name="scriptId">ID of script in which look for feature.</param>
        /// <param name="languageId">ID of language in which look for feature.</param>
        /// <param name="featureId">ID of feature to look up.</param>
        /// <returns>All used glyphs during compilation.</returns>
        IEnumerable<ushort> GetGeneratedGlyphIds(GlyphTypeface typeface, uint scriptId, uint languageId, uint featureId);
    }
}
