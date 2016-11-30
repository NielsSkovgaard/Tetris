using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Models;

namespace Tetris.UI
{
    internal class StatisticsCanvas : Canvas
    {
        private readonly Statistics _statistics;

        private readonly TextBlock _textBlockLevel;
        private readonly TextBlock _textBlockScore;
        private readonly TextBlock _textBlockLines;
        private readonly TextBlock _textBlockTime;

        // Dependency injection of Statistics into StatisticsCanvas
        public StatisticsCanvas(Statistics statistics)
        {
            _statistics = statistics;

            // Level, Score, Lines, Time
            _textBlockLevel = BuildTextBlockAndAddToChildren(10);
            _textBlockScore = BuildTextBlockAndAddToChildren(30);
            _textBlockLines = BuildTextBlockAndAddToChildren(50);
            _textBlockTime = BuildTextBlockAndAddToChildren(70);

            _statistics.Changed += Statistics_Changed;
        }

        // Update the UI (StatisticsCanvas) every time the model (Statistics) changes
        // Soon after, the OnRender method is called
        private void Statistics_Changed(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // Level, Score, Lines, Time
            _textBlockLevel.Text = GetText("Level:", _statistics.Level.ToString(CultureInfo.InvariantCulture));
            _textBlockScore.Text = GetText("Score:", _statistics.Score.ToString(CultureInfo.InvariantCulture));
            _textBlockLines.Text = GetText("Lines:", _statistics.Lines.ToString(CultureInfo.InvariantCulture));
            _textBlockTime.Text = GetText("Time:", TimeSpan.FromSeconds(_statistics.Time).ToString("mm\\:ss"));
        }

        // ----------------------------------------------------------------------------------------
        // HELPER METHODS (same in StatisticsCanvas and HighScoresCanvas)
        // ----------------------------------------------------------------------------------------

        private TextBlock BuildTextBlockAndAddToChildren(double top, string text = null)
        {
            TextBlock textBlock = new TextBlock { Text = text ?? string.Empty, Foreground = Brushes.White, FontFamily = new FontFamily("Consolas, Courier New") };
            SetTop(textBlock, top);
            SetLeft(textBlock, 10);
            Children.Add(textBlock);
            return textBlock;
        }

        private string GetText(string labelText, string value)
        {
            const int totalTextLength = 15;
            return $"{labelText.PadRight(totalTextLength - value.Length, ' ')}{value}";
        }
    }
}
