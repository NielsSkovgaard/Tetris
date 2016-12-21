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
    internal partial class MainWindow : Window
    {
        // Parameters
        private const int Rows = 20;
        private const int Cols = 10; // Has to be >= 4
        private const int BlockSizeInPixels = 25;
        private const int NextPieceRows = 6;
        private const int NextPieceCols = 6;
        private const int NextPieceBlockSizeInPixels = 20;
        private const int ElementsBorderThickness = 6;
        private const int ElementsSpacing = 16;
        private const string HighScoreListFilePath = "tetris_highscores.txt";

        private readonly Statistics _statistics = new Statistics();
        private readonly HighScoreList _highScoreList = new HighScoreList(HighScoreListFilePath);

        private GameBoardViewModel _gameBoardViewModel;
        private ButtonsUserControl _buttonsUserControl;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            // GameBoard
            GameBoard gameBoard = new GameBoard(Rows, Cols);
            _gameBoardViewModel = new GameBoardViewModel(gameBoard, _statistics);

            GameBoardCanvas gameBoardCanvas = new GameBoardCanvas(gameBoard, BlockSizeInPixels)
            {
                Height = Rows * BlockSizeInPixels, // Usually 500px
                Width = Cols * BlockSizeInPixels // Usually 250px
            };

            Border gameBoardCanvasBorder = BuildBorderForFrameworkElement(gameBoardCanvas, ElementsBorderThickness);
            gameBoardCanvasBorder.Margin = new Thickness(ElementsSpacing);

            // NextPiece
            NextPieceViewModel nextPieceViewModel = new NextPieceViewModel(gameBoard, NextPieceBlockSizeInPixels, NextPieceRows, NextPieceCols);

            NextPieceCanvas nextPieceCanvas = new NextPieceCanvas(nextPieceViewModel)
            {
                Height = NextPieceRows * NextPieceBlockSizeInPixels, // Usually 120px
                Width = NextPieceCols * NextPieceBlockSizeInPixels // Usually 120px
            };

            Border nextPieceCanvasBorder = BuildBorderForFrameworkElement(nextPieceCanvas, ElementsBorderThickness);
            nextPieceCanvasBorder.Margin = new Thickness(gameBoardCanvasBorder.Width + 2 * ElementsSpacing, ElementsSpacing, ElementsSpacing, ElementsSpacing);

            // Statistics
            StatisticsViewModel statisticsViewModel = new StatisticsViewModel(_statistics);

            StatisticsUserControl statisticsUserControl = new StatisticsUserControl(statisticsViewModel)
            {
                Height = 94,
                Width = nextPieceCanvas.Width // Usually 120px
            };

            Border statisticsCanvasBorder = BuildBorderForFrameworkElement(statisticsUserControl, ElementsBorderThickness);
            statisticsCanvasBorder.Margin = new Thickness(gameBoardCanvasBorder.Width + 2 * ElementsSpacing, nextPieceCanvasBorder.Height + 2 * ElementsSpacing, ElementsSpacing, ElementsSpacing);

            // Highscores
            HighScoresViewModel highScoresViewModel = new HighScoresViewModel(_highScoreList);

            HighScoresUserControl highScoresUserControl = new HighScoresUserControl(highScoresViewModel)
            {
                Height = 135,
                Width = nextPieceCanvas.Width // Usually 120px
            };

            Border highScoresCanvasBorder = BuildBorderForFrameworkElement(highScoresUserControl, ElementsBorderThickness);
            highScoresCanvasBorder.Margin = new Thickness(gameBoardCanvasBorder.Width + 2 * ElementsSpacing, nextPieceCanvasBorder.Height + statisticsCanvasBorder.Height + 3 * ElementsSpacing, ElementsSpacing, ElementsSpacing);

            // ButtonsUserControl
            _buttonsUserControl = new ButtonsUserControl
            {
                Height = gameBoardCanvasBorder.Height - nextPieceCanvasBorder.Height - statisticsCanvasBorder.Height - highScoresCanvasBorder.Height - 3 * ElementsSpacing - 2 * ElementsBorderThickness,
                Width = nextPieceCanvas.Width // Usually 120px
            };

            Border buttonsUserControlBorder = BuildBorderForFrameworkElement(_buttonsUserControl, ElementsBorderThickness);
            buttonsUserControlBorder.Margin = new Thickness(gameBoardCanvasBorder.Width + 2 * ElementsSpacing, nextPieceCanvasBorder.Height + statisticsCanvasBorder.Height + highScoresCanvasBorder.Height + 4 * ElementsSpacing, ElementsSpacing, ElementsSpacing);

            // Make all backgrounds black
            gameBoardCanvas.Background = Brushes.Black;
            nextPieceCanvas.Background = Brushes.Black;
            statisticsUserControl.Background = Brushes.Black;
            highScoresUserControl.Background = Brushes.Black;
            _buttonsUserControl.Background = Brushes.Black;

            // Add controls to grid
            Grid2.Children.Add(gameBoardCanvasBorder);
            Grid2.Children.Add(nextPieceCanvasBorder);
            Grid2.Children.Add(statisticsCanvasBorder);
            Grid2.Children.Add(highScoresCanvasBorder);
            Grid2.Children.Add(buttonsUserControlBorder);

            // Event handling
            _buttonsUserControl.ButtonNewGame.Click += ButtonsUserControl_ButtonNewGame_Click;
            _buttonsUserControl.ButtonPauseResume.Click += ButtonsUserControl_ButtonPauseResume_Click;
            gameBoard.GameOver += GameBoard_GameOver;
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
            _gameBoardViewModel.StartNewGame();
            _buttonsUserControl.ButtonPauseResume.IsEnabled = true;
            _buttonsUserControl.ButtonPauseResume.Content = "Pause";
        }

        private void ButtonsUserControl_ButtonPauseResume_Click(object sender, RoutedEventArgs e)
        {
            if (_gameBoardViewModel.IsGamePaused)
            {
                _gameBoardViewModel.ResumeGame();
                _buttonsUserControl.ButtonPauseResume.Content = "Pause";
            }
            else
            {
                _gameBoardViewModel.PauseGame();
                _buttonsUserControl.ButtonPauseResume.Content = "Resume";
            }
        }

        private void MainWindow_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Lines 2-3 ensure that the game doesn't pause when clicking GameOverUserControl.ButtonNewGame or when the focus returns to the window afterwards
            if (!_gameBoardViewModel.IsGamePaused &&
                !Equals(e.KeyboardDevice.FocusedElement, _buttonsUserControl.ButtonNewGame) &&
                !Equals(e.KeyboardDevice.FocusedElement, this))
            {
                _gameBoardViewModel.PauseGame();
                _buttonsUserControl.ButtonPauseResume.Content = "Resume";
            }
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            _gameBoardViewModel.KeyDown(e.Key, e.IsRepeat);
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            _gameBoardViewModel.KeyUp(e.Key);
        }

        private void GameBoard_GameOver(object sender, int score)
        {
            _buttonsUserControl.ButtonPauseResume.IsEnabled = false;

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
            _gameBoardViewModel.StartNewGame();
            _buttonsUserControl.ButtonPauseResume.IsEnabled = true;
            _buttonsUserControl.ButtonPauseResume.Content = "Pause";

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

// TODO: Organize project using MVVM or MVVMC pattern (Status: DONE!)
// - http://www.markwithall.com/programming/2013/03/01/worlds-simplest-csharp-wpf-mvvm-example.html
// - https://msdn.microsoft.com/en-us/library/ff798384.aspx
// - http://skimp-blog.blogspot.dk/2012/02/mvvm-is-dead-long-live-mvvmc.html

// TODO: Have a separate Thread to do UI stuff
// - https://chainding.wordpress.com/2011/07/07/build-more-responsive-apps-with-the-dispatcher/
// - http://stackoverflow.com/questions/5959217/wpf-forcing-redraw-of-canvas

// TODO: Have a Tetris lock delay? See: http://harddrop.com/wiki/Lock_delay
// TODO: Define WPF controls with XAML
