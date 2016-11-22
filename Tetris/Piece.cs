using System;
using System.Windows.Media;

namespace Tetris
{
    public class Piece
    {
        public PieceType PieceType { get; set; }
        public int[] Coordinates { get; set; }
        public int Rotation { get; set; }

        public Piece(PieceType pieceType)
        {
            PieceType = pieceType;

            //Todo: Set coordinates
        }

        public int[,] CurrentBlocks => PieceBlockManager.GetBlocks(PieceType, Rotation);
        public int[,] NextBlocks => PieceBlockManager.GetBlocks(PieceType, Rotation + 1);
    }
}
