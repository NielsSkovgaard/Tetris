using System;

namespace Tetris
{
    internal class Piece
    {
        public PieceType PieceType { get; set; }
        public int CoordsY { get; set; } = 0;
        public int CoordsX { get; set; } = 0;
        public int Rotation { get; set; } = 0;
        public int[,] CurrentBlocks { get; set; }
        public int LeftmostBlockIndex { get; private set; }
        public int RightmostBlockIndex { get; private set; }

        public Piece(PieceType pieceType)
        {
            PieceType = pieceType;
        }

        public void UpdateCurrentBlocks(PieceBlockManager pieceBlockManager)
        {
            CurrentBlocks = pieceBlockManager.GetBlocks(PieceType, Rotation);
            LeftmostBlockIndex = GetLeftmostBlockIndex();
            RightmostBlockIndex = GetRightmostBlockIndex();
        }

        private int GetLeftmostBlockIndex()
        {
            int yLength = CurrentBlocks.GetLength(0); //4
            int xLength = CurrentBlocks.GetLength(1); //4

            for (int x = 0; x < xLength; x++)
                for (int y = 0; y < yLength; y++)
                    if (CurrentBlocks[y, x] > 0)
                        return x;

            throw new InvalidOperationException();
        }

        private int GetRightmostBlockIndex()
        {
            int yLength = CurrentBlocks.GetLength(0); //4
            int xLength = CurrentBlocks.GetLength(1); //4

            for (int x = xLength - 1; x >= 0; x--)
                for (int y = 0; y < yLength; y++)
                    if (CurrentBlocks[y, x] > 0)
                        return x;

            throw new InvalidOperationException();
        }
    }
}
