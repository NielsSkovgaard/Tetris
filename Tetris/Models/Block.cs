namespace Tetris.Models
{
    internal class Block
    {
        public int OffsetY { get; }
        public int OffsetX { get; }

        public Block(int offsetY, int offsetX)
        {
            OffsetY = offsetY;
            OffsetX = offsetX;
        }

        protected bool Equals(Block other)
        {
            return OffsetY == other.OffsetY && OffsetX == other.OffsetX;
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
                var hashCode = OffsetY;
                hashCode = (hashCode * 397) ^ OffsetX;
                return hashCode;
            }
        }
    }
}
