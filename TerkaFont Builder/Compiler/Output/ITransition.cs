namespace Terka.FontBuilder.Compiler.Output
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents transition from one <see cref="State"/> to another. The state holding this object is the "from" state.
    /// This transition can occur only if the <see cref="IsGlyphIdMatching">transition codition</see> is met.
    /// </summary>
    public interface ITransition
    {
        /// <summary>
        /// Gets or sets the the target state.
        /// </summary>
        /// <value>
        /// The target state.
        /// </value>
        State TargetState { get; set; }

        /// <summary>
        /// Gets a value indicating whether this transition is a fallback transition.
        /// </summary>
        /// <value>
        /// <c>true</c> if this transition is fallback; otherwise, <c>false</c>.
        /// </value>
        bool IsFallback { get; }

        /// <summary>
        /// Gets the value which determines how this transition is ordered in a collection of transitions.
        /// </summary>
        /// <value>
        /// The ordering key.
        /// </value>
        int SortingKey { get; }

        /// <inheritdoc />
        int HeadShift { get; set; }

        /// <inheritdoc />
        LookupFlags LookupFlags { get; set; }

        /// <summary>
        /// Gets or sets action associated with this transition.
        /// </summary>
        ITransitionAction Action { get; set; }

        /// <summary>
        /// Determines whether the <see cref="StateMachine" /> can transition to <see cref="TargetState" /> if the head is
        /// currently pointing to a <see cref="Glyph" /> with ID <paramref name="glyphId"/>.
        /// </summary>
        /// <param name="glyphId">The glyph ID to check.</param>
        /// <returns>
        ///   <c>true</c> if glyph matches the transition; otherwise, <c>false</c>.
        /// </returns>
        bool IsGlyphIdMatching(ushort glyphId);

        /// <summary>
        /// Creates a deep copy of this instance. The target state is not cloned (it will be null in the new copy).
        /// </summary>
        /// <returns>Deep copy of the instance.</returns>
        ITransition Clone();

        /// <summary>
        /// Gets a collection of individual transitions represented by this transition.
        /// </summary>
        /// <returns>A collection of individual transitions. Always returns new instances.</returns>
        IEnumerable<ITransition> GetUngroupedTransitions();
    }
}