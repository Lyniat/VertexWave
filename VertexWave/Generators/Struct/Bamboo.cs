using System;
using VertexWave.Generators;
using Voxeland.Generators.BlockTypes;

namespace Voxeland.Generators.Struct
{
    public class Bamboo : Struct
    {
        public Bamboo(int x, int y, int z, Block[] blocks) : base(blocks)
        {
            var height = new Random(x+y+z).Next(2, 7);
            for (var yy = 0; yy < height; yy++)
            {
                AddBlock(x,y+yy + 1,z,BlockIDs.Bamboo);
            }
        }
    }
}