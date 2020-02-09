namespace Terka.FontBuilder.Compiler
{
    using System;

    /// <summary>
    /// Thrown when a <see cref="IStateMachineBuilder"/> path is found not to have be in the correct format.
    /// </summary>
    public class InvalidPathException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPathException" /> class.
        /// </summary>
        /// <param name="s">The s.</param>
        public InvalidPathException(string s) : base(s)
        {
        }
    }
}