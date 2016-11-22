using System.Collections.Generic;

namespace Tetris
{
    public class PieceBlockManager
    {
        public Dictionary<PieceType, int[][,]> PieceTypeBlockRotations { get; set; }

        public PieceBlockManager()
        {
            BuildPieceTypeBlockRotations();
        }

        public int[,] GetBlocks(PieceType pieceType, int rotation)
        {
            int[][,] blocksForAllRotations = PieceTypeBlockRotations[pieceType];
            int[,] blocks = blocksForAllRotations[rotation % blocksForAllRotations.Length];
            return blocks;
        }

        private void BuildPieceTypeBlockRotations()
        {
            PieceTypeBlockRotations = new Dictionary<PieceType, int[][,]>
            {
                {
                    PieceType.I, new[]
                    {
                        new[,] {{0, 1, 0, 0}, {0, 1, 0, 0}, {0, 1, 0, 0}, {0, 1, 0, 0}},
                        new[,] {{0, 0, 0, 0}, {1, 1, 1, 1}, {0, 0, 0, 0}, {0, 0, 0, 0}}
                    }
                },
                {
                    PieceType.O, new[]
                    {
                        new[,] {{0, 0, 0, 0}, {0, 2, 2, 0}, {0, 2, 2, 0}, {0, 0, 0, 0}}
                    }
                },
                {
                    PieceType.T, new[]
                    {
                        new[,] {{0, 3, 0, 0}, {0, 3, 3, 0}, {0, 3, 0, 0}, {0, 0, 0, 0}},
                        new[,] {{0, 0, 0, 0}, {0, 3, 3, 3}, {0, 0, 3, 0}, {0, 0, 0, 0}},
                        new[,] {{0, 0, 0, 0}, {0, 0, 3, 0}, {0, 3, 3, 0}, {0, 0, 3, 0}},
                        new[,] {{0, 0, 0, 0}, {0, 3, 0, 0}, {3, 3, 3, 0}, {0, 0, 0, 0}}
                    }
                },
                {
                    PieceType.J, new[]
                    {
                        new[,] {{0, 0, 4, 0}, {0, 0, 4, 0}, {0, 4, 4, 0}, {0, 0, 0, 0}},
                        new[,] {{0, 0, 0, 0}, {0, 4, 0, 0}, {0, 4, 4, 4}, {0, 0, 0, 0}},
                        new[,] {{0, 0, 0, 0}, {0, 4, 4, 0}, {0, 4, 0, 0}, {0, 4, 0, 0}},
                        new[,] {{0, 0, 0, 0}, {4, 4, 4, 0}, {0, 0, 4, 0}, {0, 0, 0, 0}}
                    }
                },
                {
                    PieceType.L, new[]
                    {
                        new[,] {{0, 5, 0, 0}, {0, 5, 0, 0}, {0, 5, 5, 0}, {0, 0, 0, 0}},
                        new[,] {{0, 0, 0, 0}, {0, 5, 5, 5}, {0, 5, 0, 0}, {0, 0, 0, 0}},
                        new[,] {{0, 0, 0, 0}, {0, 5, 5, 0}, {0, 0, 5, 0}, {0, 0, 5, 0}},
                        new[,] {{0, 0, 0, 0}, {0, 0, 5, 0}, {5, 5, 5, 0}, {0, 0, 0, 0}}
                    }
                },
                {
                    PieceType.S, new[]
                    {
                        new[,] {{0, 0, 0, 0}, {0, 6, 6, 0}, {6, 6, 0, 0}, {0, 0, 0, 0}},
                        new[,] {{0, 6, 0, 0}, {0, 6, 6, 0}, {0, 0, 6, 0}, {0, 0, 0, 0}}
                    }
                },
                {
                    PieceType.Z, new[]
                    {
                        new[,] {{0, 0, 0, 0}, {7, 7, 0, 0}, {0, 7, 7, 0}, {0, 0, 0, 0}},
                        new[,] {{0, 0, 7, 0}, {0, 7, 7, 0}, {0, 7, 0, 0}, {0, 0, 0, 0}}
                    }
                }
            };
        }
    }
}
