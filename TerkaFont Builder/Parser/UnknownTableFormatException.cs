namespace Terka.FontBuilder.Parser
{
    using System;

    /// <summary>
    /// Thrown when an unhandled subtable format is encountered.
    /// </summary>
    public class UnknownTableFormatException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownTableFormatException" /> class.
        /// </summary>
        /// <param name="tableType">Type of the table.</param>
        /// <param name="format">The format.</param>
        public UnknownTableFormatException(Type tableType, ushort format) : base("Unknown " + tableType.Name + " format " + format + ".")
        {
        }
    }
}
