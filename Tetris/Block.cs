namespace Tetris
{
    internal class Block
    {
        public int CoordsY { get; }
        public int CoordsX { get; }
        public int BlockType { get; }

        public Block(int coordsY, int coordsX, int blockType)
        {
            CoordsY = coordsY;
            CoordsX = coordsX;
            BlockType = blockType;
        }

        protected bool Equals(Block other)
        {
            return CoordsY == other.CoordsY && CoordsX == other.CoordsX && BlockType == other.BlockType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Block)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CoordsY;
                hashCode = (hashCode * 397) ^ CoordsX;
                hashCode = (hashCode * 397) ^ BlockType;
                return hashCode;
            }
        }
    }
}
