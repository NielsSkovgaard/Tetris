using System;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Model;

namespace Tetris.View
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
            for (int i = 0; i < _highScoreList.List.Count; i++)
            {
                _textBlockArrayHighScores[i] = BuildTextBlock(10, 100 + i * 20);
                Children.Add(_textBlockArrayHighScores[i]);
            }

            _gameBoard.GameBoardStatusChanged += GameBoard_GameBoardStatusChanged;
            _highScoreList.HighScoreListChanged += HighScoreList_HighScoreListChanged;
        }

        private TextBlock BuildTextBlock(double left, double top)
        {
            TextBlock textBlock = new TextBlock { Foreground = GraphicsConstants.TextBrush };
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
        private void HighScoreList_HighScoreListChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // Level, Score, Lines, Time
            _textBlockLevel.Text = "Level: " + _gameBoard.Level;
            _textBlockScore.Text = "Score: " + _gameBoard.Score;
            _textBlockLines.Text = "Lines: " + _gameBoard.Lines;
            _textBlockTime.Text = "Time: " + _gameBoard.Time;

            // HighScoreList
            for (int i = 0; i < _textBlockArrayHighScores.Length; i++)
            {
                HighScoreEntry highScoreEntry = _highScoreList.List[i];
                _textBlockArrayHighScores[i].Text = $"{highScoreEntry.Name} - {highScoreEntry.Score}";
            }
        }
    }
}
