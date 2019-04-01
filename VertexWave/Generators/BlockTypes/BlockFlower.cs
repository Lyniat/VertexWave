using System.Collections.Generic;
using Microsoft.Xna.Framework;
using VertexWave;

namespace Voxeland.Generators.BlockTypes
{

    public class BlockFlower : DefaultBlock
    {

        public override void CreateBlock(List<VertexPositionColorLine> vertices, List<Color> colors, List<int> index, ref int blockNum, Vector3 pos, int nextZ, Color color, Block block)
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