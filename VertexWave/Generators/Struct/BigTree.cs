using VertexWave;
using VertexWave.Generators;
using Voxeland.Generators.BlockTypes;

namespace Voxeland.Generators.Struct
{
    public class BigTree : Struct
    {
        private readonly int _height = 14;


        private readonly BlockIDs _leaves;

        public BigTree(int x, int y, int z, byte variation, Block[] world) : base(world)
        {
            switch (variation)
            {
                case 1:
                    _leaves = BlockIDs.LeavesOrange;
                    break;
                case 2:
                    _leaves = BlockIDs.LeavesYellow;
                    break;
                case 3:
                    _leaves = BlockIDs.LeavesGreen;
                    break;
                default:
                    _leaves = BlockIDs.Leaves;
                    break;
            }

            for (var yy = 0; yy < _height; yy++)
            {
                if (y + yy > WorldGenerator.ChunkHeight - 1) continue;

                /*
                world[
                    x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                    (y + yy) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.Trunk];
                    */
                AddBlock(x, y + yy, z, BlockIDs.Trunk);

                if (yy < 7) continue;


                if (yy % 4 == 0)
                {
                    for (var i = 0; i < 3; i++)
                        /*
                            world[
                                (x+i) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                (y + yy) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.Trunk];
                                */
                        AddBlock(x + i, y + yy, z, BlockIDs.Trunk);
                    CreateLeavesAt(x + 3, y + yy, z);
                }

                if (yy % 4 == 1)
                {
                    for (var i = 0; i < 3; i++)
                        /*
                            world[
                                (x) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                (y + yy) * (WorldGenerator.ChunkSize) + (z+i)] = Blocks.blocks[BlockIDs.Trunk];
                                */
                        AddBlock(x, y + yy, z + i, BlockIDs.Trunk);
                    CreateLeavesAt(x, y + yy, z + 3);
                }

                if (yy % 4 == 2)
                {
                    for (var i = 0; i < 3; i++)
                        /*
                            world[
                                (x-i) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                (y + yy) * (WorldGenerator.ChunkSize) + (z)] = Blocks.blocks[BlockIDs.Trunk];
                                */
                        AddBlock(x - i, y + yy, z, BlockIDs.Trunk);

                    CreateLeavesAt(x - 3, y + yy, z);
                }

                if (yy % 4 == 3)
                {
                    for (var i = 0; i < 3; i++)
                        /*
                            world[
                                (x) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                (y + yy) * (WorldGenerator.ChunkSize) + (z-i)] = Blocks.blocks[BlockIDs.Trunk];
                                */
                            AddBlock(x, y + yy, z - i, BlockIDs.Trunk);
                        CreateLeavesAt(x, y + yy, z - 3);
                        }
                        }
                        }

                        private void CreateLeavesAt(int x, int y, int z)
                        {
                            for (var yyy = -1; yyy <= 1; yyy++)
                            for (var xxx = -1 + yyy; xxx <= 1 - yyy; xxx++)
                            for (var zzz = -1 + yyy; zzz <= 1 - yyy; zzz++)
                            {
                                /*
                                        if (_world[
                                                (x + xxx) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                                (y + yyy) * (WorldGenerator.ChunkSize) + (z + zzz)].id !=
                                            (byte) BlockIDs.Air)
                                        {
                                            continue;
                                        }
                                        */

                                if (GetBlock(x + xxx, y + yyy, z + zzz) != BlockIDs.Air)
                                    continue;

                                /*
                                        _world[
                                                (x + xxx) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                                (y + yyy) * (WorldGenerator.ChunkSize) + (z + zzz)] =
                                            leaves;
                                            */
                                AddBlock(x + xxx, y + yyy, z + zzz, _leaves);
                            }
                        }
                        }
                        }