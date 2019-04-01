using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using VertexWave.Generators;
using Voxeland.Generators.BlockTypes;

namespace VertexWave
{

    public class Chunk
    {

        const bool Optimize = false;
        // Member variables here, example:
        // private int a = 2;
        // private string b = "textvar";

        List<VertexPositionColorLine> vertices = new List<VertexPositionColorLine>();
        List<Color> colors = new List<Color>();
        List<int> index = new List<int>();

        //private static SpatialMaterial material;

        int blockNum = 0;

        private Vector3 position;

        private int _x;
        private int _y;
        private int _z;

        public Chunk(int x, int y, int z, Block[] blocks)
        {
            _x = x;
            _y = y;
            _z = z;
            position = new Vector3(x, y, z);
            CreateFromArray(blocks);
        }

        void CreateFromArray(Block[] blocks)
        {
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
                            block.renderer.CreateBlock(vertices, colors, index, ref blockNum,
                                new Vector3(x, y, z) + position, 0,
                                new Color(block.r / 255.0f, block.g / 255.0f, block.b / 255.0f), block);
                        }


                    }
                }
            }
        }


        public (VertexPositionColorLine[], int[]) GetData()
        {
            VertexPositionColorLine[] vert = vertices.ToArray();
            int[] ind = index.ToArray();

            return (vert, ind);
        }

    }
}
