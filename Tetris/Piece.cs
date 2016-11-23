namespace Tetris
{
    internal class Piece
    {
        public PieceType PieceType { get; }
        public int CoordsY { get; set; } = 0;
        public int CoordsX { get; set; } = 0;
        public int Rotation { get; private set; }
        public int[,] CurrentBlocks { get; private set; }

        private readonly PieceBlockManager _pieceBlockManager;

        public Piece(PieceType pieceType, PieceBlockManager pieceBlockManager)
        {
            PieceType = pieceType;
            _pieceBlockManager = pieceBlockManager;
            UpdateCurrentBlocks();
        }

        public void Rotate()
        {
            Rotation++;
            UpdateCurrentBlocks();
        }

        private void UpdateCurrentBlocks()
        {
            CurrentBlocks = _pieceBlockManager.GetBlocks(PieceType, Rotation);
        }
    }
}
