namespace Terka.TinyFonts.TFConvert
{
    /// <summary>
    /// Shifts a character or range of characters right or left.
    /// </summary>
    /// <remarks>
    /// The OffsetX command applies to the characters specified by any
    /// <see cref="ImportRange" />, <see cref="ImportRangeAndMap" />, or <see cref="SetAsDefaultCharacter" />
    /// commands that follow it in the definition and precede any subsequent OffsetX commands in the same definition.
    /// </remarks>
    [TinyCommand]
    public class OffsetX : TinyAdjustmentCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetX"/> command.
        /// </summary>
        public OffsetX()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetX"/> command.
        /// </summary>
        /// <param name="adjustment">
        /// An integer indicating the number of EM units to shift a character left or right.          
        /// A positive number shifts the characters left, and a negative number shifts characters to the right.
        /// </param>
        public OffsetX(short adjustment) : base(adjustment)
        {
        }
    }
}
