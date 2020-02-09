using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace Terka.TinyFonts.TFConvert
{
    class TFConvertConsole : TFConvert
    {
        private static CommandLine Line;

        private static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            try { Line = CommandLineHelper.Parse<CommandLine>(args); }
            catch { Line = new CommandLine { ShowHelp = true }; }

            if (Line != null)
            {
                if (Line.ShowHelp || Line.IsValid == false)
                {
                    Line.WriteHelp(Console.Out);
                }
                else
                {
                    TinyDefinition definition = new TinyDefinition();
                    TextReader definitionReader;

                    try
                    {
                        // separate try catch block for file issues for compatibility reasons
                        definitionReader = new StreamReader(Line.InputFile, Encoding.UTF8, true);
                    }
                    catch
                    {
                        Console.WriteLine("Cannot open '{0}'!", Line.InputFile);
                        return;
                    }

                    try
                    {
                        definition.Load(definitionReader);
                    }
                    catch (Exception e)
                    {
                        // the TFConvert exception messages correspond to the native error strings
                        Console.WriteLine(e.Message);
                        return;
                    }

                    try
                    {
                        definition.Validate();
                    }
                    catch (KeyNotFoundException)
                    {
                        // currently the only required command
                        Console.WriteLine("SelectFont command not found in .FNTDEF file");
                        return;
                    }

                    definitionReader.Close();

                    TinyFont font = TFConvert.Convert(definition);
                    DumpFont(definition, font, Console.Out);

                    try
                    {
                        font.Save(Line.OutputFile);
                    }
                    catch
                    {
                        Console.WriteLine("Cannot open '{0}' for writing!", Line.OutputFile);
                        return;
                    }
                }
            }
        }

        private static void DumpFont(TinyDefinition definition, TinyFont font, TextWriter textWriter)
        {
            
        }
    }
}
