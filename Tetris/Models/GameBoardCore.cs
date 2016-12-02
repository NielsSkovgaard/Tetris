using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetris.Models
{
    internal class GameBoardCore
    {
        public event EventHandler LockedBlocksOrCurrentPieceChanged;
        public event EventHandler NextPieceChanged;
        public event EventHandler<int> GameOver;

        // Input parameters
        public int Rows { get; } // Usually 20
        public int Cols { get; } // Usually 10

        public int[,] LockedBlocks { get; private set; }
        public Piece CurrentPiece { get; private set; }
        public Piece NextPiece { get; private set; }

        private readonly Random _random = new Random();

        public GameBoardCore(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;

            Reset();
        }

        public void Reset()
        {
            LockedBlocks = new int[Rows, Cols];
            CurrentPiece = BuildRandomPiece();
            NextPiece = BuildRandomPiece();

            // Raise events
            RaiseLockedBlocksOrCurrentPieceChangedEvent();
            RaiseNextPieceChangedEvent();
        }

        private Piece BuildRandomPiece()
        {
            // The random number is >= 1 and < 8, i.e. in the interval 1..7
            Piece piece = new Piece((PieceType)_random.Next(1, 8));

            // Position the Piece in the top middle of the game board
            piece.CoordsX = (Cols - PieceBlockManager.GetWidthOfBlockArray(piece.PieceType)) / 2;
            return piece;
        }

        public virtual void RaiseLockedBlocksOrCurrentPieceChangedEvent()
        {
            LockedBlocksOrCurrentPieceChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual void RaiseNextPieceChangedEvent()
        {
            NextPieceChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual void RaiseGameOverEvent(int score)
        {
            GameOver?.Invoke(this, score);
        }

        public bool TryMoveCurrentPieceLeft()
        {
            if (CurrentPiece.Blocks.All(block =>
                CurrentPiece.CoordsX + block.CoordsX >= 1 && // Check that CurrentPiece is not up against the left side
                LockedBlocks[CurrentPiece.CoordsY + block.CoordsY, CurrentPiece.CoordsX + block.CoordsX - 1] == 0)) // Check that CurrentPiece won't collide with the locked blocks
            {
                CurrentPiece.MoveLeft();
                RaiseLockedBlocksOrCurrentPieceChangedEvent();
                return true;
            }

            return false;
        }

        public bool TryMoveCurrentPieceRight()
        {
            if (CurrentPiece.Blocks.All(block =>
                CurrentPiece.CoordsX + block.CoordsX + 2 <= Cols && // Check that CurrentPiece is not up against the right side
                LockedBlocks[CurrentPiece.CoordsY + block.CoordsY, CurrentPiece.CoordsX + block.CoordsX + 1] == 0)) // Check that CurrentPiece won't collide with the locked blocks
            {
                CurrentPiece.MoveRight();
                RaiseLockedBlocksOrCurrentPieceChangedEvent();
                return true;
            }

            return false;
        }

        public bool TryRotateCurrentPiece()
        {
            bool isNextRotationInValidPosition = CurrentPiece.BlocksInNextRotation.All(block =>
                CurrentPiece.CoordsX + block.CoordsX >= 0 && // Check that the rotated CurrentPiece is within the bounds in the left side
                CurrentPiece.CoordsX + block.CoordsX + 1 <= Cols && // Check that the rotated CurrentPiece is within the bounds in the right side
                CurrentPiece.CoordsY + block.CoordsY + 1 <= Rows && // Check that the rotated CurrentPiece is within the bounds in the bottom
                LockedBlocks[CurrentPiece.CoordsY + block.CoordsY, CurrentPiece.CoordsX + block.CoordsX] == 0); // Check that CurrentPiece won't collide with the locked blocks

            if (isNextRotationInValidPosition)
            {
                CurrentPiece.Rotate();
                RaiseLockedBlocksOrCurrentPieceChangedEvent();
                return true;
            }

            return false;
        }

        public bool TryMovePieceDown()
        {
            bool canMovePieceDown = CurrentPiece.Blocks.All(block =>
                CurrentPiece.CoordsY + block.CoordsY + 2 <= Rows && // Check that CurrentPiece is not on the bottom row (e.g. 15 + 3 + 2 <= 20 = true)
                LockedBlocks[CurrentPiece.CoordsY + block.CoordsY + 1, CurrentPiece.CoordsX + block.CoordsX] == 0); // Check that CurrentPiece won't collide with the locked blocks

            if (canMovePieceDown)
            {
                CurrentPiece.MoveDown();
                RaiseLockedBlocksOrCurrentPieceChangedEvent();
                return true;
            }

            return false;
        }

        public int AddCurrentPieceToLockedBlocksAndRemoveCompleteRows()
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

            return rowsOccupiedByPieceAndAreComplete.Count;
        }

        public bool NextPieceCollidesWithLockedBlocks()
        {
            // Game Over if NextPiece collides with LockedBlocks array
            bool nextPieceCollidesWithLockedBlocks = NextPiece.Blocks.Any(block =>
                LockedBlocks[NextPiece.CoordsY + block.CoordsY, NextPiece.CoordsX + block.CoordsX] != 0);

            return nextPieceCollidesWithLockedBlocks;
        }

        public void UpdateCurrentPieceAndNextPiece()
        {
            // Make CurrentPiece refer to NextPiece. Then build a new NextPiece
            CurrentPiece = NextPiece;
            NextPiece = BuildRandomPiece();
            RaiseNextPieceChangedEvent();
        }
    }
}
