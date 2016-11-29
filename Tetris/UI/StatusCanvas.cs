using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Models;

namespace Tetris.UI
{
    internal class StatusCanvas : Canvas
    {
        private readonly GameBoard _gameBoard;
        private readonly HighScoreList _highScoreList;

        private readonly TextBlock _textBlockLevel;
        private readonly TextBlock _textBlockScore;
        private readonly TextBlock _textBlockLines;
        private readonly TextBlock _textBlockTime;

        private readonly TextBlock[] _textBlockArrayHighScores = new TextBlock[5];

        // Dependency injection of GameBoard and HighScoreList into StatusCanvas
        public StatusCanvas(GameBoard gameBoard, HighScoreList highScoreList)
        {
            _gameBoard = gameBoard;
            _highScoreList = highScoreList;

            // Level, Score, Lines, Time
            _textBlockLevel = BuildTextBlock(10, 10);
            _textBlockScore = BuildTextBlock(10, 30);
            _textBlockLines = BuildTextBlock(10, 50);
            _textBlockTime = BuildTextBlock(10, 70);
            Children.Add(_textBlockLevel);
            Children.Add(_textBlockScore);
            Children.Add(_textBlockLines);
            Children.Add(_textBlockTime);

            // HighScoreList
            Children.Add(BuildTextBlock(10, 200, "HIGH SCORES:"));

            for (int i = 0; i < _highScoreList.List.Count; i++)
            {
                _textBlockArrayHighScores[i] = BuildTextBlock(10, 220 + i * 20);
                Children.Add(_textBlockArrayHighScores[i]);
            }

            _gameBoard.GameBoardStatusChanged += GameBoard_GameBoardStatusChanged;
            _highScoreList.Changed += HighScoreList_Changed;
        }

        private TextBlock BuildTextBlock(double left, double top, string text = null)
        {
            TextBlock textBlock = new TextBlock { Text = text ?? string.Empty, Foreground = GraphicsConstants.TextBrush, FontFamily = new FontFamily("Consolas, Courier New") };
            SetLeft(textBlock, left);
            SetTop(textBlock, top);
            return textBlock;
        }

        // Update the UI (StatusCanvas) every time the model (GameBoard properties Level, Score, Lines, and Time) changes
        // Soon after, the OnRender method is called
        private void GameBoard_GameBoardStatusChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        // Update the UI (StatusCanvas) every time the model (HighScoreList) changes
        // Soon after, the OnRender method is called
        private void HighScoreList_Changed(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            CultureInfo culture = CultureInfo.InvariantCulture;

            // Level, Score, Lines, Time
            _textBlockLevel.Text = GetText("Level:", _gameBoard.Level.ToString(culture));
            _textBlockScore.Text = GetText("Score:", _gameBoard.Score.ToString(culture));
            _textBlockLines.Text = GetText("Lines:", _gameBoard.Lines.ToString(culture));
            _textBlockTime.Text = GetText("Time:", TimeSpan.FromSeconds(_gameBoard.Time).ToString("mm\\:ss"));

            // HighScoreList
            for (int i = 0; i < _textBlockArrayHighScores.Length; i++)
            {
                HighScoreEntry highScoreEntry = _highScoreList.List[i];
                _textBlockArrayHighScores[i].Text = GetText(highScoreEntry.Initials, highScoreEntry.Score.ToString(culture));
            }
        }

        private string GetText(string labelText, string value)
        {
            const int totalTextLength = 15;
            return $"{labelText.PadRight(totalTextLength - value.Length, ' ')}{value}";
        }
    }
}
