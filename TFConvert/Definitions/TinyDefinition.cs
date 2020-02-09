namespace Terka.TinyFonts.TFConvert
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Tiny Definition class.
    /// </summary>
    public class TinyDefinition : List<TinyCommandBase>
    {
        private static readonly Type CommandBaseType = typeof(TinyCommandBase);

        private static Dictionary<string, Type> allCommandsByName = new Dictionary<string, Type>();
        private static Dictionary<Type, bool> allCommandsByType = new Dictionary<Type, bool>();

        static TinyDefinition()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(CommandBaseType))
                {
                    object[] attributes = type.GetCustomAttributes(typeof(TinyCommandAttribute), false);
                    if (attributes.Length > 0)
                    {
                        TinyCommandAttribute attribute = (TinyCommandAttribute)attributes[0];

                        string commandString = attribute.CommandString ?? type.Name;
                        bool isRequired = attribute.IsRequired;

                        allCommandsByName[commandString] = type;
                        allCommandsByType[type] = isRequired;
                    }
                }
            }
        }

        /// <summary>
        /// Creates new instance of definition.
        /// </summary>
        public TinyDefinition() 
        {
        }
        
        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="commands">Associated commands.</param>
        public TinyDefinition(IEnumerable<TinyCommandBase> commands) : base(commands) 
        {
        }

        /// <summary>
        /// Gets if TinyDefinition is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                try
                {
                    this.Validate();
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Checks if sequence of commands is valid.
        /// </summary>
        public void Validate()
        {
            foreach (KeyValuePair<Type, bool> command in allCommandsByType)
            {
                if (command.Value)
                {
                    foreach (TinyCommandBase localCommand in this)
                    {
                        if (localCommand != null && localCommand.GetType() == command.Key)
                        {
                            goto nextCommand;
                        }
                    }

                    throw new KeyNotFoundException();
                }

            nextCommand: ;
            }

            // TODO: správné pořadí
        }

        /// <summary>
        /// Loads new TinyDefiniton file and parses parameters
        /// </summary>
        /// <param name="path">Path to load.</param>
        public void Load(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                this.Load(reader);
            }
        }

        /// <summary>
        /// Loads new TinyDefiniton file and parses parameters
        /// </summary>
        /// <param name="reader">TextReader for loading content from file.</param>
        public void Load(TextReader reader)
        {
            while (true)
            {
                string line = reader.ReadLine();

                if (line == null)
                {
                    break;
                }

                if (line.Length == 0 || line[0] == '#')
                {
                    continue;
                }

                string[] tokens = ExtractTokens(line);

                Type commandType = null;
                if (!TinyDefinition.allCommandsByName.TryGetValue(tokens[0], out commandType))
                {
                    throw new FileFormatException(string.Format("Unknown option: {0}", tokens[0]));
                }

                ConstructorInfo ctor = commandType.GetConstructor(Type.EmptyTypes);
                if (ctor == null)
                {
                    throw new InvalidOperationException(string.Format("Unavailable option: {0}", tokens[0]));
                }

                TinyCommandBase command = (TinyCommandBase)ctor.Invoke(null);
                command.ParseParameters(tokens, 1, tokens.Length - 1);

                this.Add(command);
            }
        }

        private static string[] ExtractTokens(string line)
        {
            Debug.Assert(line != null, "line cannot be null");

            int quoteStart = -1;
            while (true)
            {
                quoteStart = line.IndexOf('"', quoteStart + 1);
                
                if (quoteStart == -1)
                {
                    return line.Split(null as char[], StringSplitOptions.RemoveEmptyEntries);
                }

                if (quoteStart > 0 && char.IsWhiteSpace(line, quoteStart - 1))
                {
                    // quote should not be taken as separator
                    break;
                }
            }

            int quoteEnd = quoteStart;
            while (true)
            {
                quoteEnd = line.IndexOf('"', quoteEnd + 1);

                if (quoteEnd == -1)
                {
                    throw new FileFormatException(string.Format("Unterminated string parameter: {0}", line.Substring(quoteStart)));
                }

                if (line[quoteEnd - 1] != '\\')
                {
                    break;
                }
            }

            string[] beforeQuotes = line.Substring(0, quoteStart).Split(null as char[], StringSplitOptions.RemoveEmptyEntries);
            string quotes = line.Substring(quoteStart, quoteEnd - quoteStart + 1);
            string[] afterQuotes = new string[0];

            if (quoteEnd + 1 < line.Length)
                afterQuotes = ExtractTokens(line.Substring(quoteEnd + 1));

            for (int i = 0; i < beforeQuotes.Length; i++)
                beforeQuotes[i] = Environment.ExpandEnvironmentVariables(beforeQuotes[i]);
            quotes = Environment.ExpandEnvironmentVariables(quotes);

            string[] tokens = new string[beforeQuotes.Length + 1 + afterQuotes.Length];
            beforeQuotes.CopyTo(tokens, 0);
            tokens[beforeQuotes.Length] = quotes;
            afterQuotes.CopyTo(tokens, beforeQuotes.Length + 1);

            return tokens;
        }

        /// <summary>
        /// Parses TinyFont definition.
        /// </summary>
        /// <param name="fntdef">String contaning TinyFont definition file.</param>
        public void Parse(string fntdef)
        {
            using (StringReader reader = new StringReader(fntdef))
            {
                this.Load(reader);
            }
        }

        /// <summary>
        /// Saves the TinyFont definition into text file.
        /// </summary>
        /// <param name="path">Path to file.</param>
        public void Save(string path)
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                this.Save(sw);
            }
        }

        /// <summary>
        /// Saves the TinyFont definition.
        /// </summary>
        /// <param name="writer">TextWriter for save the content.</param>
        public void Save(TextWriter writer)
        {
            for (int i = 0; i < this.Count; i++)
            {
                writer.WriteLine(base[i].ToString());
            }
        }
    }
}
