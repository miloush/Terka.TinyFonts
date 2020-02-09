namespace Terka.FontBuilder.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Terka.FontBuilder.Compiler.Output;

    /// <summary>
    /// Compares two transitions without comparing the states they target.
    /// </summary>
    public class TransitionNonrecursiveEqualityComparer : IEqualityComparer<ITransition>
    {
        private readonly TransitionActionEqualityComparer actionComparer = new TransitionActionEqualityComparer();

        /// <inheritdoc />
        public bool Equals(ITransition x, ITransition y)
        {
            if (x == null || y == null)
            {
                return x == y;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }
            
            if (x.IsFallback != y.IsFallback || x.HeadShift != y.HeadShift || x.LookupFlags != y.LookupFlags)
            {
                return false;                
            }

            if (!this.actionComparer.Equals(x.Action, y.Action))
            {
                return false;
            }

            if (x is SimpleTransition && y is SimpleTransition)
            {
                return (x as SimpleTransition).GlyphId == (y as SimpleTransition).GlyphId;
            }
            else if (x is SetTransition && y is SetTransition)
            {
                // HashSet doesn't guarantee order -> can't use ValuesEqual.
                return
                    (x as SetTransition).GlyphIdSet.All(
                        p => ((SetTransition)y).GlyphIdSet.Contains(p)) &&
                    (y as SetTransition).GlyphIdSet.All(
                        p => ((SetTransition)x).GlyphIdSet.Contains(p));
            }
            else if (x is AlwaysTransition && y is AlwaysTransition)
            {
                return true;
            }
            else
            {
                throw new ArgumentOutOfRangeException("x");
            }
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public int GetHashCode(ITransition obj)
        {
            // Transition objects have hash code generator in them.
            return obj.GetHashCode();
        }
    }
}