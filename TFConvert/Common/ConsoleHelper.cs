namespace Terka
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;

    // This is .NET Framework 4.0 and above only version!
    internal static class ConsoleHelper
    {
        #region Interactive

        public static string InteractiveGetPath(string defaultPath, bool mustExist = true, string pathName = "path")
        {
            Console.WriteLine("Current {1} is {0}.", defaultPath, pathName);
            string path = InteractiveDefaultLine("Enter a new one or leave empty to use the current one:", defaultPath, true);

            while (true)
            {
                if (string.IsNullOrWhiteSpace(path))
                    path = defaultPath;

                path = path.Trim('"');

                if (!mustExist || Directory.Exists(path) || File.Exists(path))
                    break;

                Console.Write("This one does not exist. Enter a new one: ");
                path = Console.ReadLine();
            }

            Console.WriteLine();
            return path;
        }
        public static string InteractiveDefaultLine(string line, string def, bool newLine = false)
        {
            if (newLine) Console.WriteLine(line);
            else Console.Write(line);

            string read = Console.ReadLine();
            return string.IsNullOrWhiteSpace(read) ? def : read;
        }
        public static int InteractiveNumericChoice(string question, params string[] answers)
        {
            global::System.Diagnostics.Debug.Assert(answers.Length <= 10);

            Console.WriteLine(question);

            char minChar = answers.Length >= 10 ? '0' : '1';
            char maxChar = (char)((int)minChar + answers.Length);
            int i = 0;
            foreach (string answer in answers)
                Console.WriteLine(" [{0}] {1}", (++i) % 10, answer);

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.KeyChar >= minChar && key.KeyChar < maxChar)
                {
                    Console.WriteLine();
                    return (int)key.KeyChar - (int)minChar;
                }

                Console.Beep();
            }
        }
        public static bool InteractiveBoolean(string question)
        {
            return InteractiveBoolean(question, "Yes", "No", ConsoleKey.Y, ConsoleKey.N);
        }
        public static bool InteractiveBoolean(string question, string yes, string no, ConsoleKey yesKey, ConsoleKey noKey)
        {
            Console.Write("{0} [{1}/{2}] ", question, yesKey, noKey);
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Y || key.Key == ConsoleKey.Spacebar || key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(yes);
                    return true;
                }
                else if (key.Key == ConsoleKey.N || key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine(no);
                    return false;
                }
                else
                    Console.Beep();
            }
        }
        public static T InteractiveNumericEnum<T>(string question) where T : struct
        {

            SortedList<T, string> names = new SortedList<T, string>();
            foreach (var pair in from name in Enum.GetNames(typeof(T))
                                 group name by (T)Enum.Parse(typeof(T), name) into byNames
                                 select new KeyValuePair<T, string>(byNames.Key, string.Join(" = ", byNames)))
                names.Add(pair.Key, pair.Value);

            int index = InteractiveNumericChoice(question, names.Values.ToArray());
            return names.Keys[index];
        }
        public static T InteractiveConvertible<T>(string question, T @default)
        {
            return InteractiveConvertible(question, @default, TypeDescriptor.GetConverter(typeof(T)));
        }
        public static T InteractiveConvertible<T>(string question, T @default, TypeConverter customConverter)
        {
            if (!customConverter.CanConvertFrom(typeof(string)))
                throw new ArgumentException("Type '" + typeof(T) + "' is not convertible from string.");

            while (true)
            {
                Console.WriteLine(question);
                string read = Console.ReadLine();

                if (string.IsNullOrEmpty(read))
                    return @default;

                try { return (T)customConverter.ConvertFromString(read); }
                catch (Exception e) { WriteLine(ConsoleColor.Red, e.Message); }
            }
        }

        #endregion

        public static void Write(ConsoleColor color, string format, params object[] args)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.Write(format, args);

            Console.ForegroundColor = currentColor;
        }
        public static void WriteLine(ConsoleColor color, string format, params object[] args)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.WriteLine(format, args);

            Console.ForegroundColor = currentColor;
        }
    }
}
