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

        public bool[,] CurrentBlocks => PieceBlockManager.GetBlocks(PieceType, Rotation);
        public bool[,] NextBlocks => PieceBlockManager.GetBlocks(PieceType, Rotation + 1);

        public Color Color
        {
            get
            {
                switch (PieceType)
                {
                    case PieceType.I:
                        return Colors.Cyan;
                    case PieceType.O:
                        return Colors.Yellow;
                    case PieceType.T:
                        return Colors.Purple;
                    case PieceType.J:
                        return Colors.Blue;
                    case PieceType.L:
                        return Colors.Orange;
                    case PieceType.S:
                        return Colors.Green;
                    case PieceType.Z:
                        return Colors.Red;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(PieceType), PieceType, null);
                }
            }
        }
    }
}
