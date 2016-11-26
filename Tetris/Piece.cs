namespace Tetris
{
    internal class Piece
    {
        public PieceType PieceType { get; }
        public int CoordsX { get; set; }
        public int CoordsY { get; set; }
        public int Rotation { get; private set; }
        public int[,] Blocks { get; private set; }

        private readonly PieceBlockManager _pieceBlockManager;

        public Piece(PieceType pieceType, PieceBlockManager pieceBlockManager)
        {
            PieceType = pieceType;
            _pieceBlockManager = pieceBlockManager;
            UpdateCurrentBlocks();
        }

        public void MoveLeft()
        {
            CoordsX--;
        }

        public void MoveRight()
        {
            CoordsX++;
        }

        public void MoveDown()
        {
            CoordsY++;
        }

        public void Rotate()
        {
            Rotation++;
            UpdateCurrentBlocks();
        }

        private void UpdateCurrentBlocks()
        {
            Blocks = _pieceBlockManager.GetBlocks(PieceType, Rotation);
        }
    }
}
