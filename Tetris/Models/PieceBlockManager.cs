namespace Tetris.Models
{
    internal static class PieceBlockManager
    {
        private static int[][][,] _nonOptimizedBlocks; // PieceType, Rotation, Block coordinates
        private static Block[][][] _optimizedBlocks;
        private const int NumberOfBlocksPerPiece = 4;

        static PieceBlockManager()
        {
            BuildNonOptimizedBlocks();
            BuildOptimizedBlocks();
        }

        public static Block[] Blocks(PieceType pieceType, int rotation)
        {
            Block[][] blocksForAllRotations = _optimizedBlocks[(int)pieceType - 1];
            return blocksForAllRotations[rotation % blocksForAllRotations.Length];
        }

        public static int NumberOfRowsOfBlockArray(PieceType pieceType) => _nonOptimizedBlocks[(int)pieceType - 1][0].GetLength(0);
        public static int NumberOfColsOfBlockArray(PieceType pieceType) => _nonOptimizedBlocks[(int)pieceType - 1][0].GetLength(1);

        private static void BuildNonOptimizedBlocks()
        {
            _nonOptimizedBlocks = new[]
            {
                new[] // PieceType.I (array index 0)
                {
                    new[,] {{0,0,0,0}, {1,1,1,1}, {0,0,0,0}, {0,0,0,0}}, // Rotation 0
                    new[,] {{0,0,1,0}, {0,0,1,0}, {0,0,1,0}, {0,0,1,0}}, // Rotation 1
                    new[,] {{0,0,0,0}, {0,0,0,0}, {1,1,1,1}, {0,0,0,0}}, // Rotation 2
                    new[,] {{0,1,0,0}, {0,1,0,0}, {0,1,0,0}, {0,1,0,0}} // Rotation 3
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
            // PieceTypes
            int numberOfPieceTypes = _nonOptimizedBlocks.Length;
            _optimizedBlocks = new Block[numberOfPieceTypes][][];

            for (int pieceType = 0; pieceType < numberOfPieceTypes; pieceType++)
            {
                // Rotations
                int numberOfRotations = _nonOptimizedBlocks[pieceType].Length;
                _optimizedBlocks[pieceType] = new Block[numberOfRotations][];

                for (int rotation = 0; rotation < numberOfRotations; rotation++)
                {
                    // Block coordinates
                    int[,] blockArray = _nonOptimizedBlocks[pieceType][rotation];
                    int numberOfRows = blockArray.GetLength(0);
                    int numberOfCols = blockArray.GetLength(1);

                    _optimizedBlocks[pieceType][rotation] = new Block[NumberOfBlocksPerPiece];

                    int blockIndex = 0;

                    for (int row = 0; row < numberOfRows; row++)
                    {
                        for (int col = 0; col < numberOfCols; col++)
                        {
                            if (blockArray[row, col] != 0)
                            {
                                _optimizedBlocks[pieceType][rotation][blockIndex] = new Block(row, col);
                                blockIndex++;

                                // Continue with next Rotation when all four Blocks have been found for current PieceType and Rotation
                                if (blockIndex == NumberOfBlocksPerPiece)
                                {
                                    row = numberOfRows;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
