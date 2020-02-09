using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Terka.TinyFonts
{
    internal static class RectExtensions
    {
        public static Rect Add(this Rect rect, Thickness t)
        {
            double left = rect.Left - t.Left;
            double top = rect.Top - t.Top;
            double right = rect.Right + t.Right;
            double bottom = rect.Bottom + t.Bottom;

            double width = right - left;
            double height = bottom - top;

            if (width < 0)
            {
                width = -width;
                left -= width;
            }

            if (height < 0)
            {
                height = -height;
                top -= height;
            }

            return new Rect(left, top, width, height);
        }

        public static Thickness Subtract(this Rect rect, Rect r)
        {
            return new Thickness(rect.Left - r.Left, rect.Top - r.Top, r.Right - rect.Right, r.Bottom - rect.Bottom);
        }
    }
}
