namespace Terka.TinyFonts.TFConvert
{
    // [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Fields do not corrupt state and distingush computed values.")]

    /// <summary>
    /// Specifies the verbosity level of TFConvert output. 
    /// </summary>
    [TinyCommand]
    public partial class Verbosity : TinyCommandBase
    {
        /// <summary>
        /// The verbosity level of TFConvert output.
        /// </summary>
        [TinyParameter]
        public VerbosityLevel Level;

        /// <summary>
        /// Initializes a new instance of the <see cref="Verbosity"/> command.
        /// </summary>
        public Verbosity() 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Verbosity"/> command with specified level.
        /// </summary>
        /// <param name="level">The verbosity level of TFConvert output.</param>
        public Verbosity(VerbosityLevel level)
        {
            this.Level = level;
        }

        /// <summary>
        /// Returns a string that represents this command. 
        /// </summary>
        /// <returns>A string that represents this command.</returns>
        public override string ToString()
        {
            return this.CommandString + " " + (int)this.Level;
        }
    }
}
