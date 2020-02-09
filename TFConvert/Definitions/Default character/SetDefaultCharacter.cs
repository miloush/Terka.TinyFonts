namespace Terka.TinyFonts.TFConvert
{
    /// <summary>
    /// Indicates that the default character defined by a TrueType font will be substituted for 
    /// characters not imported to the TinyFont.
    /// </summary>
    /// <remarks>
    /// A <see cref="SelectFont" /> command that specifies the font from which to import glyphs
    /// must precede the SetDefaultCharacter command in the definition, otherwise TFConvert will fail.
    /// If there are multiple <see cref="SelectFont" /> statements, the one that more closely precedes
    /// the SetDefaultCharacter statement will be used.
    /// </remarks>
    [TinyCommand(IsGlobal = true)]
    [RequiresCommand(typeof(SelectFont), Before = true)]
    public class SetDefaultCharacter : TinyCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetDefaultCharacter"/> command.
        /// </summary>
        public SetDefaultCharacter()
        {
        }
    }
}
