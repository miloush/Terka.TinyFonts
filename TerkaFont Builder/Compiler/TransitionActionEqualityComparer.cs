namespace Terka.FontBuilder.Compiler
{
    using System;
    using System.Collections.Generic;

    using Terka.FontBuilder.Compiler.Output;
    using Terka.FontBuilder.Extensions;

    /// <summary>
    /// Compares two states without comparing their transitions.
    /// </summary>
    public class TransitionActionEqualityComparer : IEqualityComparer<ITransitionAction>
    {
        /// <inheritdoc/>
        public bool Equals(ITransitionAction x, ITransitionAction y)
        {
            if (x == null || y == null)
            {
                return x == y;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            if (x is SubstitutionAction && y is SubstitutionAction)
            {
                return
                    (x as SubstitutionAction).ReplacedGlyphCount == (y as SubstitutionAction).ReplacedGlyphCount &&
                    (x as SubstitutionAction).SkippedGlyphCount == (y as SubstitutionAction).SkippedGlyphCount &&
                    (x as SubstitutionAction).ReplacementGlyphIds.ValuesEqual((y as SubstitutionAction).ReplacementGlyphIds);
            }
            else if (x is PositioningAdjustmentAction && y is PositioningAdjustmentAction)
            {
                return (x as PositioningAdjustmentAction).PositionChanges.ValuesEqual(
                           (y as PositioningAdjustmentAction).PositionChanges);
            }
            else if (x is AnchorPointToAnchorPointAction && y is AnchorPointToAnchorPointAction)
            {
                return
                    (x as AnchorPointToAnchorPointAction).CurrentGlyphAnchorPoint == (y as AnchorPointToAnchorPointAction).CurrentGlyphAnchorPoint
                    && (x as AnchorPointToAnchorPointAction).PreviousGlyphAnchorPoint == (y as AnchorPointToAnchorPointAction).PreviousGlyphAnchorPoint;
            }
            else
            {
                throw new ArgumentOutOfRangeException("x");
            }
        }

        /// <inheritdoc/>
        public int GetHashCode(ITransitionAction obj)
        {
            // Each state type has this implemented separately.
            return obj.GetHashCode();
        }
    }
}
