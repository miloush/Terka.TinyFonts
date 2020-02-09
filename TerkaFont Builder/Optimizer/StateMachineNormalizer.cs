namespace Terka.FontBuilder.Optimizer
{
    using System.Collections.Generic;
    using System.Linq;

    using Terka.FontBuilder.Compiler;
    using Terka.FontBuilder.Compiler.Output;

    /// <summary>
    /// Converts a state machine into a normal form, where all transitions are listed in specific order and all transitions which can be grouped together are grouped.
    /// </summary>
    public class StateMachineNormalizer
    {
        // TODO: Stavet seznam setu.

        /// <summary>
        /// Adds the transition to the transition index if a transition equivalent to it is not there yet. If it is, it uses the already existing transition instead. 
        /// </summary>
        /// <param name="transitionsByTargetState">The transition index (indexed by the target state).</param>
        /// <param name="key">The target state.</param>
        /// <param name="value">The new transition.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>The new transition if it wasn't in the index yet, the existing tranwsition otherwise.</returns>
        private ITransition AddTransitionOrGetExisting(Dictionary<State, HashSet<ITransition>> transitionsByTargetState, State key, ITransition value, TransitionNonrecursiveEqualityComparer comparer)
        {
            if (!transitionsByTargetState.ContainsKey(key))
            {
                transitionsByTargetState.Add(key, new HashSet<ITransition>(comparer) { value });
                return value;
            }

            var set = transitionsByTargetState[key];
            if (set.Contains(value))
            {
                return set.Single(p => comparer.Equals(p, value));
            }

            set.Add(value);
            return value;            
        }

        /// <summary>
        /// Normalizes the specified input machine.
        /// </summary>
        /// <param name="inputMachine">The input machine. This state machine will not be changed (a copy of all involved objects is constructed).</param>
        /// <returns>
        /// The normalized machine.
        /// </returns>
        public StateMachine Normalize(StateMachine inputMachine)
        {
            return this.Normalize(inputMachine, null);
        }

        /// <summary>
        /// Normalizes the specified input machine.
        /// </summary>
        /// <param name="inputMachine">The input machine. This state machine will not be changed (a copy of all involved objects is constructed).</param>
        /// <param name="allowedGlyphIds">The set of glyph IDs, which are allowed to have transitions in the machine. If null, all glyphs are allowed.</param>
        /// <returns>
        /// The normalized machine.
        /// </returns>
        public StateMachine Normalize(StateMachine inputMachine, IEnumerable<ushort> allowedGlyphIds)
        {
            var allowedGlyphIdsSet = 
                allowedGlyphIds != null ? 
                    new HashSet<ushort>(allowedGlyphIds) : 
                    new HashSet<ushort>(Enumerable.Range(ushort.MinValue, ushort.MaxValue - ushort.MinValue + 1).Select(p => (ushort)p));

            var queue = new Queue<State>();
            queue.Enqueue(inputMachine.EntryState);

            var visitedOrQueuedStates = new HashSet<State> { inputMachine.EntryState };
            var oldToNewMap = new Dictionary<State, State>();

            var transitionsByTargetState = new Dictionary<State, HashSet<ITransition>>();
            var transitionComparer = new TransitionNonrecursiveEqualityComparer();

            var actionComparer = new TransitionActionEqualityComparer();

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

                var groupedTransitions = currentState.Transitions.GroupBy(p => p.TargetState);
                foreach (var transitionGroup in groupedTransitions)
                {
                    var glyphIdsToMerge = new List<ushort>();

                    // Transitions that can't be merged (fallback and always transitions, or transitions that don't match others) 
                    // have to be cloned as they are.
                    var transitionsToClone = new List<ITransition>(); 

                    var oldTargetState = transitionGroup.First().TargetState;
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

                    ITransition lastMergedTransition = null;
                    foreach (var transition in transitionGroup)
                    {
                        if (transition is SimpleTransition || transition is SetTransition)
                        {
                            // Only transitions which share all attributes but target and transition number can be merged.
                            if (
                                lastMergedTransition == null || 
                                (
                                    transition.IsFallback == lastMergedTransition.IsFallback &&
                                    transition.HeadShift == lastMergedTransition.HeadShift &&
                                    transition.LookupFlags == lastMergedTransition.LookupFlags && 
                                    actionComparer.Equals(transition.Action, lastMergedTransition.Action)
                                )
                            )
                            {
                                if (transition is SimpleTransition) 
                                {
                                    glyphIdsToMerge.Add((transition as SimpleTransition).GlyphId);                                    
                                }
                                else
                                {
                                    glyphIdsToMerge.AddRange((transition as SetTransition).GlyphIdSet);                                    
                                }

                                lastMergedTransition = transition;
                            }
                            else 
                            {
                                transitionsToClone.Add(transition);
                            }
                        }
                        else
                        {
                            transitionsToClone.Add(transition);
                        }
                    }

                    var filteredGlyphIdsToMerge = new HashSet<ushort>(glyphIdsToMerge);
                    filteredGlyphIdsToMerge.IntersectWith(allowedGlyphIdsSet);

                    var newTransitions = new List<ITransition>();

                    foreach (var transition in transitionsToClone)
                    {
                        var newTransition = transition.Clone();
                        newTransition.TargetState = newTargetState;
                        newTransition = this.AddTransitionOrGetExisting(transitionsByTargetState, newTargetState, newTransition, transitionComparer);

                        if (transition is SimpleTransition)
                        {
                            if (allowedGlyphIdsSet.Contains(((SimpleTransition)transition).GlyphId))
                            {
                                newTransitions.Add(newTransition);   
                            }                            
                        }
                        else if (transition is SetTransition)
                        {
                            ((SetTransition)transition).GlyphIdSet.IntersectWith(allowedGlyphIdsSet);

                            if (((SetTransition)transition).GlyphIdSet.Count > 0)
                            {
                                newTransitions.Add(newTransition);
                            }
                        }
                        else
                        {
                            newTransitions.Add(newTransition);
                        }
                    }

                    // If there are no glyphs remaining in the set, no transition will be added.
                    if (filteredGlyphIdsToMerge.Count == 1)
                    {
                        var newTransition = (ITransition)new SimpleTransition {
                            GlyphId = filteredGlyphIdsToMerge.Single(),
                            HeadShift = lastMergedTransition.HeadShift,
                            LookupFlags = lastMergedTransition.LookupFlags,
                            Action = lastMergedTransition.Action == null ? null : lastMergedTransition.Action.Clone(),
                            TargetState = newTargetState
                        };
                        newTransition = this.AddTransitionOrGetExisting(transitionsByTargetState, newTargetState, newTransition, transitionComparer);
                        newTransitions.Add(newTransition);                        
                    }
                    else if (filteredGlyphIdsToMerge.Count > 1)
                    {
                        var newTransition = (ITransition)new SetTransition
                        {
                            GlyphIdSet = filteredGlyphIdsToMerge,
                            HeadShift = lastMergedTransition.HeadShift,
                            LookupFlags = lastMergedTransition.LookupFlags,
                            Action = lastMergedTransition.Action == null ? null : lastMergedTransition.Action.Clone(),
                            TargetState = newTargetState
                        };
                        newTransition = this.AddTransitionOrGetExisting(transitionsByTargetState, newTargetState, newTransition, transitionComparer);
                        newTransitions.Add(newTransition);
                    }

                    foreach (var newTransition in newTransitions)
                    {
                        newState.Transitions.Add(newTransition);
                    }                    

                    if (!visitedOrQueuedStates.Contains(oldTargetState))
                    {
                        queue.Enqueue(oldTargetState);
                        visitedOrQueuedStates.Add(oldTargetState);
                    }
                }
            }

            var unsortedMachine = new StateMachine(oldToNewMap[inputMachine.EntryState]);
            
            var sorter = new StateMachineTransitionSorter();
            return sorter.SortTransitions(unsortedMachine);
        }
    }
}
