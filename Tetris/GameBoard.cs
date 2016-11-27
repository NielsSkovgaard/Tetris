using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace Tetris
{
    internal class GameBoard
    {
        public event GameBoardChangedEventHandler GameBoardChanged;

        // Input parameters
        public int Rows { get; } // Usually 20
        public int Cols { get; } // Usually 10

        // Static blocks and the currently moving piece
        public int[,] StaticBlocks { get; }
        public Piece Piece { get; private set; }

        private readonly Random _random = new Random();

        // Timers for holding down a key to repeat an action (move left/right, rotate, or move down)
        private readonly DispatcherTimer _movePieceLeftRightTimer = new DispatcherTimer();
        private readonly DispatcherTimer _rotatePieceTimer = new DispatcherTimer();
        private readonly DispatcherTimer _movePieceDownTimer = new DispatcherTimer();

        private bool _isLeftKeyDown;
        private bool _isRightKeyDown;
        private bool _leftKeyHasPriority;

        // TODO: Status
        public int Score { get; set; }
        //public int Level { get; set; }
        public int Lines { get; set; }

        public GameBoard(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;

            StaticBlocks = new int[rows, cols];
            ResetPiece();

            // Timers for holding down a key to repeat an action (move left/right, rotate, or move down)
            _movePieceLeftRightTimer.Interval = new TimeSpan(1000000); //1000000 ticks = 100 ms = 10 FPS
            _rotatePieceTimer.Interval = new TimeSpan(2500000); //2500000 ticks = 250 ms = 4 FPS
            _movePieceDownTimer.Interval = new TimeSpan(500000); //500000 ticks = 50 ms = 20 FPS
            _movePieceLeftRightTimer.Tick += (sender, args) => TryMovePieceLeftOrRight();
            _rotatePieceTimer.Tick += (sender, args) => TryRotatePiece();
            _movePieceDownTimer.Tick += (sender, args) => TryMovePieceDown();
        }

        private void ResetPiece()
        {
            // Reset the currently moving piece (randomly selected, and positioned in the top middle of the canvas)
            // The random number is >= 1 and < 8, i.e. in the interval 1..7
            Piece = new Piece((PieceType)_random.Next(1, 8));
            Piece.CoordsX = (Cols - PieceBlockManager.GetWidthOfBlockArray(Piece.PieceType)) / 2;
        }

        protected virtual void RaiseGameBoardChangedEvent()
        {
            GameBoardChanged?.Invoke(this, EventArgs.Empty);
        }

        public void KeyDown(Key key, bool isRepeat)
        {
            if (isRepeat)
                return;

            switch (key)
            {
                case Key.Left:
                case Key.A:
                    _isLeftKeyDown = true;
                    _leftKeyHasPriority = true;
                    TryMovePieceLeft();
                    _movePieceLeftRightTimer.Start();
                    break;
                case Key.Right:
                case Key.D:
                    _isRightKeyDown = true;
                    _leftKeyHasPriority = false;
                    TryMovePieceRight();
                    _movePieceLeftRightTimer.Start();
                    break;
                case Key.Up:
                case Key.W:
                    TryRotatePiece();
                    _rotatePieceTimer.Start();
                    break;
                case Key.Down:
                case Key.S:
                    TryMovePieceDown();
                    _movePieceDownTimer.Start();
                    break;
            }
        }

        public void KeyUp(Key key)
        {
            switch (key)
            {
                case Key.Left:
                case Key.A:
                    _isLeftKeyDown = false;
                    _leftKeyHasPriority = false;

                    if (!_isRightKeyDown)
                        _movePieceLeftRightTimer.Stop();
                    break;
                case Key.Right:
                case Key.D:
                    _isRightKeyDown = false;
                    _leftKeyHasPriority = true;

                    if (!_isLeftKeyDown)
                        _movePieceLeftRightTimer.Stop();
                    break;
                case Key.Up:
                case Key.W:
                    _rotatePieceTimer.Stop();
                    break;
                case Key.Down:
                case Key.S:
                    _movePieceDownTimer.Stop();
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
            if (Piece.Blocks.All(block =>
                Piece.CoordsX + block.CoordsX >= 1 && // Check that the Piece is not up against the left side
                StaticBlocks[Piece.CoordsY + block.CoordsY, Piece.CoordsX + block.CoordsX - 1] == 0)) // Check that the Piece won't collide with the static blocks
            {
                Piece.MoveLeft();
                RaiseGameBoardChangedEvent();
            }
        }

        private void TryMovePieceRight()
        {
            if (Piece.Blocks.All(block =>
                Piece.CoordsX + block.CoordsX + 2 <= Cols && // Check that the Piece is not up against the right side
                StaticBlocks[Piece.CoordsY + block.CoordsY, Piece.CoordsX + block.CoordsX + 1] == 0)) // Check that the Piece won't collide with the static blocks
            {
                Piece.MoveRight();
                RaiseGameBoardChangedEvent();
            }
        }

        private void TryRotatePiece()
        {
            bool isNextRotationInValidPosition = Piece.BlocksInNextRotation.All(block =>
                Piece.CoordsX + block.CoordsX >= 0 && // Check that the rotated Piece is within the bounds in the left side
                Piece.CoordsX + block.CoordsX + 1 <= Cols && // Check that the rotated Piece is within the bounds in the right side
                Piece.CoordsY + block.CoordsY + 1 <= Rows && // Check that the rotated Piece is within the bounds in the bottom
                StaticBlocks[Piece.CoordsY + block.CoordsY, Piece.CoordsX + block.CoordsX] == 0); // Check that the Piece won't collide with the static blocks

            if (isNextRotationInValidPosition)
            {
                Piece.Rotate();
                RaiseGameBoardChangedEvent();
            }
        }

        private void TryMovePieceDown()
        {
            bool canMovePieceDown = Piece.Blocks.All(block =>
                Piece.CoordsY + block.CoordsY + 2 <= Rows && // Check that the Piece is not on the bottom row (e.g. 15 + 3 + 2 <= 20 = true)
                StaticBlocks[Piece.CoordsY + block.CoordsY + 1, Piece.CoordsX + block.CoordsX] == 0); // Check that the Piece won't collide with the static blocks

            if (canMovePieceDown)
            {
                // TODO: Award point for moving Piece down fast

                Piece.MoveDown();
                RaiseGameBoardChangedEvent();
            }
            else
            {
                // Add all Piece blocks to the StaticBlocks array
                foreach (Block block in Piece.Blocks)
                    StaticBlocks[Piece.CoordsY + block.CoordsY, Piece.CoordsX + block.CoordsX] = (int)Piece.PieceType;

                // Build array of unique row numbers for the Piece. Any of these rows might be complete
                // Complete rows should be removed from StaticBlocks, then points should be awarded
                int[] uniqueRowNumbersForPiece = Piece.Blocks
                    .Select(block => Piece.CoordsY + block.CoordsY)
                    .Distinct()
                    .OrderByDescending(rowNumber => rowNumber)
                    .ToArray();

                int numberOfCompleteRows = 0;

                for (int arrayIndex = 0; arrayIndex < uniqueRowNumbersForPiece.Length; arrayIndex++)
                {
                    int row = uniqueRowNumbersForPiece[arrayIndex];

                    // Only consider complete rows for removal
                    bool isRowComplete = Enumerable
                        .Range(0, StaticBlocks.GetLength(1))
                        .All(col => StaticBlocks[row, col] > 0);

                    if (isRowComplete)
                    {
                        numberOfCompleteRows++;

                        for (int col = 0; col < StaticBlocks.GetLength(1); col++)
                        {
                            // TODO: Clear rows
                            StaticBlocks[row, col] = 6;
                        }
                    }
                }

                AwardPointsForClearingRows(numberOfCompleteRows);
                RaiseGameBoardChangedEvent();

                // TODO: Stop drop timer until down button is pressed again?

                ResetPiece();
            }
        }

        private void AwardPointsForClearingRows(int numberOfCompleteRows)
        {
            //TODO: Multiply by Level factor. Increment Level when having reached a certain amount of points.

            Lines += numberOfCompleteRows;

            switch (numberOfCompleteRows)
            {
                case 1:
                    Score += 100;
                    return;
                case 2:
                    Score += 100 + 200;
                    return;
                case 3:
                    Score += 100 + 200 + 300;
                    return;
                case 4:
                    Score += 100 + 200 + 300 + 400;
                    return;
            }
        }
    }
}
