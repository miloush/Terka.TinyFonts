namespace Terka.TinyFonts.TFConvert
{
    // [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Fields do not corrupt state and distingush computed values.")]

    /// <summary>
    /// Specifies the path to a TrueType font to use for building a TinyFont. 
    /// </summary>
    [TinyCommand]
    public class AddFontToProcess : TinyCommandBase
    {
        /// <summary>
        /// The fully qualified path to a TrueType font file.
        /// </summary>
        [TinyParameter(RequiresQuotes = true, RequiresEscapement = true)]
        public string FontPath;

        /// <summary>
        /// Creates a new instance of <see cref="AddFontToProcess"/> command.
        /// </summary>
        public AddFontToProcess() 
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="AddFontToProcess"/> command.
        /// </summary>
        /// <param name="path">The fully qualified path to a TrueType font file.</param>
        /// <remarks>
        /// TFConvert attempts to match the font characteristics specified
        /// in the SelectFont option to the font named in path,
        /// as well as to any fonts installed on the system.
        /// The TinyFont file will be generated from the first match found,
        /// but it is not guaranteed that the path argument will be searched first. 
        /// </remarks>
        public AddFontToProcess(string path)
        {
            this.FontPath = path;
        }
    }
}
