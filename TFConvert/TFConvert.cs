namespace Terka.TinyFonts.TFConvert
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// TFConvert class allows generating TinyFont file format.
    /// </summary>
    public partial class TFConvert
    {
        private class RangeAdjustments
        {
            public int Start;
            public int End;
            public int LeftMargin;
            public int RightMargin;

            public RangeAdjustments(int start, int end, int leftMargin, int rightMargin)
            {
                Start = start;
                End = end;
                LeftMargin = leftMargin;
                RightMargin = rightMargin;
            }
        }

        private class GlyphRangeAdjustments
        {
            public ushort Start;
            public ushort End;
            public int LeftMargin;
            public int RightMargin;

            public GlyphRangeAdjustments(ushort start, ushort end, int leftMargin, int rightMargin)
            {
                Start = start;
                End = end;
                LeftMargin = leftMargin;
                RightMargin = rightMargin;
            }
        }

        private const int ReplacementCharacter = 0xFFFD;
        private const ushort NotDefGlyph = 0;

        private TinyFontBuilder _builder;
        private Dictionary<string, FontFamily> _addedFamilies = new Dictionary<string, FontFamily>();

        private MatrixTransform _transform = new MatrixTransform(Matrix.Identity);
        private List<RangeAdjustments> _postBuildAdjustments = new List<RangeAdjustments>();
        private List<GlyphRangeAdjustments> _postBuildGlyphAdjustments = new List<GlyphRangeAdjustments>();

        private short _adjustAscent;
        private short _adjustDescent;
        private short _adjustExternalLeading;
        private short _adjustInternalLeading;
        private short _adjustLeftMargin;
        private short _adjustRightMargin;
        private bool _noDefaultImport;

        /// <summary>
        /// Creates new instance.
        /// </summary>
        protected TFConvert()
        {

        }

        /// <summary>
        /// Creates new TinyFont based on the rules from <see cref="TinyDefinition"/>.
        /// </summary>
        /// <param name="definition">Definition rules for font.</param>
        /// <returns>An instance of <see cref="TinyFont"/>.</returns>
        public static TinyFont Convert(TinyDefinition definition)
        {
            return new TFConvert().ConvertWithState(definition);
        }

        /// <summary>
        /// Creates new TinyFont based on the rules from <see cref="TinyDefinition"/>.
        /// </summary>
        /// <param name="definition">Definition rules for font.</param>
        /// <returns>An instance of <see cref="TinyFont"/>.</returns>
        protected TinyFont ConvertWithState(TinyDefinition definition)
        {
            if (_builder == null)
            {
                _builder = new TinyFontBuilder();
                _builder.GlyphTransform = _transform;
#if TERKA_FEATURES
                _builder.OpenTypeCompiler = new Terka.FontBuilder.OpenTypeCompiler();
#endif
            }

            Type[] type = new Type[1];
            object[] parameter = new object[1];

            foreach (TinyCommandBase command in definition)
                if (command != null)
                {
                    type[0] = command.GetType();
                    parameter[0] = command;

                    MethodInfo processMethod = typeof(TFConvert).GetMethod("Process", BindingFlags.NonPublic | BindingFlags.Instance, null, type, null);

                    if (processMethod != null)
                        processMethod.Invoke(this, parameter);
                }

            if (_noDefaultImport == false)
                Process(new SetDefaultCharacter());

            TinyFont font = _builder.Build();
            MakePostBuildAdjustments(font);
            return font;
        }

        private void Process(AddFontToProcess addFontToProcess)
        {
            ICollection<FontFamily> families = Fonts.GetFontFamilies(addFontToProcess.FontPath);

            if (families.Count < 1)
                throw new ArgumentException(string.Format("Cannot load font specified in AddFontToProcess: {0}", addFontToProcess.FontPath));

            foreach (FontFamily family in families)
                foreach (var name in family.FamilyNames.Values)
                    _addedFamilies[name] = family;
        }
        private void Process(AdjustAscent command)
        {
            _adjustAscent = command.Adjustment;
        }
        private void Process(AdjustDescent command)
        {
            _adjustDescent = command.Adjustment;
        }
        private void Process(AdjustExternalLeading command)
        {
            _adjustExternalLeading = command.Adjustment;
        }
        private void Process(AdjustInternalLeading command)
        {
            _adjustInternalLeading = command.Adjustment;
        }
        private void Process(AdjustLeftMargin command)
        {
            _adjustLeftMargin = command.Adjustment;
        }
        private void Process(AdjustRightMargin command)
        {
            _adjustRightMargin = command.Adjustment;
        }
        private void Process(AntiAlias alias)
        {
            _builder.AntialiasingLevel = (AntialiasingLevel)alias.BitsPerPixel;
        }
        private void Process(ImportRange range)
        {
            _builder.Import(Enumerable.Range(range.Start, range.End - range.Start + 1));

            if (_adjustLeftMargin != 0 || _adjustRightMargin != 0)
                _postBuildAdjustments.Add(new RangeAdjustments(range.Start, range.End, _adjustLeftMargin, _adjustRightMargin));
        }
        private void Process(ImportRangeAndMap range)
        {
            Process((ImportRange)range);
        }
        private void Process(NoDefaultCharacter command)
        {
            _noDefaultImport = true;
        }
        private void Process(OffsetX command)
        {
            Matrix m = _transform.Matrix;
            m.OffsetX = command.Adjustment;
            _transform.Matrix = m;
        }
        private void Process(OffsetY command)
        {
            Matrix m = _transform.Matrix;
            m.OffsetY = command.Adjustment;
            _transform.Matrix = m;
        }
        private void Process(SelectFont font)
        {
            string familyName = font.FaceName;

            FontFamily family;
            if (!_addedFamilies.TryGetValue(familyName, out family))
                family = new FontFamily(familyName);

            Typeface typeface = new Typeface(
                family,
                font.Italic == true ? FontStyles.Italic : FontStyles.Normal,
                FontWeight.FromOpenTypeWeight(font.Weight ?? FontWeights.Normal.ToOpenTypeWeight()),
                FontStretches.Normal);

            GlyphTypeface glyphTypeface;
            if (typeface.TryGetGlyphTypeface(out glyphTypeface))
                _builder.GlyphTypeface = glyphTypeface;
            else
                throw new NotSupportedException("Composite fonts not supported.");

            if (font.Height == null)
                font.Height = 11;

            if (font.Height > 0)
                _builder.EmSize = font.Height.Value;
            else
                _builder.EmSize = -font.Height.Value * glyphTypeface.Height;

            if (font.Escapement.GetValueOrDefault() != 0)
            {
                Matrix m = Matrix.Identity;
                m.Rotate(-font.Escapement.GetValueOrDefault() / 10.0);
                m.OffsetX = _transform.Matrix.OffsetX;
                m.OffsetY = _transform.Matrix.OffsetY;
                _transform.Matrix = m;
            }
        }
        private void Process(SetAsDefaultCharacter command)
        {
            _noDefaultImport = true;
            _builder.ImportAsDefault(command.CharacterCode);

            if (_adjustLeftMargin != 0 || _adjustRightMargin != 0)
                _postBuildAdjustments.Add(new RangeAdjustments(ReplacementCharacter, ReplacementCharacter, _adjustLeftMargin, _adjustRightMargin));
        }
        private void Process(SetDefaultCharacter command)
        {
            _noDefaultImport = true;
            _builder.ImportAsDefault(NotDefGlyph);

            if (_adjustLeftMargin != 0 || _adjustRightMargin != 0)
                _postBuildAdjustments.Add(new RangeAdjustments(ReplacementCharacter, ReplacementCharacter, _adjustLeftMargin, _adjustRightMargin));
        }
        private void Process(Verbosity verbosity)
        {

        }
#if TERKA_FEATURES
        private void Process(ImportGlyphRange range)
        {
            _builder.Import(Enumerable.Range(range.Start, range.End - range.Start + 1).Select(g => (ushort)g));

            if (_adjustLeftMargin != 0 || _adjustRightMargin != 0)
                _postBuildGlyphAdjustments.Add(new GlyphRangeAdjustments(range.Start, range.End, _adjustLeftMargin, _adjustRightMargin));
        }
        private void Process(ImportGlyphRangeAndMap range)
        {
            Process((ImportGlyphRange)range);
        }
        private void Process(ImportFeature command)
        {
            _builder.ImportFeature(command.Script, command.Language, command.Feature);
        }
#endif
        private void MakePostBuildAdjustments(TinyFont font)
        {
            checked
            {
                foreach (RangeAdjustments adjustments in _postBuildAdjustments)
                    for (int i = adjustments.Start; i <= adjustments.End; i++)
                    {
                        CharacterInfo info = font.GetCharacterInfo(i);
                        info.MarginLeft += (sbyte)adjustments.LeftMargin;
                        info.MarginRight += (sbyte)adjustments.RightMargin;
                    }

                IDictionary<ushort, int> mapping = _builder.GetAssignedGlyphCharacters();
                foreach (GlyphRangeAdjustments adjustments in _postBuildGlyphAdjustments)
                    for (ushort i = adjustments.Start; i <= adjustments.End; i++)
                    {
                        CharacterInfo info = font.GetCharacterInfo(mapping[i]);
                        info.MarginLeft += (sbyte)adjustments.LeftMargin;
                        info.MarginRight += (sbyte)adjustments.RightMargin;
                    }

                font.Metrics.Ascent += _adjustAscent;
                if (font.Metrics.Ascent < 0)
                    throw new ArgumentException("Font cannot have negative ascent.");

                font.Metrics.Descent += _adjustDescent;
                if (font.Metrics.Descent < 0)
                    throw new ArgumentException("Font cannot have negative descent.");

                font.Metrics.ExternalLeading += _adjustExternalLeading;
                if (font.Metrics.ExternalLeading < 0)
                    throw new ArgumentException("Font cannot have negative external leading.");

                font.Metrics.InternalLeading += _adjustInternalLeading;
                if (font.Metrics.InternalLeading < 0)
                    throw new ArgumentException("Font cannot have negative internal leading.");
            }
        }
    }
}
