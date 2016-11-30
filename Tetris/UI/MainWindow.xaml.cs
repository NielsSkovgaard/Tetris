using System.Windows;
using System.Windows.Controls;
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
        private const int Cols = 10; // Has to be >= 4
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

            const int borderThickness = 5;
            const int spacingBetweenElements = 20;

            // Dependency injection of GameBoard into GameCanvas
            GameCanvas gameCanvas = new GameCanvas(_gameBoard, BlockSizeInPixels)
            {
                Height = Rows * BlockSizeInPixels, // Usually 500px
                Width = Cols * BlockSizeInPixels, // Usually 250px
                Background = Brushes.Black
            };

            Border gameCanvasBorder = BuildBorderForFrameworkElement(gameCanvas, borderThickness);
            gameCanvasBorder.Margin = new Thickness(spacingBetweenElements);

            // Dependency injection of GameBoard into NextPieceCanvas
            NextPieceCanvas nextPieceCanvas = new NextPieceCanvas(_gameBoard, NextPieceBlockSizeInPixels, NextPieceRows, NextPieceCols)
            {
                Height = NextPieceRows * NextPieceBlockSizeInPixels, // Usually 120px
                Width = NextPieceCols * NextPieceBlockSizeInPixels, // Usually 120px
                Background = Brushes.Black
            };

            Border nextPieceCanvasBorder = BuildBorderForFrameworkElement(nextPieceCanvas, borderThickness);
            nextPieceCanvasBorder.Margin = new Thickness(gameCanvasBorder.Width + 2 * spacingBetweenElements, spacingBetweenElements, spacingBetweenElements, spacingBetweenElements);

            // Dependency injection of GameBoard into StatisticsCanvas
            StatisticsCanvas statisticsCanvas = new StatisticsCanvas(_gameBoard)
            {
                Height = 95, // TODO! Before: = gameCanvasBorder.Height - nextPieceCanvasBorder.Height - spacingBetweenElements - 2 * borderThickness,
                Width = nextPieceCanvas.Width, // Usually 120px
                Background = Brushes.Black
            };

            Border statisticsCanvasBorder = BuildBorderForFrameworkElement(statisticsCanvas, borderThickness);
            statisticsCanvasBorder.Margin = new Thickness(gameCanvasBorder.Width + 2 * spacingBetweenElements, nextPieceCanvasBorder.Height + 2 * spacingBetweenElements, spacingBetweenElements, spacingBetweenElements);

            // Dependency injection of HighScoreList into HighScoresCanvas
            HighScoresCanvas highScoresCanvas = new HighScoresCanvas(_highScoreList)
            {
                Height = 136, // TODO
                Width = nextPieceCanvas.Width, // Usually 120px
                Background = Brushes.Black
            };

            Border highScoresCanvasBorder = BuildBorderForFrameworkElement(highScoresCanvas, borderThickness);
            highScoresCanvasBorder.Margin = new Thickness(gameCanvasBorder.Width + 2 * spacingBetweenElements, nextPieceCanvasBorder.Height + statisticsCanvasBorder.Height + 3 * spacingBetweenElements, spacingBetweenElements, spacingBetweenElements);

            Grid2.Children.Add(gameCanvasBorder);
            Grid2.Children.Add(nextPieceCanvasBorder);
            Grid2.Children.Add(statisticsCanvasBorder);
            Grid2.Children.Add(highScoresCanvasBorder);

            _gameBoard.GameOver += GameBoard_GameOver;
            HighScoreInputUserControl1.ButtonOk.Click += HighScoreInputUserControl1_ButtonOk_Click;
        }

        private Border BuildBorderForFrameworkElement(FrameworkElement element, int borderWidth)
        {
            return new Border
            {
                Height = element.Height + 2 * borderWidth,
                Width = element.Width + 2 * borderWidth,
                BorderThickness = new Thickness(borderWidth),
                BorderBrush = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Child = element
            };
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
                HighScoreInputUserControl1.TextBoxInitials.Text = string.Empty;

                RectangleOverlay.Visibility = Visibility.Visible;
                HighScoreInputUserControl1.Visibility = Visibility.Visible;
                HighScoreInputUserControl1.TextBoxInitials.Focus();
            }
        }

        private void HighScoreInputUserControl1_ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            _highScoreList.Add(HighScoreInputUserControl1.TextBoxInitials.Text, HighScoreInputUserControl1.Score);

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

// Guidelines for building Tetris:
// http://tetris.wikia.com/wiki/Tetris_Guideline
// https://en.wikipedia.org/wiki/Tetromino
// http://www.colinfahey.com/tetris/tetris.html

// Js Tetris: http://web.itu.edu.tr/~msilgu/tetris/tetris.html

// ------------------------------------------------------------------------------------------------
// TODO LIST:
// ------------------------------------------------------------------------------------------------

// TODO: Organize project with MVVM pattern
// - http://www.markwithall.com/programming/2013/03/01/worlds-simplest-csharp-wpf-mvvm-example.html
// - https://msdn.microsoft.com/en-us/library/ff798384.aspx

// TODO: Have a separate Thread to do UI stuff
// - https://chainding.wordpress.com/2011/07/07/build-more-responsive-apps-with-the-dispatcher/
// - http://stackoverflow.com/questions/5959217/wpf-forcing-redraw-of-canvas

// TODO: Start/Pause buttons. See: http://www.colinfahey.com/tetris/tetris.html
// TODO: Have a lock delay? See: http://harddrop.com/wiki/Lock_delay
// TODO: Define WPF controls with XAML
// TODO: Have a grid with 10x20 predefined rectangles to color in the GameCanvas.OnRender method, instead of generating them on every rendering
// TODO: Focus TextBox in popup to enter high score initials
