namespace Terka.TinyFonts.TFConvert
{
    using System;

    /// <summary>
    /// Defines required dependencies for commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RequiresCommandAttribute : Attribute
    {
        private readonly Type[] commandType;

        /// <summary>
        /// Creates new instance of attribute.
        /// </summary>
        /// <param name="command">Required commands.</param>
        public RequiresCommandAttribute(params Type[] command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (!Array.TrueForAll<Type>(
                command,
                delegate(Type t)
                {
                    return t.IsSubclassOf(typeof(TinyCommandBase));
                }))
            {
                throw new ArgumentException();
            }

            this.commandType = command;
        }

        /// <summary>
        /// Gets or sets if command is required immediately after current command.
        /// </summary>
        public bool Immediately { get; set; }
        
        /// <summary>
        /// Gets or sets if command is required before current command.
        /// </summary>
        public bool Before { get; set; }

        /// <summary>
        /// Gets or sets if command is required after current command.
        /// </summary>
        public bool After { get; set; }
    }
}
