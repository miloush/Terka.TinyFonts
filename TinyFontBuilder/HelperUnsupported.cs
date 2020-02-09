using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media;
using Debug = System.Diagnostics.Debug;

namespace Terka.TinyFonts
{
    internal static class HelperUnsupported
    {
        private const BindingFlags InstanceNonPublic = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private static FieldInfo _GlyphRunTextFormattingMode;

        static HelperUnsupported()
        {
            _GlyphRunTextFormattingMode = typeof(GlyphRun).GetField("_textFormattingMode", InstanceNonPublic);
        }

        public static void SetTextFormattingMode(GlyphRun glyphRun, TextFormattingMode mode)
        {
            Debug.Assert(glyphRun != null, "GlyphRun cannot be null.");

            if (_GlyphRunTextFormattingMode != null)
            {
                _GlyphRunTextFormattingMode.SetValue(glyphRun, mode);
            }
        }
    }
}
