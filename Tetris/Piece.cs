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

        public int[,] CurrentBlocks => PieceBlockManager.GetBlocks(PieceType, Rotation);
        public int[,] BlocksAfterNextRotation => PieceBlockManager.GetBlocks(PieceType, Rotation + 1);
    }
}
