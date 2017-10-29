using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using Tetris.Models;

namespace Tetris.ViewModels
{
    internal class GameBoardViewModel
    {
        private readonly GameBoard _gameBoard;

        // Statistics (Level, Score, Lines, Time)
        private readonly Statistics _statistics;

        public bool IsGamePaused { get; private set; }

        // TimeSpans and DispatcherTimers:
        // Left/right: Repeat every x milliseconds while pressing Left and/or Right button
        // Rotate: Repeat every x milliseconds while pressing Up (rotate) button
        // Down: Either gravity (repeat every x milliseconds) or soft dropping (repeat every x milliseconds while pressing Down button)
        // Statistics.Time: Time the game has been running
        private readonly TimeSpan _timeSpanLeftRight = TimeSpan.FromMilliseconds(100); // 100 ms = 10 FPS
        private readonly TimeSpan _timeSpanRotate = TimeSpan.FromMilliseconds(250); // 250 ms = 4 FPS
        private readonly TimeSpan _timeSpanDownSoftDrop = TimeSpan.FromMilliseconds(50); // 50 ms = 20 FPS
        private readonly TimeSpan _timeSpanStatisticsTime = TimeSpan.FromMilliseconds(1000); // 1000 ms = 1 FPS
        private readonly int[] _downGravityIntervalsPerLevelMilliseconds = { 800, 656, 538, 441, 362, 297, 243, 199, 164, 134, 110, 90, 74, 61, 50 }; // 18% speed increase per level
        private readonly DispatcherTimer _timerLeftRight = new DispatcherTimer();
        private readonly DispatcherTimer _timerRotate = new DispatcherTimer();
        private readonly DispatcherTimer _timerDown = new DispatcherTimer();
        private readonly DispatcherTimer _timerStatisticsTime = new DispatcherTimer();

        // For handling that _timerMovePieceLeftOrRight runs when moving left or right; otherwise, timer should be stopped
        private bool _isLeftKeyDown;
        private bool _isRightKeyDown;

        // Priority horizontal direction when moving left and right at same time
        private bool _leftKeyHasPriority;

        private bool _isSoftDropping;

        public GameBoardViewModel(GameBoard gameBoard, Statistics statistics)
        {
            _gameBoard = gameBoard;
            _statistics = statistics;

            // Timers
            _timerLeftRight.Interval = _timeSpanLeftRight;
            _timerRotate.Interval = _timeSpanRotate;
            _timerStatisticsTime.Interval = _timeSpanStatisticsTime;
            _timerLeftRight.Tick += (sender, args) => TryMoveLeftOrRight();
            _timerRotate.Tick += (sender, args) => _gameBoard.TryRotateCurrentPiece();
            _timerDown.Tick += (sender, args) => MoveDownProcess();
            _timerStatisticsTime.Tick += (sender, args) => _statistics.IncrementTime();

            StartNewGame();
        }

        public void StartNewGame()
        {
            _gameBoard.Reset();
            _statistics.Reset();

            PauseGame();
            ResumeGame();
        }

        public void PauseGame()
        {
            IsGamePaused = true;

            _isLeftKeyDown = false;
            _isRightKeyDown = false;
            _isSoftDropping = false;

            // Timers
            _timerLeftRight.Stop();
            _timerRotate.Stop();
            _timerDown.Stop();
            _timerStatisticsTime.Stop();
        }

        public void ResumeGame()
        {
            IsGamePaused = false;

            // Timers
            _timerDown.Interval = DownGravityIntervalBasedOnLevel();
            _timerDown.Start();
            _timerStatisticsTime.Start();
        }

        private TimeSpan DownGravityIntervalBasedOnLevel()
        {
            return TimeSpan.FromMilliseconds(_downGravityIntervalsPerLevelMilliseconds[_statistics.Level - 1]);
        }

