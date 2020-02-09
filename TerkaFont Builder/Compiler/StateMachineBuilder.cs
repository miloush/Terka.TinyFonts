namespace Terka.FontBuilder.Compiler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Terka.FontBuilder.Compiler.Output;
    using Terka.FontBuilder.Extensions;

    // TODO: Fallback transition and lookup flags?

    /// <summary>
    /// Constructs a state machine from smaller parts.
    /// </summary>
    public class StateMachineBuilder : IStateMachineBuilder
    {
        /// <summary>
        /// Entry state of the state machine being constructed. This is actually enough to represent entire state machine (full lists of transitions and
        /// states are only built when the final <see cref="StateMachine"/> object is being assembled).
        /// </summary>
        private readonly State entryState = new State();

        /// <summary>
        /// The processing direction for the machine being built.
        /// </summary>
        private ProcessingDirection processingDirection = ProcessingDirection.StartToEnd;

        /// <inheritdoc />
        public StateMachine GetStateMachine()
        {            
            return new StateMachine(this.entryState, processingDirection);
        }

        /// <inheritdoc />
        public void AddPath(IEnumerable<ITransition> path)
        {
            foreach (var ungroupedPath in this.UngroupPath(path))
            {
                this.AddUngroupedPath(ungroupedPath);
            }
        }

        /// <summary>
        /// Adds an ungrouped path to the machine.
        /// </summary>
        /// <param name="path">The path.</param>
        public void AddUngroupedPath(IEnumerable<ITransition> path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            
            var pathList = path.ToList();

            if (!pathList.Any())
            {
                return;
            }
            
            var transitionComparer = new TransitionNonrecursiveEqualityComparer();

            /* Now go from the entry state and step-by-step merge the path into the accumulated state machine. 
             * Each step of the path will first check, if it already exists in the state machine and then either
             * leave the transition in the machine (if it already exists in there) or add a new transition
             * (if it didn't exist). */
            var stateMachineStates = new State[pathList.Count];
            State currentState = this.entryState;
            int i = 0;
            foreach (var step in pathList)
            {
                /* List of state which are projection of the path into the machine must be kept for the case,
                 * when there are back-transitions in the path. */
                stateMachineStates[i] = currentState;

                var addedTransition = step.Clone();
                addedTransition.TargetState = new State();
                
                var existingTransition = currentState.Transitions.FirstOrDefault(p => p.SortingKey == addedTransition.SortingKey);

                if (existingTransition == null)
                {
                    /* The path states may contain transitions, which lead several path steps back. The states are not allowed to contain
                     * any other explicit transitions (except those definedby the path itself). */
                    var backTransition = step.TargetState != null ? step.TargetState.Transitions.SingleOrDefault() : null;
                    if (backTransition != null)
                    {
                        /* Such path ALWAYS leads back to the same path, to the states which were already generated into the resulting
                         * state machine. To connect the owner state to the target state, the corresponding state in the generated
                         * state machine must be found. */
                        var backTransitionTargetIndex = pathList.FindIndex(s => object.ReferenceEquals(s.TargetState, backTransition.TargetState));

                        if (backTransitionTargetIndex == -1)
                        {
                            /* The transition target isn't present in the already visited path of the transition. */
                            throw new InvalidPathException("Off-path transitions must always head back to a state which is within the same path and closer to the entry state.");
                        }
                        if (!backTransition.IsFallback)
                        {
                            throw new InvalidPathException("Path back-transitions must be fallback transitions.");
                        }

                        /* Add the transition to the generated state machine. */
                        var backtransitionClone = backTransition.Clone();
                        backtransitionClone.TargetState = backTransitionTargetIndex == i ?
                            addedTransition.TargetState :
                            stateMachineStates[backTransitionTargetIndex + 1];
                        addedTransition.TargetState.Transitions.Add(backtransitionClone);
                    }
                    
                    currentState.Transitions.Add(addedTransition);

                    currentState = addedTransition.TargetState;
                }                
                else if (!transitionComparer.Equals(step, existingTransition))
                {
                    // Transition ambiguity!
                    // TODO: Add trace warning here?
                    return;
                }
                else
                {                    
                    currentState = existingTransition.TargetState;
                }

                i++;
            }
        }

        public IEnumerable<IEnumerable<ITransition>> UngroupPath(IEnumerable<ITransition> path)
        {
            // Path ungrouping might be needed for rare cases, but increases memory and time requirements significantly.

#if UNGROUP_PATHS
            var pathList = path.ToList();
            
            var queue = new Queue<Tuple<ITransition, int, List<ITransition>>>();
            queue.Enqueue(Tuple.Create(pathList[0], 0, new List<ITransition>()));
            while (queue.Count > 0)
            {
                var currentItem = queue.Dequeue();
                
                var currentLevel = currentItem.Item2;
                var currentTransition = pathList[currentLevel];
                var currentPath = currentItem.Item3;

                State targetState = null;
                if (currentTransition.TargetState != null)
                {
                    targetState = new State();

                    var backTransition = currentTransition.TargetState.Transitions.SingleOrDefault();
                    if (backTransition != null)
                    {
                        /* Such path ALWAYS leads back to the same path, to the states which were already generated into the resulting
                         * state machine. To connect the owner state to the target state, the corresponding state in the generated
                         * state machine must be found. */
                        var backTransitionTargetIndex = pathList.FindIndex(s => object.ReferenceEquals(s.TargetState, backTransition.TargetState));

                        if (backTransitionTargetIndex == -1)
                        {
                            /* The transition target isn't present in the already visited path of the transition. */
                            throw new InvalidPathException("Off-path transitions must always head back to a state which is within the same path and closer to the entry state.");
                        }
                        if (!backTransition.IsFallback)
                        {
                            throw new InvalidPathException("Path back-transitions must be fallback transitions.");
                        }

                        /* Add the transition to the path. */
                        var backtransitionClone = backTransition.Clone();
                        backtransitionClone.TargetState = backTransitionTargetIndex == currentLevel ?
                            targetState :
                            currentPath[backTransitionTargetIndex].TargetState;

                        targetState.Transitions.Add(backtransitionClone);
                    }
                }

                foreach (var ungroupedTransition in currentTransition.GetUngroupedTransitions())
                {
                    ungroupedTransition.TargetState = targetState;

                    if (pathList.Count == currentLevel + 1)
                    {
                        yield return currentPath.Append(ungroupedTransition);
                    }
                    else
                    {
                        queue.Enqueue(Tuple.Create((ITransition)null, currentLevel + 1, currentPath.Append(ungroupedTransition).ToList()));
                    }
                }
            }
#else
            return new [] { path };
#endif
        }

        /// <inheritdoc />
        public void SetProcessingDirection(ProcessingDirection direction)
        {
            if (direction != this.processingDirection && this.entryState.Transitions.Count > 0)
            {
                throw new InvalidOperationException("Can't change state machine processin direction after any transitions were added to it.");    
            }

            this.processingDirection = direction;
        }
    }
}