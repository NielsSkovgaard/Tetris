using System;

namespace Tetris
{
    internal static class PieceBlockManager
    {
        private static int[][][,] _pieceTypeBlockRotations;

        static PieceBlockManager()
        {
            BuildPieceTypeBlockRotations();
        }

        public static int[,] GetBlocks(PieceType pieceType, int rotation)
        {
            int[][,] blocksForAllRotations = _pieceTypeBlockRotations[(int)pieceType - 1];
            int[,] blocks = blocksForAllRotations[rotation % blocksForAllRotations.Length];
            return blocks;
        }

        public static int GetLeftmostBlockIndex(int[,] blocks)
        {
            // The rows and cols of the blocks of a piece
            int rows = blocks.GetLength(0); // 3 or 4
            int cols = blocks.GetLength(1); // 3 or 4

            for (int c = 0; c < cols; c++)
                for (int r = 0; r < rows; r++)
                    if (blocks[r, c] > 0)
                        return c;

            throw new InvalidOperationException();
        }

        public static int GetRightmostBlockIndex(int[,] blocks)
        {
            // The rows and cols of the blocks of a piece
            int rows = blocks.GetLength(0); // 3 or 4
            int cols = blocks.GetLength(1); // 3 or 4

            for (int c = cols - 1; c >= 0; c--)
                for (int r = 0; r < rows; r++)
                    if (blocks[r, c] > 0)
                        return c;

            throw new InvalidOperationException();
        }

        private static void BuildPieceTypeBlockRotations()
        {
            _pieceTypeBlockRotations = new[]
            {
                new[] // PieceType.I (array index 0)
                {
                    new[,] {{0,0,0,0}, {1,1,1,1}, {0,0,0,0}, {0,0,0,0}},
                    new[,] {{0,0,1,0}, {0,0,1,0}, {0,0,1,0}, {0,0,1,0}},
                    new[,] {{0,0,0,0}, {0,0,0,0}, {1,1,1,1}, {0,0,0,0}},
                    new[,] {{0,1,0,0}, {0,1,0,0}, {0,1,0,0}, {0,1,0,0}}
                },
                new[] // PieceType.O (array index 1)
                {
                    new[,] {{0,2,2,0}, {0,2,2,0}, {0,0,0,0}}
                },
                new[] // PieceType.T (array index 2)
                {
                    new[,] {{0,3,0}, {3,3,3}, {0,0,0}},
                    new[,] {{0,3,0}, {0,3,3}, {0,3,0}},
                    new[,] {{0,0,0}, {3,3,3}, {0,3,0}},
                    new[,] {{0,3,0}, {3,3,0}, {0,3,0}}
                },
                new[] // PieceType.J (array index 3)
                {
                    new[,] {{4,0,0}, {4,4,4}, {0,0,0}},
                    new[,] {{0,4,4}, {0,4,0}, {0,4,0}},
                    new[,] {{0,0,0}, {4,4,4}, {0,0,4}},
                    new[,] {{0,4,0}, {0,4,0}, {4,4,0}}
                },
                new[] // PieceType.L (array index 4)
                {
                    new[,] {{0,0,5}, {5,5,5}, {0,0,0}},
                    new[,] {{0,5,0}, {0,5,0}, {0,5,5}},
                    new[,] {{0,0,0}, {5,5,5}, {5,0,0}},
                    new[,] {{5,5,0}, {0,5,0}, {0,5,0}}
                },
                new[] // PieceType.S (array index 5)
                {
                    new[,] {{0,6,6}, {6,6,0}, {0,0,0}},
                    new[,] {{0,6,0}, {0,6,6}, {0,0,6}},
                    new[,] {{0,0,0}, {0,6,6}, {6,6,0}},
                    new[,] {{6,0,0}, {6,6,0}, {0,6,0}}
                },
                new[] // PieceType.Z (array index 6)
                {
                    new[,] {{7,7,0}, {0,7,7}, {0,0,0}},
                    new[,] {{0,0,7}, {0,7,7}, {0,7,0}},
                    new[,] {{0,0,0}, {7,7,0}, {0,7,7}},
                    new[,] {{0,7,0}, {7,7,0}, {7,0,0}}
                }
            };
        }
    }
}
