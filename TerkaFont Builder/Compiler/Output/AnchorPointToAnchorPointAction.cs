namespace Terka.FontBuilder.Compiler.Output
{
    public class AnchorPointToAnchorPointAction : ITransitionAction
    {
        /// <summary>
        /// Gets or sets the anchor point which is being attached to the previous glyph.
        /// </summary>
        /// <value>
        /// The current glyph anchor point.
        /// </value>
        public AnchorPoint CurrentGlyphAnchorPoint { get; set; }

        /// <summary>
        /// Gets or sets the previous glyph's anchor point.
        /// </summary>
        /// <value>
        /// The target anchor point.
        /// </value>
        public AnchorPoint PreviousGlyphAnchorPoint { get; set; }
        
        /// <inheritdoc />
        public ITransitionAction Clone()
        {
            return new AnchorPointToAnchorPointAction
            {
                CurrentGlyphAnchorPoint = this.CurrentGlyphAnchorPoint,
                PreviousGlyphAnchorPoint = this.PreviousGlyphAnchorPoint
            };
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCodeBuilder.BuildHashCode(
                71,
                this.GetType(),
                this.CurrentGlyphAnchorPoint,
                this.PreviousGlyphAnchorPoint
            );
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "AnchorPointToAnchorPoint, Previous: " + this.PreviousGlyphAnchorPoint + ", Current: " + this.CurrentGlyphAnchorPoint;
        }
    }
}
