using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Tetris.Model;
using Tetris.View;

namespace Tetris
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

            Grid1.Children.Add(gameCanvas);
            Grid1.Children.Add(nextPieceCanvas);
            Grid1.Children.Add(statusCanvas);
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            _gameBoard.KeyDown(e.Key, e.IsRepeat);
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            _gameBoard.KeyUp(e.Key);
        }
    }
}

// Official guidelines for building Tetris:
// http://tetris.wikia.com/wiki/Tetris_Guideline
// https://en.wikipedia.org/wiki/Tetromino

// TODO ITEMS:
// -----------
// TODO: Organize project as MVVM: http://www.markwithall.com/programming/2013/03/01/worlds-simplest-csharp-wpf-mvvm-example.html

// TODO: Have a separate Thread to do UI stuff:
// - https://chainding.wordpress.com/2011/07/07/build-more-responsive-apps-with-the-dispatcher/
// - http://stackoverflow.com/questions/5959217/wpf-forcing-redraw-of-canvas
// From StackOverflow: canvas.Dispatcher.Invoke(emptyDelegate, DispatcherPriority.Render); where emptyDelegate is Action emptyDelegate = delegate { };

// TODO: Is InvalidateVisual(); the right way to update the UI?
// TODO: Increasing Piece speed
// TODO: Start/Pause buttons -- see http://www.colinfahey.com/tetris/tetris.html
// TODO: Consider having extra space in top of rows for new piece, and a line showing the border above which the Piece will give Game Over
// TODO: Correct y positioning of Piece in the top of the game board
// TODO: Stop all timers on game over