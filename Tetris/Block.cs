namespace Tetris
{
    internal class Block
    {
        public int CoordsY { get; }
        public int CoordsX { get; }

        public Block(int coordsY, int coordsX)
        {
            CoordsY = coordsY;
            CoordsX = coordsX;
        }

        protected bool Equals(Block other)
        {
            return CoordsY == other.CoordsY && CoordsX == other.CoordsX;
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
                return hashCode;
            }
        }
    }
}
