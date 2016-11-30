using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Models;

namespace Tetris.UI
{
    internal class StatisticsCanvas : Canvas
    {
        private readonly GameBoard _gameBoard;

        private readonly TextBlock _textBlockLevel;
        private readonly TextBlock _textBlockScore;
        private readonly TextBlock _textBlockLines;
        private readonly TextBlock _textBlockTime;

        // Dependency injection of GameBoard into StatisticsCanvas
        public StatisticsCanvas(GameBoard gameBoard)
        {
            _gameBoard = gameBoard;

            // Level, Score, Lines, Time
            _textBlockLevel = BuildTextBlockAndAddToChildren(10);
            _textBlockScore = BuildTextBlockAndAddToChildren(30);
            _textBlockLines = BuildTextBlockAndAddToChildren(50);
            _textBlockTime = BuildTextBlockAndAddToChildren(70);

            _gameBoard.StatisticsChanged += GameBoard_StatisticsChanged;
        }

        // Update the UI (StatisticsCanvas) every time the model (GameBoard properties Level, Score, Lines, and Time) changes
        // Soon after, the OnRender method is called
        private void GameBoard_StatisticsChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // Level, Score, Lines, Time
            _textBlockLevel.Text = GetText("Level:", _gameBoard.Level.ToString(CultureInfo.InvariantCulture));
            _textBlockScore.Text = GetText("Score:", _gameBoard.Score.ToString(CultureInfo.InvariantCulture));
            _textBlockLines.Text = GetText("Lines:", _gameBoard.Lines.ToString(CultureInfo.InvariantCulture));
            _textBlockTime.Text = GetText("Time:", TimeSpan.FromSeconds(_gameBoard.Time).ToString("mm\\:ss"));
        }

        // ----------------------------------------------------------------------------------------
        // HELPER METHODS (same in StatisticsCanvas and HighScoresCanvas)
        // ----------------------------------------------------------------------------------------

        private TextBlock BuildTextBlockAndAddToChildren(double top, string text = null)
        {
            TextBlock textBlock = new TextBlock { Text = text ?? string.Empty, Foreground = GraphicsConstants.TextBrush, FontFamily = new FontFamily("Consolas, Courier New") };
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
