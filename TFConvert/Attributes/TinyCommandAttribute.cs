namespace Terka.TinyFonts.TFConvert
{
    using System;

    /// <summary>
    /// Attribute for TinyFont commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TinyCommandAttribute : Attribute
    {
        /// <summary>
        /// Creates new instance.
        /// </summary>
        public TinyCommandAttribute() 
        {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="commandString">Command.</param>
        public TinyCommandAttribute(string commandString)
        {
            this.CommandString = commandString;
        }

        /// <summary>
        /// Gets or sets if command is global.
        /// </summary>
        public bool IsGlobal { get; set; }

        /// <summary>
        /// Gets or sets if command is mandatory.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public string CommandString { get; set; }
    }
}
