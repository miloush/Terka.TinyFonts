namespace Terka.TinyFonts.TFConvert
{
    using System;
    using System.Text;

    /// <summary>
    /// Used for serialization into TFConvert script.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class TinyParameterAttribute : Attribute
    {
        private readonly int position;
        private char[] charactersToEscape = new char[] { '\\' };

        /// <summary>
        /// Creates new instance.
        /// </summary>
        public TinyParameterAttribute()
        {
            this.position = 0;
        }

        /// <summary>
        /// Creates new instance and sets parameter's position.
        /// </summary>
        /// <param name="position">Parameter's position.</param>
        public TinyParameterAttribute(int position)
        {
            this.position = position;
        }

        /// <summary>
        /// Gets parameter's position.
        /// </summary>
        public int Position
        {
            get { return this.position; }
        }
        
        /// <summary>
        /// Gets or sets charaters that have to be escaped.
        /// </summary>
        public char[] CharactersToEscape
        {
            get { return this.charactersToEscape; }
            set { this.charactersToEscape = value; }
        }

        /// <summary>
        /// Gets or sets if parameter requires quotes.
        /// </summary>
        public bool RequiresQuotes { get; set; }
        
        /// <summary>
        /// Gets or sets if parameter requres escapement.
        /// </summary>
        public bool RequiresEscapement { get; set; }

        /// <summary>
        /// Formats parameter to valid TFConvert value.
        /// </summary>
        /// <param name="parameter">Parameter to format.</param>
        /// <returns>Formatted string representation of parameter.</returns>
        public string FormatParameter(object parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            return this.EscapeParameter(parameter.ToString());
        }

        /// <summary>
        /// Returns escaped parameter.
        /// </summary>
        /// <param name="parameter">Parameter to escape.</param>
        /// <returns>Escaped parameter.</returns>
        public string EscapeParameter(string parameter)
        {
            StringBuilder output = new StringBuilder(parameter);

            if (this.RequiresEscapement)
            {
                foreach (char ch in this.charactersToEscape)
                {
                    output = output.Replace(ch.ToString(), "\\" + ch.ToString());
                }
            }

            if (this.RequiresQuotes)
            {
                output.Insert(0, '"');
                output.Append('"');
            }

            return output.ToString();
        }

        /// <summary>
        /// Removes escaping from parameter.
        /// </summary>
        /// <param name="parameter">Espacaped parameter.</param>
        /// <returns>Clean parameter.</returns>
        public string UnescapeParameter(string parameter)
        {
            StringBuilder output = new StringBuilder(parameter);

            if (this.RequiresEscapement)
            {
                foreach (char ch in this.charactersToEscape)
                {
                    output = output.Replace("\\" + ch.ToString(), ch.ToString());
                }
            }

            if (this.RequiresQuotes)
            {
                if (output.Length > 0 && output[0] == '"')
                {
                    output.Remove(0, 1);
                }

                if (output.Length > 0 && output[output.Length - 1] == '"')
                {
                    output.Remove(output.Length - 1, 1);
                }
            }

            return output.ToString();
        }
    }
}
