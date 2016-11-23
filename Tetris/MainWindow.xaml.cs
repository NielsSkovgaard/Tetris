using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int VerticalBlocks = 20;
        private const int HorizontalBlocks = 10;
        private const int BlockSizeInPixels = 25;

        private readonly GameBoard _gameBoard = new GameBoard(VerticalBlocks, HorizontalBlocks, BlockSizeInPixels);

        public MainWindow()
        {
            InitializeComponent();

            GameCanvas gameCanvas = new GameCanvas(_gameBoard)
            {
                Name = "GameCanvas1",
                Height = 500,
                Width = 250,
                Margin = new Thickness(10, 10, 10, 10), //"10,10,234,61"
                Background = Brushes.Black
            };

            Grid1.Children.Add(gameCanvas);

            // TODO: This is just test code
            int[,] staticBlocks = _gameBoard.StaticBlocks;
            staticBlocks[19, 0] = 1;
            staticBlocks[19, 1] = 2;
            staticBlocks[19, 2] = 3;
            staticBlocks[19, 3] = 4;
            staticBlocks[19, 4] = 5;
            staticBlocks[19, 5] = 6;
            staticBlocks[19, 6] = 7;
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            _gameBoard.KeyPressed(e.Key);
        }
    }
}

// Official guidelines for building Tetris:
// http://tetris.wikia.com/wiki/Tetris_Guideline
// https://en.wikipedia.org/wiki/Tetromino

// TODO ITEMS:
// -----------
// TODO: Organize project as MVVM: http://www.markwithall.com/programming/2013/03/01/worlds-simplest-csharp-wpf-mvvm-example.html
// TODO: Play MIDI with game theme and sounds when putting pieces etc.
// TODO: Sometimes the program is still in task manager after closing the program? Maybe because of global exception handling in App class

// TODO: Have a separate Thread to do UI stuff:
// - https://chainding.wordpress.com/2011/07/07/build-more-responsive-apps-with-the-dispatcher/
// - http://stackoverflow.com/questions/5959217/wpf-forcing-redraw-of-canvas
// From StackOverflow: canvas.Dispatcher.Invoke(emptyDelegate, DispatcherPriority.Render); where emptyDelegate is Action emptyDelegate = delegate { };

// TODO: Is InvalidateVisual(); the right way to update the UI?
