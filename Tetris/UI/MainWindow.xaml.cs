using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Tetris.Models;

namespace Tetris.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Input parameters
        private const int Rows = 20;
        private const int Cols = 10;
        private const int BlockSizeInPixels = 25;

        private const int NextPieceRows = 6;
        private const int NextPieceCols = 6;
        private const int NextPieceBlockSizeInPixels = 20;

        private const string HighScoreListFilePath = "tetris_highscores.txt";

        private readonly GameBoard _gameBoard = new GameBoard(Rows, Cols);
        private readonly HighScoreList _highScoreList = new HighScoreList(HighScoreListFilePath);

        public MainWindow()
        {
            InitializeComponent();

            // Dependency injection of GameBoard into GameCanvas
            GameCanvas gameCanvas = new GameCanvas(_gameBoard, BlockSizeInPixels)
            {
                Height = Rows * BlockSizeInPixels, // Usually 500px
                Width = Cols * BlockSizeInPixels, // Usually 250px
                Margin = new Thickness(10),
                Background = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            // Dependency injection of GameBoard into NextPieceCanvas
            NextPieceCanvas nextPieceCanvas = new NextPieceCanvas(_gameBoard, NextPieceBlockSizeInPixels, NextPieceRows, NextPieceCols)
            {
                Height = NextPieceRows * NextPieceBlockSizeInPixels, // Usually 120px
                Width = NextPieceCols * NextPieceBlockSizeInPixels, // Usually 120px
                Margin = new Thickness(gameCanvas.Width + 20, 10, 10, 10),
                Background = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            // Dependency injection of GameBoard into StatusCanvas
            StatusCanvas statusCanvas = new StatusCanvas(_gameBoard, _highScoreList)
            {
                Height = gameCanvas.Height - nextPieceCanvas.Height - 10, // Usually 370px
                Width = nextPieceCanvas.Width, // Usually 120px
                Margin = new Thickness(gameCanvas.Width + 20, nextPieceCanvas.Height + 20, 10, 10),
                Background = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            Grid2.Children.Add(gameCanvas);
            Grid2.Children.Add(nextPieceCanvas);
            Grid2.Children.Add(statusCanvas);

            _gameBoard.GameOver += GameBoard_GameOver;
            HighScoreInputUserControl1.ButtonOk.Click += HighScoreInputUserControl1_ButtonOk_Click;
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            _gameBoard.KeyDown(e.Key, e.IsRepeat);
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            _gameBoard.KeyUp(e.Key);
        }

        private void GameBoard_GameOver(object sender, int score)
        {
            if (_highScoreList.IsRecord(score))
            {
                HighScoreInputUserControl1.Score = score;
                HighScoreInputUserControl1.TextBoxName.Text = string.Empty;

                RectangleOverlay.Visibility = Visibility.Visible;
                HighScoreInputUserControl1.Visibility = Visibility.Visible;
                HighScoreInputUserControl1.TextBoxName.Focus();
            }
        }

        private void HighScoreInputUserControl1_ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            _highScoreList.Add(HighScoreInputUserControl1.TextBoxName.Text.Trim(), HighScoreInputUserControl1.Score);

            RectangleOverlay.Visibility = Visibility.Collapsed;
            HighScoreInputUserControl1.Visibility = Visibility.Collapsed;
        }
    }
}

// ------------------------------------------------------------------------------------------------
// NOTES:
// ------------------------------------------------------------------------------------------------

// Standard rules: https://www.reddit.com/r/Tetris/comments/3jnsjy/best_versions_for_marathon_mode/
// 15 levels of increasing difficulty
// Each level requires to clear 10 lines to progress
// Scoring system:
// - Single = 100 points, Double = 300 points, Triple = 500 points, Tetris = 800 points
// - T-Single = 800 points, T-Double = 1200 points, T-Triple = 1600 points // TODO: Not implemented
// 50% Back-to-Back bonus & line clear points are multiplied by the current level
// 1 point per soft dropped row
// 2 points per hard dropped row // TODO: Not implemented

// Official guidelines for building Tetris:
// http://tetris.wikia.com/wiki/Tetris_Guideline
// https://en.wikipedia.org/wiki/Tetromino

// ------------------------------------------------------------------------------------------------
// TODO LIST:
// ------------------------------------------------------------------------------------------------

// TODO: Organize project as MVVM: http://www.markwithall.com/programming/2013/03/01/worlds-simplest-csharp-wpf-mvvm-example.html

// TODO: Have a separate Thread to do UI stuff:
// - https://chainding.wordpress.com/2011/07/07/build-more-responsive-apps-with-the-dispatcher/
// - http://stackoverflow.com/questions/5959217/wpf-forcing-redraw-of-canvas

// TODO: Increasing CurrentPiece speed
// TODO: Start/Pause buttons. See: http://www.colinfahey.com/tetris/tetris.html
// TODO: Have a lock delay? See: http://harddrop.com/wiki/Lock_delay
// TODO: Define WPF controls with XAML
// TODO: Have a grid with 10x20 predefined rectangles to color in the GameCanvas.OnRender method, instead of generating them on every rendering
