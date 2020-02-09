namespace Terka.FontBuilder.Compiler.Output.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Terka.FontBuilder.Extensions;

    /// <summary>
    /// Compares two state machines for equality.
    /// </summary>
    public class StateMachineEqualityComparer : IEqualityComparer<StateMachine>
    {
        /// <inheritdoc />
        public bool Equals(StateMachine x, StateMachine y)
        {
            var transitionComparer = new TransitionNonrecursiveEqualityComparer();

            var visitedStatesX = new List<State>();

            var queueX = new Queue<State>();
            var queueY = new Queue<State>();
            queueX.Enqueue(x.EntryState);
            queueY.Enqueue(y.EntryState);
            while (queueX.Any() && queueY.Any())
            {
                var currentStateX = queueX.Dequeue();
                var currentStateY = queueY.Dequeue();

                if (visitedStatesX.Contains(currentStateX))
                {
                    continue;
                }

                visitedStatesX.Add(currentStateX);

                if (currentStateX.Transitions.Count != currentStateY.Transitions.Count)
                {
                    return false;
                }

                var zip = currentStateX.Transitions.Zip(currentStateY.Transitions);
                foreach (var tuple in zip)
                {
                    if (!transitionComparer.Equals(tuple.Item1, tuple.Item2))
                    {
                        return false;
                    }

                    queueX.Enqueue(tuple.Item1.TargetState);
                    queueY.Enqueue(tuple.Item2.TargetState);
                }
            }

            if (queueX.Any() != queueY.Any())
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public int GetHashCode(StateMachine obj)
        {
            throw new NotImplementedException();
        }
    }
}