using System;

namespace Tetris
{
    public static class PieceBlockManager
    {
        public static int[,] GetBlocks(PieceType pieceType, int rotation)
        {
            int[][,] blocksForAllRotations = GetBlocksForAllRotations(pieceType);
            int[,] blocks = blocksForAllRotations[rotation % blocksForAllRotations.Length];
            return blocks;
        }

        public static int[][,] GetBlocksForAllRotations(PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.I:
                    return new[]
                        {
                            new[,] { { 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 1, 0, 0 } },
                            new[,] { { 0, 0, 0, 0 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } }
                        };
                case PieceType.O:
                    return new[]
                        {
                            new[,] { { 0, 0, 0, 0 }, { 0, 2, 2, 0 }, { 0, 2, 2, 0 }, { 0, 0, 0, 0 } }
                        };
                case PieceType.T:
                    return new[]
                        {
                            new[,] { { 0, 3, 0, 0 }, { 0, 3, 3, 0 }, { 0, 3, 0, 0 }, { 0, 0, 0, 0 } },
                            new[,] { { 0, 0, 0, 0 }, { 0, 3, 3, 3 }, { 0, 0, 3, 0 }, { 0, 0, 0, 0 } },
                            new[,] { { 0, 0, 0, 0 }, { 0, 0, 3, 0 }, { 0, 3, 3, 0 }, { 0, 0, 3, 0 } },
                            new[,] { { 0, 0, 0, 0 }, { 0, 3, 0, 0 }, { 3, 3, 3, 0 }, { 0, 0, 0, 0 } }
                        };
                case PieceType.J:
                    return new[]
                        {
                            new[,] { { 0, 0, 4, 0 }, { 0, 0, 4, 0 }, { 0, 4, 4, 0 }, { 0, 0, 0, 0 } },
                            new[,] { { 0, 0, 0, 0 }, { 0, 4, 0, 0 }, { 0, 4, 4, 4 }, { 0, 0, 0, 0 } },
                            new[,] { { 0, 0, 0, 0 }, { 0, 4, 4, 0 }, { 0, 4, 0, 0 }, { 0, 4, 0, 0 } },
                            new[,] { { 0, 0, 0, 0 }, { 4, 4, 4, 0 }, { 0, 0, 4, 0 }, { 0, 0, 0, 0 } }
                        };
                case PieceType.L:
                    return new[]
                        {
                            new[,] { { 0, 5, 0, 0 }, { 0, 5, 0, 0 }, { 0, 5, 5, 0 }, { 0, 0, 0, 0 } },
                            new[,] { { 0, 0, 0, 0 }, { 0, 5, 5, 5 }, { 0, 5, 0, 0 }, { 0, 0, 0, 0 } },
                            new[,] { { 0, 0, 0, 0 }, { 0, 5, 5, 0 }, { 0, 0, 5, 0 }, { 0, 0, 5, 0 } },
                            new[,] { { 0, 0, 0, 0 }, { 0, 0, 5, 0 }, { 5, 5, 5, 0 }, { 0, 0, 0, 0 } }
                        };
                case PieceType.S:
                    return new[]
                        {
                            new[,] { { 0, 0, 0, 0 }, { 0, 6, 6, 0 }, { 6, 6, 0, 0 }, { 0, 0, 0, 0 } },
                            new[,] { { 0, 6, 0, 0 }, { 0, 6, 6, 0 }, { 0, 0, 6, 0 }, { 0, 0, 0, 0 } }
                        };
                case PieceType.Z:
                    return new[]
                        {
                            new[,] { { 0, 0, 0, 0 }, { 7, 7, 0, 0 }, { 0, 7, 7, 0 }, { 0, 0, 0, 0 } },
                            new[,] { { 0, 0, 7, 0 }, { 0, 7, 7, 0 }, { 0, 7, 0, 0 }, { 0, 0, 0, 0 } }
                        };
                default:
                    throw new ArgumentOutOfRangeException(nameof(pieceType), pieceType, null);
            }
        }
    }
}
