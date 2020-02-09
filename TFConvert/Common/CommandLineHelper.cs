namespace Terka
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Base class for application specific command line parsers.
    /// </summary>
    public abstract class CommandLineHelper
    {
        /// <summary>
        /// Parses all arguments.
        /// </summary>
        /// <param name="args">Arguments from application.</param>
        public void Parse(IEnumerable<string> args) 
        {
            bool first = true;
            string sw = null;
            Queue<string> parameters = new Queue<string>();

            foreach (string arg in args)
            {
                if (IsSwitch(arg))
                {
                    if (!first)
                    {
                        while (!ParseArgument(sw, parameters)) ;

                        if (parameters.Count > 0)
                            ParseArgument(null, parameters);
                    }

                    sw = Unescape(arg);
                    parameters.Clear();
                }
                else
                    parameters.Enqueue(Unescape(arg));

                first = false;
            }

            while (!ParseArgument(sw, parameters)) ;

            if (parameters.Count > 0)
                ParseArgument(null, parameters);
        }

        /// <summary>
        /// Parses command line arguments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">Arguments from command line.</param>
        /// <returns>Parsed command line.</returns>
        public static T Parse<T>(IEnumerable<string> args) where T : CommandLineHelper, new()
        {
            T commandLine = new T();
            commandLine.Parse(args);
            return commandLine;
        }

        /// <summary>
        /// Checks if argument is switch - starts with switch character.
        /// </summary>
        /// <param name="arg">Argument to test.</param>
        /// <returns>True if is switch.</returns>
        protected virtual bool IsSwitch(string arg)
        {
            if (arg == null || arg.Length < 2)
                return false;

            int switchCharIndex = Array.IndexOf(SwitchCharacters, arg[0]);
            if (switchCharIndex == -1)
                return false;

            return arg[1] != SwitchCharacters[switchCharIndex];
        }

        /// <summary>
        /// Removes escaping from arguments.
        /// </summary>
        /// <param name="arg">Argument to unescape.</param>
        /// <returns>Argument without escaping.</returns>
        protected virtual string Unescape(string arg)
        {
            if (string.IsNullOrEmpty(arg))
                return arg;

            if (Array.IndexOf(SwitchCharacters, arg[0]) != -1)
                return arg.Substring(1);

            return arg;
        }

        /// <summary>
        /// Slash character.
        /// </summary>
        protected static char[] SwitchSlash = new char[] { '/' };
        /// <summary>
        /// Dash character.
        /// </summary>
        protected static char[] SwitchDash = new char[] { '-' };
        /// <summary>
        /// Slash and dash together.
        /// </summary>
        protected static char[] SwitchSlashOrDash = new char[] { '/', '-' };
        
        /// <summary>
        /// Gets characters for console switch.
        /// </summary>
        protected virtual char[] SwitchCharacters
        {
            get
            {
                return SwitchSlashOrDash;
            }
        }
        /// <summary>
        /// Parses argument from command line.
        /// </summary>
        /// <param name="arg">Argument to parse.</param>
        /// <param name="parameters">Available parameters from commandline.</param>
        /// <returns>True if argument was sucessfully parsed.</returns>
        protected abstract bool ParseArgument(string arg, Queue<string> parameters);

        /// <summary>
        /// Writes help about parameters usage.
        /// </summary>
        /// <param name="writer">Where write output.</param>
        public void WriteHelp(TextWriter writer)
        {
            WriteHelp(writer, Path.GetFileName(System.Environment.GetCommandLineArgs()[0]));
        }
        /// <summary>
        /// Writes help about parameters usage.
        /// </summary>
        /// <param name="writer">Where write output.</param>
        /// <param name="executableName">Executable name of this console application.</param>
        protected virtual void WriteHelp(TextWriter writer, string executableName)
        { 

        }

        /// <summary>
        /// Gets if command line contains valid data.
        /// </summary>
        public virtual bool IsValid
        {
            get { return true; }
        }
    }
}
