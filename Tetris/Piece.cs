namespace Tetris
{
    internal class Piece
    {
        public PieceType PieceType { get; set; }
        public int CoordsY { get; set; } = 0;
        public int CoordsX { get; set; } = 0;
        public int Rotation { get; set; } = 0;
        public int[,] CurrentBlocks { get; set; }

        public Piece(PieceType pieceType)
        {
            PieceType = pieceType;
        }

        public void UpdateCurrentBlocks(PieceBlockManager pieceBlockManager)
        {
            CurrentBlocks = pieceBlockManager.GetBlocks(PieceType, Rotation);
        }
    }
}
