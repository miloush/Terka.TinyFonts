namespace Terka.FontBuilder.Compiler
{
    using System.Collections.Generic;
    using Terka.FontBuilder.Compiler.Output;
    using Terka.FontBuilder.Parser.Output.Substitution;

    /// <summary>
    /// Direction from which the state machine enters the string being processed.
    /// </summary>
    public enum ProcessingDirection
    {
        /// <summary>
        /// The machine will start with its head pointing to the first glyph. This is the default direction.
        /// </summary>
        StartToEnd,

        /// <summary>
        /// The machine will start with its head pointing to the last glyph. This is only used by <see cref="ReverseChainingContextSubstitutionTable"/>.
        /// </summary>
        EndToStart,
    }

    /// <summary>
    /// Interface for <see cref="StateMachineBuilder"/>.
    /// </summary>
    public interface IStateMachineBuilder
    {
        /// <summary>
        /// Adds a linear path of states and transitions into the machine.
        /// </summary>
        /// <param name="path">The path. The transitions in the path are treated as connected (<see cref="State.Transitions" /> property is not considered in any way).</param>
        void AddPath(IEnumerable<ITransition> path);

        /// <summary>
        /// Sets the processing direction. If this is not called, the default direction is <see cref="ProcessingDirection.StartToEnd"/>. 
        /// Processing direction can only be changed before any paths are added to the machine (subsequent calls must not change the direction).
        /// </summary>
        /// <param name="direction">The direction.</param>
        void SetProcessingDirection(ProcessingDirection direction);
    }
}