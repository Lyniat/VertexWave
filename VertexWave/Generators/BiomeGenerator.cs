using System.Collections.Generic;
using VertexWave.Interfaces;

namespace VertexWave.Generators
{
    public class BiomeGenerator : IBiomeGenerator
    {
        private List<(int x, int z, Biomes[,])> biomemapList = new List<(int x, int z, Biomes[,])>();

        private FastNoise biome;
        public BiomeGenerator(int seed)
        {
            biome = new FastNoise();
            biome.SetSeed(seed++);
            biome.SetNoiseType(FastNoise.NoiseType.Cellular);
            biome.SetFrequency(0.005f);
            biome.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Natural);
            biome.SetCellularReturnType(FastNoise.CellularReturnType.Distance);
            biome.SetCellularJitter(0.3f);
            biome.SetGradientPerturbAmp(40f);
        }
        
        public Biomes[,] GetBiomemapAt(int x, int z)
        {
            return CreateBiomeMapAt(x, z);
        }

        private Biomes[,] CreateBiomeMapAt(int xPos, int zPos)
        {
            var biomeMap = new Biomes[WorldGenerator.ChunkSize, WorldGenerator.ChunkSize];
            for (var x = 0; x < WorldGenerator.ChunkSize; x++)
            {
                for (var z = 0; z < WorldGenerator.ChunkSize; z++)
                {
                    var worldX = xPos * WorldGenerator.ChunkSize + x;
                    var worldZ = zPos * WorldGenerator.ChunkSize + z;
                    //biome.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
                    var b = biome.GetNoise(worldX, worldZ);
                    Biomes biomeType;

                    biomeType = Biomes.Tundra;
                    if (b > 0.2)
                    {
                        biomeType = Biomes.Plains;
                    }

                    if (b > 0.4)
                    {
                        biomeType = Biomes.Forest;
                    }

                    if (b > 0.6)
                    {
                        biomeType = Biomes.Jungle;
                    }

                    biomeMap[x, z] = biomeType;
                }
            }

            return biomeMap;
        }
    }
}
