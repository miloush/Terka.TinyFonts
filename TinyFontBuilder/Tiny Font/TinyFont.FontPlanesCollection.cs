using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terka.TinyFonts
{
    public partial class TinyFont
    {
        /// <summary>
        /// Collection of font planes in Tiny Font.
        /// </summary>
        public class FontPlanesCollection
        {
            private TinyFont _font;

            /// <summary>
            /// Creates new instance of the collection.
            /// </summary>
            /// <param name="font"></param>
            public FontPlanesCollection(TinyFont font)
            {
                _font = font;
            }

            /// <summary>
            /// Gets Unicode planes appendix containing font planes.
            /// </summary>
            public UnicodePlanesAppendix Appendix
            {
                get { return _font.GetOrAddNewAppendix<UnicodePlanesAppendix>(); }
            }

            /// <summary>
            /// Gets or sets Font Plane at speficied <paramref name="planeNumber"/>.
            /// </summary>
            /// <param name="planeNumber">Number of plane.</param>
            /// <returns>Font plane or null if plane does not exists.</returns>
            public FontPlane this[int planeNumber]
            {
                get
                {
                    if (planeNumber == 0)
                        return _font._baseFontPlane;

                    return Appendix.GetPlane(planeNumber);
                }
                set
                {
                    if (planeNumber == 0)
                        _font._baseFontPlane = value;
                    else
                        Appendix.SetPlane(planeNumber, value);
                }
            }
        }
    }
}
