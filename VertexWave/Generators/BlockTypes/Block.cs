using System.Collections.Generic;
using Microsoft.Xna.Framework;
using VertexWave;

namespace Voxeland.Generators.BlockTypes
{
    public struct Block
    {

        public byte id;
        public byte variant;
        public byte r;
        public byte g;
        public byte b;
        public byte mask;
        public byte textureTop;
        public byte textureSide;
        public byte textureBottom;
        public byte light;
        public byte visible;
        public byte passable;
        public byte calculated;
        public byte transperency;
        public DefaultBlock renderer;
    }
    
    public class DefaultBlock{

    public virtual void CreateBlock(VertexPositionColorLine[] vertices, Color[] colors, int[] indices, ref int vertexNum, ref int colorNum, ref int indexNum, Vector3 pos, int nextZ, Color color, Block block)
    {
        block.light = 255;
            //x-
            if ((block.mask & 0b01000000) > 0)
            {
                vertices[vertexNum + 0] = (new VertexPositionColorLine(new Vector3(0, 0, 0) + pos, color));
                vertices[vertexNum + 1] = (new VertexPositionColorLine(new Vector3(0, 1, 0) + pos, color));
                vertices[vertexNum + 2] = (new VertexPositionColorLine(new Vector3(0, 1, 1 + nextZ) + pos, color));
                vertices[vertexNum + 3] = (new VertexPositionColorLine(new Vector3(0, 0, 1 + nextZ) + pos, color));

                indices[indexNum++] = (vertexNum + 0);
                indices[indexNum++] = (vertexNum + 1);
                indices[indexNum++] = (vertexNum + 1);
                indices[indexNum++] = (vertexNum + 2);
                indices[indexNum++] = (vertexNum + 2);
                indices[indexNum++] = (vertexNum + 3);
                indices[indexNum++] = (vertexNum + 3);
                indices[indexNum++] = (vertexNum + 0);

                vertexNum += 4;

                colors[colorNum++] = color;
                colors[colorNum++] = color;
                colors[colorNum++] = color;
                colors[colorNum++] = color;
            }

            //x+
            if ((block.mask & 0b10000000) > 0)
            {
                vertices[vertexNum + 0] = (new VertexPositionColorLine(new Vector3(1, 0, 0) + pos, color));
                vertices[vertexNum + 1] = (new VertexPositionColorLine(new Vector3(1, 1, 0) + pos, color));
                vertices[vertexNum + 2] = (new VertexPositionColorLine(new Vector3(1, 1, 1 + nextZ) + pos, color));
                vertices[vertexNum + 3] = (new VertexPositionColorLine(new Vector3(1, 0, 1 + nextZ) + pos, color));

                indices[indexNum++] = (vertexNum + 0);
                indices[indexNum++] = (vertexNum + 1);
                indices[indexNum++] = (vertexNum + 1);
                indices[indexNum++] = (vertexNum + 2);
                indices[indexNum++] = (vertexNum + 2);
                indices[indexNum++] = (vertexNum + 3);
                indices[indexNum++] = (vertexNum + 3);
                indices[indexNum++] = (vertexNum + 0);

                vertexNum += 4;

                colors[colorNum++] = color;
                colors[colorNum++] = color;
                colors[colorNum++] = color;
                colors[colorNum++] = color;
            }

            //z+
            if ((block.mask & 0b00001000) > 0)
            {
                vertices[vertexNum + 0] = (new VertexPositionColorLine(new Vector3(0, 0, 1 + nextZ) + pos, color));
                vertices[vertexNum + 1] = (new VertexPositionColorLine(new Vector3(1, 0, 1 + nextZ) + pos, color));
                vertices[vertexNum + 2] = (new VertexPositionColorLine(new Vector3(1, 1, 1 + nextZ) + pos, color));
                vertices[vertexNum + 3] = (new VertexPositionColorLine(new Vector3(0, 1, 1 + nextZ) + pos, color));

                indices[indexNum++] = (vertexNum + 0);
                indices[indexNum++] = (vertexNum + 1);
                indices[indexNum++] = (vertexNum + 1);
                indices[indexNum++] = (vertexNum + 2);
                indices[indexNum++] = (vertexNum + 2);
                indices[indexNum++] = (vertexNum + 3);
                indices[indexNum++] = (vertexNum + 3);
                indices[indexNum++] = (vertexNum + 0);

                vertexNum += 4;

                colors[colorNum++] = color;
                colors[colorNum++] = color;
                colors[colorNum++] = color;
                colors[colorNum++] = color;
            }

            //z-
            if ((block.mask & 0b00000100) > 0)
            {
                vertices[vertexNum + 0] = (new VertexPositionColorLine(new Vector3(0, 0, 0) + pos, color));
                vertices[vertexNum + 1] = (new VertexPositionColorLine(new Vector3(1, 0, 0) + pos, color));
                vertices[vertexNum + 2] = (new VertexPositionColorLine(new Vector3(1, 1, 0) + pos, color));
                vertices[vertexNum + 3] = (new VertexPositionColorLine(new Vector3(0, 1, 0) + pos, color));

                indices[indexNum++] = (vertexNum + 0);
                indices[indexNum++] = (vertexNum + 1);
                indices[indexNum++] = (vertexNum + 1);
                indices[indexNum++] = (vertexNum + 2);
                indices[indexNum++] = (vertexNum + 2);
                indices[indexNum++] = (vertexNum + 3);
                indices[indexNum++] = (vertexNum + 3);
                indices[indexNum++] = (vertexNum + 0);

                vertexNum += 4;

                colors[colorNum++] = color;
                colors[colorNum++] = color;
                colors[colorNum++] = color;
                colors[colorNum++] = color;
            }

            //y- (bottom)
            if ((block.mask & 0b00010000) > 0)
            {
                vertices[vertexNum + 0] = (new VertexPositionColorLine(new Vector3(0, 0, 0) + pos, color));
                vertices[vertexNum + 1] = (new VertexPositionColorLine(new Vector3(0, 0, 1 + nextZ) + pos, color));
                vertices[vertexNum + 2] = (new VertexPositionColorLine(new Vector3(1, 0, 1 + nextZ) + pos, color));
                vertices[vertexNum + 3] = (new VertexPositionColorLine(new Vector3(1, 0, 0) + pos, color));

                indices[indexNum++] = (vertexNum + 0);
                indices[indexNum++] = (vertexNum + 1);
                indices[indexNum++] = (vertexNum + 1);
                indices[indexNum++] = (vertexNum + 2);
                indices[indexNum++] = (vertexNum + 2);
                indices[indexNum++] = (vertexNum + 3);
                indices[indexNum++] = (vertexNum + 3);
                indices[indexNum++] = (vertexNum + 0);

                vertexNum += 4;

                colors[colorNum++] = color;
                colors[colorNum++] = color;
                colors[colorNum++] = color;
                colors[colorNum++] = color;
            }

            //y+ (top)
            if ((block.mask & 0b00100000) > 0)
            {
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

                colors[colorNum++] = color;
                colors[colorNum++] = color;
                colors[colorNum++] = color;
                colors[colorNum++] = color;
            }

        }
    }
}