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
            // Build random piece, and position it in top middle of game board
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
            // Not possible to move left if CurrentPiece is up against left side
            if (CurrentPiece.Blocks.Any(block => CurrentPiece.CoordsX + block.OffsetX == 0))
                return false;

            // Not possible to move left if any of CurrentPiece.Blocks has a locked block to the left
            // Only check blocks that have entered game board so far
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
            // Not possible to move right if CurrentPiece is up against right side
            if (CurrentPiece.Blocks.Any(block => CurrentPiece.CoordsX + block.OffsetX + 1 == Cols))
                return false;

            // Not possible to move right if any of CurrentPiece.Blocks has a locked block to the right
            // Only check blocks that have entered game board so far
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
            Block[] blocksInNextRotation = CurrentPiece.BlocksInNextRotation;

            // Not possible to rotate if next rotation has blocks outside game board
            bool nextRotationHasBlocksOutsideGameBoard = blocksInNextRotation
                .Any(block =>
                    CurrentPiece.CoordsX + block.OffsetX < 0 || // Left side
                    CurrentPiece.CoordsX + block.OffsetX + 1 > Cols || // Right side
                    CurrentPiece.CoordsY + block.OffsetY + 1 > Rows); // Bottom

            if (nextRotationHasBlocksOutsideGameBoard)
                return false;

            // Not possible to rotate if next rotation has blocks that collide with locked blocks
            // Only check blocks that after next rotation are within game board (in the top)
            bool nextRotationCollidesWithLockedBlocks = blocksInNextRotation
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
            // Not possible to move down if CurrentPiece is on bottom row
            if (CurrentPiece.Blocks.Any(block => CurrentPiece.CoordsY + block.OffsetY + 1 == Rows))
                return false;

            // Not possible to move down if any of CurrentPiece.Blocks has a locked block below
            // Only check blocks that after moving down will have entered game board so far
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
            // Add all CurrentPiece blocks to LockedBlocks array
            foreach (Block block in CurrentPiece.Blocks)
                LockedBlocks[CurrentPiece.CoordsY + block.OffsetY, CurrentPiece.CoordsX + block.OffsetX] = (int)CurrentPiece.PieceType;

            // Build HashSet of row numbers occupied by CurrentPiece and are complete
            // Complete rows should be removed from LockedBlocks array - then points should be awarded
            HashSet<int> rowsOccupiedByCurrentPieceAndAreComplete = new HashSet<int>(
                CurrentPiece.Blocks
                    .Select(block => CurrentPiece.CoordsY + block.OffsetY)
                    .Distinct()
                    .Where(row => Enumerable.Range(0, Cols)
                        .All(col => LockedBlocks[row, col] != 0)));

            // When row is complete, rows above it should be moved down (in LockedBlocks array)
            int completeRowsBelowAndIncludingCurrentRow = 0;

            for (int row = Rows - 1; row >= 0; row--)
            {
                bool isRowComplete = rowsOccupiedByCurrentPieceAndAreComplete.Contains(row);

                if (isRowComplete)
                    completeRowsBelowAndIncludingCurrentRow++;

                // Move row down a number of times, equal to number of complete rows below
                // However, don't consider moving bottom row down, or complete rows
                if (row != Rows - 1 && !isRowComplete)
                {
                    for (int col = 0; col < Cols; col++)
                        LockedBlocks[row + completeRowsBelowAndIncludingCurrentRow, col] = LockedBlocks[row, col];
                }
            }

            // Clear top x rows where x is number of rows completed by CurrentPiece
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
            // Only check blocks that are on game board when NextPiece is added
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
