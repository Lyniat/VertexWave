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
        public const int PathSize = 8; //5;
        public const int PathHeight = 20;
        private const int ShadowSize = 2;
        private const int InterpolationSize = 16;
        private const bool CalculateShadow = false;
        private const bool ShadowModeFast = false;

        private const int SeaLevel = 60;

        private static Dictionary<(int, int), ChunkStruct> world = new Dictionary<(int, int), ChunkStruct>();
        private readonly FastNoise _biome;

        //private HeightGenerator _heightGenerator;
        private readonly BiomeGenerator _biomeGenerator;
        private readonly FastNoise _cave;

        private readonly FastNoise _caveNoise;
        private readonly FastNoise _caveNoiseHeight;

        private readonly FastNoise _cell;

        private readonly FastNoise _color;

        private readonly FastNoise _noise;

        private readonly FastNoise _river;

        private readonly int _seed;
        private readonly FastNoise _technicalBiome;
        private readonly FastNoise _technicalBiomeValue;

        private readonly FastNoise _white;

        private Biomes[,] _biomeMap;

        private bool _showArrows = true;

        private int _xPos;

        private int _zPos;
        //private RiverGenerator _riverGenerator;

        public WorldGenerator()
        {
            _seed = DateTime.Now.Millisecond - new DateTime(1970, 1, 1).Millisecond;

            //_heightGenerator = new HeightGenerator(seed++);
            _biomeGenerator = new BiomeGenerator(_seed++);
            //_riverGenerator = new RiverGenerator(seed++);

            _noise = new FastNoise();
            _noise.SetSeed(_seed++);
            _noise.SetNoiseType(FastNoise.NoiseType.CubicFractal);

            _cell = new FastNoise();
            _cell.SetSeed(_seed++);
            _cell.SetFrequency(0.05f);
            _cell.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            _cell.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Euclidean);

            _biome = new FastNoise();
            _biome.SetSeed(_seed++);
            _biome.SetNoiseType(FastNoise.NoiseType.Cellular);
            _biome.SetFrequency(0.005f);
            _biome.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Natural);
            _biome.SetCellularReturnType(FastNoise.CellularReturnType.Distance);
            _biome.SetCellularJitter(0.3f);
            _biome.SetGradientPerturbAmp(40f);

            _technicalBiome = new FastNoise();
            _technicalBiome.SetSeed(_seed++);
            _technicalBiome.SetNoiseType(FastNoise.NoiseType.Cellular);
            _technicalBiome.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Euclidean);

            _technicalBiomeValue = new FastNoise();
            _technicalBiomeValue.SetSeed(_seed++);
            _technicalBiomeValue.SetNoiseType(FastNoise.NoiseType.SimplexFractal);

            _cave = new FastNoise();
            _cave.SetSeed(_seed++);
            _cave.SetNoiseType(FastNoise.NoiseType.SimplexFractal);

            _caveNoise = new FastNoise();
            _caveNoise.SetSeed(_seed++);
            _caveNoise.SetNoiseType(FastNoise.NoiseType.Cellular);
            _caveNoise.SetFrequency(0.005f);
            _caveNoise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Natural);
            _caveNoise.SetCellularReturnType(FastNoise.CellularReturnType.Distance2Div);
            _caveNoise.SetCellularJitter(0.3f);
            _caveNoise.SetGradientPerturbAmp(40f);

            _caveNoiseHeight = new FastNoise();
            _caveNoiseHeight.SetSeed(_seed++);
            _caveNoiseHeight.SetNoiseType(FastNoise.NoiseType.PerlinFractal);

            _white = new FastNoise();
            _white.SetSeed(_seed++);
            _white.SetNoiseType(FastNoise.NoiseType.WhiteNoise);

            _river = new FastNoise();
            _river.SetSeed(_seed++);
            _river.SetNoiseType(FastNoise.NoiseType.Cellular);
            _river.SetFrequency(0.005f);
            _river.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Natural);
            _river.SetCellularReturnType(FastNoise.CellularReturnType.Distance2Div);
            _river.SetCellularJitter(0.3f);
            _river.SetGradientPerturbAmp(40);

            _color = new FastNoise();
            _color.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        }

        public ChunkStruct GetChunkAt(int xPos, int zPos)
        {
            var chunkStruct = new ChunkStruct();
            chunkStruct.block = Calculate(xPos, zPos, true);
            _xPos = xPos;
            _zPos = zPos;
            return chunkStruct;
        }

        public Block[] Calculate(int xPos, int zPos, bool calcMesh = false)
        {
            _biomeMap = _biomeGenerator.GetBiomemapAt(xPos, zPos);

            //int[,] heightmap = _heightGenerator.GetHeightmapAt(xPos, zPos);

            var blocks = World.GetChunkAt(xPos, zPos);

            if (blocks == null) blocks = new Block[ChunkSize * ChunkHeight * ChunkSize];

            var caveNoise = new FastNoise();
            caveNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            caveNoise.SetFrequency(0.02f);
            caveNoise.SetFractalOctaves(2);
            caveNoise.SetFractalType(FastNoise.FractalType.Billow);
            caveNoise.SetFractalLacunarity(1.5f);
            caveNoise.SetFractalGain(0.7f);

            //var riverNoiseMap = _riverGenerator.GetRivermapAt(xPos, zPos);

            blocks[0] = Blocks.blocks[BlockIDs.Dummy];

            for (var x = 0; x < ChunkSize; x++)
            for (var z = 0; z < ChunkSize; z++)
            {
                var worldX = xPos * ChunkSize + x;
                var worldZ = zPos * ChunkSize + z;


                for (var y = /*heightmap[x, z] - 1*/ 0; y <= PathHeight; y++)
                {
                    var extraSize = 140 + worldZ / 2;
                    if (extraSize < 0)
                    {
                        _showArrows = false;
                        extraSize = 0;
                    }
                    else
                    {
                        _showArrows = true;
                    }

                    var path = Math.Sin(worldZ / 15f) * 5f + Math.Sin(worldZ / 20f) * 3f + Math.Sin(worldZ / 50f) * 15f;

                    var newPathSize = PathSize - Math.Abs(Math.Sin(worldZ / 11f) * 3f) + extraSize;

                    var underPath = worldX > -newPathSize + path - (PathHeight - y) / 2f &&
                                    worldX < newPathSize + path + (PathHeight - y) / 2f;
                    if (underPath)
                    {
                        if (y == PathHeight)
                            blocks[
                                x * ChunkSize * ChunkHeight +
                                y * ChunkSize + z] = Blocks.blocks[BlockIDs.MatrixFloor];
                        else
                            continue;
                    }

                    var noise3Dx = worldX;
                    var noise3Dz = worldZ;

                    if (x == 0) noise3Dx--;

                    if (z == 0) noise3Dz--;

                    if (caveNoise.GetNoise(noise3Dx, y, noise3Dz) < 0) continue;

                    if (y < 0) continue;

                    if (blocks[
                            x * ChunkSize * ChunkHeight +
                            y * ChunkSize + z].id != 0)
                        continue;

                    var absX = ChunkSize + x;
                    var absZ = ChunkSize + z;

                    var block = new Block();

                    if (y <= PathHeight) block = Blocks.blocks[BlockIDs.Dirt];

                    if (y == 0) block = Blocks.blocks[BlockIDs.Dirt];

                    var noiseAtX = worldX;
                    var noiseAtZ = worldZ;

                    if (x == 0) noiseAtX--;

                    if (z == 0) noiseAtZ--;

                    blocks[
                        x * ChunkSize * ChunkHeight +
                        y * ChunkSize + z] = block;
                }
            }

            for (var x = 0; x < ChunkSize; x++)
            for (var z = 0; z < ChunkSize; z++)
            {
                var worldX = xPos * ChunkSize + x;
                var worldZ = zPos * ChunkSize + z;
                _cave.SetNoiseType(FastNoise.NoiseType.WhiteNoise);
                {
                    if (_cave.GetNoise(-worldX, -worldZ) < 0.8) continue;
                }
                for (var y = ChunkHeight - 1; y > 0; y--)
                {
                    var id = blocks[
                        x * ChunkSize * ChunkHeight +
                        y * ChunkSize + z].id;
                    if (id == (byte) BlockIDs.Water) break;

                    if (id == (byte) BlockIDs.Dirt || id == (byte) BlockIDs.GrassTop ||
                        id == (byte) BlockIDs.Snow || id == (byte) BlockIDs.Sand || id == (byte) BlockIDs.Stone)
                    {
                        var n = (_white.GetNoise(-worldX, -worldZ) + 1) / 2f;

                        switch (_biomeMap[x, z])
                        {
                            case Biomes.Plains:
                                if (n > 0.997)
                                    new Tree(x, y, z, blocks);
                                else if (n > 0.2)
                                    blocks[
                                            x * ChunkSize * ChunkHeight +
                                            (y + 1) * ChunkSize + z] =
                                        Blocks.blocks[BlockIDs.FlowerPoppy];
                                else if (n > -0.4)
                                    blocks[
                                        x * ChunkSize * ChunkHeight +
                                        (y + 1) * ChunkSize + z] = Blocks.blocks[BlockIDs.FlowerTulip];

                                break;

                            case Biomes.Tundra:
                                if (n > 0.97)
                                    new Spruce(x, y, z, blocks);

                                else if (n > 0.4)
                                    blocks[
                                        x * ChunkSize * ChunkHeight +
                                        (y + 1) * ChunkSize + z] = Blocks.blocks[BlockIDs.Plant];

                                break;

                            case Biomes.Jungle:

                                if (n > 0.95) new JungleTreeSmall(x, y, z, blocks);

                                if (n > 0.96)
                                    new JungleTree(x, y, z, blocks);
                                else if (n > 0.6)
                                    blocks[
                                        x * ChunkSize * ChunkHeight +
                                        (y + 1) * ChunkSize + z] = Blocks.blocks[BlockIDs.FlowerTulip];
                                else if (n > 0.3)
                                    blocks[
                                        x * ChunkSize * ChunkHeight +
                                        (y + 1) * ChunkSize + z] = Blocks.blocks[BlockIDs.FlowerPoppy];


                                break;

                            case Biomes.Forest:
                                if (n > 0.9)
                                {
                                    var nextNoise = (_white.GetNoise(worldZ, worldX) + 1) / 2;
                                    byte type = 0;
                                    if (nextNoise > 0.3) type = 3;

                                    if (nextNoise > 0.95) type = 1;

                                    if (nextNoise > 0.975) type = 2;

                                    new BigTree(x, y, z, type, blocks);
                                }
                                else if (n > 0.80)
                                {
                                    blocks[
                                        x * ChunkSize * ChunkHeight +
                                        (y + 1) * ChunkSize + z] = Blocks.blocks[BlockIDs.Plant];
                                }

                                break;
                        }

                        break;
                    }
                }
            }

            for (var x = 0; x < ChunkSize; x++)
            for (var z = 0; z < ChunkSize; z++)
            for (var y = PathHeight; y < ChunkHeight; y++)
                if (y < ChunkHeight - 1)
                {
                    var block = blocks[
                        x * ChunkSize * ChunkHeight +
                        y * ChunkSize + z];
                    if (block.id == (byte) BlockIDs.Dirt || block.id == (byte) BlockIDs.Stone)
                    {
                        var over = blocks[
                            x * ChunkSize * ChunkHeight +
                            (y + 1) * ChunkSize + z];
                        if (over.id == (byte) BlockIDs.Air || over.visible == 1)
                        {
                            switch (_biomeMap[x, z])
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
                                x * ChunkSize * ChunkHeight +
                                y * ChunkSize + z] = block;
                        }
                    }
                }

            if (_showArrows)
                for (var z = 1; z < ChunkSize - 1; z += 4)
                for (var x = 1; x < ChunkSize - 1; x++)
                {
                    var y = PathHeight;

                    if (blocks[
                            (x + 1) * ChunkSize * ChunkHeight +
                            y * ChunkSize + z].id == (byte) BlockIDs.MatrixFloor &&
                        blocks[
                            (x - 1) * ChunkSize * ChunkHeight +
                            y * ChunkSize + z].id == (byte) BlockIDs.Air)
                        blocks[
                            x * ChunkSize * ChunkHeight +
                            (y + 1) * ChunkSize + z] = Blocks.blocks[BlockIDs.ArrowRight];


                    if (blocks[
                            (x - 1) * ChunkSize * ChunkHeight +
                            y * ChunkSize + z].id == (byte) BlockIDs.MatrixFloor &&
                        blocks[
                            (x + 1) * ChunkSize * ChunkHeight +
                            y * ChunkSize + z].id == (byte) BlockIDs.Air)
                        blocks[
                            x * ChunkSize * ChunkHeight +
                            (y + 1) * ChunkSize + z] = Blocks.blocks[BlockIDs.ArrowLeft];
                }

            if (calcMesh)
            {
                World.AddChunk(xPos, zPos, blocks);
                for (var x = 0; x < 3; x++)
                for (var z = 0; z < 3; z++)
                {
                    if (x == 1 && z == 1) continue;

                    Calculate(xPos + x - 1, zPos + z - 1, false);
                }

                for (var x = 0; x < ChunkSize; x++)
                for (var z = 0; z < ChunkSize; z++)
                {
                    var worldX = xPos * ChunkSize + x;
                    var worldZ = zPos * ChunkSize + z;

                    var red = _color.GetNoise(worldX, worldZ) * 5;
                    var blue = _color.GetNoise(worldZ, worldX) * 5;

                    for (var y = 0 /*heightmap[x, z]*/; y < ChunkHeight; y++)
                    {
                        var block = blocks[
                            x * ChunkSize * ChunkHeight +
                            y * ChunkSize + z];
                        var newR = block.r + (int) ((Math.Sin(y * 1.5f) + 1) * 20 + red);
                        var newG = block.g + (int) ((Math.Sin(y * 1.5f) + 1) * 20);
                        var newB = block.b + (int) ((Math.Sin(y * 1.5f) + 1) * 20 + blue);

                        if (newR > 255) newR = 255;

                        if (newG > 255) newG = 255;

                        if (newB > 255) newB = 255;

                        block.r = (byte) newR;
                        block.g = (byte) newG;
                        block.b = (byte) newB;

                        blocks[
                            x * ChunkSize * ChunkHeight +
                            y * ChunkSize + z] = block;
                    }
                }

                for (var x = 0; x < ChunkSize; x++)
                for (var z = 0; z < ChunkSize; z++)
                for (var y = 0 /*heightmap[x, z]*/; y < ChunkHeight; y++)
                {
                    var absX = x;
                    var absZ = z;

                    if (blocks[
                            x * ChunkSize * ChunkHeight +
                            y * ChunkSize + z].id == 0)
                    {
                        blocks[
                            x * ChunkSize * ChunkHeight +
                            y * ChunkSize + z].mask = 0;
                        continue;
                    }

                    if (blocks[
                            x * ChunkSize * ChunkHeight +
                            y * ChunkSize + z].visible == 1)
                    {
                        blocks[
                            x * ChunkSize * ChunkHeight +
                            y * ChunkSize + z].mask = 255;
                        continue;
                    }

                    byte mask = 0b00000000;


                    if (x != ChunkSize - 1)
                    {
                        if (blocks[
                                (x + 1) * ChunkSize * ChunkHeight +
                                y * ChunkSize + z].id == 0)
                            mask = (byte) (mask | 0b10000000);
                        else if (blocks[
                                     (x + 1) * ChunkSize * ChunkHeight +
                                     y * ChunkSize + z].visible == 1)
                            mask = (byte) (mask | 0b10000000);
                    }

                    if (x != 0)
                    {
                        if (blocks[
                                (x - 1) * ChunkSize * ChunkHeight +
                                y * ChunkSize + z].id == 0)
                            mask = (byte) (mask | 0b01000000);
                        else if (blocks[
                                     (x - 1) * ChunkSize * ChunkHeight +
                                     y * ChunkSize + z].visible == 1)
                            mask = (byte) (mask | 0b01000000);
                    }

                    if (y != ChunkSize - 1)
                    {
                        if (blocks[
                                x * ChunkSize * ChunkHeight +
                                (y + 1) * ChunkSize + z].id == 0)
                            mask = (byte) (mask | 0b00100000);
                        else if (blocks[
                                     x * ChunkSize * ChunkHeight +
                                     (y + 1) * ChunkSize + z].visible == 1)
                            mask = (byte) (mask | 0b00100000);
                    }

                    if (y != 0)
                    {
                        if (blocks[
                                x * ChunkSize * ChunkHeight +
                                (y - 1) * ChunkSize + z].id == 0)
                            mask = (byte) (mask | 0b00010000);
                        else if (blocks[
                                     x * ChunkSize * ChunkHeight +
                                     (y - 1) * ChunkSize + z].visible == 1)
                            mask = (byte) (mask | 0b00010000);
                    }

                    if (z != ChunkSize - 1)
                    {
                        if (blocks[
                                x * ChunkSize * ChunkHeight +
                                y * ChunkSize + z + 1].id == 0)
                            mask = (byte) (mask | 0b00001000);
                        else if (blocks[
                                     x * ChunkSize * ChunkHeight +
                                     y * ChunkSize + z + 1].visible == 1)
                            mask = (byte) (mask | 0b00001000);
                    }

                    if (z != 0)
                    {
                        if (blocks[
                                x * ChunkSize * ChunkHeight +
                                y * ChunkSize + z - 1].id == 0)
                            mask = (byte) (mask | 0b00000100);
                        else if (blocks[
                                     x * ChunkSize * ChunkHeight +
                                     y * ChunkSize + z - 1].visible == 1)
                            mask = (byte) (mask | 0b00000100);
                    }


                    blocks[
                        x * ChunkSize * ChunkHeight +
                        y * ChunkSize + z].mask = mask;
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