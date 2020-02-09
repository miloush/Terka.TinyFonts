namespace Terka.FontBuilder.Compiler.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Terka.FontBuilder.Compiler.Output;

    /// <summary>
    /// Compares two transition paths (as used by <see cref="IStateMachineBuilder.AddPath"/>).
    /// </summary>
    public class PathEqualityComparer : IEqualityComparer<IEnumerable<ITransition>>
    {
        /// <inheritdoc />
        public bool Equals(IEnumerable<ITransition> expected, IEnumerable<ITransition> actual)
        {
            var xlist = expected.ToList();
            var ylist = actual.ToList();

            if (xlist.Count != ylist.Count)
            {
                return false;
            }

            var transitionComparer = new TransitionNonrecursiveEqualityComparer();

            foreach (var pair in xlist.Zip(ylist, (xp, yp) => new { Expected = xp, Actual = yp }))
            {
                if (!transitionComparer.Equals(pair.Expected, pair.Actual))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public int GetHashCode(IEnumerable<ITransition> obj)
        {
            throw new NotImplementedException();
        }
    }
}