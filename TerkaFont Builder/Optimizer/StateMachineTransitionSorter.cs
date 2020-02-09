using System.Collections.Generic;
using System.Linq;

namespace Terka.FontBuilder.Optimizer
{
    using Terka.FontBuilder.Compiler.Output;

    /// <summary>
    /// Orders transitions so transitions in each state are ordered by their <see cref="ITransition.SortingKey"/>.
    /// </summary>
    internal class StateMachineTransitionSorter
    {
        /// <summary>
        /// Orders transitions so transitions in each state are ordered by their <see cref="ITransition.SortingKey"/>.
        /// </summary>
        /// <param name="inputMachine">The input machine. This state machine will not be changed (a copy of all involved objects is constructed).</param>
        /// <returns>The machine with sorting transitions.</returns>
        public StateMachine SortTransitions(StateMachine inputMachine)
        {
            var queue = new Queue<State>();
            queue.Enqueue(inputMachine.EntryState);

            var visitedOrQueuedStates = new HashSet<State> { inputMachine.EntryState };
            var oldToNewMap = new Dictionary<State, State>();

            while (queue.Any())
            {
                var currentState = queue.Dequeue();

                State newState;
                if (oldToNewMap.ContainsKey(currentState))
                {
                    newState = oldToNewMap[currentState];
                }
                else
                {
                    newState = new State();
                    oldToNewMap.Add(currentState, newState);
                }

                var newTransitions = new List<ITransition>();

                foreach (var transition in currentState.Transitions)
                {
                    var oldTargetState = transition.TargetState;
                    State newTargetState;
                    if (oldToNewMap.ContainsKey(oldTargetState))
                    {
                        newTargetState = oldToNewMap[oldTargetState];
                    }
                    else
                    {
                        newTargetState = new State();
                        oldToNewMap[oldTargetState] = newTargetState;
                    }

                    var newTransition = transition.Clone();
                    newTransition.TargetState = newTargetState;
                    newTransitions.Add(newTransition);

                    if (!visitedOrQueuedStates.Contains(oldTargetState))
                    {
                        queue.Enqueue(oldTargetState);
                        visitedOrQueuedStates.Add(oldTargetState);
                    }
                }

                newState.Transitions = newTransitions.OrderBy(p => p.SortingKey).ToList();
            }

            return new StateMachine(oldToNewMap[inputMachine.EntryState]);
        }
    }
}
