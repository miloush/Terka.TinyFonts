using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using BitArray = System.Collections.BitArray;

namespace Terka.TinyFonts
{

    /// <summary>
    /// Tiny Font Builder.
    /// </summary>
    public partial class TinyFontBuilder
    {
        private const int LastPlaneFirstCharacter = 0x100000;
        private const int LastPlaneLastCharacter = 0x10FFFF;
        private const int LastPlane = 16;

        private const int OpacityOpaque = 256;
        private const int ReplacementCharacter = 0xFFFD;
        private const int BasicPlaneLastCharacter = 0xFFFF;

        private const ushort NotDefGlyph = 0;
        private const ushort NullGlyph = 1;

        private const int BitsPerByte = 8;
        private const int BitsPerInt32 = 32;
        private const int BytesPerInt32 = 4;

        /// <summary>
        /// Gets a trace source for the builder.
        /// </summary>
        protected static TraceSource Trace { get { return TerkaTraceSources.TinyFontBuilderSource; } }

        private bool _importGraphemeClusterBoundaries;
        private bool _importVerticalMetrics;
        private GlyphTypeface _typeface;
        private Dictionary<ushort, int> _glyphToCharacterMap;
        private Dictionary<int, CharacterImportInfo> _characterImportList;
        private List<CharacterImportInfo> _glyphImportList;

        private IOpenTypeCompiler _compiler;
        private List<FeatureImportInfo> _features;

        private AntialiasingLevel? _antialiasLevelUsed;
        private AntialiasingLevel _antialiasLevel;
        private double _emSize;
        private Transform _transform;
        private Point _transformOrigin;

        /// <summary>
        /// Gets or sets glyph typeface.
        /// </summary>
        public GlyphTypeface GlyphTypeface
        {
            get
            {
                return _typeface;
            }
            set
            {
                _typeface = value;

                InvalidateGlyphToCharacterMap();
            }
        }
        /// <summary>
        /// Gets a glyph to character map for the current typeface.
        /// </summary>
        protected IDictionary<ushort, int> GlyphToCharacterMap
        {
            get
            {
                EnsureGlyphToCharacterMap();

                return _glyphToCharacterMap;
            }
        }

        /// <summary>
        /// Gets or sets anti-aliasing level.
        /// </summary>
        public AntialiasingLevel AntialiasingLevel
        {
            get { return _antialiasLevel; }
            set
            {
                ValidateAntialiasLevel(value);
                _antialiasLevel = value;
            }
        }
        /// <summary>
        /// Gets or sets glyph transformation.
        /// </summary>
        public Transform GlyphTransform
        {
            get { return _transform; }
            set { _transform = value; }
        }
        /// <summary>
        /// Gets or sets glyph transformation origin.
        /// </summary>
        public Point GlyphTransformOrigin
        {
            get { return _transformOrigin; }
            set { _transformOrigin = value; }
        }

