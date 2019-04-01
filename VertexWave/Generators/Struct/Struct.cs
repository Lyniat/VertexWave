using VertexWave;
using VertexWave.Generators;
using Voxeland.Generators.BlockTypes;

namespace Voxeland.Generators.Struct
{
    public abstract class Struct
    {
        private Block[] _world;

        protected Struct(Block[] world)
        {
            _world = world;
        }
        
        protected void AddBlock(int x, int y, int z, BlockIDs id)
        {
            var xChunk = 1;
            var zChunk = 1;

            if (x >= WorldGenerator.ChunkSize)
            {
                return;
            }else if (x < 0)
            {
                return;
            }
        
            if (z >= WorldGenerator.ChunkSize)
            {
                return;
            }else if (z < 0)
            {
                return;
            }
        
            _world[
                    x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                    y * (WorldGenerator.ChunkSize) + z] =
                Blocks.blocks[id];
        }
    
        protected BlockIDs GetBlock(int x, int y, int z)
        {
            if (x >= WorldGenerator.ChunkSize)
            {
                return 0;
            }else if (x < 0)
            {
                return 0;
            }
        
            if (z >= WorldGenerator.ChunkSize)
            {
                return 0;
            }
            else if (z < 0)
            {
                return 0;
            }
        
            byte block = _world[
                x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                y * (WorldGenerator.ChunkSize) + z].id;

            return (BlockIDs) block;
        }
    }
}