namespace Terka.FontBuilder.Compiler.Output
{
    using System.Collections.Generic;
    using System.Linq;

    public class PositioningAdjustmentAction : ITransitionAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositioningAdjustmentAction"/> class.
        /// </summary>
        public PositioningAdjustmentAction()
        {
            this.PositionChanges = Enumerable.Empty<GlyphPositionChange>();
        }

        /// <summary>
        /// Gets or sets the position changes.
        /// </summary>
        /// <value>
        /// The position changes.
        /// </value>
        public IEnumerable<GlyphPositionChange> PositionChanges { get; set; }

        /// <inheritdoc />
        public ITransitionAction Clone()
        {
            return new PositioningAdjustmentAction
            {
                PositionChanges = this.PositionChanges.ToList()
            };
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCodeBuilder.BuildHashCode(
                67,
                this.GetType(),
                this.PositionChanges
            );
        }
    }
}
