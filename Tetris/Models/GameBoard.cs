using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetris.Models
{
    internal class GameBoard
    {
        public event EventHandler Changed;
        public event EventHandler NextPieceChanged;
        public event EventHandler<int> GameOver;

        // Input parameters
        public int Rows { get; } // Usually 20
        public int Cols { get; } // Usually 10

        public int[,] LockedBlocks { get; private set; }
        public Piece CurrentPiece { get; private set; }
        public Piece NextPiece { get; private set; }

        public GameBoard(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;

            Reset();
        }

        public void Reset()
        {
            LockedBlocks = new int[Rows, Cols];
            CurrentPiece = BuildAndPositionRandomPiece();
            NextPiece = BuildAndPositionRandomPiece();

            // Raise events
            OnChanged();
            OnNextPieceChanged();
        }

        private Piece BuildAndPositionRandomPiece()
        {
            // Build random piece, and position it in the top middle of the game board
            Piece piece = Piece.BuildRandomPiece();
            piece.CoordsY = 0 - piece.Blocks.Max(block => block.OffsetY);
            piece.CoordsX = (Cols - PieceBlockManager.NumberOfColsOfBlockArray(piece.PieceType)) / 2;
            return piece;
        }

        public virtual void OnChanged() => Changed?.Invoke(this, EventArgs.Empty);
        public virtual void OnNextPieceChanged() => NextPieceChanged?.Invoke(this, EventArgs.Empty);
        public virtual void OnGameOver(int score) => GameOver?.Invoke(this, score);

        public bool TryMoveCurrentPieceLeft()
        {
            // Check that CurrentPiece is not up against the left side
            bool notUpAgainstLeftSide = CurrentPiece.Blocks.All(block => CurrentPiece.CoordsX + block.OffsetX >= 1);

            if (notUpAgainstLeftSide)
            {
                // Check that none of the CurrentPiece.Blocks that have entered the game board so far will collide with the locked blocks when moving left
                bool lockedBlocksAreNotInTheWay = CurrentPiece.Blocks
                    .Where(block => CurrentPiece.CoordsY + block.OffsetY >= 0)
                    .All(block => LockedBlocks[CurrentPiece.CoordsY + block.OffsetY, CurrentPiece.CoordsX + block.OffsetX - 1] == 0);

                if (lockedBlocksAreNotInTheWay)
                {
                    CurrentPiece.MoveLeft();
                    OnChanged();
                    return true;
                }
            }

            return false;
        }

        public bool TryMoveCurrentPieceRight()
        {
            // Check that CurrentPiece is not up against the right side
            bool notUpAgainstRightSide = CurrentPiece.Blocks.All(block => CurrentPiece.CoordsX + block.OffsetX + 2 <= Cols);

            if (notUpAgainstRightSide)
            {
                // Check that none of the CurrentPiece.Blocks that have entered the game board so far will collide with the locked blocks when moving right
                bool lockedBlocksAreNotInTheWay = CurrentPiece.Blocks
                    .Where(block => CurrentPiece.CoordsY + block.OffsetY >= 0)
                    .All(block => LockedBlocks[CurrentPiece.CoordsY + block.OffsetY, CurrentPiece.CoordsX + block.OffsetX + 1] == 0);

                if (lockedBlocksAreNotInTheWay)
                {
                    CurrentPiece.MoveRight();
                    OnChanged();
                    return true;
                }
            }

            return false;
        }

        public bool TryRotateCurrentPiece()
        {
            bool nextRotationNotUpAgainstLeftRightOrBottom = CurrentPiece.BlocksInNextRotation
                .All(block =>
                    CurrentPiece.CoordsX + block.OffsetX >= 0 && // Check that the next rotation will be within the bounds in the left side
                    CurrentPiece.CoordsX + block.OffsetX + 1 <= Cols && // Check that the next rotation will be within the bounds in the right side
                    CurrentPiece.CoordsY + block.OffsetY + 1 <= Rows); // Check that the next rotation will be within the bounds in the bottom

            if (nextRotationNotUpAgainstLeftRightOrBottom)
            {
                // Check that the next rotation won't collide with the locked blocks
                bool lockedBlocksAreNotInTheWay = CurrentPiece.BlocksInNextRotation
                    .Where(block => CurrentPiece.CoordsY + block.OffsetY >= 0) // Only check those blocks that after next rotation will be within the game board (i.e. not outside in the top)
                    .All(block => LockedBlocks[CurrentPiece.CoordsY + block.OffsetY, CurrentPiece.CoordsX + block.OffsetX] == 0);

                if (lockedBlocksAreNotInTheWay)
                {
                    CurrentPiece.Rotate();
                    OnChanged();
                    return true;
                }
            }

            return false;
        }

        public bool TryMoveCurrentPieceDown()
        {
            bool canMoveDown = CurrentPiece.Blocks
                .Where(block => CurrentPiece.CoordsY + block.OffsetY >= -1) // Only check those blocks that after next move down will have entered the game board
                .All(block =>
                    CurrentPiece.CoordsY + block.OffsetY + 2 <= Rows && // Check that CurrentPiece is not on the bottom row (e.g. 15 + 3 + 2 <= 20 = true)
                    LockedBlocks[CurrentPiece.CoordsY + block.OffsetY + 1, CurrentPiece.CoordsX + block.OffsetX] == 0); // Check that CurrentPiece won't collide with the locked blocks

            if (canMoveDown)
            {
                CurrentPiece.MoveDown();
                OnChanged();
                return true;
            }

            return false;
        }

        public int AddCurrentPieceToLockedBlocksAndRemoveCompleteRows()
        {
            // Add all CurrentPiece blocks to the LockedBlocks array
            foreach (Block block in CurrentPiece.Blocks)
                LockedBlocks[CurrentPiece.CoordsY + block.OffsetY, CurrentPiece.CoordsX + block.OffsetX] = (int)CurrentPiece.PieceType;

            // Build HashSet of row numbers occupied by CurrentPiece and that are complete
            // Complete rows should be removed from the LockedBlocks array, and then points should be awarded
            HashSet<int> rowsOccupiedByPieceAndAreComplete = new HashSet<int>(
                CurrentPiece.Blocks
                    .Select(block => CurrentPiece.CoordsY + block.OffsetY)
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
            bool nextPieceCollidesWithLockedBlocks = NextPiece.Blocks
                .Where(block => NextPiece.CoordsY + block.OffsetY >= 0)
                .Any(block => LockedBlocks[NextPiece.CoordsY + block.OffsetY, NextPiece.CoordsX + block.OffsetX] != 0);

            return nextPieceCollidesWithLockedBlocks;
        }

        public void UpdateCurrentPieceAndNextPiece()
        {
            // Make CurrentPiece refer to NextPiece. Then build a new NextPiece
            CurrentPiece = NextPiece;
            NextPiece = BuildAndPositionRandomPiece();
            OnNextPieceChanged();
        }
    }
}
