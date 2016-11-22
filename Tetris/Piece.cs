using System;

namespace Tetris
{
    public class Piece
    {
        public PieceType PieceType { get; set; }
        public int CoordsY { get; set; } = 0;
        public int CoordsX { get; set; } = 0;
        public int Rotation { get; set; } = 0;

        public Piece(PieceType pieceType)
        {
            PieceType = pieceType;
        }

        public int[,] GetCurrentBlocks(PieceBlockManager pieceBlockManager) => pieceBlockManager.GetBlocks(PieceType, Rotation);
        public int[,] GetBlocksAfterNextRotation(PieceBlockManager pieceBlockManager) => pieceBlockManager.GetBlocks(PieceType, Rotation + 1);
    }
}
