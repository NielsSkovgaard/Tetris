using System;

namespace Tetris
{
    internal class PieceBlockManager
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

        public static int GetLeftmostBlockIndex(int[,] blockArray)
        {
            int yLength = blockArray.GetLength(0); //1 or 4
            int xLength = blockArray.GetLength(1); //3 or 4

            for (int x = 0; x < xLength; x++)
                for (int y = 0; y < yLength; y++)
                    if (blockArray[y, x] > 0)
                        return x;

            throw new InvalidOperationException();
        }

        public static int GetRightmostBlockIndex(int[,] blockArray)
        {
            int yLength = blockArray.GetLength(0); //1 or 4
            int xLength = blockArray.GetLength(1); //3 or 4

            for (int x = xLength - 1; x >= 0; x--)
                for (int y = 0; y < yLength; y++)
                    if (blockArray[y, x] > 0)
                        return x;

            throw new InvalidOperationException();
        }

        private void BuildPieceTypeBlockRotations()
        {
            PieceTypeBlockRotations = new[]
            {
                new[] //PieceType.I (array index 0)
                {
                    new[,] {{0,0,0,0}, {1,1,1,1}, {0,0,0,0}, {0,0,0,0}},
                    new[,] {{0,0,1,0}, {0,0,1,0}, {0,0,1,0}, {0,0,1,0}},
                    new[,] {{0,0,0,0}, {0,0,0,0}, {1,1,1,1}, {0,0,0,0}},
                    new[,] {{0,1,0,0}, {0,1,0,0}, {0,1,0,0}, {0,1,0,0}}
                },
                new[] //PieceType.O (array index 1)
                {
                    new[,] {{0,2,2,0}, {0,2,2,0}, {0,0,0,0}}
                },
                new[] //PieceType.T (array index 2)
                {
                    new[,] {{0,3,0}, {3,3,3}, {0,0,0}},
                    new[,] {{0,3,0}, {0,3,3}, {0,3,0}},
                    new[,] {{0,0,0}, {3,3,3}, {0,3,0}},
                    new[,] {{0,3,0}, {3,3,0}, {0,3,0}}
                },
                new[] //PieceType.J (array index 3)
                {
                    new[,] {{4,0,0}, {4,4,4}, {0,0,0}},
                    new[,] {{0,4,4}, {0,4,0}, {0,4,0}},
                    new[,] {{0,0,0}, {4,4,4}, {0,0,4}},
                    new[,] {{0,4,0}, {0,4,0}, {4,4,0}}
                },
                new[] //PieceType.L (array index 4)
                {
                    new[,] {{0,0,5}, {5,5,5}, {0,0,0}},
                    new[,] {{0,5,0}, {0,5,0}, {0,5,5}},
                    new[,] {{0,0,0}, {5,5,5}, {5,0,0}},
                    new[,] {{5,5,0}, {0,5,0}, {0,5,0}}
                },
                new[] //PieceType.S (array index 5)
                {
                    new[,] {{0,6,6}, {6,6,0}, {0,0,0}},
                    new[,] {{0,6,0}, {0,6,6}, {0,0,6}},
                    new[,] {{0,0,0}, {0,6,6}, {6,6,0}},
                    new[,] {{6,0,0}, {6,6,0}, {0,6,0}}
                },
                new[] //PieceType.Z (array index 6)
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
