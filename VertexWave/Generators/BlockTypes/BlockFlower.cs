using System.Collections.Generic;
using Microsoft.Xna.Framework;
using VertexWave;

namespace Voxeland.Generators.BlockTypes
{
    public class BlockFlower : DefaultBlock
    {
        public override void CreateBlock(List<VertexPositionColorLine> vertices, List<Color> colors, List<int> index,
            ref int blockNum, Vector3 pos, int nextZ, Color color, Block block)
        {
            block.light = 255;
            var texTopPosX = block.textureTop % 16;
            var texTopPosY = block.textureTop / 16;

            var texBottomPosX = block.textureBottom % 16;
            var texBottomPosY = block.textureBottom / 16;

            var texSidePosX = block.textureSide % 16;
            var texSidePosY = block.textureSide / 16;

            var blockSize = 1f / 16;
            var texTopXPos = texTopPosX * blockSize;
            var texTopYPos = texTopPosY * blockSize;

            var texTopXPosEnd = (texTopPosX + 1) * blockSize;
            var texTopYPosEnd = (texTopPosY + 1) * blockSize;

            var texBottomXPos = texBottomPosX * blockSize;
            var texBottomYPos = texBottomPosY * blockSize;

            var texBottomXPosEnd = (texBottomPosX + 1) * blockSize;
            var texBottomYPosEnd = (texBottomPosY + 1) * blockSize;

            var texSideXPos = texSidePosX * blockSize;
            var texSideYPos = texSidePosY * blockSize;

            var texSideXPosEnd = (texSidePosX + 1) * blockSize;
            var texSideYPosEnd = (texSidePosY + 1) * blockSize;

            var colorGreen = new Color(0, 255, 0);

            //line
            vertices.Add(new VertexPositionColorLine(new Vector3(0.5f, 0, 0.5f) + pos, colorGreen));
            vertices.Add(new VertexPositionColorLine(new Vector3(0.5f, 0.7f, 0.5f) + pos, colorGreen));

            index.Add(blockNum + 0);
            index.Add(blockNum + 1);

            blockNum += 2;

            colors.Add(color);
            colors.Add(color);

            vertices.Add(new VertexPositionColorLine(new Vector3(0.5f, 0.7f, 0.5f) + pos, color));

            vertices.Add(new VertexPositionColorLine(new Vector3(0.2f, 1f, 0.5f) + pos, color));

            vertices.Add(new VertexPositionColorLine(new Vector3(0.8f, 1f, 0.5f) + pos, color));

            vertices.Add(new VertexPositionColorLine(new Vector3(0.5f, 1f, 0.2f) + pos, color));

            vertices.Add(new VertexPositionColorLine(new Vector3(0.5f, 1f, 0.8f) + pos, color));

            vertices.Add(new VertexPositionColorLine(new Vector3(0.5f, 1f, 0.2f) + pos, color));

            vertices.Add(new VertexPositionColorLine(new Vector3(0.8f, 1f, 0.8f) + pos, color));

            vertices.Add(new VertexPositionColorLine(new Vector3(0.2f, 1f, 0.8f) + pos, color));

            vertices.Add(new VertexPositionColorLine(new Vector3(0.8f, 1f, 0.2f) + pos, color));

            vertices.Add(new VertexPositionColorLine(new Vector3(0.2f, 1f, 0.2f) + pos, color));

            index.Add(blockNum + 0);
            index.Add(blockNum + 1);

            index.Add(blockNum + 0);
            index.Add(blockNum + 2);

            index.Add(blockNum + 0);
            index.Add(blockNum + 3);

            index.Add(blockNum + 0);
            index.Add(blockNum + 4);

            index.Add(blockNum + 0);
            index.Add(blockNum + 5);

            index.Add(blockNum + 0);
            index.Add(blockNum + 6);

            index.Add(blockNum + 0);
            index.Add(blockNum + 7);

            index.Add(blockNum + 0);
            index.Add(blockNum + 8);

            index.Add(blockNum + 0);
            index.Add(blockNum + 9);


            blockNum += 10;

            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
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