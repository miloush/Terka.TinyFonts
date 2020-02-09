namespace Terka.TinyFonts.TFConvert
{
    // [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Fields do not corrupt state and distingush computed values.")]

    /// <summary>
    /// Specifies a list of properties that define the TinyFont to create. 
    /// This command is required at least once in a definition.
    /// </summary>
    /// <remarks>
    /// TFConvert searches all TrueType fonts installed on the system,
    /// as well as any fonts specified in an <see cref="AddFontToProcess" /> argument,
    /// to find a font that matches the characteristics specified in the command properties.
    /// The TinyFont file is generated based on the first match. 
    /// A SelectFont statement "selects" the current font that subsequent statements in the TinyFont file refer to.
    /// A SelectFont statement must be followed by an <see cref="ImportRange" /> or <see cref="ImportRangeAndMap" />
    /// statement that specifies the characters to import from the currently selected font. 
    /// If any <see cref="ImportRange" /> or <see cref="ImportRangeAndMap" /> options appear in a definition 
    /// before a font has been selected using a SelectFont statement, TFConvert fails. 
    /// </remarks>
    [RequiresCommand(typeof(ImportRange), typeof(ImportRangeAndMap), After = true)]
    [TinyCommand(IsRequired = true)]
    public class SelectFont : TinyCommandBase
    {
        /// <summary>
        /// The typeface name of the font.
        /// This field should always be specified to ensure consistent results.
        /// The prefix for this property is FN.
        /// </summary>
        public string FaceName;

        /// <summary>
        /// Specifies the weight of the font in the range 0 through 1000.
        /// This value is optional, but should be specified to ensure consistent results.
        /// If this field is not provided, a normal weight is assumed.
        /// The prefix for this property is WE.
        /// </summary>
        public int? Weight;

        /// <summary>
        /// Specifies the height, in logical units, of the font's character cell or character.
        /// The character height value (also known as the em height) is the character cell height
        /// value minus the internal-leading value.
        /// For all height comparisons, the font mapper looks for the largest font that does not exceed
        /// the requested size. 
        /// If this value is greater than 0, the font mapper transforms this value into device units
        /// and matches it against the cell height of the available fonts.
        /// If this value is less than 0, the font mapper transforms this value into device units
        /// and matches its absolute value against the character height of the available fonts.
        /// This value is optional, but should be specified to ensure consistent results.
        /// If not specified, a default height is used. 
        /// The prefix for this property is HE.
        /// </summary>
        public int? Height;

        /// <summary>
        /// Specifies the average width, in logical units, of characters in the font.
        /// The prefix for this property is WI.
        /// </summary>
        public int? Width;

        /// <summary>
        /// Specifies the angle, in tenths of degrees, between the escapement vector and the x-axis of the device.
        /// The escapement vector is parallel to the base line of a row of text.
        /// A positive escapement value will rotate characters counter-clockwise. 
        /// The prefix for this property is ES.
        /// </summary>
        public int? Escapement;

        /// <summary>
        /// Specifies the angle, in tenths of degrees, between each character's base line and the x-axis of the device.
        /// If you provide the Orientation value, you should set it to be the same as the Escapement value. 
        /// The prefix for this property is OR.
        /// </summary>
        public int? Orientation;

        /// <summary>
        /// Specifies an italic font if set to true.
        /// The prefix for this property is IT.
        /// </summary>
        public bool? Italic;

        /// <summary>
        /// Specifies an underlined font if set to true.
        /// The prefix for this property is UN.
        /// </summary>
        public bool? Underline;

        /// <summary>
        /// An integer that specifies the character set.
        /// The character set defaults to ANSI if this field is not provided.
        /// Make sure that the value of the CharacterSet matches the character set of the typeface
        /// specified in the FaceName field. 
        /// The prefix for this property is CS.
        /// </summary>
        public int? CharacterSet;

        /// <summary>
        /// Specifies the output precision.
        /// The prefix for this property is OP.
        /// </summary>
        public int? OutputPrecision;

        /// <summary>
        /// Specifies the clip precision.
        /// The prefix for this property is CP.
        /// </summary>
        public int? ClipPrecision;

        /// <summary>
        /// Specifies the quality.
        /// The prefix for this property is QA.
        /// </summary>
        public int? Quality;

        /// <summary>
        /// Specifies the pitch and family.
        /// The prefix for this property is PF.
        /// </summary>
        public int? PitchAndFamily;

        /// <summary>
        /// A string that specifies the full font name, which may include publisher and version information.
        /// The prefix for this property is FullName.
        /// </summary>
        public string FullName;

        /// <summary>
        /// A string that specifies the character set of the font.
        /// The prefix for this property is Script.
        /// </summary>
        public string Script;

        /// <summary>
        /// A string that specifies the font style.
        /// The prefix for this property is Style.
        /// </summary>
        public string Style;

        /// <summary>
        /// Creates a new instance of SelectFont command.
        /// </summary>
        public SelectFont() 
        {
        }

        /// <summary>
        /// Creates a new instance of SelectFont command.
        /// </summary>
        /// <param name="faceName">
        /// The typeface name of the font.
        /// </param>
        public SelectFont(string faceName)
        {
            this.FaceName = faceName;
        }

        /// <summary>
        /// Gets or sets a quoted string containing comma-delimited fields that indicate the characteristics
        /// of the TinyFont to create. The fields correspond to fields in the LOGFONT structure 
        /// and the ENUMLOGFONTEX structure. 
        /// Each field in SelectionString consists of one of the prefixes, followed by a string.
        /// </summary>
        /// <returns>
        /// A quoted string containing comma-delimited fields that indicate the characteristics
        /// of the TinyFont to create.  
        /// </returns>
        [TinyParameter(RequiresQuotes = true)]
        public string SelectionString
        {
            get
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder("FN:" + this.FaceName);
                if (this.Weight.HasValue)
                {
                    builder.Append(",WE:" + this.Weight.Value);
                }

                if (this.Height.HasValue)
                {
                    builder.Append(",HE:" + this.Height.Value);
                }

                if (this.Width.HasValue)
                {
                    builder.Append(",WI:" + this.Width.Value);
                }

                if (this.Escapement.HasValue)
                {
                    builder.Append(",ES:" + this.Escapement.Value);
                }

                if (this.Orientation.HasValue)
                {
                    builder.Append(",OR:" + this.Orientation.Value);
                }

                if (this.Italic.HasValue)
                {
                    builder.Append(",IT:" + (this.Italic.Value ? '1' : '0'));
                }

                if (this.Underline.HasValue)
                {
                    builder.Append(",UN:" + (this.Underline.Value ? '1' : '0'));
                }

                if (this.CharacterSet.HasValue)
                {
                    builder.Append(",CS:" + this.CharacterSet.Value);
                }

                if (this.OutputPrecision.HasValue)
                {
                    builder.Append(",OP:" + this.OutputPrecision.Value);
                }

                if (this.ClipPrecision.HasValue)
                {
                    builder.Append(",CP:" + this.ClipPrecision.Value);
                }

                if (this.Quality.HasValue)
                {
                    builder.Append(",QA:" + this.Quality.Value);
                }

                if (this.PitchAndFamily.HasValue)
                {
                    builder.Append(",PF:" + this.PitchAndFamily.Value);
                }

                if (!string.IsNullOrEmpty(this.FullName))
                {
                    builder.Append(",FullName:" + this.FullName);
                }

                if (!string.IsNullOrEmpty(this.Script))
                {
                    builder.Append(",Script:" + this.Script);
                }

                if (!string.IsNullOrEmpty(this.Style))
                {
                    builder.Append(",Style:" + this.Style);
                }

                return builder.ToString();
            }

            set
            {
                string[] characteristics = value.Split(',');
                foreach (string characteristic in characteristics)
                {
                    string[] keyValuePair = characteristic.Split(new char[] { ':' }, 2);
                    switch (keyValuePair[0])
                    {
                        case "FN": 
                            this.FaceName = keyValuePair[1].Trim('"'); 
                            break;

                        case "WE": 
                            this.Weight = int.Parse(keyValuePair[1]); 
                            break;

                        case "HE": 
                            this.Height = int.Parse(keyValuePair[1]); 
                            break;

                        case "WI": 
                            this.Width = int.Parse(keyValuePair[1]); 
                            break;

                        case "ES": 
                            this.Escapement = int.Parse(keyValuePair[1]);
                            break;

                        case "OR": 
                            this.Orientation = int.Parse(keyValuePair[1]);
                            break;

                        case "IT": 
                            this.Italic = keyValuePair[1] == "1"; 
                            break;

                        case "UN": 
                            this.Underline = keyValuePair[1] == "1";
                            break;

                        case "CS": 
                            this.CharacterSet = int.Parse(keyValuePair[1]);
                            break;

                        case "OP": 
                            this.OutputPrecision = int.Parse(keyValuePair[1]); 
                            break;

                        case "CP": 
                            this.ClipPrecision = int.Parse(keyValuePair[1]);
                            break;

                        case "QA": 
                            this.Quality = int.Parse(keyValuePair[1]);
                            break;

                        case "PF": 
                            this.PitchAndFamily = int.Parse(keyValuePair[1]);
                            break;

                        case "FullName": 
                            this.FullName = keyValuePair[1].Trim('"');
                            break;

                        case "Script": 
                            this.Script = keyValuePair[1].Trim('"');
                            break;

                        case "Style":
                            this.Style = keyValuePair[1].Trim('"');
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}
