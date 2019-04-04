using System.Collections.Generic;
using Microsoft.Xna.Framework;
using VertexWave;

namespace Voxeland.Generators.BlockTypes
{
    public class BlockMatrixFloor : DefaultBlock
    {
        public override void CreateBlock(VertexPositionColorLine[] vertices, Color[] colors, int[] indices, ref int vertexNum, ref int colorNum, ref int indexNum, Vector3 pos, int nextZ, Color color, Block block)
        {
            block.light = 255;

            //y+ (top)
            vertices[vertexNum + 0] = (new VertexPositionColorLine(new Vector3(0, 1, 0) + pos, color));
            vertices[vertexNum + 1] = (new VertexPositionColorLine(new Vector3(0, 1, 1 + nextZ) + pos, color));
            vertices[vertexNum + 2] = (new VertexPositionColorLine(new Vector3(1, 1, 1 + nextZ) + pos, color));
            vertices[vertexNum + 3] = (new VertexPositionColorLine(new Vector3(1, 1, 0) + pos, color));

            indices[indexNum++] = (vertexNum + 0);
            indices[indexNum++] = (vertexNum + 1);
            indices[indexNum++] = (vertexNum + 1);
            indices[indexNum++] = (vertexNum + 2);
            indices[indexNum++] = (vertexNum + 2);
            indices[indexNum++] = (vertexNum + 3);
            indices[indexNum++] = (vertexNum + 3);
            indices[indexNum++] = (vertexNum + 0);

            vertexNum += 4;

            colors[colorNum++] = (color);
            colors[colorNum++] = (color);
            colors[colorNum++] = (color);
            colors[colorNum++] = (color);
        }
    }
}