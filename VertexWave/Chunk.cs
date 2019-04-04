using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using VertexWave.Generators;
using Voxeland.Generators.BlockTypes;

namespace VertexWave
{

    public class Chunk
    {

        static VertexPositionColorLine[] _vertices = new VertexPositionColorLine[WorldGenerator.MaxVertices];
        static Color[] _colors = new Color[WorldGenerator.MaxVertices];
        static int[] _indices = new int[WorldGenerator.MaxIndices];

        public static (VertexPositionColorLine[], int[]) CreateFromArray(Block[] blocks, int _x, int _y, int _z)
        {
            var position = new Vector3(_x * WorldGenerator.ChunkSize, _y, _z * WorldGenerator.ChunkSize);
            int _vertexNum = 0;
            int _indexNum = 0;
            int _colorNum = 0;
            for (var x = 0; x < WorldGenerator.ChunkSize; x++)
            {
                for (var y = 0; y < WorldGenerator.ChunkHeight; y++)
                {
                    for (var z = 0; z < WorldGenerator.ChunkSize; z++)
                    {
                        var block = blocks[
                            x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) +
                            y * (WorldGenerator.ChunkSize) + z];


                        if (block.id != (byte) BlockIDs.Air)
                        {
                            block.renderer.CreateBlock(_vertices, _colors, _indices, ref _vertexNum, ref _colorNum, ref _indexNum,
                                new Vector3(x, y, z) + position, 0,
                                new Color(block.r / 255.0f, block.g / 255.0f, block.b / 255.0f), block);
                        }


                    }
                }
            }
            var vertices = new VertexPositionColorLine[_vertexNum];
            var indices = new int[_indexNum];
            Array.Copy(_vertices, 0, vertices, 0, _vertexNum);
            Array.Copy(_indices, 0, indices, 0, _indexNum);

            return (vertices, indices);
        }
    }
}
