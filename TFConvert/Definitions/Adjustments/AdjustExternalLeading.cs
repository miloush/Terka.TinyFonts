namespace Terka.TinyFonts.TFConvert
{
    /// <summary>
    /// Adjusts the external leading measurement of the font being created.
    /// Only one AdjustExternalLeading command is allowed per definition.
    /// </summary>
    /// <remarks>
    /// The external leading measurement cannot be decreased to less than 0. For example, if the original font
    /// has an external leading measurement of 3, and adjustment is -4, TFConvert will fail.    
    /// </remarks>
    [TinyCommand(IsGlobal = true)]
    public class AdjustExternalLeading : TinyAdjustmentCommandBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AdjustExternalLeading"/> command.
        /// </summary>
        public AdjustExternalLeading()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AdjustExternalLeading"/> command.
        /// </summary>
        /// <param name="adjustment">
        /// An integer indicating the number of EM units to adjust the external leading measurement.
        /// A positive number increases the external leading measurement, and a negative number decreases it.
        /// </param>
        public AdjustExternalLeading(short adjustment) : base(adjustment) 
        {
        }
    }
}
