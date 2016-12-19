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

        private static readonly Random Random = new Random();

        public Piece(PieceType pieceType)
        {
            PieceType = pieceType;
            Blocks = BlocksInCurrentRotation;
        }

        public static Piece BuildRandomPiece()
        {
            // randomNumber is >= 1 and < 8, i.e. in the interval 1..7
            int numberOfPieceTypes = Enum.GetNames(typeof(PieceType)).Length; // Usually 8
            int randomNumber = Random.Next(1, numberOfPieceTypes);
            return new Piece((PieceType)randomNumber);
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
