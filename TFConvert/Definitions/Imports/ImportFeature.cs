namespace Terka.TinyFonts.TFConvert
{
    // [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Fields do not corrupt state and distingush computed values.")]

    /// <summary>
    /// Specifies a range of Unicode characters to import.
    /// </summary>
    /// <remarks>
    /// A <see cref="SelectFont" /> command that specifies the font from which to import features 
    /// must precede the ImportFeature command in definition, otherwise TFConvert will fail.
    /// If there are multiple <see cref="SelectFont" /> commands,
    /// the one that more closely precedes the ImportFeature statement will be used.
    /// The ImportFeature command may be applied multiple times per definition,
    /// to import multiple features.
    /// </remarks>
    [TinyCommand]
    [RequiresCommand(typeof(SelectFont), Before = true)]
    public class ImportFeature : TinyCommandBase
    {
        /// <summary>
        /// OpenType tag for script.
        /// </summary>
        [TinyParameter(0)]
        public string Script;

        /// <summary>
        /// OpenType tag for language.
        /// </summary>
        [TinyParameter(1)]
        public string Language;

        /// <summary>
        /// OpenType tag for feature.
        /// </summary>
        [TinyParameter(2)]
        public string Feature;

        /// <summary>
        /// Creates a new instance of <see cref="ImportFeature"/> command.
        /// </summary>
        public ImportFeature()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="ImportFeature"/> command.
        /// </summary>
        /// <param name="script">Tag for script.</param>
        /// <param name="language">Tag for language.</param>
        /// <param name="feature">Tag for feature.</param>
        public ImportFeature(string script, string language, string feature)
        {
            this.Script = script;
            this.Language = language;
            this.Feature = feature;
        }
    }
}