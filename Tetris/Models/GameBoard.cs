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
            // Not possible to move left if CurrentPiece is up against the left side
            if (CurrentPiece.Blocks.Any(block => CurrentPiece.CoordsX + block.OffsetX == 0))
                return false;

            // Not possible to move left if any of the CurrentPiece.Blocks has a locked block to the left
            // Only check those blocks that have entered the game board so far
            bool hasLockedBlockToTheLeft = CurrentPiece.Blocks
                .Where(block => CurrentPiece.CoordsY + block.OffsetY >= 0)
                .Any(block => LockedBlocks[CurrentPiece.CoordsY + block.OffsetY, CurrentPiece.CoordsX + block.OffsetX - 1] != 0);

            if (hasLockedBlockToTheLeft)
                return false;

            // Move left
            CurrentPiece.MoveLeft();
            OnChanged();
            return true;
        }

        public bool TryMoveCurrentPieceRight()
        {
            // Not possible to move right if CurrentPiece is up against the right side
            if (CurrentPiece.Blocks.Any(block => CurrentPiece.CoordsX + block.OffsetX + 1 == Cols))
                return false;

            // Not possible to move right if any of the CurrentPiece.Blocks has a locked block to the right
            // Only check those blocks that have entered the game board so far
            bool hasLockedBlockToTheRight = CurrentPiece.Blocks
                .Where(block => CurrentPiece.CoordsY + block.OffsetY >= 0)
                .Any(block => LockedBlocks[CurrentPiece.CoordsY + block.OffsetY, CurrentPiece.CoordsX + block.OffsetX + 1] != 0);

            if (hasLockedBlockToTheRight)
                return false;

            // Move right
            CurrentPiece.MoveRight();
            OnChanged();
            return true;
        }

        public bool TryRotateCurrentPiece()
        {
            // Not possible to rotate if next rotation has blocks outside the game board
            bool nextRotationHasBlocksOutsideGameBoard = CurrentPiece.BlocksInNextRotation
                .Any(block =>
                    CurrentPiece.CoordsX + block.OffsetX < 0 || // Left side
                    CurrentPiece.CoordsX + block.OffsetX + 1 > Cols || // Right side
                    CurrentPiece.CoordsY + block.OffsetY + 1 > Rows); // Bottom

            if (nextRotationHasBlocksOutsideGameBoard)
                return false;

            // Not possible to rotate if next rotation has blocks that collide with the locked blocks
            // Only check those blocks that after next rotation are within the game board in the top
            bool nextRotationCollidesWithLockedBlocks = CurrentPiece.BlocksInNextRotation
                .Where(block => CurrentPiece.CoordsY + block.OffsetY >= 0)
                .Any(block => LockedBlocks[CurrentPiece.CoordsY + block.OffsetY, CurrentPiece.CoordsX + block.OffsetX] != 0);

            if (nextRotationCollidesWithLockedBlocks)
                return false;

            // Rotate
            CurrentPiece.Rotate();
            OnChanged();
            return true;
        }

        public bool TryMoveCurrentPieceDown()
        {
            // Not possible to move down if CurrentPiece is on the bottom row
            if (CurrentPiece.Blocks.Any(block => CurrentPiece.CoordsY + block.OffsetY + 1 == Rows))
                return false;

            // Not possible to move down if any of the CurrentPiece.Blocks has a locked block below
            // Only check those blocks that after moving down will have entered the game board so far
            bool hasLockedBlockBelow = CurrentPiece.Blocks
                .Where(block => CurrentPiece.CoordsY + block.OffsetY >= -1)
                .Any(block => LockedBlocks[CurrentPiece.CoordsY + block.OffsetY + 1, CurrentPiece.CoordsX + block.OffsetX] != 0);

            if (hasLockedBlockBelow)
                return false;

            // Move down
            CurrentPiece.MoveDown();
            OnChanged();
            return true;
        }

        public int AddCurrentPieceToLockedBlocksAndRemoveCompleteRows()
        {
            // Add all CurrentPiece blocks to the LockedBlocks array
            foreach (Block block in CurrentPiece.Blocks)
                LockedBlocks[CurrentPiece.CoordsY + block.OffsetY, CurrentPiece.CoordsX + block.OffsetX] = (int)CurrentPiece.PieceType;

            // Build HashSet of row numbers occupied by CurrentPiece and that are complete
            // Complete rows should be removed from the LockedBlocks array, and then points should be awarded
            HashSet<int> rowsOccupiedByCurrentPieceAndAreComplete = new HashSet<int>(
                CurrentPiece.Blocks
                    .Select(block => CurrentPiece.CoordsY + block.OffsetY)
                    .Distinct()
                    .Where(row => Enumerable.Range(0, Cols)
                        .All(col => LockedBlocks[row, col] != 0)));

            // When a row is complete, rows above it should be moved down (in the LockedBlocks array)
            int completeRowsBelowAndIncludingCurrentRow = 0;

            for (int row = Rows - 1; row >= 0; row--)
            {
                bool isRowComplete = rowsOccupiedByCurrentPieceAndAreComplete.Contains(row);

                if (isRowComplete)
                    completeRowsBelowAndIncludingCurrentRow++;

                // Move this row down a number of times, equal to the number of complete rows below
                // However, don't consider moving down the bottom row, or rows that are complete
                if (row != Rows - 1 && !isRowComplete)
                {
                    for (int col = 0; col < Cols; col++)
                        LockedBlocks[row + completeRowsBelowAndIncludingCurrentRow, col] = LockedBlocks[row, col];
                }
            }

            // Clear top x rows where x is the number of rows completed by CurrentPiece
            for (int row = 0; row < rowsOccupiedByCurrentPieceAndAreComplete.Count; row++)
            {
                for (int col = 0; col < Cols; col++)
                    LockedBlocks[row, col] = 0;
            }

            return rowsOccupiedByCurrentPieceAndAreComplete.Count;
        }

        public bool NextPieceCollidesWithLockedBlocks()
        {
            // Game Over if NextPiece collides with LockedBlocks array
            // Only check those blocks that are on the game board when the NextPiece is added
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
