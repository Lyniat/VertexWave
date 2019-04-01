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

    public virtual void CreateBlock(List<VertexPositionColorLine> vertices, List<Color> colors, List<int> index, ref int blockNum, Vector3 pos, int nextZ, Color color, Block block)
    {
        block.light = 255;
            int texTopPosX = block.textureTop % 16;
            int texTopPosY = block.textureTop / 16;
            
            int texBottomPosX = block.textureBottom % 16;
            int texBottomPosY = block.textureBottom / 16;

            int texSidePosX = block.textureSide % 16;
            int texSidePosY = block.textureSide / 16;
            
            float blockSize = 1f / 16;
            float texTopXPos = texTopPosX * blockSize;
            float texTopYPos = texTopPosY * blockSize;

            float texTopXPosEnd = (texTopPosX + 1) * blockSize;
            float texTopYPosEnd = (texTopPosY + 1) * blockSize;
            
            float texBottomXPos = texBottomPosX * blockSize;
            float texBottomYPos = texBottomPosY * blockSize;

            float texBottomXPosEnd = (texBottomPosX + 1) * blockSize;
            float texBottomYPosEnd = (texBottomPosY + 1) * blockSize;

            float texSideXPos = texSidePosX * blockSize;
            float texSideYPos = texSidePosY * blockSize;

            float texSideXPosEnd = (texSidePosX + 1) * blockSize;
            float texSideYPosEnd = (texSidePosY + 1) * blockSize;
            //x-
            if ((block.mask & 0b01000000) > 0)
            {
                vertices.Add(new VertexPositionColorLine(new Vector3(0, 0, 0) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(0, 1, 0) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(0, 1, 1 + nextZ) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(0, 0, 1 + nextZ) + pos, color));

                index.Add(blockNum + 0);
                index.Add(blockNum + 1);
                index.Add(blockNum + 1);
                index.Add(blockNum + 2);
                index.Add(blockNum + 2);
                index.Add(blockNum + 3);
                index.Add(blockNum + 3);
                index.Add(blockNum + 0);

                blockNum += 4;

                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
            }

            //x+
            if ((block.mask & 0b10000000) > 0)
            {
                vertices.Add(new VertexPositionColorLine(new Vector3(1, 0, 0) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(1, 1, 0) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(1, 1, 1 + nextZ) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(1, 0, 1 + nextZ) + pos, color));

                index.Add(blockNum + 0);
                index.Add(blockNum + 1);
                index.Add(blockNum + 1);
                index.Add(blockNum + 2);
                index.Add(blockNum + 2);
                index.Add(blockNum + 3);
                index.Add(blockNum + 3);
                index.Add(blockNum + 0);

                blockNum += 4;

                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
            }

            //z+
            if ((block.mask & 0b00001000) > 0)
            {
                vertices.Add(new VertexPositionColorLine(new Vector3(0, 0, 1 + nextZ) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(1, 0, 1 + nextZ) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(1, 1, 1 + nextZ) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(0, 1, 1 + nextZ) + pos, color));

                index.Add(blockNum + 0);
                index.Add(blockNum + 1);
                index.Add(blockNum + 1);
                index.Add(blockNum + 2);
                index.Add(blockNum + 2);
                index.Add(blockNum + 3);
                index.Add(blockNum + 3);
                index.Add(blockNum + 0);

                blockNum += 4;

                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
            }

            //z-
            if ((block.mask & 0b00000100) > 0)
            {
                vertices.Add(new VertexPositionColorLine(new Vector3(0, 0, 0) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(1, 0, 0) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(1, 1, 0) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(0, 1, 0) + pos, color));

                index.Add(blockNum + 0);
                index.Add(blockNum + 1);
                index.Add(blockNum + 1);
                index.Add(blockNum + 2);
                index.Add(blockNum + 2);
                index.Add(blockNum + 3);
                index.Add(blockNum + 3);
                index.Add(blockNum + 0);

                blockNum += 4;

                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
            }

            //y- (bottom)
            if ((block.mask & 0b00010000) > 0)
            {
                vertices.Add(new VertexPositionColorLine(new Vector3(0, 0, 0) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(0, 0, 1 + nextZ) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(1, 0, 1 + nextZ) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(1, 0, 0) + pos, color));

                index.Add(blockNum + 0);
                index.Add(blockNum + 1);
                index.Add(blockNum + 1);
                index.Add(blockNum + 2);
                index.Add(blockNum + 2);
                index.Add(blockNum + 3);
                index.Add(blockNum + 3);
                index.Add(blockNum + 0);

                blockNum += 4;

                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
            }

            //y+ (top)
            if ((block.mask & 0b00100000) > 0)
            {
                vertices.Add(new VertexPositionColorLine(new Vector3(0, 1, 0) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(0, 1, 1 + nextZ) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(1, 1, 1 + nextZ) + pos, color));
                vertices.Add(new VertexPositionColorLine(new Vector3(1, 1, 0) + pos, color));

                index.Add(blockNum + 0);
                index.Add(blockNum + 1);
                index.Add(blockNum + 1);
                index.Add(blockNum + 2);
                index.Add(blockNum + 2);
                index.Add(blockNum + 3);
                index.Add(blockNum + 3);
                index.Add(blockNum + 0);

                blockNum += 4;

                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
            }

        }
    }
}