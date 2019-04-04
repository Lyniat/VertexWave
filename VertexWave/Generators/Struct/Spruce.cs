using VertexWave;
using VertexWave.Generators;
using Voxeland.Generators.BlockTypes;

namespace Voxeland.Generators.Struct
{
    public class Spruce : Struct
    {
        public Spruce(int x, int y, int z, Block[] world) : base(world)
        {
            var height = 20;
            for (var yy = 0; yy < height; yy++)
            {
                if (y + yy > WorldGenerator.ChunkHeight - 1) continue;

                /*
                world[
                    x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                    (y + yy) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.Trunk];
                    */
                AddBlock(x, y + yy, z, BlockIDs.Trunk);

                if (yy % 3 != 0)
                {
                    var rad = (height - yy / 2) / 4 - yy % 3;
                    for (var xx = -rad; xx <= rad; xx++)
                    for (var zz = -rad; zz <= rad; zz++)
                    {
                        if (xx == 0 && zz == 0) continue;
                        /*
                            world[
                                (x+xx) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                (y + yy) * (WorldGenerator.ChunkSize) + (z+zz)] = Blocks.blocks[BlockIDs.LeavesSpruce];
                                */
                        AddBlock(x + xx, y + yy, z + zz, BlockIDs.LeavesSpruce);
                    }
                }

                if (yy == height - 1) AddBlock(x, y + yy, z, BlockIDs.LeavesSpruce);
            }
        }
    }
}