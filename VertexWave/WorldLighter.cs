using System;
using Voxeland;
using Voxeland.Generators.BlockTypes;

namespace VertexWave
{
    public class WorldLighter
    {
        const int ShadowSize = 2;

        const int MaxParallelLighting = 25;

        public static int CurrentLigthings { get; private set; }

        public static bool Ready
        {
            get
            {
                return CurrentLigthings < MaxParallelLighting;

            }
        }

        public static Block[] LightBlocks(int xPos, int zPos, Block[] blocks)
        {
            CurrentLigthings++;
            var ChunkSize = WorldGenerator.ChunkSize;
            var ChunkHeight = WorldGenerator.ChunkHeight;
            byte[] final = new byte[ChunkSize * ChunkSize * ChunkHeight];

            var diff = Enviroment.MaxLight - Enviroment.Light;

            for (var x = 0; x < WorldGenerator.ChunkSize; x++)
            {
                for (var z = 0; z < WorldGenerator.ChunkSize; z++)
                {
                    for (var y = 0; y < WorldGenerator.ChunkHeight; y++)
                    {
                        var i = 0;
                        var j = 0;

                        var counter = 0;

                        float avgLight = 0;

                        var ownLight = blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + y * (WorldGenerator.ChunkSize) + z].light;

                        for (var partX = -ShadowSize; partX <= ShadowSize; partX++)
                        {
                            for (var partZ = -ShadowSize; partZ <= ShadowSize; partZ++)
                            {
                                for (var partY = 0; partY <= ShadowSize; partY++)
                                {

                                    if(partX != 0 && partZ != 0){
                                        continue;
                                    }
                                    var oldX = x;
                                    var oldY = y;
                                    var oldZ = z;

                                    x += partX;
                                    y += partY;
                                    z += partZ;

                                    if (y >= ChunkHeight)
                                    {
                                        i = 0;
                                        j = 0;

                                        x = oldX;
                                        y = oldY;
                                        z = oldZ;
                                        continue;
                                    }


                                    if (x > ChunkSize - 1)
                                    {
                                        x = x - ChunkSize;
                                        i++;
                                    }
                                    else if (x < 0)
                                    {
                                        x = ChunkSize + x;
                                        i--;
                                    }
                                    if (z > ChunkSize - 1)
                                    {
                                        z = z - ChunkSize;
                                        j++;
                                    }

                                    else if (z < 0)
                                    {
                                        z = ChunkSize + z;
                                        j--;
                                    }

                                    counter++;

                                    byte blockLight = 0;

                                    if(i != 0 || j != 0){
                                        var chunk = World.GetChunkAt(xPos - i, zPos - j);
                                        if(chunk != null){
                                            blockLight = chunk[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + y * (WorldGenerator.ChunkSize) + z].light;
                                        }
                                    }else{
                                        blockLight = blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + y * (WorldGenerator.ChunkSize) + z].light;
                                    }

                                    var val = blockLight - (blockLight / (float)Enviroment.MaxLight) * diff;

                                    //add only influnce of brightener tiles. this also fixed dark lines between chunks 
                                    if (val < ownLight){
                                        counter--;
                                        //continue;
                                    }else{
                                        avgLight += val;
                                    }


                                    i = 0;
                                    j = 0;

                                    x = oldX;
                                    y = oldY;
                                    z = oldZ;
                                }
                            }
                        }
                        final[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + y * (WorldGenerator.ChunkSize) + z] = (byte)(avgLight / counter);
                        /*final[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + y * (WorldGenerator.ChunkSize) + z] = light[
                        i * 3 * ChunkSize * ChunkHeight * ChunkSize +
                                        j * ChunkSize * ChunkHeight * ChunkSize + x * ChunkHeight * ChunkSize +
                            y * ChunkSize + z];*/
                    }
                }
            }
            for (var x = 0; x < ChunkSize; x++)
            {
                for (var z = 0; z < ChunkSize; z++)
                {
                    for (var y = 0/*heightmap[x, z]*/; y < ChunkHeight; y++)
                    {

                        var worldX = xPos * ChunkSize + x;
                        var worldZ = zPos * ChunkSize + z;

                        var block = blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + y * (WorldGenerator.ChunkSize) + z];
                        
                        var lightValue = Math.Pow(final[x * (ChunkSize * ChunkHeight) + y * (ChunkSize) + z], 1) / Enviroment.MaxLight;

                        lightValue /= 5;
                        lightValue *= 4;
                        lightValue *= 255;
                        lightValue += 0.2f;

                        /*
                        var newR = block.r * lightValue;
                        var newG = block.g * lightValue;
                        var newB = block.b * lightValue;

                        if (newR > 255)
                        {
                            newR = 255;
                        }

                        if (newG > 255)
                        {
                            newG = 255;
                        }

                        if (newB > 255)
                        {
                            newB = 255;
                        }

                        if (newR < 0)
                        {
                            newR = 0;
                        }

                        if (newG < 0)
                        {
                            newG = 0;
                        }

                        if (newB < 0)
                        {
                            newB = 0;
                        }

                        block.r = (byte)newR;
                        block.g = (byte)newG;
                        block.b = (byte)newB;

*/
                        block.calculated = (byte)lightValue;
                        blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + y * (WorldGenerator.ChunkSize) + z] = block;
                    }
                }
            }

            CurrentLigthings--;

            return blocks;
        }
    }
}
