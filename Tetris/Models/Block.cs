namespace Tetris.Models
{
    internal struct Block
    {
        public int OffsetY { get; }
        public int OffsetX { get; }

        public Block(int offsetY, int offsetX)
        {
            OffsetY = offsetY;
            OffsetX = offsetX;
        }
    }
}
