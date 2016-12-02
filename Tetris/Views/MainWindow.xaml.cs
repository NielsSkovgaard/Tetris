using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Tetris.Models;
using Tetris.ViewModels;

namespace Tetris.Views
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

        private readonly GameBoard _gameBoard;
        private readonly Statistics _statistics = new Statistics();
        private const string HighScoreListFilePath = "tetris_highscores.txt";
        private readonly HighScoreList _highScoreList = new HighScoreList(HighScoreListFilePath);

        private const int ElementsBorderThickness = 6;
        private const int ElementsSpacing = 16;

        private readonly ButtonsUserControl _buttonsUserControl;

        public MainWindow()
        {
            InitializeComponent();

            // ------------------------------------------------------------------------------------
            // GameBoardCore/LockedBlocksAndCurrentPieceCanvas: MVVM dependency injections
            // ------------------------------------------------------------------------------------

            // TODO: Rename to LockedBlocksAndCurrentPieceModel?
            GameBoardCore gameBoardCore = new GameBoardCore(Rows, Cols);

            // TODO: Rename to GameBoardViewModel or maybe LockedBlocksAndCurrentPieceViewModel?
            _gameBoard = new GameBoard(gameBoardCore, _statistics);

            LockedBlocksAndCurrentPieceCanvas lockedBlocksAndCurrentPieceCanvas = new LockedBlocksAndCurrentPieceCanvas(gameBoardCore, BlockSizeInPixels)
            {
                Height = Rows * BlockSizeInPixels, // Usually 500px
                Width = Cols * BlockSizeInPixels, // Usually 250px
                Background = Brushes.Black
            };

            Border lockedBlocksAndCurrentPieceCanvasBorder = BuildBorderForFrameworkElement(lockedBlocksAndCurrentPieceCanvas, ElementsBorderThickness);
            lockedBlocksAndCurrentPieceCanvasBorder.Margin = new Thickness(ElementsSpacing);

            // ------------------------------------------------------------------------------------
            // NextPiece: MVVM dependency injections
            // ------------------------------------------------------------------------------------

            NextPieceViewModel nextPieceViewModel = new NextPieceViewModel(gameBoardCore, NextPieceBlockSizeInPixels, NextPieceRows, NextPieceCols);

            NextPieceCanvas nextPieceCanvas = new NextPieceCanvas(nextPieceViewModel)
            {
                Height = NextPieceRows * NextPieceBlockSizeInPixels, // Usually 120px
                Width = NextPieceCols * NextPieceBlockSizeInPixels, // Usually 120px
                Background = Brushes.Black
            };

            Border nextPieceCanvasBorder = BuildBorderForFrameworkElement(nextPieceCanvas, ElementsBorderThickness);
            nextPieceCanvasBorder.Margin = new Thickness(lockedBlocksAndCurrentPieceCanvasBorder.Width + 2 * ElementsSpacing, ElementsSpacing, ElementsSpacing, ElementsSpacing);

            // ------------------------------------------------------------------------------------
            // Statistics: MVVM dependency injections
            // ------------------------------------------------------------------------------------

            StatisticsViewModel statisticsViewModel = new StatisticsViewModel(_statistics);

            StatisticsCanvas statisticsCanvas = new StatisticsCanvas(statisticsViewModel)
            {
                Height = 94,
                Width = nextPieceCanvas.Width, // Usually 120px
                Background = Brushes.Black
            };

            Border statisticsCanvasBorder = BuildBorderForFrameworkElement(statisticsCanvas, ElementsBorderThickness);
            statisticsCanvasBorder.Margin = new Thickness(lockedBlocksAndCurrentPieceCanvasBorder.Width + 2 * ElementsSpacing, nextPieceCanvasBorder.Height + 2 * ElementsSpacing, ElementsSpacing, ElementsSpacing);

            // ------------------------------------------------------------------------------------
            // HighScores: MVVM dependency injections
            // ------------------------------------------------------------------------------------

            HighScoresViewModel highScoresViewModel = new HighScoresViewModel(_highScoreList);

            HighScoresCanvas highScoresCanvas = new HighScoresCanvas(highScoresViewModel)
            {
                Height = 135,
                Width = nextPieceCanvas.Width, // Usually 120px
                Background = Brushes.Black
            };

            Border highScoresCanvasBorder = BuildBorderForFrameworkElement(highScoresCanvas, ElementsBorderThickness);
            highScoresCanvasBorder.Margin = new Thickness(lockedBlocksAndCurrentPieceCanvasBorder.Width + 2 * ElementsSpacing, nextPieceCanvasBorder.Height + statisticsCanvasBorder.Height + 3 * ElementsSpacing, ElementsSpacing, ElementsSpacing);

            // ------------------------------------------------------------------------------------
            // ButtonsUserControl
            // ------------------------------------------------------------------------------------

            _buttonsUserControl = new ButtonsUserControl
            {
                Height = lockedBlocksAndCurrentPieceCanvasBorder.Height - nextPieceCanvasBorder.Height - statisticsCanvasBorder.Height - highScoresCanvasBorder.Height - 3 * ElementsSpacing - 2 * ElementsBorderThickness,
                Width = nextPieceCanvas.Width, // Usually 120px
                Background = Brushes.Black
            };

            Border buttonsUserControlBorder = BuildBorderForFrameworkElement(_buttonsUserControl, ElementsBorderThickness);
            buttonsUserControlBorder.Margin = new Thickness(lockedBlocksAndCurrentPieceCanvasBorder.Width + 2 * ElementsSpacing, nextPieceCanvasBorder.Height + statisticsCanvasBorder.Height + highScoresCanvasBorder.Height + 4 * ElementsSpacing, ElementsSpacing, ElementsSpacing);

            // ------------------------------------------------------------------------------------
            // Add controls to grid
            // ------------------------------------------------------------------------------------

            Grid2.Children.Add(lockedBlocksAndCurrentPieceCanvasBorder);
            Grid2.Children.Add(nextPieceCanvasBorder);
            Grid2.Children.Add(statisticsCanvasBorder);
            Grid2.Children.Add(highScoresCanvasBorder);
            Grid2.Children.Add(buttonsUserControlBorder);

            // ------------------------------------------------------------------------------------
            // Event handling
            // ------------------------------------------------------------------------------------

            _buttonsUserControl.ButtonNewGame.Click += ButtonsUserControl_ButtonNewGame_Click;
            _buttonsUserControl.ButtonPauseResume.Click += ButtonsUserControl_ButtonPauseResume_Click;

            gameBoardCore.GameOver += GameBoard_GameOver;
            HighScoreInputUserControl1.ButtonOk.Click += HighScoreInputUserControl1_ButtonOk_Click;
            GameOverUserControl1.ButtonNewGame.Click += GameOverUserControl1_ButtonNewGame_Click;
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

        private void ButtonsUserControl_ButtonNewGame_Click(object sender, RoutedEventArgs e)
        {
            _gameBoard.StartNewGame();
            _buttonsUserControl.ButtonPauseResume.Content = "Pause";
        }

        private void ButtonsUserControl_ButtonPauseResume_Click(object sender, RoutedEventArgs e)
        {
            _gameBoard.PauseResumeGame();
            _buttonsUserControl.ButtonPauseResume.Content = _gameBoard.IsGamePaused ? "Resume" : "Pause";
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

                // See: http://stackoverflow.com/questions/13955340/keyboard-focus-does-not-work-on-text-box-in-wpf
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                {
                    HighScoreInputUserControl1.TextBoxInitials.Focus();
                    Keyboard.Focus(HighScoreInputUserControl1.TextBoxInitials);
                }));
            }
            else
            {
                RectangleOverlay.Visibility = Visibility.Visible;
                GameOverUserControl1.Visibility = Visibility.Visible;
            }
        }

        private void HighScoreInputUserControl1_ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            _highScoreList.Add(HighScoreInputUserControl1.TextBoxInitials.Text, HighScoreInputUserControl1.Score);

            RectangleOverlay.Visibility = Visibility.Collapsed;
            HighScoreInputUserControl1.Visibility = Visibility.Collapsed;
        }

        private void GameOverUserControl1_ButtonNewGame_Click(object sender, RoutedEventArgs e)
        {
            _gameBoard.StartNewGame();

            RectangleOverlay.Visibility = Visibility.Collapsed;
            GameOverUserControl1.Visibility = Visibility.Collapsed;
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

// TODO: Organize project using MVVM or MVVMC pattern
// - http://www.markwithall.com/programming/2013/03/01/worlds-simplest-csharp-wpf-mvvm-example.html
// - https://msdn.microsoft.com/en-us/library/ff798384.aspx
// - http://skimp-blog.blogspot.dk/2012/02/mvvm-is-dead-long-live-mvvmc.html

// TODO: Have a separate Thread to do UI stuff
// - https://chainding.wordpress.com/2011/07/07/build-more-responsive-apps-with-the-dispatcher/
// - http://stackoverflow.com/questions/5959217/wpf-forcing-redraw-of-canvas

// TODO: Have a Tetris lock delay? See: http://harddrop.com/wiki/Lock_delay
// TODO: Define WPF controls with XAML
// TODO: Consider having a grid with 10x20 predefined rectangles to color in the LockedBlocksAndCurrentPieceCanvas.OnRender method, instead of generating them on every rendering
