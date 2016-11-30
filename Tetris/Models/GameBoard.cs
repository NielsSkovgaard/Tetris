using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace Tetris.Models
{
    internal class GameBoard
    {
        public event EventHandler Changed;
        public event EventHandler NextPieceChanged;
        public event EventHandler StatisticsChanged;
        public event EventHandler<int> GameOver;

        // Input parameters
        public int Rows { get; } // Usually 20
        public int Cols { get; } // Usually 10

        public int[,] LockedBlocks { get; private set; }
        public Piece CurrentPiece { get; private set; }
        public Piece NextPiece { get; private set; }

        public bool IsGamePaused { get; private set; }
        public bool IsGameOver { get; private set; }

        private readonly Random _random = new Random();

        // TimeSpans and Timers:
        // - for holding down a key to repeat a command (move left/right or rotate) every x milliseconds
        private readonly TimeSpan _timeSpanMovePieceLeftOrRight = TimeSpan.FromMilliseconds(100); // 100 ms = 10 FPS
        private readonly TimeSpan _timeSpanRotatePiece = TimeSpan.FromMilliseconds(250); // 250 ms = 4 FPS
        private readonly DispatcherTimer _timerMovePieceLeftOrRight = new DispatcherTimer();
        private readonly DispatcherTimer _timerRotatePiece = new DispatcherTimer();

        // - to add gravity to CurrentPiece. It will move down faster when soft dropping (by holding down the down key)
        private readonly int[] _movePieceDownIntervalsInMilisecondsPerLevel = { 887, 820, 753, 686, 619, 552, 469, 368, 285, 184, 167, 151, 134, 117, 100 }; // See http://tetrisconcept.net/wiki/index.php?title=Tetris_(Game_Boy)
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

        // Level, Score, Lines, Time
        public int Level { get; private set; } // Between 1 and 15
        public int Score { get; private set; }
        public int Lines { get; private set; }
        public int Time { get; private set; }

        private const int MaximumLevel = 15;
        private const int NumberOfRowsToIncreaseLevel = 10;
        private readonly int[] _scoresToAddForCompletingRows = { 100, 300, 500, 800 };
        private bool _isSoftDropping;

        public GameBoard(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;

            // Timers
            _timerMovePieceLeftOrRight.Interval = _timeSpanMovePieceLeftOrRight;
            _timerRotatePiece.Interval = _timeSpanRotatePiece;
            _timerSecondsGameHasBeenRunning.Interval = _timeSpanSecondsGameHasBeenRunning;
            _timerMovePieceLeftOrRight.Tick += (sender, args) => TryMovePieceLeftOrRight();
            _timerRotatePiece.Tick += (sender, args) => TryRotatePiece();
            _timerMovePieceDown.Tick += (sender, args) => TryMovePieceDown();
            _timerSecondsGameHasBeenRunning.Tick += (sender, args) => IncrementGameTime();

            StartNewGame();
        }

        public void StartNewGame()
        {
            LockedBlocks = new int[Rows, Cols];
            CurrentPiece = BuildRandomPiece();
            NextPiece = BuildRandomPiece();

            Level = 1;
            Score = 0;
            Lines = 0;
            Time = 0;

            // Raise events
            RaiseChangedEvent();
            RaiseNextPieceChangedEvent();
            RaiseStatisticsChangedEvent();

            // Calling PauseResumeGame will "resume" the game, in other words start it
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

                //Timers
                _timerMovePieceDown.Stop();
                _timerSecondsGameHasBeenRunning.Stop();
            }
        }

        private Piece BuildRandomPiece()
        {
            // The random number is >= 1 and < 8, i.e. in the interval 1..7
            Piece piece = new Piece((PieceType)_random.Next(1, 8));

            // Position the Piece in the top middle of the canvas
            piece.CoordsX = (Cols - PieceBlockManager.GetWidthOfBlockArray(piece.PieceType)) / 2;
            return piece;
        }

        private TimeSpan GetMovePieceDownTimerIntervalBasedOnLevel()
        {
            return TimeSpan.FromMilliseconds(_movePieceDownIntervalsInMilisecondsPerLevel[Level - 1]);
        }

        protected virtual void RaiseChangedEvent()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void RaiseNextPieceChangedEvent()
        {
            NextPieceChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void RaiseStatisticsChangedEvent()
        {
            StatisticsChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void RaiseGameOverEvent()
        {
            GameOver?.Invoke(this, Score);
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
                    TryMovePieceLeft();
                    _timerMovePieceLeftOrRight.Start();
                    break;
                case Key.Right:
                case Key.D:
                    _isRightKeyDown = true;
                    _leftKeyHasPriority = false;
                    TryMovePieceRight();
                    _timerMovePieceLeftOrRight.Start();
                    break;
                case Key.Up:
                case Key.W:
                    TryRotatePiece();
                    _timerRotatePiece.Start();
                    break;
                case Key.Down:
                case Key.S:
                    _isSoftDropping = true;
                    TryMovePieceDown();
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
                TryMovePieceLeft();
            else
                TryMovePieceRight();
        }

        private void TryMovePieceLeft()
        {
            if (CurrentPiece.Blocks.All(block =>
                CurrentPiece.CoordsX + block.CoordsX >= 1 && // Check that CurrentPiece is not up against the left side
                LockedBlocks[CurrentPiece.CoordsY + block.CoordsY, CurrentPiece.CoordsX + block.CoordsX - 1] == 0)) // Check that CurrentPiece won't collide with the locked blocks
            {
                CurrentPiece.MoveLeft();
                RaiseChangedEvent();
            }
        }

        private void TryMovePieceRight()
        {
            if (CurrentPiece.Blocks.All(block =>
                CurrentPiece.CoordsX + block.CoordsX + 2 <= Cols && // Check that CurrentPiece is not up against the right side
                LockedBlocks[CurrentPiece.CoordsY + block.CoordsY, CurrentPiece.CoordsX + block.CoordsX + 1] == 0)) // Check that CurrentPiece won't collide with the locked blocks
            {
                CurrentPiece.MoveRight();
                RaiseChangedEvent();
            }
        }

        private void TryRotatePiece()
        {
            bool isNextRotationInValidPosition = CurrentPiece.BlocksInNextRotation.All(block =>
                CurrentPiece.CoordsX + block.CoordsX >= 0 && // Check that the rotated CurrentPiece is within the bounds in the left side
                CurrentPiece.CoordsX + block.CoordsX + 1 <= Cols && // Check that the rotated CurrentPiece is within the bounds in the right side
                CurrentPiece.CoordsY + block.CoordsY + 1 <= Rows && // Check that the rotated CurrentPiece is within the bounds in the bottom
                LockedBlocks[CurrentPiece.CoordsY + block.CoordsY, CurrentPiece.CoordsX + block.CoordsX] == 0); // Check that CurrentPiece won't collide with the locked blocks

            if (isNextRotationInValidPosition)
            {
                CurrentPiece.Rotate();
                RaiseChangedEvent();
            }
        }

        private void TryMovePieceDown()
        {
            bool canMovePieceDown = CurrentPiece.Blocks.All(block =>
                CurrentPiece.CoordsY + block.CoordsY + 2 <= Rows && // Check that CurrentPiece is not on the bottom row (e.g. 15 + 3 + 2 <= 20 = true)
                LockedBlocks[CurrentPiece.CoordsY + block.CoordsY + 1, CurrentPiece.CoordsX + block.CoordsX] == 0); // Check that CurrentPiece won't collide with the locked blocks

            if (canMovePieceDown)
            {
                if (_isSoftDropping)
                {
                    // Award points for soft dropping CurrentPiece
                    Score++;
                    RaiseStatisticsChangedEvent();
                }

                CurrentPiece.MoveDown();
                RaiseChangedEvent();
            }
            else
            {
                // Add all CurrentPiece blocks to the LockedBlocks array
                foreach (Block block in CurrentPiece.Blocks)
                    LockedBlocks[CurrentPiece.CoordsY + block.CoordsY, CurrentPiece.CoordsX + block.CoordsX] = (int)CurrentPiece.PieceType;

                // Build HashSet of row numbers occupied by CurrentPiece and that are complete
                // Complete rows should be removed from the LockedBlocks array, and then points should be awarded
                HashSet<int> rowsOccupiedByPieceAndAreComplete = new HashSet<int>(
                    CurrentPiece.Blocks
                    .Select(block => CurrentPiece.CoordsY + block.CoordsY)
                    .Where(row => Enumerable
                        .Range(0, Cols)
                        .All(col => LockedBlocks[row, col] != 0)));

                // When a row is complete, rows above it should be moved down (in the LockedBlocks array)
                int completeRowsBelowAndIncludingCurrentRow = 0;

                for (int row = Rows - 1; row >= 0; row--)
                {
                    bool isRowComplete = rowsOccupiedByPieceAndAreComplete.Contains(row);

                    if (isRowComplete)
                        completeRowsBelowAndIncludingCurrentRow++;

                    // Don't move down the bottom row or rows that are complete
                    if (row != Rows - 1 && !isRowComplete)
                    {
                        for (int col = 0; col < Cols; col++)
                            LockedBlocks[row + completeRowsBelowAndIncludingCurrentRow, col] = LockedBlocks[row, col];
                    }
                }

                // Clear top x rows where x is the number of rows completed by CurrentPiece
                for (int row = 0; row < rowsOccupiedByPieceAndAreComplete.Count; row++)
                {
                    for (int col = 0; col < Cols; col++)
                        LockedBlocks[row, col] = 0;
                }

                // Update Level, Score, and Lines
                UpdateLevelScoreAndLines(rowsOccupiedByPieceAndAreComplete.Count);

                // Game Over if NextPiece collides with LockedBlocks array
                bool nextPieceCollidesWithLockedBlocks = NextPiece.Blocks.Any(block =>
                    LockedBlocks[NextPiece.CoordsY + block.CoordsY, NextPiece.CoordsX + block.CoordsX] != 0);

                if (nextPieceCollidesWithLockedBlocks)
                {
                    // Game Over: Stop all timers, and raise GameOver event (in order to eventually add high score)
                    IsGameOver = true;
                    _timerMovePieceLeftOrRight.Stop();
                    _timerRotatePiece.Stop();
                    _timerMovePieceDown.Stop();
                    _timerSecondsGameHasBeenRunning.Stop();
                    RaiseGameOverEvent();
                }
                else
                {
                    // Make CurrentPiece refer to NextPiece. Then build a new NextPiece
                    CurrentPiece = NextPiece;
                    NextPiece = BuildRandomPiece();
                    RaiseNextPieceChangedEvent();
                }

                // Raise the changed event if any rows have been completed or CurrentPiece has been updated
                if (rowsOccupiedByPieceAndAreComplete.Any() || !nextPieceCollidesWithLockedBlocks)
                    RaiseChangedEvent();
            }
        }

        private void UpdateLevelScoreAndLines(int numberOfCompleteRows)
        {
            if (numberOfCompleteRows > 0)
            {
                Lines += numberOfCompleteRows;
                Score += _scoresToAddForCompletingRows[numberOfCompleteRows - 1];

                if (Level < MaximumLevel)
                {
                    int newLevel = Lines / NumberOfRowsToIncreaseLevel + 1;

                    if (newLevel > Level)
                    {
                        Level = newLevel;

                        // Make the _timerMovePieceDown tick faster (unless we are soft dropping, which is using the same timer)
                        if (!_isSoftDropping)
                            _timerMovePieceDown.Interval = GetMovePieceDownTimerIntervalBasedOnLevel();
                    }
                }

                RaiseStatisticsChangedEvent();
            }
        }

        private void IncrementGameTime()
        {
            Time++;
            RaiseStatisticsChangedEvent();
        }
    }
}
