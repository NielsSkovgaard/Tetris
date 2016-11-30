using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Models;

namespace Tetris.UI
{
    internal class HighScoresCanvas : Canvas
    {
        private readonly HighScoreList _highScoreList;
        private readonly TextBlock[] _textBlockArrayHighScores = new TextBlock[5];

        // Dependency injection of HighScoreList into HighScoresCanvas
        public HighScoresCanvas(HighScoreList highScoreList)
        {
            _highScoreList = highScoreList;

            BuildTextBlockAndAddToChildren(10, "HIGH SCORES:");

            for (int i = 0; i < _highScoreList.List.Count; i++)
                _textBlockArrayHighScores[i] = BuildTextBlockAndAddToChildren(30 + i * 20);

            _highScoreList.Changed += HighScoreList_Changed;
        }

        // Update the UI (HighScoresCanvas) every time the model (HighScoreList) changes
        // Soon after, the OnRender method is called
        private void HighScoreList_Changed(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            for (int i = 0; i < _textBlockArrayHighScores.Length; i++)
            {
                HighScoreEntry highScoreEntry = _highScoreList.List[i];
                _textBlockArrayHighScores[i].Text = GetText(highScoreEntry.Initials, highScoreEntry.Score.ToString(CultureInfo.InvariantCulture));
            }
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
