using System;
using System.Collections.Generic;
using VertexWave.Generators;
using VertexWave.Interfaces;
using Voxeland.Generators.BlockTypes;
using Voxeland.Generators.Struct;

namespace VertexWave
{
    public class WorldGenerator : IWorldGenerator
    {
        public const int ChunkSize = 16;
        public const int ChunkHeight = 100;
        public const int PathSize = 8;//5;
        public const int PathHeight = 20;
        const int ShadowSize = 2;
        const int InterpolationSize = 16;
        const bool CalculateShadow = false;
        const bool ShadowModeFast = false;

        private const int SeaLevel = 60;

        private static Dictionary<(int, int), ChunkStruct> world = new Dictionary<(int, int), ChunkStruct>();

        private Biomes[,] biomeMap;

        private FastNoise noise;

        private FastNoise cell;
        private FastNoise biome;
        private FastNoise technicalBiomeValue;
        private FastNoise technicalBiome;
        private FastNoise cave;
        private FastNoise white;

        private FastNoise caveNoise;
        private FastNoise caveNoiseHeight;

        private FastNoise river;

        private FastNoise color;

        int seed;

        private int _xPos;
        private int _zPos;

        private bool showArrows = true;

        //private HeightGenerator _heightGenerator;
        private BiomeGenerator _biomeGenerator;
        //private RiverGenerator _riverGenerator;

        public WorldGenerator()
        {
            seed = DateTime.Now.Millisecond - new DateTime(1970, 1, 1).Millisecond;

            //_heightGenerator = new HeightGenerator(seed++);
            _biomeGenerator = new BiomeGenerator(seed++);
            //_riverGenerator = new RiverGenerator(seed++);

            noise = new FastNoise();
            noise.SetSeed(seed++);
            noise.SetNoiseType(FastNoise.NoiseType.CubicFractal);

            cell = new FastNoise();
            cell.SetSeed(seed++);
            cell.SetFrequency(0.05f);
            cell.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            cell.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Euclidean);

            biome = new FastNoise();
            biome.SetSeed(seed++);
            biome.SetNoiseType(FastNoise.NoiseType.Cellular);
            biome.SetFrequency(0.005f);
            biome.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Natural);
            biome.SetCellularReturnType(FastNoise.CellularReturnType.Distance);
            biome.SetCellularJitter(0.3f);
            biome.SetGradientPerturbAmp(40f);

