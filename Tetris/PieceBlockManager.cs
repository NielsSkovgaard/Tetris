namespace Tetris
{
    public class PieceBlockManager
    {
        public int[][][,] PieceTypeBlockRotations { get; set; }

        public PieceBlockManager()
        {
            BuildPieceTypeBlockRotations();
        }

        public int[,] GetBlocks(PieceType pieceType, int rotation)
        {
            int[][,] blocksForAllRotations = PieceTypeBlockRotations[(int)pieceType - 1];
            int[,] blocks = blocksForAllRotations[rotation % blocksForAllRotations.Length];
            return blocks;
        }

        private void BuildPieceTypeBlockRotations()
        {
            PieceTypeBlockRotations = new[]
            {
                new[] //PieceType.I (array index 0)
                {
                    new[,] {{0, 1, 0, 0}, {0, 1, 0, 0}, {0, 1, 0, 0}, {0, 1, 0, 0}},
                    new[,] {{0, 0, 0, 0}, {1, 1, 1, 1}, {0, 0, 0, 0}, {0, 0, 0, 0}}
                },
                new[] //PieceType.O (array index 1)
                {
                    new[,] {{0, 0, 0, 0}, {0, 2, 2, 0}, {0, 2, 2, 0}, {0, 0, 0, 0}}
                },
                new[] //PieceType.T (array index 2)
                {
                    new[,] {{0, 3, 0, 0}, {0, 3, 3, 0}, {0, 3, 0, 0}, {0, 0, 0, 0}},
                    new[,] {{0, 0, 0, 0}, {0, 3, 3, 3}, {0, 0, 3, 0}, {0, 0, 0, 0}},
                    new[,] {{0, 0, 0, 0}, {0, 0, 3, 0}, {0, 3, 3, 0}, {0, 0, 3, 0}},
                    new[,] {{0, 0, 0, 0}, {0, 3, 0, 0}, {3, 3, 3, 0}, {0, 0, 0, 0}}
                },
                new[] //PieceType.J (array index 3)
                {
                    new[,] {{0, 0, 4, 0}, {0, 0, 4, 0}, {0, 4, 4, 0}, {0, 0, 0, 0}},
                    new[,] {{0, 0, 0, 0}, {0, 4, 0, 0}, {0, 4, 4, 4}, {0, 0, 0, 0}},
                    new[,] {{0, 0, 0, 0}, {0, 4, 4, 0}, {0, 4, 0, 0}, {0, 4, 0, 0}},
                    new[,] {{0, 0, 0, 0}, {4, 4, 4, 0}, {0, 0, 4, 0}, {0, 0, 0, 0}}
                },
                new[] //PieceType.L (array index 4)
                {
                    new[,] {{0, 5, 0, 0}, {0, 5, 0, 0}, {0, 5, 5, 0}, {0, 0, 0, 0}},
                    new[,] {{0, 0, 0, 0}, {0, 5, 5, 5}, {0, 5, 0, 0}, {0, 0, 0, 0}},
                    new[,] {{0, 0, 0, 0}, {0, 5, 5, 0}, {0, 0, 5, 0}, {0, 0, 5, 0}},
                    new[,] {{0, 0, 0, 0}, {0, 0, 5, 0}, {5, 5, 5, 0}, {0, 0, 0, 0}}
                },
                new[] //PieceType.S (array index 5)
                {
                    new[,] {{0, 0, 0, 0}, {0, 6, 6, 0}, {6, 6, 0, 0}, {0, 0, 0, 0}},
                    new[,] {{0, 6, 0, 0}, {0, 6, 6, 0}, {0, 0, 6, 0}, {0, 0, 0, 0}}
                },
                new[] //PieceType.Z (array index 6)
                {
                    new[,] {{0, 0, 0, 0}, {7, 7, 0, 0}, {0, 7, 7, 0}, {0, 0, 0, 0}},
                    new[,] {{0, 0, 7, 0}, {0, 7, 7, 0}, {0, 7, 0, 0}, {0, 0, 0, 0}}
                }
            };
        }
    }
}
