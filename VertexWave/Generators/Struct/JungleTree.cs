using System;
using VertexWave;
using VertexWave.Generators;
using Voxeland.Generators.BlockTypes;

namespace Voxeland.Generators.Struct
{
    public class JungleTree : Struct
    {
        int height = 25;
        Random r = new Random();
        public JungleTree(int x, int y, int z, Block[] world) : base(world)
        {
            var saveZone = 1;

            if (x < saveZone || z < saveZone || x > WorldGenerator.ChunkSize - 1 - saveZone ||
                z > WorldGenerator.ChunkSize - 1 - saveZone)
            {
                return;
            }

            for (var yy = 0; yy < height; yy++)
            {
                for (var xx = 0; xx < 2; xx++)
                {
                    for (var zz = 0; zz < 2; zz++)
                    {
                        /*
                        world[
                            (x + xx) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                            (y + yy) * (WorldGenerator.ChunkSize) + (z + zz)] = Blocks.blocks[BlockIDs.Trunk];
                            */
                        AddBlock(x + xx, y + yy, z + zz, BlockIDs.Trunk);
                    }
                }

                if (yy >= 3 && yy <= height-4)
                {
                    var xx = (int) ((yy - 3) / 2);
                    /*
                    world[
                        (x + xx) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                        (y + yy) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.Trunk];
                    world[
                        (x + xx) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                        (y - 1+ yy) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.Trunk];
                        */
                    AddBlock(x + xx, y + yy, z, BlockIDs.Trunk);
                    AddBlock(x + xx , y + yy - 1, z, BlockIDs.Trunk);
                    if (yy == height - 4)
                    {
                        CreateLeavesAt(x+xx,y+yy,z);
                    }
                }

                if (yy >= 6 && yy <= height-3)
                {
                    var zz = (int) ((yy - 6) / 2);
                    /*
                    world[
                        (x) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                        (y + yy) * (WorldGenerator.ChunkSize) + (z + zz)] = Blocks.blocks[BlockIDs.Trunk];
                    world[
                        (x) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                        (y -1 + yy) * (WorldGenerator.ChunkSize) + (z + zz)] = Blocks.blocks[BlockIDs.Trunk];
                        */
                    AddBlock(x, y + yy, z + zz, BlockIDs.Trunk);
                    AddBlock(x , y + yy - 1, z + zz, BlockIDs.Trunk);
                    if (yy == height - 3)
                    {
                        CreateLeavesAt(x,y+yy,z+zz);
                    }
                }

                if (yy >= 9 && yy <= height-3)
                {
                    var xx = (int) ((yy - 9) / 2);
                    /*
                    world[
                        (x - xx) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                        (y + yy) * (WorldGenerator.ChunkSize) + (z)] = Blocks.blocks[BlockIDs.Trunk];
                    world[
                        (x - xx) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                        (y -1 + yy) * (WorldGenerator.ChunkSize) + (z)] = Blocks.blocks[BlockIDs.Trunk];
                        */
                    AddBlock(x-xx, y + yy, z, BlockIDs.Trunk);
                    AddBlock(x-xx , y + yy - 1, z, BlockIDs.Trunk);
                    
                    if (yy == height - 3)
                    {
                        CreateLeavesAt(x-xx,y+yy,z);
                    }
                }

                if (yy >= 9 && yy <= height-2)
                {
                    var zz = (int) ((yy - 9) / 2);
                    /*
                    world[
                        (x) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                        (y + yy) * (WorldGenerator.ChunkSize) + (z - zz)] = Blocks.blocks[BlockIDs.Trunk];
                    world[
                        (x) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                        (y + yy -1) * (WorldGenerator.ChunkSize) + (z - zz)] = Blocks.blocks[BlockIDs.Trunk];
                        */
                    AddBlock(x, y + yy, z - zz, BlockIDs.Trunk);
                    AddBlock(x , y + yy - 1, z - zz, BlockIDs.Trunk);

                    if (yy == height - 2)
                    {
                        CreateLeavesAt(x,y+yy,z-zz);
                    }
                }

                if (yy == height - 1)
                {
                    for (var yyy = -4; yyy <= 4; yyy++)
                    {
                        var addY = 4-Math.Abs(yyy);
                        for (var xxx = -3 -addY; xxx <= 3 + addY; xxx++)
                        {
                            for (var zzz = -3 -addY; zzz <= 3 + addY; zzz++)
                            {
                                /*
                                if (_world[
                                        (x + xxx) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                        (y +yy + yyy) * (WorldGenerator.ChunkSize) + (z + zzz)].id !=
                                    (byte) BlockIDs.Air)
                                {
                                    continue;
                                }

                                _world[
                                        (x + xxx) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                        (y + yy + yyy) * (WorldGenerator.ChunkSize) + (z + zzz)] =
                                    Blocks.blocks[BlockIDs.LeavesGreen];
                                    */
                                if (GetBlock(x + xxx, y + yy+  yyy, z + zzz) != BlockIDs.Air)
                                {
                                    continue;
                                }
                                AddBlock(x + xxx, y + yy + yyy, z + zzz, BlockIDs.LeavesGreen);
                            }
                        }
                    }
                }
            }

        }

    private void CreateLeavesAt(int x, int y, int z){
        for (var yyy = -3; yyy <= 0; yyy++)
        {
            for (var xxx = -1 + yyy; xxx <= 1 - yyy; xxx++)
            {
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
                    /*
                    _world[
                            (x + xxx) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                            (y + yyy) * (WorldGenerator.ChunkSize) + (z + zzz)] =
                        Blocks.blocks[BlockIDs.LeavesGreen];
                        */
                    if (GetBlock(x + xxx, y + yyy, z + zzz) != BlockIDs.Air)
                    {
                        continue;
                    }
                    AddBlock(x + xxx, y + yyy, z + zzz, BlockIDs.LeavesGreen);
                }
            }
        }
    }
}

}