namespace Terka.TinyFonts.TFConvert
{
    /// <summary>
    /// Adjusts the internal leading measurement of the font being created.
    /// Only one AdjustInternalLeading command is allowed per definition.
    /// </summary>
    /// <remarks>
    /// The internal leading measurement cannot be decreased to less than 0. For example, if the original font
    /// has an internal leading measurement of 3, and adjustment is -4, TFConvert will fail.
    /// </remarks>
    [TinyCommand(IsGlobal = true)]
    public class AdjustInternalLeading : TinyAdjustmentCommandBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="AdjustInternalLeading"/> command.
        /// </summary>
        /// <param name="adjustment">
        /// An integer indicating the number of EM units to adjust the internal leading measurement.
        /// A positive number increases the internal leading measurement, and a negative number decreases it.
        /// </param>
        public AdjustInternalLeading(short adjustment) : base(adjustment) { }
        /// <summary>
        /// Creates a new instance of <see cref="AdjustInternalLeading"/> command.
        /// </summary>
        public AdjustInternalLeading() : base() { }
    }
}
