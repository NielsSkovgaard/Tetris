namespace Tetris
{
    internal class Piece
    {
        public PieceType PieceType { get; }
        public int CoordsX { get; set; }
        public int CoordsY { get; set; }
        public int Rotation { get; private set; }
        public int[,] Blocks { get; private set; }

        public Piece(PieceType pieceType)
        {
            PieceType = pieceType;
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
            Blocks = PieceBlockManager.GetBlocks(PieceType, Rotation);
        }
    }
}
