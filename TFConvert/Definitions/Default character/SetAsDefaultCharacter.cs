namespace Terka.TinyFonts.TFConvert
{
    // [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Fields do not corrupt state and distingush computed values.")]

    /// <summary>
    /// Specifies the value of the character to substitute for characters not imported into the TinyFont.
    /// </summary>
    /// <remarks>
    /// A <see cref="SelectFont"/> statement that specifies the font from which to import glyphs
    /// must precede the SetAsDefaultCharacter statement in the definition, otherwise TFConvert will fail.
    /// If there are multiple <see cref="SelectFont"/> statements, the one that more closely precedes
    /// the SetAsDefaultCharacter statement will be used.
    /// </remarks>
    [TinyCommand(IsGlobal = true)]
    [RequiresCommand(typeof(SelectFont), Before = true)]
    public class SetAsDefaultCharacter : TinyCommandBase
    {
        /// <summary>
        /// The Unicode code point for the default character.
        /// </summary>
        [TinyParameter]
        public int CharacterCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetAsDefaultCharacter"/> command.
        /// </summary>    
        public SetAsDefaultCharacter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetAsDefaultCharacter"/> command with specified character.
        /// </summary>
        /// <param name="defaultChar">
        /// The desired default character.
        /// </param>
        public SetAsDefaultCharacter(char defaultChar)
        {
            this.Character = defaultChar;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetAsDefaultCharacter"/> command with specified Unicode code point.
        /// </summary>
        /// <param name="defaultCharCode">
        /// The Unicode code point for the default character.
        /// </param>
        public SetAsDefaultCharacter(int defaultCharCode)
        {
            this.CharacterCode = defaultCharCode;
        }

        /// <summary>
        /// Gets or sets the desired default character.
        /// </summary>
        /// <returns>
        /// The desired default character.
        /// </returns>
        public char Character
        {
            get { return (char)this.CharacterCode; }
            set { this.CharacterCode = (int)value; }
        }
    }
}
