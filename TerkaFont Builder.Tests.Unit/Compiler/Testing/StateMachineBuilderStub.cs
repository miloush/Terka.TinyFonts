namespace Terka.FontBuilder.Compiler.Testing
{
    using System;
    using System.Collections.Generic;
    using Terka.FontBuilder.Compiler.Output;

    /// <summary>
    /// Records calls to <see cref="StateMachineBuilder.AddPath"/>.
    /// </summary>
    public class StateMachineBuilderStub : IStateMachineBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineBuilderStub" /> class.
        /// </summary>
        public StateMachineBuilderStub()
        {
            this.Paths = new List<IEnumerable<ITransition>>();
        }

        /// <summary>
        /// Gets or sets the paths.
        /// </summary>
        /// <value>
        /// The paths.
        /// </value>
        public List<IEnumerable<ITransition>> Paths { get; set; }

        public ProcessingDirection ProcessingDirection { get; set; }

        /// <inheritdoc />
        public StateMachine GetStateMachine()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void AddPath(IEnumerable<ITransition> path)
        {
            this.Paths.Add(path);
        }

        public void SetProcessingDirection(ProcessingDirection direction)
        {
            if (direction != this.ProcessingDirection && this.Paths.Count > 0)
            {
                throw new InvalidOperationException();
            }

            this.ProcessingDirection = direction;
        }
    }
}