        public void KeyDown(Key key, bool isRepeat)
        {
            if (IsGamePaused || isRepeat)
                return;

            switch (key)
            {
                case Key.Left:
                case Key.A:
                    _isLeftKeyDown = true;
                    _leftKeyHasPriority = true;
                    _gameBoard.TryMoveCurrentPieceLeft();
                    _timerLeftRight.Start();
                    break;
                case Key.Right:
                case Key.D:
                    _isRightKeyDown = true;
                    _leftKeyHasPriority = false;
                    _gameBoard.TryMoveCurrentPieceRight();
                    _timerLeftRight.Start();
                    break;
                case Key.Up:
                case Key.W:
                    _gameBoard.TryRotateCurrentPiece();
                    _timerRotate.Start();
                    break;
                case Key.Down:
                case Key.S:
                    _isSoftDropping = true;
                    MoveDownProcess();
                    _timerDown.Interval = _timeSpanDownSoftDrop;
                    break;
                case Key.Space:
                    // TODO: Hard drop CurrentPiece (and handle that it won't result in clicking buttons)
                    break;
            }
        }

        public void KeyUp(Key key)
        {
            if (IsGamePaused)
                return;

            switch (key)
            {
                case Key.Left:
                case Key.A:
                    _isLeftKeyDown = false;
                    _leftKeyHasPriority = false;

                    if (!_isRightKeyDown)
                        _timerLeftRight.Stop();
                    break;
                case Key.Right:
                case Key.D:
                    _isRightKeyDown = false;
                    _leftKeyHasPriority = true;

                    if (!_isLeftKeyDown)
                        _timerLeftRight.Stop();
                    break;
                case Key.Up:
                case Key.W:
                    _timerRotate.Stop();
                    break;
                case Key.Down:
                case Key.S:
                    _isSoftDropping = false;
                    _timerDown.Interval = DownGravityIntervalBasedOnLevel();
                    break;
            }
        }

        private void TryMoveLeftOrRight()
        {
            if (_leftKeyHasPriority)
                _gameBoard.TryMoveCurrentPieceLeft();
            else
                _gameBoard.TryMoveCurrentPieceRight();
        }

        private void MoveDownProcess()
        {
            // TODO: Check inside this method that if not all CurrentPiece.Blocks fit on the GameBoard, don't move it down, but it's game over
            bool canMoveDown = _gameBoard.TryMoveCurrentPieceDown();

            if (canMoveDown)
            {
                if (_isSoftDropping)
                {
                    // Award points for soft dropping CurrentPiece
                    _statistics.IncrementScoreForSoftDroppingOneLine();
                }
            }
            else
            {
                // Game Over if either:
                // - not all CurrentPiece blocks fit on game board when trying to lock them (some of them are outside in top)
                // - NextPiece would collide with LockedBlocks if added
                bool allCurrentPieceBlocksFitOnGameBoard = _gameBoard.CurrentPiece.Blocks.All(block => _gameBoard.CurrentPiece.CoordsY + block.OffsetY >= 0);
                bool nextPieceCollidesWithLockedBlocks = _gameBoard.NextPieceCollidesWithLockedBlocks();

                // TODO: The first situation could actually be legal (therefore, extend LockedBlocks, but don't render the first few rows in the top)

                if (!allCurrentPieceBlocksFitOnGameBoard || nextPieceCollidesWithLockedBlocks)
                {
                    // Game Over: Stop all timers, and raise GameOver event (in order to eventually add high score)
                    PauseGame();
                    _gameBoard.OnGameOver(_statistics.Score);
                }
                else
                {
                    // Add CurrentPiece to LockedBlocks and remove complete rows
                    int numberOfCompleteRows = _gameBoard.AddCurrentPieceToLockedBlocksAndRemoveCompleteRows();

                    // Make CurrentPiece refer to NextPiece. Then build a new NextPiece
                    _gameBoard.UpdateCurrentPieceAndNextPiece();

                    // Update Level, Score, and Lines
                    // If reaching new level, make _timerMovePieceDown tick faster (unless we are soft dropping, which uses same timer)
                    int levelBefore = _statistics.Level;
                    _statistics.UpdateOnCompletingRows(numberOfCompleteRows);
                    int levelAfter = _statistics.Level;
                    if (levelAfter > levelBefore && !_isSoftDropping)
                        _timerDown.Interval = DownGravityIntervalBasedOnLevel();

                    _gameBoard.OnChanged();
                }
            }
        }
    }
}
