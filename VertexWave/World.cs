using System;
using System.Collections.Generic;
using Voxeland.Generators.BlockTypes;

namespace VertexWave
{
    public class World
    {
        private static List<(int, int, Block[])> chunks = new List<(int, int, Block[])>();

        public static Block[] GetChunkAt(int x, int z){
            lock (chunks)
            {
                foreach (var c in chunks)
                {
                    if (c.Item1 == x && c.Item2 == z)
                    {
                        return c.Item3;
                    }
                }
            }

            return null;
        }

        public static void AddChunk(int x, int z, Block[] blocks){
            lock (chunks)
            {
                foreach (var c in chunks)
                {
                    if (c.Item1 == x && c.Item2 == z)
                    {
                        return;
                    }
                }

                chunks.Add((x, z, blocks));
            }
        }

        public static void RemoveChunk(int x, int z){
            lock (chunks)
            {
                foreach (var c in chunks)
                {
                    if (c.Item1 == x && c.Item2 == z)
                    {
                        chunks.Remove(c);
                        return;
                    }
                }
            }
        }

        public static bool IsCollisionAt(float x, float y, float z){
            /*
            var chunkX = worldX / WorldGenerator.ChunkSize;
            var chunkZ = worldZ / WorldGenerator.ChunkSize;
            var key = (chunkX, chunkZ);
            if(y < 0 || y >= WorldGenerator.ChunkHeight){
                return false;
            }
            if (!chunks.ContainsKey(key))
            {
                return true;
            }
            var chunk = chunks[key];
            var x = Math.Abs(worldX) % WorldGenerator.ChunkSize;
            var z = Math.Abs(worldZ) % WorldGenerator.ChunkSize;
            var block = chunk[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + y * (WorldGenerator.ChunkSize) + z];

            if(block.id == 0){
                return false;
            }
            return true;
            
            */

            int worldX = (int) x;
            int worldY = (int) y;
            int worldZ = (int) z;
                
            
            if(y < 0 || y >= WorldGenerator.ChunkHeight){
                return false;
            }

            int chunkX;
            int chunkZ;
            int localX;
            int localZ;

            if (worldX >= 0)
            {
                chunkX = worldX / WorldGenerator.ChunkSize;
                localX = worldX % WorldGenerator.ChunkSize;
            }
            else
            {
                chunkX = worldX / WorldGenerator.ChunkSize - 1;
                localX = WorldGenerator.ChunkSize + worldX % WorldGenerator.ChunkSize - 1;
            }
            
            if (worldZ >= 0)
            {
                chunkZ = worldZ / WorldGenerator.ChunkSize;
                localZ = worldZ % WorldGenerator.ChunkSize;
            }
            else
            {
                chunkZ = worldZ / WorldGenerator.ChunkSize - 1;
                localZ = WorldGenerator.ChunkSize + worldZ % WorldGenerator.ChunkSize -1;
            }

            var chunk = GetChunkAt(chunkX, chunkZ);
            if (chunk == null)
            {
                return true;
            }
            
            var block = chunk[localX * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + worldY * (WorldGenerator.ChunkSize) + localZ];
            
            if(block.id == 0){
                return false;
            }
            
            return block.passable == 0;
        }

        public static byte GetLightAt(int worldX, int y, int worldZ)
        {
            return 0;
            /*
            var chunkX = worldX / WorldGenerator.ChunkSize;
            var chunkZ = worldZ / WorldGenerator.ChunkSize;
            var key = (chunkX, chunkZ);
            if (y < 0 || y >= WorldGenerator.ChunkHeight)
            {
                return 0;
            }
            if (!chunks.ContainsKey(key))
            {
                return 0;
            }
            var chunk = chunks[key];
            var x = Math.Abs(worldX) % WorldGenerator.ChunkSize;
            var z = Math.Abs(worldZ) % WorldGenerator.ChunkSize;
            var block = chunk[x * (WorldGenerator.ChunkSize * WorldGenerator.ChunkHeight) + y * (WorldGenerator.ChunkSize) + z];

            return block.light;
            */
        }

    }
}
