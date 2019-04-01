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
        private Block[,,] tree;
        private Block[,,] savannahTree;

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

        private bool[,] modifiedNeighbours;

        private int _xPos;
        private int _zPos;

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
                    /*
                    if (x + z == 0)
                    {
                        if (blocks[
                                1 * 3 * ChunkSize * ChunkHeight * ChunkSize +
                                1 * ChunkSize * ChunkHeight * ChunkSize + x * ChunkHeight * ChunkSize +
                                0 * ChunkSize + z].id != 0)
                        {
                            continue;
                        }
                    }
                    */
                    var worldX = xPos * ChunkSize + x;
                    var worldZ = zPos * ChunkSize + z;


                    for (var y = /*heightmap[x, z] - 1*/ 0; y <= PathHeight; y++)
                    {
                        var extraSize = 140 + worldZ/2;
                        if(extraSize < 0)
                        {
                            extraSize = 0;
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
                        /*
                        if (y <= height - 8)
                        {
                            block = Blocks.blocks[BlockIDs.Stone];
                        }
                        */

                        if (y == 0)
                        {
                            block = Blocks.blocks[BlockIDs.Dirt];
                        }


                        /*
                        if (y > height && y < ChunkHeight-1)
                        {
                            if(GetBlockAt(xPos,y,zPos,x,z) == 1){
                                block.id = 1;
                                block.r = 0;
                                block.g = 255;
                                block.b = 0;
                            }
                        }*/

                        /*
                        if (biomeType == Biome.Grass)
                        {
                            if (y <= height)
                            {
                                block.id = 1;
                                block.r = 0;
                                block.g = 255;
                                block.b = 0;
                            }
                        }
                        else
                        {
                            if (y <= height)
                            {
                                block.id = 3;
                                block.r = 255;
                                block.g = 255;
                                block.b = 100;
                                block.texture = 18;
                            }
                        }
                        if (y < height - 2)
                        {
                            block.id = 2;
                            block.r = 127;
                            block.g = 127;
                            block.b = 127;
                            block.texture = 17;
                        }
                        if (y > height && y <= heigthExtra)
                        {
                            block.id = 2;
                            block.r = 127;
                            block.g = 127;
                            block.b = 127;
                            block.texture = 17;
                            if (caveNoise.GetSimplexFractal(worldX, y, worldZ) > 0)
                            {
                                block.id = 0;
                            }
                            else if (y >= heigthExtra - 3)
                            {
                                block.id = 1;
                                block.r = 0;
                                block.g = 255;
                                block.b = 0;
                                block.texture = 0;
                            }
                        }
                        
                        */

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

                        /*
                        var riverNoise = riverNoiseMap[x, z];
                        if (y == height + 1 && height > 8)
                        {
                            Block water = Blocks.blocks[BlockIDs.Water];
                            Block air = Blocks.blocks[BlockIDs.Air];
                            Block sand = Blocks.blocks[BlockIDs.Sand];
                            sand.visible = 1;
                            if (riverNoise > 0.93)
                            {
                                blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + y * (WorldGenerator.ChunkSize) + z] = air;
                                blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + (y - 1) * (WorldGenerator.ChunkSize) + z] = air;
                                blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + (y - 2) * (WorldGenerator.ChunkSize) + z] = air;
                                blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + (y - 3) * (WorldGenerator.ChunkSize) + z] = water;
                                blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + (y - 4) * (WorldGenerator.ChunkSize) + z] = water;
                                blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + (y - 5) * (WorldGenerator.ChunkSize) + z] = water;
                                blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + (y - 6) * (WorldGenerator.ChunkSize) + z] = sand;
                            }else if (riverNoise > 0.89)
                            {
                                blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + y * (WorldGenerator.ChunkSize) + z] = air;
                                blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + (y - 1) * (WorldGenerator.ChunkSize) + z] = air;
                                blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + (y - 2) * (WorldGenerator.ChunkSize) + z] = sand;
                                blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + (y - 3) * (WorldGenerator.ChunkSize) + z] = sand;
                                blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + (y - 4) * (WorldGenerator.ChunkSize) + z] = sand;
                                blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + (y - 5) * (WorldGenerator.ChunkSize) + z] = sand;
                                blocks[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + (y - 6) * (WorldGenerator.ChunkSize) + z] = sand;
                            }
                        }

                        */
                        
                        /*
                        
                        if (y > height && y < ChunkHeight - 1)
                        {
                            if (riverNoise < 0.91f && y < height + 7)
                            {
                                BlockIDs id = GetBlockAt(xPos, y, zPos, x, z);
                                block = Blocks.blocks[id];
                            }
                        }
                        
                        */

                        blocks[
                            x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                            y * (WorldGenerator.ChunkSize) + z] = block;


                    }
                }
            }

            for (var x = 0; x < ChunkSize; x++)
            {
                break;
                for (var z = 0; z < ChunkSize; z++)
                {
                    var worldX = xPos * ChunkSize + x;
                    var worldZ = zPos * ChunkSize + z;

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

                    var oldSeed = river.GetSeed();

                    for (var i = 0; i < 8; i++)
                    {

                        river.SetSeed(oldSeed + i);
                        var caveN = river.GetNoise(noiseAtZ, noiseAtX);
                        if (caveN > 0.95)
                        {
                            int caveHeight =
                                (int) (Math.Abs(caveNoiseHeight.GetNoise(noiseAtX, i, noiseAtZ) * ChunkHeight));

                            Console.WriteLine(caveHeight);

                            var size = 1;

                            for (var xx = -size; xx <= size; xx++)
                            {
                                for (var zz = -size; zz <= size; zz++)
                                {
                                    for (var yy = -2; yy <= 2; yy++)
                                    {
                                        var newX = xx + x;
                                        var newZ = zz + z;
                                        var newY = yy + caveHeight;

                                        if (newX > ChunkSize - 2 || newX < 1)
                                        {
                                            continue;
                                        }

                                        if (newZ > ChunkSize - 2 || newZ < 1)
                                        {
                                            continue;
                                        }

                                        if (newY > ChunkHeight - 2 || newY < 1)
                                        {
                                            continue;
                                        }

                                        blocks[
                                            newX * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                            newY * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.Air];
                                    }
                                }
                            }
                        }
                    }

                    river.SetSeed(oldSeed);
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
                            /*
                            if (riverNoiseMap[x,z] > 0.88) //extra case if river
                            {
                                if (n > 0.4 )
                                {
                                    blocks[
                                        x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                        (y + 1) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.ReedBottom];
                                    blocks[
                                        x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                        (y + 2) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.ReedTop];
                                    break;
                                }
                                
                            }
                            */
     
                            switch (biomeMap[x, z])
                            {
                                case Biomes.Plains:
                                    if (n > 0.997)
                                    {
                                        new Tree(x, y, z,blocks);
                                    }
                                    else if (n > 0.2)
                                    {
                                        /*
                                        blocks[
                                                x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                                (y + 1) * (WorldGenerator.ChunkSize) + z] =
                                            Blocks.blocks[BlockIDs.FlowerPoppy];
                                            */
                                    }
                                    else if (n > -0.4)
                                    {
                                        /*
                                        blocks[
                                            x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                                            (y + 1) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.FlowerTulip];
                                            */
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
                                            (y + 1) * (WorldGenerator.ChunkSize) + z] = Blocks.blocks[BlockIDs.PlantSnow];
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
                                    else
                                    {
                                        if(cell.GetNoise(x,z)  > 0.1)
                                        new Bamboo(x, y, z, blocks);
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


            /*
            for (var x = 0; x < 3; x++)
            {
                for (var z = 0; z < 3; z++)
                {
                    if (x == 1 && z == 1)
                    {
                        continue;
                    }

                    if (!modifiedNeighbours[x, z])
                    {
                        continue;
                    }
                    //MeshNotifier.OnModified(xPos + x -1, zPos + z -1);
                }
            }
            */

            //CalculateLightLevel(blocks);

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


        /*

        private BlockIDs GetBlockAt(int xPos, int y, int zPos, int x, int z)
        {
            if (x == 0)
            {
                x -= 1;
            }
            if (z == 0)
            {
                z -= 1;
            }
            var worldX = xPos * ChunkSize + x;
            var worldZ = zPos * ChunkSize + z;
            BlockIDs id = BlockIDs.Air;

            var multiplier = 10;
            var digCaves = false;
            
            if (technicalBiome.GetValue(worldX, worldZ) < 0.3)
            {
                multiplier = 20;
                digCaves = false;
            }

            if (technicalBiome.GetValue(worldX, worldZ) < 0.6)
            {
                multiplier = 40;
                digCaves = true;
            }
            if (technicalBiome.GetValue(worldX, worldZ) < 0.9)
            {
                multiplier = 80;
                digCaves = true;
            }

            var heightVal = cave.GetSimplexFractal(worldX, worldZ) * multiplier + GetHeigthAt(worldX, worldZ)*2;
            if (y < heightVal)
            {
                if (!digCaves)
                {
                    if (y < heightVal - 6)
                    {
                        id = BlockIDs.Stone;
                    }
                    else
                    {
                        id = BlockIDs.Dirt;
                    }
                    
                }
                else if (cave.GetSimplexFractal(worldX, y, worldZ) > 0)
                {
                    if (y < heightVal - 6)
                    {
                        id = BlockIDs.Stone; 
                    }
                    else
                    {
                        id = BlockIDs.Dirt;
                    }
                }
                /*
                if (cave.GetSimplexFractal(worldX, y, worldZ) > 0.5)
                {
                    id = BlockIDs.Stone;
                }*/
        /*
    }
    return id;
}
*/
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