namespace Terka.TinyFonts.TFConvert
{
    /// <summary>
    /// Specifies that no default character will substitute for characters not imported into the TinyFont.
    /// Only one NoDefaultCharacter command is allowed per definition.
    /// </summary>
    /// <remarks>
    /// A <see cref="SelectFont" /> command that specifies the font from which to import glyphs
    /// must precede the NoDefaultCharacter command in the definition, otherwise TFConvert will fail.
    /// If there are multiple <see cref="SelectFont" /> statements, the one that more closely precedes
    /// the NoDefaultCharacter statement will be used.
    /// </remarks>
    [TinyCommand(IsGlobal = true)]
    [RequiresCommand(typeof(SelectFont), Before = true)]
    public class NoDefaultCharacter : TinyCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoDefaultCharacter" /> command.
        /// </summary>
        public NoDefaultCharacter()
        {
        }
    }
}
