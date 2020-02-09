namespace Terka.TinyFonts.TFConvert
{
    /// <summary>
    /// Adjusts the right margin of a range of characters.
    /// </summary>
    /// <remarks>
    /// By default, the margins applied to each character are set to 0. 
    /// If the left margin is decreased to less than 0,
    /// the character may shift to the right far enough to overlap with the next character.
    /// </remarks>
    [TinyCommand]
    public class AdjustRightMargin : TinyAdjustmentCommandBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="AdjustRightMargin"/> command.
        /// </summary>
        public AdjustRightMargin()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="AdjustRightMargin"/> command.
        /// </summary>
        /// <param name="adjustment">
        /// An integer indicating the number of EM units to add to the right margin.
        /// A positive number increases the right margin on each glyph in the next <see cref="ImportRange"/>,
        /// and a negative number decreases it.
        /// </param>
        public AdjustRightMargin(short adjustment) : base(adjustment)
        {
        }
    }
}
