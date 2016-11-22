using System;

namespace Tetris
{
    public static class PieceBlockManager
    {
        public static bool[,] GetBlocks(PieceType pieceType, int rotation)
        {
            bool[][,] blocksForAllRotations = GetBlocksForAllRotations(pieceType);
            bool[,] blocks = blocksForAllRotations[rotation % blocksForAllRotations.Length];
            return blocks;
        }

        public static bool[][,] GetBlocksForAllRotations(PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.I:
                    return new[]
                        {
                            new[,] { { false, true, false, false }, { false, true, false, false }, { false, true, false, false }, { false, true, false, false } },
                            new[,] { { false, false, false, false }, { true, true, true, true }, { false, false, false, false }, { false, false, false, false } }
                        };
                case PieceType.O:
                    return new[]
                        {
                            new[,] { { false, false, false, false }, { false, true, true, false }, { false, true, true, false }, { false, true, true, false } }
                        };
                case PieceType.T:
                    return new[]
                        {
                            new[,] { { false, true, false, false }, { false, true, true, false }, { false, true, false, false }, { false, false, false, false } },
                            new[,] { { false, false, false, false }, { false, true, true, true }, { false, false, true, false }, { false, false, false, false } },
                            new[,] { { false, false, false, false }, { false, false, true, false }, { false, true, true, false }, { false, false, true, false } },
                            new[,] { { false, false, false, false }, { false, true, false, false }, { true, true, true, false }, { false, false, false, false } }
                        };
                case PieceType.J:
                    return new[]
                        {
                            new[,] { { false, false, true, false }, { false, false, true, false }, { false, true, true, false }, { false, false, false, false } },
                            new[,] { { false, false, false, false }, { false, true, false, false }, { false, true, true, true }, { false, false, false, false } },
                            new[,] { { false, false, false, false }, { false, true, true, false }, { false, true, false, false }, { false, true, false, false } },
                            new[,] { { false, false, false, false }, { true, true, true, false }, { false, false, true, false }, { false, false, false, false } }
                        };
                case PieceType.L:
                    return new[]
                        {
                            new[,] { { false, true, false, false }, { false, true, false, false }, { false, true, true, false }, { false, false, false, false } },
                            new[,] { { false, false, false, false }, { false, true, true, true }, { false, true, false, false }, { false, false, false, false } },
                            new[,] { { false, false, false, false }, { false, true, true, false }, { false, false, true, false }, { false, false, true, false } },
                            new[,] { { false, false, false, false }, { false, false, true, false }, { true, true, true, false }, { false, false, false, false } }
                        };
                case PieceType.S:
                    return new[]
                        {
                            new[,] { { false, false, false, false }, { false, true, true, false }, { true, true, false, false }, { false, false, false, false } },
                            new[,] { { false, true, false, false }, { false, true, true, false }, { false, false, true, false }, { false, false, false, false } }
                        };
                case PieceType.Z:
                    return new[]
                        {
                            new[,] { { false, false, false, false }, { true, true, false, false }, { false, true, true, false }, { false, false, false, false } },
                            new[,] { { false, false, true, false }, { false, true, true, false }, { false, true, false, false }, { false, false, false, false } }
                        };
                default:
                    throw new ArgumentOutOfRangeException(nameof(pieceType), pieceType, null);
            }
        }
    }
}
