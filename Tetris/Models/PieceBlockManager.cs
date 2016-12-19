namespace Tetris.Models
{
    internal static class PieceBlockManager
    {
        private static int[][][,] _blocksBeforeOptimization;
        private static Block[][][] _optimizedBlocks;
        private const int NumberOfBlocksPerPiece = 4;

        static PieceBlockManager()
        {
            BuildBlocksBeforeOptimization();
            BuildOptimizedBlocks();
        }

        public static Block[] Blocks(PieceType pieceType, int rotation)
        {
            Block[][] blocksForAllRotations = _optimizedBlocks[(int)pieceType - 1];
            return blocksForAllRotations[rotation % blocksForAllRotations.Length];
        }

        public static int NumberOfRowsOfBlockArray(PieceType pieceType) => _blocksBeforeOptimization[(int)pieceType - 1][0].GetLength(0);
        public static int NumberOfColsOfBlockArray(PieceType pieceType) => _blocksBeforeOptimization[(int)pieceType - 1][0].GetLength(1);

        private static void BuildBlocksBeforeOptimization()
        {
            _blocksBeforeOptimization = new[]
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

        private static void BuildOptimizedBlocks()
        {
            _optimizedBlocks = new Block[_blocksBeforeOptimization.Length][][];

            for (int pieceType = 0; pieceType < _blocksBeforeOptimization.Length; pieceType++)
            {
                _optimizedBlocks[pieceType] = new Block[_blocksBeforeOptimization[pieceType].Length][];

                for (int rotation = 0; rotation < _blocksBeforeOptimization[pieceType].Length; rotation++)
                {
                    _optimizedBlocks[pieceType][rotation] = new Block[NumberOfBlocksPerPiece];
                    int blockIndex = 0;

                    for (int row = 0; row < _blocksBeforeOptimization[pieceType][rotation].GetLength(0); row++)
                    {
                        for (int col = 0; col < _blocksBeforeOptimization[pieceType][rotation].GetLength(1); col++)
                        {
                            int blockType = _blocksBeforeOptimization[pieceType][rotation][row, col];

                            if (blockType != 0)
                            {
                                _optimizedBlocks[pieceType][rotation][blockIndex] = new Block(row, col);
                                blockIndex++;
                            }
                        }
                    }
                }
            }
        }
    }
}
