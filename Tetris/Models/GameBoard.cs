using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace Tetris.Models
{
    internal class GameBoard
    {
        private readonly GameBoardCore _gameBoardCore;

        // Statistics (Level, Score, Lines, Time)
        private readonly Statistics _statistics;

        public bool IsGamePaused { get; private set; }
        public bool IsGameOver { get; private set; }

        // TimeSpans and Timers:
        // - for holding down a key to repeat a command (move left/right or rotate) every x milliseconds
        private readonly TimeSpan _timeSpanMovePieceLeftOrRight = TimeSpan.FromMilliseconds(100); // 100 ms = 10 FPS
        private readonly TimeSpan _timeSpanRotatePiece = TimeSpan.FromMilliseconds(250); // 250 ms = 4 FPS
        private readonly DispatcherTimer _timerMovePieceLeftOrRight = new DispatcherTimer();
        private readonly DispatcherTimer _timerRotatePiece = new DispatcherTimer();

        // - to add gravity to CurrentPiece. It will move down faster when soft dropping (by holding down the down key)
        // Every level increases the speed by 18%. Notice that the max level speed is equal to the soft drop speed
        private readonly int[] _movePieceDownIntervalsInMilisecondsPerLevel = { 800, 656, 538, 441, 362, 297, 243, 199, 164, 134, 110, 90, 74, 61, 50 };

        private readonly TimeSpan _timeSpanMovePieceDownSoftDrop = TimeSpan.FromMilliseconds(50); // 50 ms = 20 FPS
        private readonly DispatcherTimer _timerMovePieceDown = new DispatcherTimer();

        // - to count the number of seconds the game has been running
        private readonly TimeSpan _timeSpanSecondsGameHasBeenRunning = TimeSpan.FromMilliseconds(1000); // 1000 ms = 1 FPS
        private readonly DispatcherTimer _timerSecondsGameHasBeenRunning = new DispatcherTimer();

        // Handle that _timerMovePieceLeftOrRight runs when moving left or right; otherwise, it should be stopped
        private bool _isLeftKeyDown;
        private bool _isRightKeyDown;

        // Priority horizontal direction when moving left and right at the same time
        private bool _leftKeyHasPriority;

        private bool _isSoftDropping;

        public GameBoard(GameBoardCore gameBoardCore, Statistics statistics)
        {
            _gameBoardCore = gameBoardCore;
            _statistics = statistics;

            // Timers
            _timerMovePieceLeftOrRight.Interval = _timeSpanMovePieceLeftOrRight;
            _timerRotatePiece.Interval = _timeSpanRotatePiece;
            _timerSecondsGameHasBeenRunning.Interval = _timeSpanSecondsGameHasBeenRunning;
            _timerMovePieceLeftOrRight.Tick += (sender, args) => TryMovePieceLeftOrRight();
            _timerRotatePiece.Tick += (sender, args) => _gameBoardCore.TryRotateCurrentPiece();
            _timerMovePieceDown.Tick += (sender, args) => MoveCurrentPieceDownProcess();
            _timerSecondsGameHasBeenRunning.Tick += (sender, args) => _statistics.IncrementTime();

            StartNewGame();
        }

        public void StartNewGame()
        {
            _gameBoardCore.Reset();
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

            _timerMovePieceLeftOrRight.Stop();
            _timerRotatePiece.Stop();

            if (IsGamePaused)
            {
                // Resume game
                IsGamePaused = false;

                _isLeftKeyDown = false;
                _isRightKeyDown = false;
                _leftKeyHasPriority = false;
                _isSoftDropping = false;

                // Timers
                _timerMovePieceDown.Interval = GetMovePieceDownTimerIntervalBasedOnLevel();
                _timerMovePieceDown.Start();
                _timerSecondsGameHasBeenRunning.Start();
            }
            else
            {
                // Pause game
                IsGamePaused = true;

                // Timers
                _timerMovePieceDown.Stop();
                _timerSecondsGameHasBeenRunning.Stop();
            }
        }

        private TimeSpan GetMovePieceDownTimerIntervalBasedOnLevel()
        {
            return TimeSpan.FromMilliseconds(_movePieceDownIntervalsInMilisecondsPerLevel[_statistics.Level - 1]);
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
                    _gameBoardCore.TryMoveCurrentPieceLeft();
                    _timerMovePieceLeftOrRight.Start();
                    break;
                case Key.Right:
                case Key.D:
                    _isRightKeyDown = true;
                    _leftKeyHasPriority = false;
                    _gameBoardCore.TryMoveCurrentPieceRight();
                    _timerMovePieceLeftOrRight.Start();
                    break;
                case Key.Up:
                case Key.W:
                    _gameBoardCore.TryRotateCurrentPiece();
                    _timerRotatePiece.Start();
                    break;
                case Key.Down:
                case Key.S:
                    _isSoftDropping = true;
                    MoveCurrentPieceDownProcess();
                    _timerMovePieceDown.Interval = _timeSpanMovePieceDownSoftDrop;
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
                        _timerMovePieceLeftOrRight.Stop();
                    break;
                case Key.Right:
                case Key.D:
                    _isRightKeyDown = false;
                    _leftKeyHasPriority = true;

                    if (!_isLeftKeyDown)
                        _timerMovePieceLeftOrRight.Stop();
                    break;
                case Key.Up:
                case Key.W:
                    _timerRotatePiece.Stop();
                    break;
                case Key.Down:
                case Key.S:
                    _isSoftDropping = false;
                    _timerMovePieceDown.Interval = GetMovePieceDownTimerIntervalBasedOnLevel();
                    break;
            }
        }

        private void TryMovePieceLeftOrRight()
        {
            if (_leftKeyHasPriority)
                _gameBoardCore.TryMoveCurrentPieceLeft();
            else
                _gameBoardCore.TryMoveCurrentPieceRight();
        }

        private void MoveCurrentPieceDownProcess()
        {
            bool canMoveDown = _gameBoardCore.TryMovePieceDown();

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
                // In this case, all CurrentPiece blocks were just added to the LockedBlocks array

                // Remove complete rows
                int numberOfCompleteRows = _gameBoardCore.AddCurrentPieceToLockedBlocksAndRemoveCompleteRows();

                // Update Level, Score, and Lines
                // If reaching a new level, make the _timerMovePieceDown tick faster (unless we are soft dropping, which is using the same timer)
                int levelBefore = _statistics.Level;
                _statistics.UpdateOnCompletingRows(numberOfCompleteRows);
                int levelAfter = _statistics.Level;
                if (levelAfter > levelBefore && !_isSoftDropping)
                    _timerMovePieceDown.Interval = GetMovePieceDownTimerIntervalBasedOnLevel();

                // Check if NextPiece collides with LockedBlocks (= Game Over)
                bool nextPieceCollidesWithLockedBlocks = _gameBoardCore.NextPieceCollidesWithLockedBlocks();

                if (nextPieceCollidesWithLockedBlocks)
                {
                    // Game Over: Stop all timers, and raise GameOver event (in order to eventually add high score)
                    IsGameOver = true;

                    _timerMovePieceLeftOrRight.Stop();
                    _timerRotatePiece.Stop();
                    _timerMovePieceDown.Stop();
                    _timerSecondsGameHasBeenRunning.Stop();

                    _gameBoardCore.OnGameOverEvent(_statistics.Score);
                }
                else
                {
                    // Make CurrentPiece refer to NextPiece. Then build a new NextPiece
                    _gameBoardCore.UpdateCurrentPieceAndNextPiece();
                }

                // Raise the LockedBlocksOrCurrentPieceChanged event if any rows have been completed or CurrentPiece has been set to refer to NextPiece
                if (numberOfCompleteRows > 0 || !nextPieceCollidesWithLockedBlocks)
                    _gameBoardCore.OnLockedBlocksOrCurrentPieceChangedEvent();
            }
        }
    }
}
