using System;

namespace Tetris.Models
{
    internal class Piece
    {
        public PieceType PieceType { get; }
        public int CoordsY { get; set; }
        public int CoordsX { get; set; }
        public int Rotation { get; private set; }
        public Block[] Blocks { get; private set; }

        private static readonly int NumberOfPieceTypes = Enum.GetNames(typeof(PieceType)).Length; // Usually 7
        private static readonly Random Random = new Random();

        public Piece(PieceType pieceType)
        {
            PieceType = pieceType;
            Blocks = BlocksInCurrentRotation;
        }

        public static Piece BuildRandomPiece()
        {
            // Random number >= 1 and < (7 + 1), i.e. range 1..7
            return new Piece((PieceType)Random.Next(1, NumberOfPieceTypes + 1));
        }

        public void MoveLeft() => CoordsX--;
        public void MoveRight() => CoordsX++;
        public void MoveDown() => CoordsY++;

        public void Rotate()
        {
            Rotation++;
            Blocks = BlocksInCurrentRotation;
        }

        public Block[] BlocksInCurrentRotation => PieceBlockManager.Blocks(PieceType, Rotation);
        public Block[] BlocksInNextRotation => PieceBlockManager.Blocks(PieceType, Rotation + 1);
    }
}