        /// <summary>
        /// Gets or sets OpenType compiler.
        /// </summary>
        public IOpenTypeCompiler OpenTypeCompiler { get { return _compiler; } set { _compiler = value; } }
        /// <summary>
        /// Gets or sets characters's Em Size.
        /// </summary>
        public double EmSize
        {
            get { return _emSize; }
            set { _emSize = value; }
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        public TinyFontBuilder() : this(null) { }
        /// <summary>
        /// Creates new instance with defined glyph typeface.
        /// </summary>
        /// <param name="typeface">Glyph typeface to use.</param>
        public TinyFontBuilder(GlyphTypeface typeface) : this(typeface, SystemFonts.MessageFontSize) { }
        /// <summary>
        /// Creates new instance with defined glyph typeface and Em size.
        /// </summary>
        /// <param name="typeface">Glyph typface to use.</param>
        /// <param name="emSize">Character's Em Size.</param>
        public TinyFontBuilder(GlyphTypeface typeface, double emSize) : this(typeface, emSize, AntialiasingLevel.None) { }
        /// <summary>
        /// Creates new instance with defined glyph typeface, Em size and anti-aliasing level.
        /// </summary>
        /// <param name="typeface">Glyph typface to use.</param>
        /// <param name="emSize">Character's Em Size.</param>
        /// <param name="antialiasingLevel">Anti-aliasing level.</param>
        public TinyFontBuilder(GlyphTypeface typeface, double emSize, AntialiasingLevel antialiasingLevel)
        {
            _characterImportList = new Dictionary<int, CharacterImportInfo>();
            _glyphImportList = new List<CharacterImportInfo>();
            _features = new List<FeatureImportInfo>();

            _emSize = emSize;
            _typeface = typeface;
            _antialiasLevel = antialiasingLevel;
        }

        private static uint ToTagValue(string tag)
        {
            tag = tag.PadLeft(4);

            return
                (uint)tag[0] << 24 |
                (uint)tag[1] << 16 |
                (uint)tag[2] << 8 |
                (uint)tag[3];
        }

#if TERKA_FEATURES
        /// <summary>
        /// Imports OpenType font's feature to Tiny Font.
        /// </summary>
        /// <param name="script">Tag of script in which look for feature.</param>
        /// <param name="language">Tag of language in which look for feature.</param>
        /// <param name="feature">Tag of feature which look up.</param>
        /// <returns>True if feature was found in typeface.</returns>
        /// <exception cref="InvalidOperationException">If <see cref="OpenTypeCompiler"/> is missing.</exception>
        public bool ImportFeature(string script, string language, string feature)
        {
            return ImportFeature(ToTagValue(script), ToTagValue(language), ToTagValue(feature));
        }
        /// <summary>
        /// Imports OpenType font's feature to Tiny Font.
        /// </summary>
        /// <param name="script">Id of script in which look for feature.</param>
        /// <param name="language">Id of language in which look for feature.</param>
        /// <param name="feature">Id of feature which look up.</param>
        /// <returns>True if feature was found in typeface.</returns>
        /// <exception cref="InvalidOperationException">If <see cref="OpenTypeCompiler"/> is missing.</exception>
        public bool ImportFeature(uint script, uint language, uint feature)
        {
            if (_compiler == null)
                throw new InvalidOperationException("OpenType compiler not set.");

            FeatureImportType type;

            if (_compiler.IsSubstitutionFeaturePresent(_typeface, script, language, feature))
                type = FeatureImportType.Substitution;
            else if (_compiler.IsPositioningFeaturePresent(_typeface, script, language, feature))
                type = FeatureImportType.Positioning;
            else
                return false; //throw new ArgumentException("Feature not available.");

            _features.Add(new FeatureImportInfo(this, type, script, language, feature));
            return true;
        }
        /// <summary>
        /// Imports grapheme cluster boundaries as defined in Unicode standard to Tiny Font.
        /// </summary>
        public void ImportGraphemeClusterBoundaries()
        {
            _importGraphemeClusterBoundaries = true;
        }
        /// <summary>
        /// Imports vertical metrics needed for vertical text rendering.
        /// </summary>
        public void ImportVerticalMetrics()
        {
            _importVerticalMetrics = true;
        }
        /// <summary>
        /// Imports OpenType's font required features to Tiny Font.
        /// </summary>
        /// <param name="script">Id of script for which import features.</param>
        /// <param name="language">Id of language for which import features.</param>
        public void ImportRequiredFeatures(uint script, uint language)
        {
            _features.Add(new FeatureImportInfo(this, FeatureImportType.Positioning, script, language, 0));
            _features.Add(new FeatureImportInfo(this, FeatureImportType.Substitution, script, language, 0));
        }
        /// <summary>
        /// Imports OpenType's font required features to Tiny Font.
        /// </summary>
        /// <param name="script">Tag of script for which import features.</param>
        /// <param name="language">Tag of language for which import features.</param>
        public void ImportRequiredFeatures(string script, string language)
        {
            ImportRequiredFeatures(ToTagValue(script), ToTagValue(language));
        }
#endif

        /// <summary>
        /// Imports default character which will be showed instead of characters thar are missing in Tiny Font.
        /// </summary>
        /// <param name="defaultCharacter">Default character.</param>
        public void ImportAsDefault(int defaultCharacter)
        {
            Import(new int[0], defaultCharacter);
        }
        /// <summary>
        /// Imports collection of characters specified by its code.
        /// </summary>
        /// <param name="characters">Collection of characters.</param>
        public void Import(IEnumerable<int> characters)
        {
            Import(characters, null /* defaultCharacter */);
        }
        private void Import(IEnumerable<int> characters, int? defaultCharacter)
        {
            if (characters == null)
                throw new ArgumentNullException("characters");

            Import(ContentFrom(characters, defaultCharacter));
        }

        /// <summary>
        /// Imports characters in string to Tiny Font.
        /// </summary>
        /// <param name="characters">String containing characters to import.</param>
        /// <exception cref="ArgumentNullException"><paramref name="characters"/> is null.</exception>
        public void Import(string characters)
        {
            if (characters == null)
                throw new ArgumentNullException("characters");

            Import(characters.ToCodePoints());
        }
        /// <summary>
        /// Imports characeters to Tiny Font.
        /// </summary>
        /// <param name="characters">Characters to import.</param>
        public void Import(params char[] characters)
        {
            Import(characters, null /* defaultCharacter */);
        }
        private void Import(IEnumerable<char> characters, char? defaultCharacter)
        {
            if (characters == null)
                throw new ArgumentNullException("characters");

            Import(ContentFrom(characters.Select(ch => (int)ch), (int?)defaultCharacter));
        }

        /// <summary>
        /// Imports glyph specified by its ID as default in Tiny Font.
        /// </summary>
        /// <param name="defaultGlyph">ID of glyph.</param>
        public void ImportAsDefault(ushort defaultGlyph)
        {
            Import(new ushort[0], defaultGlyph);
        }
        /// <summary>
        /// Imports collection of glyph IDs to Tiny Font.
        /// </summary>
        /// <param name="glyphs">Collection of glyphs.</param>
        public void Import(IEnumerable<ushort> glyphs)
        {
            Import(glyphs, null /* defaultCharacter */);
        }
        private void Import(IEnumerable<ushort> glyphs, ushort? defaultGlyph)
        {
            if (glyphs == null)
                throw new ArgumentNullException("glyphs");

            Import(ContentFrom(glyphs, defaultGlyph));
        }

        /// <summary>
        /// Imports glyph specified by its code as character.
        /// </summary>
        /// <param name="glyph">ID of glyph.</param>
        /// <param name="character">Character representing <paramref name="glyph"/></param>
        public void ImportGlyphAsCharacter(ushort glyph, int character)
        {
            if (glyph < _typeface.GlyphCount)
                Import(new CharacterGlyphPair(character, glyph));
        }

        private void Import(IEnumerable<CharacterGlyphPair> content)
        {
            Contract.Assert(content != null);

            foreach (CharacterGlyphPair pair in content)
                Import(pair);
        }
        private void Import(CharacterGlyphPair pair)
        {
            if (pair.Character.HasValue)
                _characterImportList[pair.Character.Value] = new CharacterImportInfo(this, pair);
            else
                _glyphImportList.Add(new CharacterImportInfo(this, pair));
        }
        private void ClearImport()
        {
            _characterImportList.Clear();
            _glyphImportList.Clear();
        }

        private IEnumerable<CharacterGlyphPair> ContentFrom(IEnumerable<int> characters, int? defaultCharacter)
        {
            ushort glyph;
            foreach (int character in characters)
                if (character < 0)
                {
                    throw new ArgumentException("The sequence contains an invalid character.");
                }
                else if (character == ReplacementCharacter)
                {
                    if (defaultCharacter == null)
                        defaultCharacter = character;
                }
                else if (_typeface.CharacterToGlyphMap.TryGetValue(character, out glyph))
                {
                    yield return new CharacterGlyphPair(character, glyph);
                }

            if (defaultCharacter.HasValue && _typeface.CharacterToGlyphMap.TryGetValue(defaultCharacter.Value, out glyph))
            {
                yield return new CharacterGlyphPair(ReplacementCharacter, glyph);
            }
        }
        private IEnumerable<CharacterGlyphPair> ContentFrom(IEnumerable<ushort> glyphs, ushort? defaultCharacterGlyph)
        {
            EnsureGlyphToCharacterMap();

            int character;
            foreach (ushort glyph in glyphs)
                if (_glyphToCharacterMap.TryGetValue(glyph, out character))
                {
                    if (character == ReplacementCharacter)
                    {
                        if (defaultCharacterGlyph == null)
                            defaultCharacterGlyph = glyph;
                    }
                    else
                        yield return new CharacterGlyphPair(character, glyph);
                }
                else if (glyph == NotDefGlyph)
                {
                    if (defaultCharacterGlyph == null)
                        defaultCharacterGlyph = glyph;
                }
                else if (glyph < _typeface.GlyphCount)
                    yield return new CharacterGlyphPair(null, glyph);

            if (defaultCharacterGlyph.HasValue)
                yield return new CharacterGlyphPair(ReplacementCharacter, defaultCharacterGlyph);
        }

        /// <summary>
        /// Assembles new Tiny Font from imports.
        /// </summary>
        /// <returns>Tiny Font.</returns>
        public TinyFont Build()
        {
            TinyFont font = new TinyFont();

            if (_features.Count > 0)
                FillOpenType(font);  // can add new characters/glyphs

            FillMetrics(font.Metrics);

            for (ushort plane = 0; plane <= LastPlane; plane++)
                FillCharacters(font, from import in _characterImportList.Values
                                     orderby import.Mapping.Character
                                     select import, plane);

            for (int i = 0; i < _glyphImportList.Count; i++)
                _glyphImportList[i].Mapping.Character = i + LastPlaneFirstCharacter;

            if (_glyphImportList.Count > 0)
                FillCharacters(font, _glyphImportList, LastPlane);

#if TERKA_FEATURES
            if (_importGraphemeClusterBoundaries)
                FillGraphemeClusterBoundaries(font);

            if (_importVerticalMetrics)
                FillVerticalMetrics(font);
#endif
            font.Update();

            RemapGlyphsToCharacters(font);
            return font;
        }
        /// <summary>
        /// Assembles new Tiny Font from imports and imports <paramref name="characters"/> to Tiny Font.
        /// </summary>
        /// <param name="characters">Characters to import.</param>
        /// <param name="includeDefaultCharacter">Include default character?</param>
        /// <returns>Tiny Font.</returns>
        public TinyFont Build(string characters, bool includeDefaultCharacter = true)
        {
            ClearImport();
            Import(characters.ToCodePoints(), includeDefaultCharacter ? (int?)ReplacementCharacter : null);

            return Build();
        }
        /// <summary>
        /// Assembles new Tiny Font from imports and adds <paramref name="characters"/> to the font.
        /// </summary>
        /// <param name="characters">Characters to import.</param>
        /// <param name="defaultCharacter">Default Tiny Font character.</param>
        /// <returns>Tiny Font.</returns>
        public TinyFont Build(IEnumerable<int> characters, int? defaultCharacter = null)
        {
            ClearImport();
            Import(characters, defaultCharacter);

            return Build();
        }
        /// <summary>
        /// Assembles new Tiny Font from imports and adds <paramref name="glyphs"/> to the font.
        /// </summary>
        /// <param name="glyphs">Glyphs to import.</param>
        /// <param name="defaultCharacterGlyph">Default glyph in Tiny Font.</param>
        /// <returns>Tiny Font.</returns>
        public TinyFont Build(IEnumerable<ushort> glyphs, ushort? defaultCharacterGlyph = null)
        {
            ClearImport();
            Import(glyphs, defaultCharacterGlyph);

            return Build();
        }

#if TERKA_FEATURES
        private void FillVerticalMetrics(TinyFont font)
        {
            GlyphMetadataAppendix metadata = font.GetOrAddNewAppendix<GlyphMetadataAppendix>();

            List<sbyte> marginTop = new List<sbyte>();
            List<sbyte> marginBottom = new List<sbyte>();

            foreach (CharacterImportInfo info in from import in _characterImportList.Values
                                                 orderby import.Mapping.Character
                                                 select import)
            {
                marginTop.Add(Helper.FitIntoInt8(info.EmSideBearing.Top, Trace));
                marginBottom.Add(Helper.FitIntoInt8(info.EmSideBearing.Bottom, Trace));
            }

            foreach (CharacterImportInfo info in _glyphImportList)
            {
                marginTop.Add(Helper.FitIntoInt8(info.EmSideBearing.Top, Trace));
                marginBottom.Add(Helper.FitIntoInt8(info.EmSideBearing.Bottom, Trace));
            }

            GlyphMetadataAppendix.MetadataSetOffset offsetTop = new GlyphMetadataAppendix.MetadataSetOffset();
            offsetTop.Id = GlyphMetadataAppendix.MarginTopSet;
            offsetTop.Bits = GlyphMetadataAppendix.MetadataSetBitLength.Eight;

            metadata.Sets.Add(marginTop.Select(m => (byte)m).ToArray());
            metadata.SetsOffsets.Add(offsetTop);

            GlyphMetadataAppendix.MetadataSetOffset offsetBottom = new GlyphMetadataAppendix.MetadataSetOffset();
            offsetBottom.Id = GlyphMetadataAppendix.MarginBottomSet;
            offsetBottom.Bits = GlyphMetadataAppendix.MetadataSetBitLength.Eight;

            metadata.Sets.Add(marginBottom.Select(m => (byte)m).ToArray());
            metadata.SetsOffsets.Add(offsetBottom);
        }
        private void FillGraphemeClusterBoundaries(TinyFont font)
        {
            GlyphMetadataAppendix metadata = font.GetOrAddNewAppendix<GlyphMetadataAppendix>();
            byte[] clusterBoundaries = Resources.GraphemeBreakProperty;

            BitArray data = new BitArray(0);
            foreach (CharacterInfo info in font.EnumerateAllCharacterInfos())
            {
                byte property = FindClusterProperty(clusterBoundaries, info.Codepoint);
                data.AppendLsb(property, 4);
            }

            byte[] setData = new byte[(data.Length + 7) / 8];
            data.CopyTo(setData, 0);

            metadata.Sets.Add(setData);
            GlyphMetadataAppendix.MetadataSetOffset offset = new GlyphMetadataAppendix.MetadataSetOffset();
            offset.Id = GlyphMetadataAppendix.GraphemeSet;
            offset.Bits = GlyphMetadataAppendix.MetadataSetBitLength.Four;

            metadata.SetsOffsets.Add(offset);
        }
        private static byte FindClusterProperty(byte[] clusterBoundaries, int character)
        {
            int offset = 0;
            while (offset < clusterBoundaries.Length)
            {
                int start = BitConverter.ToInt32(clusterBoundaries, offset);
                offset += sizeof(int);

                int typeAndLength = BitConverter.ToInt32(clusterBoundaries, offset);
                offset += sizeof(int);

                byte property = (byte)typeAndLength;
                int length = typeAndLength >> 8;

                if (character >= start && character < start + length)
                    return property;
            }

            return 0;
        }
#endif

        private void FillOpenType(TinyFont font)
        {
            HashSet<ushort> userGlyphs = new HashSet<ushort>();
            foreach (CharacterImportInfo info in EnumerateAllImports())
                if (info.Mapping.Glyph.HasValue)
                    userGlyphs.Add(info.Mapping.Glyph.Value);

            Dictionary<ushort, BuilderState> allGeneratedGlyphs = new Dictionary<ushort, BuilderState>();
            foreach (FeatureImportInfo feature in _features.Where(f => f.Type == FeatureImportType.Substitution))
                foreach (ushort generatedGlyph in _compiler.GetGeneratedGlyphIds(feature.BuilderState.GlyphTypeface, feature.Script, feature.Language, feature.Feature))
                    allGeneratedGlyphs[generatedGlyph] = feature.BuilderState;

            GlyphClassesAppendix classes = font.GetOrAddNewAppendix<GlyphClassesAppendix>();
            SubstitutionAppendix substitution = font.GetOrAddNewAppendix<SubstitutionAppendix>();
            PositioningAppendix positioning = font.GetOrAddNewAppendix<PositioningAppendix>();

            Dictionary<ushort, BuilderState> glyphsToAdd = new Dictionary<ushort, BuilderState>();
            foreach (FeatureImportInfo feature in _features.Where(f => f.Type == FeatureImportType.Substitution))
                foreach (ushort generatedGlyph in _compiler.CompileFeature(feature.BuilderState.GlyphTypeface, feature.Script, feature.Language, feature.Feature, substitution, classes, allGeneratedGlyphs.Keys.Concat(userGlyphs)))
                    glyphsToAdd[generatedGlyph] = feature.BuilderState;

            foreach (KeyValuePair<ushort, BuilderState> toAddPair in glyphsToAdd)
                if (!userGlyphs.Contains(toAddPair.Key))
                {
                    int character;

                    if (toAddPair.Value.GlyphToCharacterMap.TryGetValue(toAddPair.Key, out character))
                        _characterImportList[character] = new CharacterImportInfo(toAddPair.Value, new CharacterGlyphPair(character, toAddPair.Key));
                    else if (toAddPair.Key < toAddPair.Value.GlyphTypeface.GlyphCount)
                        _glyphImportList.Add(new CharacterImportInfo(toAddPair.Value, new CharacterGlyphPair(null, toAddPair.Key)));

                    userGlyphs.Add(toAddPair.Key);
                }

            foreach (FeatureImportInfo feature in _features.Where(f => f.Type == FeatureImportType.Positioning))
                _compiler.CompileFeature(feature.BuilderState.GlyphTypeface, feature.Script, feature.Language, feature.Feature, positioning, classes, allGeneratedGlyphs.Keys.Concat(userGlyphs), feature.BuilderState.EmSize);
        }

        private int RemapGlyph(IDictionary<ushort, int> mapping, ushort glyph, out byte plane, out ushort planeLess)
        {
            int value;

            if (!mapping.TryGetValue(glyph, out value))
            {
                if (!_glyphToCharacterMap.TryGetValue(glyph, out value))
                {
                    value = 0x10FFFF;
                    while (mapping.Values.Contains(value))
                        value--;

                    if (value < 0)
                        throw new InvalidOperationException("Too many glyphs.");

                    mapping[glyph] = value;
                }
            }

            plane = (byte)(value >> 16);
            planeLess = (ushort)value;
            return value;
        }
        private void RemapGlyphsToCharacters(TinyFont font)
        {
            Dictionary<ushort, int> mapping = new Dictionary<ushort, int>();

            foreach (CharacterImportInfo info in _glyphImportList)
                if (info.Mapping.Glyph.HasValue)
                    mapping[info.Mapping.Glyph.Value] = info.Mapping.Character.Value;

            foreach (CharacterImportInfo info in _characterImportList.Values)
                if (info.Mapping.Glyph.HasValue)
                    mapping[info.Mapping.Glyph.Value] = info.Mapping.Character.Value;

            byte plane;
            ushort planeLess;

            foreach (StateMachineAppendix substition in font.Appendices.OfType<StateMachineAppendix>())
                foreach (StateMachineAppendix.Feature feature in substition.Features)
                    foreach (StateMachineAppendix.Rule rule in feature.Rules)
                    {
                        if (rule.Condition == StateMachineAppendix.RuleCondition.Glyph)
                        {
                            RemapGlyph(mapping, rule.ConditionParameter, out plane, out planeLess);
                            rule.ConditionPlane = plane;
                            rule.ConditionParameter = planeLess;
                        }

                        switch (rule.Action)
                        {
                            case StateMachineAppendix.RuleAction.GlyphOverwrite:
                            case StateMachineAppendix.RuleAction.GlyphRewrite_2_1:
                            case StateMachineAppendix.RuleAction.GlyphRewrite_3_1:
                            case StateMachineAppendix.RuleAction.GlyphInsertion:
                                {
                                    RemapGlyph(mapping, rule.ActionParameter, out plane, out planeLess);
                                    rule.ActionPlane = plane;
                                    rule.ActionParameter = planeLess;
                                }
                                break;

                            case StateMachineAppendix.RuleAction.GlyphRewrite_N_M:
                                int offset = rule.ActionParameter + sizeof(ushort);
                                int glyphCount = BitConverter.ToUInt16(substition.ParametersHeap, offset);
                                offset += sizeof(ushort);

                                for (int i = 0; i < glyphCount; i++, offset += sizeof(int))
                                {
                                    int glyph = BitConverter.ToInt32(substition.ParametersHeap, offset);
                                    glyph = RemapGlyph(mapping, (ushort)glyph, out plane, out planeLess);

                                    BitConverter.GetBytes(glyph).CopyTo(substition.ParametersHeap, offset);
                                }
                                break;
                        }
                    }

            foreach (GlyphClassesAppendix coverage in font.Appendices.OfType<GlyphClassesAppendix>())
                for (int i = 0; i < coverage.CoverageGlyphs.Count; i++)
                    coverage.CoverageGlyphs[i] = RemapGlyph(mapping, (ushort)coverage.CoverageGlyphs[i], out plane, out planeLess);
        }

        /// <summary>
        /// Gets assigment of glyphs to characters.
        /// </summary>
        /// <returns>Glyph to character mapping.</returns>
        public IDictionary<ushort, int> GetAssignedGlyphCharacters()
        {
            return _glyphImportList.ToDictionary(info => info.Mapping.Glyph.Value, info => info.Mapping.Character.Value);
        }

        private void InvalidateGlyphToCharacterMap()
        {
            _glyphToCharacterMap = null;
        }
        private void EnsureGlyphToCharacterMap()
        {
            if (_glyphToCharacterMap == null)
            {
                Dictionary<ushort, int> glyphToCharacterMap = new Dictionary<ushort, int>(_typeface.CharacterToGlyphMap.Count);

                foreach (KeyValuePair<int, ushort> pair in _typeface.CharacterToGlyphMap)
                    glyphToCharacterMap[pair.Value] = pair.Key;

                _glyphToCharacterMap = glyphToCharacterMap;
            }
        }

        private void FillDescription(FontDescription description)
        {
            Contract.Assert(_antialiasLevelUsed != AntialiasingLevel.None);

            description.AntialiasingLevel = _antialiasLevelUsed ?? AntialiasingLevel.None;
            description.IsExtended = _antialiasLevelUsed.HasValue;

            // IsItalic and IsBold cannot be set because these can differ for each GlyphTypeface used.
            // .NET Micro Framework does not use these properties anyway.
            // description.IsItalic = _typeface.Style != FontStyles.Normal;
            // description.IsBold = _typeface.Weight >= FontWeights.Bold;
        }
        private void FillMetrics(FontMetrics metrics)
        {
            foreach (CharacterImportInfo import in EnumerateAllImports())
            {
                metrics.Height = Math.Max(metrics.Height, Math.Max((ushort)import.InkBox.Height, import.EmHeight));

                metrics.Ascent = Math.Max(metrics.Ascent, import.EmBaseline);
                metrics.Descent = Math.Max(metrics.Descent, checked((short)(import.EmHeight - import.EmBaseline)));
            }

            // .NET Micro Framework does not use the InternalLeading property.
            metrics.InternalLeading = 0;

            // .NET Micro Framework uses Height + ExternalLeading for line advancing.
            // We put all line advancement into Height and set ExternalLeading to zero for compatibility reasons.
            // http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/7b21047f-bce9-41d3-ab39-55ab8850caca
            metrics.ExternalLeading = 0;
        }

        private void FillCharacters(TinyFont font, IEnumerable<CharacterImportInfo> imports, ushort planeNumber)
        {
            Contract.Requires(font != null, "Font cannot be null.");
            Contract.Requires(font.Metrics != null, "Font metrics cannot be null.");

            if (font.Metrics.Height == 0)
            {
                Trace.TraceEvent(TraceEventType.Warning, 0, "TinyFont has a height of zero. No glyphs will be included.");
                return;
            }

            FontPlane plane = planeNumber == 0 ? font.FontPlanes[0] : new FontPlane();

            Contract.Requires(plane != null, "Plane cannot be null.");

            FillDescription(plane.Description);

            uint numberOfCharacters = 0;
            bool antialiasing = font.Description.IsExtended;
            short firstNonBlankLine = short.MaxValue;
            int maxCharacterWidth = font.Metrics.MaximumCharacterWidth;

            BitArray antialiasingData = new BitArray(0);
            List<BitArray> bitmapData = new List<BitArray>(font.Metrics.Height);
            for (int i = 0; i < font.Metrics.Height; i++)
                bitmapData.Add(new BitArray(0));

            int lastCharacter = -1;
            CharacterRangeDescription rangeDescription = null;
            CharacterRangeAntialiasing rangeAntialiasing = null;

            foreach (CharacterImportInfo import in imports)
            {
                Contract.Assert(import.Mapping.Character.Value >= 0 && import.Mapping.Character.Value <= LastPlaneLastCharacter, "All content must have valid two bytes characters.");
                Contract.Assert(import.Mapping.Character.Value > lastCharacter, "Content must be ordered by characters and cannot overlap.");

                int character = import.Mapping.Character.Value;

                // If character does not belong to this plane, skip it
                if ((character >> 16) != planeNumber)
                    continue;

                if (lastCharacter != (character - 1))
                {
                    if (rangeDescription != null)
                    {
                        rangeDescription.LastCharacter = (char)lastCharacter;
                        plane.CharacterRanges.Add(rangeDescription);

                        if (antialiasing)
                            plane.CharacterRangesAntialiasing.Add(rangeAntialiasing);
                    }

                    rangeDescription = new CharacterRangeDescription();
                    rangeDescription.Offset = (uint)bitmapData[0].Length;
                    rangeDescription.FirstCharacter = (char)character;
                    rangeDescription.IndexOfFirstCharacter = numberOfCharacters;

                    if (antialiasing)
                    {
                        rangeAntialiasing = new CharacterRangeAntialiasing();
                        rangeAntialiasing.Offset = (uint)(antialiasingData.Length / BitsPerByte);
                    }
                }

                lastCharacter = character;
                maxCharacterWidth = Math.Max(maxCharacterWidth, import.InkBox.Width);
                ++numberOfCharacters;

                CharacterDescription characterDescription = new CharacterDescription();
                characterDescription.LeftMargin = Helper.FitIntoInt8(import.EmSideBearing.Left, Trace);
                characterDescription.RightMargin = Helper.FitIntoInt8(import.EmSideBearing.Right, Trace);
                characterDescription.Offset = checked((ushort)(bitmapData[0].Length - rangeDescription.Offset));
                plane.Characters.Add(characterDescription);

                if (import.BitmapData != null)
                {
                    AppendBitmap(bitmapData, import);

                    short glyphFirstNonBlankLine = Helper.FitIntoInt16(import.InkBox.Y, Trace);
                    if (firstNonBlankLine > glyphFirstNonBlankLine)
                        firstNonBlankLine = glyphFirstNonBlankLine;
                }

                if (antialiasing)
                {
                    CharacterAntialiasing characterAntialiasing = new CharacterAntialiasing();
                    characterAntialiasing.Offset = import.AntialiasData == null ? CharacterAntialiasing.NoData : checked((ushort)(antialiasingData.Length / BitsPerByte - rangeAntialiasing.Offset));
                    plane.CharactersAntialiasing.Add(characterAntialiasing);

                    if (import.AntialiasData != null)
                    {
                        AppendAntialiasing(antialiasingData, import);

                        int bitPadding = antialiasingData.Length % BitsPerByte;
                        if (bitPadding > 0)
                            antialiasingData.Length += BitsPerByte - bitPadding;
                    }
                }
            }

            if (numberOfCharacters > 0)
            {
                if (rangeDescription != null)
                {
                    rangeDescription.LastCharacter = (char)lastCharacter;
                    plane.CharacterRanges.Add(rangeDescription);

                    if (antialiasing)
                        plane.CharacterRangesAntialiasing.Add(rangeAntialiasing);
                }

                plane.Metrics.Offset = firstNonBlankLine;
                plane.CharacterBitmap.Width = (uint)bitmapData[0].Length;
                plane.CharacterBitmap.Height = (uint)(bitmapData.Count - firstNonBlankLine);
                plane.CharacterBitmap.BitsPerPixel = 1;
                plane.CharacterBitmapData = ToBitmapData(bitmapData, firstNonBlankLine);

                if (antialiasing)
                {
                    plane.CharacterAntialiasingData = ToAntialiasingData(antialiasingData);
                    plane.CharacterAntialiasingMetrics.Size = (uint)plane.CharacterAntialiasingData.Length;
                }

                font.Metrics.MaximumCharacterWidth = Helper.FitIntoInt16(maxCharacterWidth, Trace);

                font.FontPlanes[planeNumber] = plane;
            }
        }

        private static void AppendBitmap(IList<BitArray> bitmapData, CharacterImportInfo import)
        {
            Contract.Requires(import != null, "Import cannot be null.");
            Contract.Requires(bitmapData != null, "Bitmap data cannot be null.");
            Contract.Requires(bitmapData.Count > 0, "Bitmap data does not contain any rows.");

            Contract.Ensures(bitmapData.Min(row => row.Count) == bitmapData.Max(row => row.Count), "Bitmap data row lengths corrupted.");

            if (import.BitmapData == null)
                return;

            int srcY = import.InkBox.Y;
            int srcX = import.InkBox.X;

            int destY = import.InkBox.Y; // bitmapData are already baseline aligned
            int destX = bitmapData[0].Length;

            int width = import.InkBox.Width;
            int height = import.InkBox.Height;

            if (destY + height > bitmapData.Count)
            {
                // bigger glyph than the font height, need to accomodate
                // this can happen when glyph transform is applied
                // or when glyph has out of metrics overhangs
                for (int y = bitmapData.Count; y < destY + height; y++)
                    bitmapData.Add(new BitArray(destX));
                //bitmapData.Insert(0, new BitArray(destX));

                // no need to update first non-blank line because current is the new first due to resizing
            }

            for (int i = 0; i < bitmapData.Count; i++)
                bitmapData[i].Length += width;

            for (int y = 0; y < height; y++)
            {
                BitArray bitmapDataRow = bitmapData[destY + y];

                for (int x = 0; x < width; x++)
                    bitmapDataRow[destX + x] = import.BitmapData[srcY + y][srcX + x];
            }
        }
        private static void AppendAntialiasing(BitArray antialiasingData, CharacterImportInfo import)
        {
            Contract.Requires(antialiasingData != null, "Antialiasing data cannot be null.");
            Contract.Requires(import != null, "Import cannot be null.");

            antialiasingData.Add(import.AntialiasData);
        }

        private byte[] ToBitmapData(IList<BitArray> bitmapData, short firstNonBlankRow)
        {
            Contract.Requires(bitmapData != null, "Bitmap data cannot be null.");
            Contract.Requires(bitmapData.Count > 0, "Bitmap data does not contain any rows.");
            Contract.Requires(bitmapData.Min(r => r.Length) == bitmapData.Max(r => r.Length), "Bitmap data rows must have the same length.");
            Contract.Requires(firstNonBlankRow < bitmapData.Count, "First non-blank row is out of the supplied number of rows.");

            if (bitmapData[0].Length == 0)
                return new byte[0];

            int height = bitmapData.Count - firstNonBlankRow;
            int width = (bitmapData[0].Length - 1) / BitsPerByte + 1;
            int stride = BytesPerInt32 * ((width - 1) / BytesPerInt32 + 1);

            byte[] data = new byte[stride * height];

            for (int y = 0; y < height; y++)
                bitmapData[firstNonBlankRow + y].CopyTo(data, stride * y);

            return data;
        }
        private byte[] ToAntialiasingData(BitArray antialiasingData)
        {
            Contract.Requires(antialiasingData != null, "Antialiasing data cannot be null.");

            if (antialiasingData.Length == 0)
                return new byte[0];

            int alignedLength = BytesPerInt32 * ((antialiasingData.Length - 1) / BitsPerInt32 + 1);

            byte[] data = new byte[alignedLength];
            antialiasingData.CopyTo(data, 0);

            for (int i = 0; i < data.Length; i++)
                data[i] = Helper.ReverseBits(data[i]);

            return data;
        }

        private void ValidateAntialiasLevel(AntialiasingLevel value)
        {
            Contract.Ensures(_antialiasLevelUsed != AntialiasingLevel.None);

            if (value == AntialiasingLevel.None)
                return;

            if (value != AntialiasingLevel.Gray5 &&
                value != AntialiasingLevel.Gray17 &&
                value != AntialiasingLevel.Gray65)
                throw new NotSupportedException("Antialiasing level not supported.");

            if (_antialiasLevelUsed.HasValue)
            {
                if (_antialiasLevelUsed != value)
                    throw new ArgumentException("Only one antialiasing level per font.");
            }
            else
            {
                _antialiasLevelUsed = value;
            }
        }

        private IEnumerable<CharacterImportInfo> EnumerateAllImports()
        {
            return _characterImportList.Values.Concat(_glyphImportList);
        }
    }
}
