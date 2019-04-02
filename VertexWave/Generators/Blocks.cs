using System;
using System.Collections.Generic;
using Voxeland.Generators.BlockTypes;

namespace VertexWave.Generators
{
    public class Blocks
    {
        public static Dictionary<BlockIDs, Block> blocks = new Dictionary<BlockIDs, Block>();

        static Blocks(){
            var defRenderer = new DefaultBlock();
            var matrixRenderer = new BlockMatrixFloor();
            var air = new Block();
            air.id = (byte)BlockIDs.Air;
            blocks[BlockIDs.Air] = air;

            var grass = new Block();
            grass.r = 0;
            grass.g = 100;
            grass.b = 0;
            grass.id = (byte)BlockIDs.Dirt;
            grass.textureTop = 2;
            grass.textureSide = 2;
            grass.textureBottom = 2;
            grass.renderer = defRenderer;
            blocks[BlockIDs.Dirt] = grass;

            var grassTop = new Block();
            grassTop.r = 0;
            grassTop.g = 100;
            grassTop.b = 0;
            grassTop.id = (byte)BlockIDs.GrassTop;
            grassTop.textureTop = 0;
            grassTop.textureSide = 3;
            grassTop.textureBottom = 2;
            grassTop.renderer = defRenderer;
            blocks[BlockIDs.GrassTop] = grassTop;

            var leaves = new Block();
            leaves.r = 0;
            leaves.g = 255;
            leaves.b = 0;
            leaves.id = (byte)BlockIDs.Leaves;
            leaves.visible = 0;
            leaves.textureTop = 15;
            leaves.textureSide = 15;
            leaves.renderer = defRenderer;
            blocks[BlockIDs.Leaves] = leaves;
            
            var leavesSpruce = new Block();
            leavesSpruce.r = 0;
            leavesSpruce.g = 255;
            leavesSpruce.b = 0;
            leavesSpruce.id = (byte)BlockIDs.LeavesSpruce;
            leavesSpruce.visible = 0;
            leavesSpruce.textureTop = 14;
            leavesSpruce.textureSide = 14;
            leavesSpruce.renderer = defRenderer;
            blocks[BlockIDs.LeavesSpruce] = leavesSpruce;
            
            var leavesOrange = new Block();
            leavesOrange.r = 255;
            leavesOrange.g = 144;
            leavesOrange.b = 9;
            leavesOrange.id = (byte)BlockIDs.LeavesOrange;
            leavesOrange.visible = 0;
            leavesOrange.textureTop = 31;
            leavesOrange.textureSide = 31;
            leavesOrange.renderer = defRenderer;
            blocks[BlockIDs.LeavesOrange] = leavesOrange;
            
            var leavesYellow = new Block();
            leavesYellow.r = 255;
            leavesYellow.g = 233;
            leavesYellow.b = 0;
            leavesYellow.id = (byte)BlockIDs.LeavesYellow;
            leavesYellow.visible = 0;
            leavesYellow.textureTop = 47;
            leavesYellow.textureSide = 47;
            leavesYellow.renderer = defRenderer;
            blocks[BlockIDs.LeavesYellow] = leavesYellow;
            
            var leavesGreen = new Block();
            leavesGreen.r = 0;
            leavesGreen.g = 255;
            leavesGreen.b = 0;
            leavesGreen.id = (byte)BlockIDs.LeavesGreen;
            leavesGreen.visible = 0;
            leavesGreen.textureTop = 30;
            leavesGreen.textureSide = 30;
            leavesGreen.renderer = defRenderer;
            blocks[BlockIDs.LeavesGreen] = leavesGreen;
 
            
            var trunk = new Block();
            trunk.r = 120;
            trunk.g = 84;
            trunk.b = 0;
            trunk.id = (byte)BlockIDs.Trunk;
            trunk.visible = 1;
            trunk.textureTop = 21;
            trunk.textureSide = 20;
            trunk.textureBottom = 21;
            trunk.renderer = defRenderer;
            blocks[BlockIDs.Trunk] = trunk;

            var water = new Block();
            water.r = 255;
            water.g = 255;
            water.b = 255;
            water.id = (byte)BlockIDs.Water;
            water.textureTop = 205;
            water.textureSide = 205;
            water.textureBottom = 205;
            water.renderer = defRenderer;
            water.visible = 0;
            water.transperency = 128;
            blocks[BlockIDs.Water] = water;

            var sand = new Block();
            sand.r = 255;
            sand.g = 200;
            sand.b = 255;
            sand.id = (byte)BlockIDs.Sand;
            sand.textureTop = 18;
            sand.textureSide = 18;
            sand.textureBottom = 18;
            sand.renderer = defRenderer;
            blocks[BlockIDs.Sand] = sand;
            
            var stone = new Block();
            stone.r = 255;
            stone.g = 255;
            stone.b = 255;
            stone.id = (byte)BlockIDs.Stone;
            stone.renderer = defRenderer;
            stone.textureTop = 1;
            stone.textureSide = 1;
            stone.textureBottom = 1;
            blocks[BlockIDs.Stone] = stone;
            
            var snow = new Block();
            snow.r = 255;
            snow.g = 255;
            snow.b = 255;
            snow.id = (byte)BlockIDs.Snow;
            snow.renderer = defRenderer;
            snow.textureTop = 67;
            snow.textureSide = 68;
            snow.textureBottom = 2;
            blocks[BlockIDs.Snow] = snow;
            
            var plant = new Block();
            plant.r = 255;
            plant.g = 200;
            plant.b = 255;
            plant.id = (byte)BlockIDs.Plant;
            plant.visible = 1;
            plant.textureTop = 39;
            plant.textureSide = 39;
            plant.renderer = defRenderer;
            plant.passable = 1;
            blocks[BlockIDs.Plant] = plant;
            
            var poppy = new Block();
            var flowerRenderer = new BlockFlower();
            poppy.r = 255;
            poppy.g = 0;
            poppy.b = 0;
            poppy.id = (byte)BlockIDs.FlowerPoppy;
            poppy.visible = 1;
            poppy.textureTop = 253;
            poppy.textureSide = 253;
            poppy.renderer = flowerRenderer;
            poppy.passable = 1;
            blocks[BlockIDs.FlowerPoppy] = poppy; 

            var matrixFloor = new Block();
            matrixFloor.r = 255;
            matrixFloor.g = 0;
            matrixFloor.b = 255;
            matrixFloor.id = (byte)BlockIDs.MatrixFloor;
            matrixFloor.visible = 1;
            matrixFloor.textureTop = 4;
            matrixFloor.textureSide = 4;
            matrixFloor.renderer = matrixRenderer;
            blocks[BlockIDs.MatrixFloor] = matrixFloor;

            var matrixFloorSafety = new Block();
            matrixFloorSafety.r = 20;
            matrixFloorSafety.g = 0;
            matrixFloorSafety.b = 20;
            matrixFloorSafety.id = (byte)BlockIDs.MatrixFloorSafety;
            matrixFloorSafety.visible = 1;
            matrixFloorSafety.textureTop = 4;
            matrixFloorSafety.textureSide = 4;
            matrixFloorSafety.renderer = matrixRenderer;
            blocks[BlockIDs.MatrixFloorSafety] = matrixFloorSafety;

            var tulip = new Block();
            tulip.r = 255;
            tulip.g = 255;
            tulip.b = 0;
            tulip.id = (byte)BlockIDs.FlowerTulip;
            tulip.visible = 1;
            tulip.textureTop = 253;
            tulip.textureSide = 253;
            tulip.renderer = flowerRenderer;
            tulip.passable = 1;
            blocks[BlockIDs.FlowerTulip] = tulip;

            var dummy = new Block();
            dummy.r = 1;
            dummy.g = 0;
            dummy.b = 0;
            dummy.id = (byte)BlockIDs.Dummy;
            dummy.visible = 1;
            dummy.textureTop = 0;
            dummy.textureSide = 0;
            dummy.renderer = matrixRenderer;
            blocks[BlockIDs.Dummy] = dummy;

            var blockArrowRightRenderer = new BlockArrowRight();
            var blockArrowRight = new Block();
            blockArrowRight.r = 255;
            blockArrowRight.g = 255;
            blockArrowRight.b = 0;
            blockArrowRight.id = (byte)BlockIDs.ArrowRight;
            blockArrowRight.visible = 1;
            blockArrowRight.renderer = blockArrowRightRenderer;
            blocks[BlockIDs.ArrowRight] = blockArrowRight;

            var blockArrowLeftRenderer = new BlockArrowLeft();
            var blockArrowLeft = new Block();
            blockArrowLeft.r = 255;
            blockArrowLeft.g = 255;
            blockArrowLeft.b = 0;
            blockArrowLeft.id = (byte)BlockIDs.ArrowLeft;
            blockArrowLeft.visible = 1;
            blockArrowLeft.renderer = blockArrowLeftRenderer;
            blocks[BlockIDs.ArrowLeft] = blockArrowLeft;


        }
    }

    public enum BlockIDs{
        Air = 0,
        Dirt = 1,
        Sand = 2,
        Leaves = 3,
        Water = 4,
        GrassTop = 5,
        Trunk = 6,
        Plant = 7,
        Stone = 8,
        Snow = 9,
        LeavesSpruce = 10,
        LeavesYellow = 11,
        LeavesOrange = 12,
        LeavesGreen = 13,
        FlowerPoppy = 14,
        MatrixFloor = 15,
        MatrixFloorSafety = 16,
        FlowerTulip = 17,
        Dummy = 18,
        ArrowRight = 19,
        ArrowLeft = 20
    }
}
