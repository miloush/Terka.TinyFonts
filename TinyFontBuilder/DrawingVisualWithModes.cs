namespace Terka.TinyFonts
{
    using System.Windows.Media;

    internal class DrawingVisualWithModes : DrawingVisual
    {
        public new TextRenderingMode VisualTextRenderingMode
        {
            get { return base.VisualTextRenderingMode; }
            set { base.VisualTextRenderingMode = value; }
        }

        public new TextHintingMode VisualTextHintingMode
        {
            get { return base.VisualTextHintingMode; }
            set { base.VisualTextHintingMode = value; }
        }

        public new EdgeMode VisualEdgeMode
        {
            get { return base.VisualEdgeMode; }
            set { base.VisualEdgeMode = value; }
        }

        public new BitmapScalingMode VisualBitmapScalingMode
        {
            get { return base.VisualBitmapScalingMode; }
            set { base.VisualBitmapScalingMode = value; }
        }
    }
}
