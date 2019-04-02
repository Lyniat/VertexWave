using System.Collections.Generic;
using Microsoft.Xna.Framework;
using VertexWave;

namespace Voxeland.Generators.BlockTypes
{

    public class BlockArrowLeft : DefaultBlock
    {

        public override void CreateBlock(List<VertexPositionColorLine> vertices, List<Color> colors, List<int> index, ref int blockNum, Vector3 pos, int nextZ, Color color, Block block)
        {

            //line
            vertices.Add(new VertexPositionColorLine(new Vector3(0.9f, 0.33f, 0.5f) + pos, color));
            vertices.Add(new VertexPositionColorLine(new Vector3(0.9f, 0.66f, 0.5f) + pos, color));

            vertices.Add(new VertexPositionColorLine(new Vector3(0.3f, 0.33f, 0.5f) + pos, color));
            vertices.Add(new VertexPositionColorLine(new Vector3(0.3f, 0.66f, 0.5f) + pos, color));

            vertices.Add(new VertexPositionColorLine(new Vector3(0.3f, 0.1f, 0.5f) + pos, color));
            vertices.Add(new VertexPositionColorLine(new Vector3(0.3f, 0.9f, 0.5f) + pos, color));

            vertices.Add(new VertexPositionColorLine(new Vector3(0.1f, 0.5f, 0.5f) + pos, color));

            index.Add(blockNum + 0);
            index.Add(blockNum + 1);

            index.Add(blockNum + 1);
            index.Add(blockNum + 3);

            index.Add(blockNum + 3);
            index.Add(blockNum + 5);

            index.Add(blockNum + 5);
            index.Add(blockNum + 6);

            index.Add(blockNum + 0);
            index.Add(blockNum + 2);

            index.Add(blockNum + 2);
            index.Add(blockNum + 4);

            index.Add(blockNum + 4);
            index.Add(blockNum + 6);

            blockNum += 7;

            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);

        }

    }
}