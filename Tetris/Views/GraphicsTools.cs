using System.Windows.Controls;
using System.Windows.Media;

namespace Tetris.Views
{
    internal static class GraphicsTools
    {
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

        public static TextBlock BuildTextBlockAndAddToChildren(Panel panel, double top, string text = null)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text ?? string.Empty,
                Foreground = Brushes.White,
                FontFamily = new FontFamily("Consolas, Courier New")
            };

            Canvas.SetTop(textBlock, top);
            Canvas.SetLeft(textBlock, 10);
            panel.Children.Add(textBlock);
            return textBlock;
        }
    }
}
