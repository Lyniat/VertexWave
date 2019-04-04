using System.Collections.Generic;
using Microsoft.Xna.Framework;
using VertexWave.Generators;
using Voxeland.Generators.BlockTypes;

namespace VertexWave
{
    public class Chunk
    {
        private const bool Optimize = false;
        private readonly List<Color> _colors = new List<Color>();
        private readonly List<int> _index = new List<int>();

        private readonly Vector3 _position;
        // Member variables here, example:
        // private int a = 2;
        // private string b = "textvar";

        private readonly List<VertexPositionColorLine> _vertices = new List<VertexPositionColorLine>();

        //private static SpatialMaterial material;

        private int _blockNum;

        private int _x;
        private int _y;
        private int _z;

        public Chunk(int x, int y, int z, Block[] blocks)
        {
            _x = x;
            _y = y;
            _z = z;
            _position = new Vector3(x, y, z);
            CreateFromArray(blocks);
        }

        private void CreateFromArray(Block[] blocks)
        {
            for (var x = 0; x < WorldGenerator.ChunkSize; x++)
            for (var y = 0; y < WorldGenerator.ChunkHeight; y++)
            for (var z = 0; z < WorldGenerator.ChunkSize; z++)
            {
                var block = blocks[
                    x * WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight +
                    y * WorldGenerator.ChunkSize + z];


                if (block.id != (byte) BlockIDs.Air)
                    block.renderer.CreateBlock(_vertices, _colors, _index, ref _blockNum,
                        new Vector3(x, y, z) + _position, 0,
                        new Color(block.r / 255.0f, block.g / 255.0f, block.b / 255.0f), block);
            }
        }


        public (VertexPositionColorLine[], int[]) GetData()
        {
            var vert = _vertices.ToArray();
            var ind = _index.ToArray();

            return (vert, ind);
        }
    }
}