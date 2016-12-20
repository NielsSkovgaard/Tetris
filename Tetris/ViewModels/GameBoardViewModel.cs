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
        public bool IsGameOver { get; private set; }

        // TimeSpans and Timers:
        // - for holding down a key to repeat a command (move left/right or rotate) every x milliseconds
        private readonly TimeSpan _timeSpanMoveCurrentPieceLeftOrRight = TimeSpan.FromMilliseconds(100); // 100 ms = 10 FPS
        private readonly TimeSpan _timeSpanRotateCurrentPiece = TimeSpan.FromMilliseconds(250); // 250 ms = 4 FPS
        private readonly DispatcherTimer _timerMoveCurrentPieceLeftOrRight = new DispatcherTimer();
        private readonly DispatcherTimer _timerRotateCurrentPiece = new DispatcherTimer();

        // - to add gravity to CurrentPiece. It will move down faster when soft dropping (by holding down the down key)
        // Every level increases the speed by 18%. Notice that the max level speed is equal to the soft drop speed
        private readonly int[] _moveCurrentPieceDownIntervalsInMilisecondsPerLevel = { 800, 656, 538, 441, 362, 297, 243, 199, 164, 134, 110, 90, 74, 61, 50 };

        private readonly TimeSpan _timeSpanMoveCurrentPieceDownSoftDrop = TimeSpan.FromMilliseconds(50); // 50 ms = 20 FPS
        private readonly DispatcherTimer _timerMoveCurrentPieceDown = new DispatcherTimer();

        // - to count the number of seconds the game has been running
        private readonly TimeSpan _timeSpanSecondsGameHasBeenRunning = TimeSpan.FromMilliseconds(1000); // 1000 ms = 1 FPS
        private readonly DispatcherTimer _timerSecondsGameHasBeenRunning = new DispatcherTimer();

        // For handling that _timerMovePieceLeftOrRight runs when moving left or right; otherwise, the timer should be stopped
        private bool _isLeftKeyDown;
        private bool _isRightKeyDown;

        // Priority horizontal direction when moving left and right at the same time
        private bool _leftKeyHasPriority;

        private bool _isSoftDropping;

        public GameBoardViewModel(GameBoard gameBoard, Statistics statistics)
        {
            _gameBoard = gameBoard;
            _statistics = statistics;

            // DispatcherTimer.Interval
            _timerMoveCurrentPieceLeftOrRight.Interval = _timeSpanMoveCurrentPieceLeftOrRight;
            _timerRotateCurrentPiece.Interval = _timeSpanRotateCurrentPiece;
            _timerSecondsGameHasBeenRunning.Interval = _timeSpanSecondsGameHasBeenRunning;

            // DispatcherTimer.Tick
            _timerMoveCurrentPieceLeftOrRight.Tick += (sender, args) => TryMoveCurrentPieceLeftOrRight();
            _timerRotateCurrentPiece.Tick += (sender, args) => _gameBoard.TryRotateCurrentPiece();
            _timerMoveCurrentPieceDown.Tick += (sender, args) => MoveCurrentPieceDownProcess();
            _timerSecondsGameHasBeenRunning.Tick += (sender, args) => _statistics.IncrementTime();

            StartNewGame();
        }

        public void StartNewGame()
        {
            _gameBoard.Reset();
            _statistics.Reset();

            // Calling PauseResumeGame() when IsGamePaused is true will "resume" the game, or in other words start it
            IsGameOver = false;
            IsGamePaused = true;
            PauseResumeGame();
        }

        public void PauseResumeGame()
        {
            if (IsGameOver)
                return;

            _timerMoveCurrentPieceLeftOrRight.Stop();
            _timerRotateCurrentPiece.Stop();

            if (IsGamePaused)
            {
                // Resume game
                IsGamePaused = false;

                _isLeftKeyDown = false;
                _isRightKeyDown = false;
                _leftKeyHasPriority = false;
                _isSoftDropping = false;

                // Timers
                _timerMoveCurrentPieceDown.Interval = GetMoveCurrentPieceDownTimerIntervalBasedOnLevel();
                _timerMoveCurrentPieceDown.Start();
                _timerSecondsGameHasBeenRunning.Start();
            }
            else
            {
                // Pause game
                IsGamePaused = true;

                // Timers
                _timerMoveCurrentPieceDown.Stop();
                _timerSecondsGameHasBeenRunning.Stop();
            }
        }

        private TimeSpan GetMoveCurrentPieceDownTimerIntervalBasedOnLevel()
        {
            return TimeSpan.FromMilliseconds(_moveCurrentPieceDownIntervalsInMilisecondsPerLevel[_statistics.Level - 1]);
        }

        public void KeyDown(Key key, bool isRepeat)
        {
            if (IsGameOver || IsGamePaused || isRepeat)
                return;

            switch (key)
            {
                case Key.Left:
                case Key.A:
                    _isLeftKeyDown = true;
                    _leftKeyHasPriority = true;
                    _gameBoard.TryMoveCurrentPieceLeft();
                    _timerMoveCurrentPieceLeftOrRight.Start();
                    break;
                case Key.Right:
                case Key.D:
                    _isRightKeyDown = true;
                    _leftKeyHasPriority = false;
                    _gameBoard.TryMoveCurrentPieceRight();
                    _timerMoveCurrentPieceLeftOrRight.Start();
                    break;
                case Key.Up:
                case Key.W:
                    _gameBoard.TryRotateCurrentPiece();
                    _timerRotateCurrentPiece.Start();
                    break;
                case Key.Down:
                case Key.S:
                    _isSoftDropping = true;
                    MoveCurrentPieceDownProcess();
                    _timerMoveCurrentPieceDown.Interval = _timeSpanMoveCurrentPieceDownSoftDrop;
                    break;
                case Key.Space:
                    // TODO: Hard drop the CurrentPiece (and handle that it won't result in clicking the buttons)
                    break;
            }
        }

        public void KeyUp(Key key)
        {
            if (IsGameOver || IsGamePaused)
                return;

            switch (key)
            {
                case Key.Left:
                case Key.A:
                    _isLeftKeyDown = false;
                    _leftKeyHasPriority = false;

                    if (!_isRightKeyDown)
                        _timerMoveCurrentPieceLeftOrRight.Stop();
                    break;
                case Key.Right:
                case Key.D:
                    _isRightKeyDown = false;
                    _leftKeyHasPriority = true;

                    if (!_isLeftKeyDown)
                        _timerMoveCurrentPieceLeftOrRight.Stop();
                    break;
                case Key.Up:
                case Key.W:
                    _timerRotateCurrentPiece.Stop();
                    break;
                case Key.Down:
                case Key.S:
                    _isSoftDropping = false;
                    _timerMoveCurrentPieceDown.Interval = GetMoveCurrentPieceDownTimerIntervalBasedOnLevel();
                    break;
            }
        }

        private void TryMoveCurrentPieceLeftOrRight()
        {
            if (_leftKeyHasPriority)
                _gameBoard.TryMoveCurrentPieceLeft();
            else
                _gameBoard.TryMoveCurrentPieceRight();
        }

        private void MoveCurrentPieceDownProcess()
        {
            // TODO: Check inside this method that if not all CurrentPiece.Blocks fit on the GameBoard, then don't move it down, but it's game over
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
                // - not all CurrentPiece blocks fit on the game board when trying to lock them (some of them are outside in the top)
                // - NextPiece would collide with LockedBlocks if added
                bool allCurrentPieceBlocksFitOnGameBoard = _gameBoard.CurrentPiece.Blocks.All(block => _gameBoard.CurrentPiece.CoordsY + block.OffsetY >= 0);
                bool nextPieceCollidesWithLockedBlocks = _gameBoard.NextPieceCollidesWithLockedBlocks();

                // TODO: The first situation could actually be legal (therefore, extend LockedBlocks, but don't render the first few rows in the top)

                if (!allCurrentPieceBlocksFitOnGameBoard || nextPieceCollidesWithLockedBlocks)
                {
                    // Game Over: Stop all timers, and raise GameOver event (in order to eventually add high score)
                    IsGameOver = true;

                    _timerMoveCurrentPieceLeftOrRight.Stop();
                    _timerRotateCurrentPiece.Stop();
                    _timerMoveCurrentPieceDown.Stop();
                    _timerSecondsGameHasBeenRunning.Stop();

                    _gameBoard.OnGameOver(_statistics.Score);
                }
                else
                {
                    // Add CurrentPiece to LockedBlocks and remove complete rows
                    int numberOfCompleteRows = _gameBoard.AddCurrentPieceToLockedBlocksAndRemoveCompleteRows();

                    // Make CurrentPiece refer to NextPiece. Then build a new NextPiece
                    _gameBoard.UpdateCurrentPieceAndNextPiece();

                    // Update Level, Score, and Lines
                    // If reaching a new level, make the _timerMovePieceDown tick faster (unless we are soft dropping, which is using the same timer)
                    int levelBefore = _statistics.Level;
                    _statistics.UpdateOnCompletingRows(numberOfCompleteRows);
                    int levelAfter = _statistics.Level;
                    if (levelAfter > levelBefore && !_isSoftDropping)
                        _timerMoveCurrentPieceDown.Interval = GetMoveCurrentPieceDownTimerIntervalBasedOnLevel();

                    _gameBoard.OnChanged();
                }
            }
        }
    }
}
