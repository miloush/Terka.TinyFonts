using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Terka
{
    internal static class TerkaTraceSources
    {
        private const string TinyFontBuilderSourceName = "Terka.TinyFonts.TinyFontBuilder";

        private static TraceSource _TinyFontBuilderSource;

        public static TraceSource TinyFontBuilderSource
        {
            get
            {
                if (_TinyFontBuilderSource == null)
                    _TinyFontBuilderSource = CreateTraceSource(TinyFontBuilderSourceName);

                return _TinyFontBuilderSource;
            }
        }

        private static TraceSource CreateTraceSource(string sourceName)
        {
            TraceSource source = new TraceSource(sourceName);

            if (source.Switch.Level == SourceLevels.Off && Debugger.IsAttached)
                source.Switch.Level = SourceLevels.Warning;

            return source;
        }
    }
}
