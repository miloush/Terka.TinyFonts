namespace Terka.TinyFonts.TFConvert
{
    /// <summary>
    /// Adjusts the ascent of the font being created.
    /// Only one AdjustAcent command is allowed per definition.
    /// </summary>
    /// <remarks>
    /// The ascent cannot be decreased to less than 0. For example, if the original font
    /// has an ascent of 10, and adjustment is -11, TFConvert will fail.
    /// </remarks>
    [TinyCommand(IsGlobal = true)]
    public class AdjustAscent : TinyAdjustmentCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdjustAscent"/> command.
        /// </summary>
        public AdjustAscent()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdjustAscent"/> command.
        /// </summary>
        /// <param name="adjustment">
        /// An integer indicating the number of EM units to adjust the ascent.
        /// A positive number increases the ascent, and a negative number decreases it.
        /// </param>
        public AdjustAscent(short adjustment) : base(adjustment)
        {
        }
    }
}
