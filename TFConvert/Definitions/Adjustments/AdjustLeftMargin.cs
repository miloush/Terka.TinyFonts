namespace Terka.TinyFonts.TFConvert
{
    /// <summary>
    /// Adjusts the left margin of a range of characters.
    /// </summary>
    /// <remarks>
    /// By default, the margins applied to each character are set to 0. 
    /// If the left margin is decreased to less than 0,
    /// the character may shift to the left far enough to overlap with the previous character.
    /// </remarks>
    [TinyCommand]
    public class AdjustLeftMargin : TinyAdjustmentCommandBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="AdjustLeftMargin"/> command.
        /// </summary>
        public AdjustLeftMargin()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="AdjustLeftMargin"/> command.
        /// </summary>
        /// <param name="adjustment">
        /// An integer indicating the number of device units to add to the left margin.
        /// A positive number increases the left margin on each glyph in the next <see cref="ImportRange"/>,
        /// and a negative number decreases it.
        /// </param>
        public AdjustLeftMargin(short adjustment) : base(adjustment) 
        {
        }        
    }
}
