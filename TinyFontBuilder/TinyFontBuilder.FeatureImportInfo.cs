using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Win32;

namespace Terka.TinyFonts
{
    partial class TinyFontBuilder
    {
        private enum FeatureImportType
        {
            Substitution,
            Positioning
        }

        private class FeatureImportInfo
        {
            private BuilderState _state;

            private FeatureImportType _type;
            private uint _script;
            private uint _language;
            private uint _feature;

            public BuilderState BuilderState { get { return _state; } }

            public FeatureImportType Type { get { return _type; } }
            public uint Script { get { return _script; } }
            public uint Language { get { return _language; } }
            public uint Feature { get { return _feature; } }

            public FeatureImportInfo(TinyFontBuilder state, FeatureImportType type, uint script, uint language, uint feature) : this(new BuilderState(state), type, script, language, feature) { }
            public FeatureImportInfo(BuilderState state, FeatureImportType type, uint script, uint language, uint feature)
            {
                _state = state;

                _type = type;
                _script = script;
                _language = language;
                _feature = feature;
            }
        }
    }
}
