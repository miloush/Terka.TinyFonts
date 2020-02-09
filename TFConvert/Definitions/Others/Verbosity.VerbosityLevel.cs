namespace Terka.TinyFonts.TFConvert
{
    /// <content>
    /// Contains the <see cref="VerbosityLevel"/> enum.
    /// </content>
    public partial class Verbosity
    {
        /// <summary>
        /// Contains valid verbosity levels.
        /// </summary>
        public enum VerbosityLevel
        {
            /// <summary>
            /// Details are not displayed.
            /// </summary>
            NoDetails = 0,

            /// <summary>
            /// Font properties are displayed.
            /// </summary>
            FontProperties = 1,

            /// <summary>
            /// Font properties and character properties, and a diagram of each character, are displayed.
            /// </summary>
            CharacterProperties = 2
        }
    }
}
