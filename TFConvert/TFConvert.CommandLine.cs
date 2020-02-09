using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Terka.TinyFonts.TFConvert
{
    partial class TFConvert
    {
        /// <summary>
        /// Command line 
        /// </summary>
        public class CommandLine : CommandLineHelper
        {
            /// <summary>
            /// Path to Font definition file.
            /// </summary>
            public string InputFile;
            /// <summary>
            /// Path to Font output file.
            /// </summary>
            public string OutputFile;

            /// <summary>
            /// Shows application's usage description.
            /// </summary>
            public bool ShowHelp;

            /// <summary>
            /// Parses paths from command line.
            /// </summary>
            /// <param name="arg">Not used in this TFConvert console.</param>
            /// <param name="parameters">Parsed parameters from commandline.</param>
            /// <returns>True if argument was sucessfully parsed.</returns>
            protected override bool ParseArgument(string arg, Queue<string> parameters)
            {
                if (string.IsNullOrEmpty(arg))
                {
                    this.InputFile = parameters.Dequeue();
                    this.OutputFile = parameters.Dequeue();

                    return true;
                }

                throw new ArgumentException(string.Format("Invalid argument '{0}'.", arg), "arg");
            }

            /// <summary>
            /// Gets if all required parameters are correct.
            /// </summary>
            public override bool IsValid
            {
                get
                {
                    return !string.IsNullOrEmpty(this.InputFile) && !string.IsNullOrEmpty(this.OutputFile);
                }
            }

            /// <summary>
            /// Writes description how this console should be used.
            /// </summary>
            /// <param name="writer">Writer to write the output.</param>
            /// <param name="executableName">Name of the executable file of this console app.</param>
            protected override void WriteHelp(TextWriter writer, string executableName)
            {
                if (string.IsNullOrEmpty(this.InputFile))
                {
                    writer.WriteLine("Missing parameter for option '<input file>'");
                }
                else if (string.IsNullOrEmpty(this.OutputFile))
                {
                    writer.WriteLine("Missing parameter for option '<output file>'");
                }

                writer.WriteLine(@"
TFConvert - .TTF to .TinyFNT conversion tool

Converts a TrueType font into .tinyfnt file for the .NET Micro Framework.

Syntax:

    {0} <input file> <output file>
      <input file>  = Font definition file (.fntdef)
      <output file> = Font output file (.tinyfnt)
", executableName);
            }
        }
    }
}