            technicalBiome = new FastNoise();
            technicalBiome.SetSeed(seed++);
            technicalBiome.SetNoiseType(FastNoise.NoiseType.Cellular);
            technicalBiome.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Euclidean);

            technicalBiomeValue = new FastNoise();
            technicalBiomeValue.SetSeed(seed++);
            technicalBiomeValue.SetNoiseType(FastNoise.NoiseType.SimplexFractal);

            cave = new FastNoise();
            cave.SetSeed(seed++);
            cave.SetNoiseType(FastNoise.NoiseType.SimplexFractal);

            caveNoise = new FastNoise();
            caveNoise.SetSeed(seed++);
            caveNoise.SetNoiseType(FastNoise.NoiseType.Cellular);
            caveNoise.SetFrequency(0.005f);
            caveNoise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Natural);
            caveNoise.SetCellularReturnType(FastNoise.CellularReturnType.Distance2Div);
            caveNoise.SetCellularJitter(0.3f);
            caveNoise.SetGradientPerturbAmp(40f);

            caveNoiseHeight = new FastNoise();
            caveNoiseHeight.SetSeed(seed++);
            caveNoiseHeight.SetNoiseType(FastNoise.NoiseType.PerlinFractal);

            white = new FastNoise();
            white.SetSeed(seed++);
            white.SetNoiseType(FastNoise.NoiseType.WhiteNoise);

            river = new FastNoise();
            river.SetSeed(seed++);
            river.SetNoiseType(FastNoise.NoiseType.Cellular);
            river.SetFrequency(0.005f);
            river.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Natural);
            river.SetCellularReturnType(FastNoise.CellularReturnType.Distance2Div);
            river.SetCellularJitter(0.3f);
            river.SetGradientPerturbAmp(40);

            color = new FastNoise();
            color.SetNoiseType(FastNoise.NoiseType.SimplexFractal);

        }

        public ChunkStruct GetChunkAt(int xPos, int zPos)
        {

            ChunkStruct chunkStruct = new ChunkStruct();
            chunkStruct.block = Calculate(xPos, zPos, true);
            _xPos = xPos;
            _zPos = zPos;
            return chunkStruct;

        }

        public Block[] Calculate(int xPos, int zPos, bool calcMesh = false)
        {

            biomeMap = _biomeGenerator.GetBiomemapAt(xPos, zPos);

            //int[,] heightmap = _heightGenerator.GetHeightmapAt(xPos, zPos);

            Block[] blocks = World.GetChunkAt(xPos, zPos);

            if (blocks == null)
            {
                blocks = new Block[ChunkSize * ChunkHeight * ChunkSize];
            }

            FastNoise caveNoise = new FastNoise();
            caveNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            caveNoise.SetFrequency(0.02f);
            caveNoise.SetFractalOctaves(2);
            caveNoise.SetFractalType(FastNoise.FractalType.Billow);
            caveNoise.SetFractalLacunarity(1.5f);
            caveNoise.SetFractalGain(0.7f);

            //var riverNoiseMap = _riverGenerator.GetRivermapAt(xPos, zPos);

            blocks[0] = Blocks.blocks[BlockIDs.Dummy];

            for (var x = 0; x < ChunkSize; x++)
            {
                for (var z = 0; z < ChunkSize; z++)
                {
                    var worldX = xPos * ChunkSize + x;
                    var worldZ = zPos * ChunkSize + z;


                    for (var y = /*heightmap[x, z] - 1*/ 0; y <= PathHeight; y++)
                    {
                        var extraSize = 140 + worldZ/2;
                        if(extraSize < 0)
                        {
                            showArrows = false;
                            extraSize = 0;
                        }
                        else
                        {
                            showArrows = true;
                        }
                        var path = Math.Sin(worldZ / 15f) * 5f + Math.Sin(worldZ / 20f) * 3f + Math.Sin(worldZ / 50f) * 15f;

                        var newPathSize = PathSize - Math.Abs((Math.Sin(worldZ / 11f) * 3f)) + extraSize;

                        var underPath = worldX > -newPathSize + path - (PathHeight - y)/2f&& worldX < newPathSize + path + (PathHeight-y)/2f;
                        if (underPath)
                        {
                            if(y == PathHeight){
                                blocks[
x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
y * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.MatrixFloor];
                            }
                            else{
                                continue;
                            }

                        }

                        var noise3DX = worldX;
                        var noise3DZ = worldZ;

                        if (x == 0)
                        {
                            noise3DX--;
                        }

                        if (z == 0)
                        {
                            noise3DZ--;
                        }

                        if (caveNoise.GetNoise(noise3DX, y, noise3DZ) < 0)
                        {
                            continue;
                        }

                        if (y < 0)
                        {
                            continue;
                        }

                        if (blocks[
                                x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                y * (WorldGenerator.ChunkSize) + z].id != 0)
                        {
                            continue;
                        }

                        var absX = ChunkSize + x;
                        var absZ = ChunkSize + z;

                        Block block = new Block();

                        if (y <= PathHeight)
                        {
                            block = Blocks.blocks[BlockIDs.Dirt];
                        }

                        if (y == 0)
                        {
                            block = Blocks.blocks[BlockIDs.Dirt];
                        }

                        var noiseAtX = worldX;
                        var noiseAtZ = worldZ;

                        if (x == 0)
                        {
                            noiseAtX--;
                        }

                        if (z == 0)
                        {
                            noiseAtZ--;
                        }

                        blocks[
                            x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                            y * (WorldGenerator.ChunkSize) + z] = block;


                    }
                }
            }

                    for (var x = 0; x < ChunkSize; x++)
            {
                
                for (var z = 0; z < ChunkSize; z++)
                {
                    var worldX = xPos * ChunkSize + x;
                    var worldZ = zPos * ChunkSize + z;
                    cave.SetNoiseType(FastNoise.NoiseType.WhiteNoise);
                    {
                        if (cave.GetNoise(-worldX, -worldZ) < 0.8)
                        {
                            continue;
                        }
                    }
                    for (var y = ChunkHeight - 1; y > 0; y--)
                    {
                        var id = blocks[
                            x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                            y * (WorldGenerator.ChunkSize) + z].id;
                        if (id == (byte) BlockIDs.Water)
                        {
                            break;
                        }

                        if (id == (byte) BlockIDs.Dirt || id == (byte) BlockIDs.GrassTop ||
                            id == (byte) BlockIDs.Snow || id == (byte) BlockIDs.Sand || id == (byte) BlockIDs.Stone)
                        {
                            var n = (white.GetNoise(-worldX, -worldZ) + 1) / 2f;
     
                            switch (biomeMap[x, z])
                            {
                                case Biomes.Plains:
                                    if (n > 0.997)
                                    {
                                        new Tree(x, y, z,blocks);
                                    }
                                    else if (n > 0.2)
                                    {
                                        
                                        blocks[
                                                x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                                (y + 1) * (WorldGenerator.ChunkSize) + z] =
                                            Blocks.blocks[BlockIDs.FlowerPoppy];
                                            
                                    }
                                    else if (n > -0.4)
                                    {
                                        
                                        blocks[
                                            x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                            (y + 1) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.FlowerTulip];
                                            
                                    }

                                    break;

                                case Biomes.Tundra:
                                    if (n > 0.97)
                                    {
                                        new Spruce(x, y, z, blocks);
                                    }
                                    
                                    else if (n > 0.4)
                                    {
                                        blocks[
                                            x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                            (y + 1) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.Plant];
                                    }

                                    break;

                                case Biomes.Jungle:
                                    
                                    if (n > 0.95)
                                    {
                                        new JungleTreeSmall(x, y, z, blocks);
                                    }
                                    
                                    if (n > 0.96)
                                    {
                                        new JungleTree(x, y, z, blocks);
                                    }
                                    else if (n > 0.6)
                                    {
                                        
                                        blocks[
                                            x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                            (y + 1) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.FlowerTulip];
                                            
                                    }
                                    else if (n > 0.3)
                                    {
                                        
                                        blocks[
                                            x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                            (y + 1) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.FlowerPoppy];
                                            
                                    }


                                    break;

                                case Biomes.Forest:
                                    if (n > 0.9)
                                    {
                                        var nextNoise = (white.GetNoise(worldZ, worldX) + 1) / 2;
                                        byte type = 0;
                                        if (nextNoise > 0.3)
                                        {
                                            type = 3;
                                        }

                                        if (nextNoise > 0.95)
                                        {
                                            type = 1;
                                        }

                                        if (nextNoise > 0.975)
                                        {
                                            type = 2;
                                        }

                                        new BigTree(x, y, z, type, blocks);
                                    }
                                    else if (n > 0.80)
                                    {
                                        blocks[
                                            x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                            (y + 1) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.Plant];
                                    }

                                    break;
                            }

                            break;
                        }

                    }
                }
            }

            for (var x = 0; x < ChunkSize; x++)
            {
                
                for (var z = 0; z < ChunkSize; z++)
                {
                    for (var y = PathHeight; y < ChunkHeight; y++)
                    {
                        if (y < ChunkHeight - 1)
                        {
                            var block = blocks[
                                x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                y * (WorldGenerator.ChunkSize) + z];
                            if (block.id == (byte) BlockIDs.Dirt || block.id == (byte) BlockIDs.Stone)
                            {
                                var over = blocks[
                                    x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                    (y + 1) * (WorldGenerator.ChunkSize) + z];
                                if (over.id == (byte) BlockIDs.Air || over.visible == 1)
                                {
                                    switch (biomeMap[x, z])
                                    {
                                        case Biomes.Tundra:
                                            block = Blocks.blocks[BlockIDs.Snow];
                                            break;
                                        case Biomes.Forest:
                                            block = Blocks.blocks[BlockIDs.GrassTop];
                                            break;
                                        default:
                                            block = Blocks.blocks[BlockIDs.GrassTop];
                                            break;
                                    }

                                    blocks[
                                        x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                        y * (WorldGenerator.ChunkSize) + z] = block;
                                }
                            }
                        }
                    }
                }
            }

            if (showArrows)
            {

                for (var z = 1; z < ChunkSize - 1; z += 4)
                {
                    for (var x = 1; x < ChunkSize - 1; x++)
                    {
                        var y = PathHeight;

                        if (blocks[
    (x + 1) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
    y * (WorldGenerator.ChunkSize) + z].id == (byte)BlockIDs.MatrixFloor &&
    blocks[
    (x - 1) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
    y * (WorldGenerator.ChunkSize) + z].id == (byte)BlockIDs.Air)
                        {
                            blocks[
    x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
    (y + 1) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.ArrowRight];
                        }


                        if (blocks[
    (x - 1) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
    y * (WorldGenerator.ChunkSize) + z].id == (byte)BlockIDs.MatrixFloor &&
    blocks[
    (x + 1) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
    y * (WorldGenerator.ChunkSize) + z].id == (byte)BlockIDs.Air)
                        {
                            blocks[
    x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
    (y + 1) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.ArrowLeft];
                        }

                    }
                }

            }

            if (calcMesh)
            {
                World.AddChunk(xPos, zPos, blocks);
                for (var x = 0; x < 3; x++)
                {
                    for (var z = 0; z < 3; z++)
                    {
                        if (x == 1 && z == 1)
                        {
                            continue;
                        }

                        Calculate(xPos + x - 1, zPos + z - 1, false);
                    }
                }

                for (var x = 0; x < ChunkSize; x++)
                {
                    for (var z = 0; z < ChunkSize; z++)
                    {
                        var worldX = xPos * ChunkSize + x;
                        var worldZ = zPos * ChunkSize + z;

                        var red = color.GetNoise(worldX, worldZ) * 5;
                        var blue = color.GetNoise(worldZ, worldX) * 5;

                        for (var y = 0 /*heightmap[x, z]*/; y < ChunkHeight; y++)
                        {

                            var block = blocks[
                                x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                y * (WorldGenerator.ChunkSize) + z];
                            int newR = block.r + (int) ((Math.Sin(y * 1.5f) + 1) * 20 + red);
                            int newG = block.g + (int) ((Math.Sin(y * 1.5f) + 1) * 20);
                            int newB = block.b + (int) ((Math.Sin(y * 1.5f) + 1) * 20 + blue);

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

                            block.r = (byte) newR;
                            block.g = (byte) newG;
                            block.b = (byte) newB;

                            blocks[
                                x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                y * (WorldGenerator.ChunkSize) + z] = block;
                        }
                    }
                }

                for (var x = 0; x < ChunkSize; x++)
                {
                    for (var z = 0; z < ChunkSize; z++)
                    {
                        for (var y = 0 /*heightmap[x, z]*/; y < ChunkHeight; y++)
                        {


                            var absX = x;
                            var absZ = z;

                            if (blocks[
                                    x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                    y * (WorldGenerator.ChunkSize) + z].id == 0)
                            {
                                blocks[
                                    x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                    y * (WorldGenerator.ChunkSize) + z].mask = 0;
                                continue;
                            }

                            if (blocks[
                                    x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                    y * (WorldGenerator.ChunkSize) + z].visible == 1)
                            {
                                blocks[
                                    x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                    y * (WorldGenerator.ChunkSize) + z].mask = 255;
                                continue;
                            }

                            byte mask = 0b00000000;


                            if (x != ChunkSize - 1)
                            {
                                if (blocks[
                                        (x + 1) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                        y * (WorldGenerator.ChunkSize) + z].id == 0)
                                {
                                    mask = (byte) (mask | 0b10000000);
                                }
                                else if (blocks[
                                             (x + 1) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                             y * (WorldGenerator.ChunkSize) + z].visible == 1)
                                {
                                    mask = (byte) (mask | 0b10000000);
                                }
                            }

                            if (x != 0)
                            {
                                if (blocks[
                                        (x - 1) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                        y * (WorldGenerator.ChunkSize) + z].id == 0)
                                {
                                    mask = (byte) (mask | 0b01000000);
                                }
                                else if (blocks[
                                             (x - 1) * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                             y * (WorldGenerator.ChunkSize) + z].visible == 1)
                                {
                                    mask = (byte) (mask | 0b01000000);
                                }
                            }

                            if (y != ChunkSize - 1)
                            {
                                if (blocks[
                                        x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                        (y + 1) * (WorldGenerator.ChunkSize) + z].id == 0)
                                {
                                    mask = (byte) (mask | 0b00100000);
                                }
                                else if (blocks[
                                             x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                             (y + 1) * (WorldGenerator.ChunkSize) + z].visible == 1)
                                {
                                    mask = (byte) (mask | 0b00100000);
                                }
                            }

                            if (y != 0)
                            {
                                if (blocks[
                                        x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                        (y - 1) * (WorldGenerator.ChunkSize) + z].id == 0)
                                {
                                    mask = (byte) (mask | 0b00010000);
                                }
                                else if (blocks[
                                             x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                             (y - 1) * (WorldGenerator.ChunkSize) + z].visible == 1)
                                {
                                    mask = (byte) (mask | 0b00010000);
                                }
                            }

                            if (z != ChunkSize - 1)
                            {
                                if (blocks[
                                        x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                        y * (WorldGenerator.ChunkSize) + z + 1].id == 0)
                                {
                                    mask = (byte) (mask | 0b00001000);
                                }
                                else if (blocks[
                                             x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                             y * (WorldGenerator.ChunkSize) + z + 1].visible == 1)
                                {
                                    mask = (byte) (mask | 0b00001000);
                                }
                            }

                            if (z != 0)
                            {
                                if (blocks[
                                        x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                        y * (WorldGenerator.ChunkSize) + z - 1].id == 0)
                                {
                                    mask = (byte) (mask | 0b00000100);
                                }
                                else if (blocks[
                                             x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                             y * (WorldGenerator.ChunkSize) + z - 1].visible == 1)
                                {
                                    mask = (byte) (mask | 0b00000100);
                                }
                            }


                            blocks[
                                x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                y * (WorldGenerator.ChunkSize) + z].mask = mask;
                        }
                    }

                }

            }

            return blocks;
        }
    }

    public struct ChunkStruct
    {
        public Block[] block;
        public bool final;
    }

    public enum Biome
    {
        Grass,
        Sand
    }
}