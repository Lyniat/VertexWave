using VertexWave;
using VertexWave.Generators;
using Voxeland.Generators.BlockTypes;

namespace Voxeland.Generators.Struct
{
    public class Tree : Struct
    {
        public Tree(int x, int y, int z, Block[] world) : base(world)
        {
            for (var yy = 0; yy < 8; yy++)
            {
                if (y + yy > WorldGenerator.ChunkHeight - 1) continue;

                /*
                world[
                    x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                    (y + yy) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.Trunk];
                    */
                AddBlock(x, y + yy, z, BlockIDs.Trunk);

                if (yy == 3 || yy == 4)
                    for (var xx = -2; xx <= 2; xx++)
                    for (var zz = -2; zz <= 2; zz++)
                    {
                        if (xx == 0 && zz == 0) continue;
                        /*
                            world[
                                (x+xx) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                (y + yy) * (WorldGenerator.ChunkSize) + (z+zz)] = Blocks.blocks[BlockIDs.Leaves];
                                */
                        AddBlock(x + xx, y + yy, z + zz, BlockIDs.Leaves);
                    }
                else if (yy == 5 || yy == 6)
                    for (var xx = -1; xx <= 1; xx++)
                    for (var zz = -1; zz <= 1; zz++)
                    {
                        if (xx == 0 && zz == 0) continue;
                        /*
                            world[
                                (x+xx) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                (y + yy) * (WorldGenerator.ChunkSize) + (z+zz)] = Blocks.blocks[BlockIDs.Leaves];
                                */
                        AddBlock(x, y + yy, z, BlockIDs.Leaves);
                    }
                else if (yy == 7) AddBlock(x, y + yy, z, BlockIDs.Leaves);
            }
        }
    }
}