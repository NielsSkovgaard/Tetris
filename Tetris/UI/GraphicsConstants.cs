using System.Windows.Media;

namespace Tetris.UI
{
    internal static class GraphicsConstants
    {
        public static Pen BlockBorderPen = new Pen { Brush = Brushes.White };
        public static SolidColorBrush TextBrush = Brushes.White;

        public static readonly SolidColorBrush[] BlockBrushes =
        {
            Brushes.Cyan,
            Brushes.Yellow,
            Brushes.Purple,
            Brushes.Blue,
            Brushes.Orange,
            Brushes.Green,
            Brushes.Red
        };
    }
}
