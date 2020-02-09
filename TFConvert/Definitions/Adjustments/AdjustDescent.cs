namespace Terka.TinyFonts.TFConvert
{
    /// <summary>
    /// Adjusts the descent of the font being created.
    /// Only one AdjustDescent command is allowed per definition.
    /// </summary>
    /// <remarks>
    /// The descent cannot be decreased to less than 0. For example, if the original font
    /// has an ascent of 3, and adjustment is -4, TFConvert will fail.
    /// </remarks>
    [TinyCommand(IsGlobal = true)]
    public class AdjustDescent : TinyAdjustmentCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdjustDescent"/> command.
        /// </summary>
        public AdjustDescent()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdjustDescent"/> command.
        /// </summary>
        /// <param name="adjustment">
        /// An integer indicating the number of EM units to adjust the descent.
        /// A positive number increases the descent, and a negative number decreases it.
        /// </param>
        public AdjustDescent(short adjustment) : base(adjustment) 
        {
        }
    }
}
