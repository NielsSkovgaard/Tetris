using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tetris
{
    internal class StatusCanvas : Canvas
    {
        private readonly GameBoard _gameBoard;

        private readonly TextBlock _textBlockScore = new TextBlock { Text = "Score: 0", Foreground = GraphicsConstants.TextBrush };
        private readonly TextBlock _textBlockLevel = new TextBlock { Text = "Level: 0", Foreground = GraphicsConstants.TextBrush };
        private readonly TextBlock _textBlockLines = new TextBlock { Text = "Lines: 0", Foreground = GraphicsConstants.TextBrush };

        // Dependency injection of GameBoard into StatusCanvas
        public StatusCanvas(GameBoard gameBoard)
        {
            _gameBoard = gameBoard;

            // Score
            SetLeft(_textBlockScore, 10);
            SetTop(_textBlockScore, 10);
            Children.Add(_textBlockScore);

            // Level
            SetLeft(_textBlockLevel, 10);
            SetTop(_textBlockLevel, 30);
            Children.Add(_textBlockLevel);

            // Lines
            SetLeft(_textBlockLines, 10);
            SetTop(_textBlockLines, 50);
            Children.Add(_textBlockLines);

            _gameBoard.GameBoardStatusChanged += GameBoard_GameBoardStatusChanged;
        }

        // Update the UI (StatusCanvas) every time the model (GameBoard.Score, GameBoard.Level, GameBoard.Lines) changes
        // Soon after, the OnRender method is called
        private void GameBoard_GameBoardStatusChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            _textBlockScore.Text = "Score: " + _gameBoard.Score;
            _textBlockLevel.Text = "Level: " + _gameBoard.Level;
            _textBlockLines.Text = "Lines: " + _gameBoard.Lines;
        }

        private void AddTextBlocktoChildren(string text, double x, double y)
        {
            TextBlock textBlock = new TextBlock { Text = text, Foreground = GraphicsConstants.TextBrush };
            SetLeft(textBlock, x);
            SetTop(textBlock, y);
            Children.Add(textBlock);
        }
    }
}
