using System;
using System.Collections.Generic;
using System.Threading;
using VertexWave.Interfaces;
using Voxeland.Generators.BlockTypes;

namespace VertexWave
{
    public class ChunkManager : Node, IPlayerMovement
    {
        //private Block[,,] blocks = new Block[ChunkSize, ChunkHeight, ChunkSize];

        public const int MaxChunkDistance = 6;

        private static readonly List<Tuple<int, int>> ToLoad = new List<Tuple<int, int>>();

        private static Block[,,] tree;

        private static float playerPosX;
        private static float playerPosZ;

        //private static List<MeshInstance> freeQueue = new List<MeshInstance>();

        private static readonly List<Tuple<int, int, Mesh, bool>> LoadedChunks =
            new List<Tuple<int, int, Mesh, bool>>();

        private readonly IWorldGenerator _generator = new WorldGenerator();

        //private static List<Tuple<int,int,MeshInstance>> meshes = new List<Tuple<int, int, MeshInstance>>();

        //unsafe byte* blocks = stackalloc byte[ChunkSize*ChunkSize*ChunkHeight*6]; // Block has 6 bytes
        private Block[] _blocks;
        private int _maxThreads = 4;

        private Node _node;

        private int _threads = 0;


        public void PlayerMovedX(float absX)
        {
            playerPosX = absX;
            CalculatePosition();
        }

        public void PlayerMovedY(float absY)
        {
            //playerPos.y = absY;
        }

        public void PlayerMovedZ(float absZ)
        {
            playerPosZ = absZ;
            CalculatePosition();
        }

        public override void Ready()
        {
            CalculatePosition();
            Console.WriteLine("ready");
            var t = new Thread(Loop);
            t.Start();
            new PlayerMovementListener().Add(this);
            //toLoad.Add(new Tuple<int, int>(0, 0));
        }

        public void OwnUpdate()
        {
            //base.Update();
            //Loop();
            //Console.WriteLine("updating");
            lock (LoadedChunks)
            {
                var again = true;
                while (again)
                {
                    again = false;
                    for (var i = 0; i < LoadedChunks.Count; i++)
                    {
                        var m = LoadedChunks[i];
                        var dis = Math.Abs(playerPosX / WorldGenerator.ChunkSize - m.Item1) +
                                  Math.Abs(playerPosZ / WorldGenerator.ChunkSize - m.Item2);
                        if (m.Item4)
                            if (dis > MaxChunkDistance * 2)
                            {
                                //GD.Print("free");
                                m.Item3.Destroy();
                                LoadedChunks.Remove(m);
                                var x = m.Item1;
                                var z = m.Item2;
                                World.RemoveChunk(x, z);
                                again = true;
                                //break;
                            }
                    }
                }
            }

            lock (ToLoad)
            {
                for (var i = 0; i < ToLoad.Count; i++)
                {
                    var m = ToLoad[i];
                    var dis = Math.Abs(playerPosX / WorldGenerator.ChunkSize - m.Item1) +
                              Math.Abs(playerPosZ / WorldGenerator.ChunkSize - m.Item2);
                    if (dis > MaxChunkDistance * 2)
                    {
                        ToLoad.Remove(m);
                        break;
                    }
                }
            }

            lock (LoadedChunks)
            {
                if (LoadedChunks.Count > 0)
                {
                    var count = -1;
                    for (var i = 0; i < LoadedChunks.Count; i++)
                        if (!LoadedChunks[i].Item4)
                        {
                            count = i;
                            break;
                        }

                    if (count >= 0)
                    {
                        var mesh = LoadedChunks[count];
                        lock (mesh.Item3)
                        {
                            //TODO: change to fna
                            var newMesh = new Tuple<int, int, Mesh, bool>(mesh.Item1, mesh.Item2, mesh.Item3, true);
                            LoadedChunks[count] = newMesh;

                            VoxeLand.root.Add(newMesh.Item3);
                            //GD.Print(loadedChunks[count].Item4);
                            //*/
                        }
                    }
                }
            }
        }

        public void AddToLoad(int x, int y, int z)
        {
            //toLoad.Add(new Tuple<int, int>(x, z));
        }

        private void Loop()
        {
            Console.WriteLine("loop");
            //new PlayerMovementListener().Add(this);
            while (true)
            {
                OwnUpdate();
                Tuple<int, int> nextPosition = null;
                lock (ToLoad)
                {
                    if (ToLoad.Count > 0)
                    {
                        var again = true;
                        while (again)
                        {
                            again = false;
                            for (var i = 0; i < ToLoad.Count - 1; i++)
                            {
                                var pos0 = Math.Sqrt(
                                    Math.Pow(playerPosX / WorldGenerator.ChunkSize - ToLoad[i].Item1, 2) +
                                    Math.Pow(playerPosZ / WorldGenerator.ChunkSize - ToLoad[i].Item2, 2));

                                var pos1 = Math.Sqrt(
                                    Math.Pow(playerPosX / WorldGenerator.ChunkSize - ToLoad[i + 1].Item1, 2) +
                                    Math.Pow(playerPosZ / WorldGenerator.ChunkSize - ToLoad[i + 1].Item2, 2));

                                if (pos0 > pos1)
                                {
                                    var temp = ToLoad[i];
                                    ToLoad[i] = ToLoad[i + 1];
                                    ToLoad[i + 1] = temp;
                                    again = true;
                                }
                            }
                        }

                        nextPosition = ToLoad[0];
                        //toLoad.RemoveAt(toLoad.Count - 1);
                    }
                }

                if (nextPosition == null) continue;

                Console.WriteLine("not null");

                var x = nextPosition.Item1;
                var z = nextPosition.Item2;

                var m = GenerateChunk(x, 0, z);
                lock (LoadedChunks)
                {
                    LoadedChunks.Add(
                        new Tuple<int, int, Mesh, bool>(x, z, m, false));
                }

                lock (ToLoad)
                {
                    foreach (var l in ToLoad)
                        if (l.Item1 == x && l.Item2 == z)
                        {
                            ToLoad.Remove(l);
                            break;
                        }
                }
            }
        }

        public Mesh GenerateChunk(int xPos, int yPos, int zPos)
        {
            Console.WriteLine("generating");
            _blocks = _generator.GetChunkAt(xPos, zPos).block;

            World.AddChunk(xPos, zPos, _blocks);

            var mesh = new Mesh(_blocks, xPos * WorldGenerator.ChunkSize, zPos * WorldGenerator.ChunkSize);
            //mesh.Translation = new Vector3(xPos * WorldGenerator.ChunkSize, yPos, zPos * WorldGenerator.ChunkSize); TODO: change to fna

            return mesh;
        }

        private void AddModel(Block[,,] world, Block[,,] model, int xPos, int yPos, int zPos, int deltaX, int deltaY,
            int deltaZ)
        {
            for (var x = 0; x < model.GetLength(0); x++)
            for (var y = 0; y < model.GetLength(1); y++)
            for (var z = 0; z < model.GetLength(2); z++)
            {
                if (model[x, y, z].id == 0) continue;

                var finalXPos = xPos + deltaX + x;
                var finalYPos = yPos + deltaY + y;
                var finalZPos = zPos + deltaZ + z;

                if (finalXPos < 0 || finalXPos > world.GetLength(0) - 1 || finalYPos < 0 ||
                    finalYPos > world.GetLength(1) - 1 || finalZPos < 0 || finalZPos > world.GetLength(2) - 1)
                    continue;

                world[finalXPos, finalYPos, finalZPos] = model[x, y, z];
            }
        }

        private void CalculatePosition()
        {
            var worldX = Math.Round(playerPosX / WorldGenerator.ChunkSize);
            var worldZ = Math.Round(playerPosZ / WorldGenerator.ChunkSize);

            for (var x = -MaxChunkDistance + 1; x <= MaxChunkDistance - 1; x++)
            for (var z = -MaxChunkDistance + 1; z <= MaxChunkDistance - 1; z++)
            {
                var xPos = Convert.ToInt32(worldX + x);
                var zPos = Convert.ToInt32(worldZ + z);

                lock (ToLoad)
                {
                    var shouldAdd = true;
                    foreach (var t in ToLoad)
                        if (t.Item1 == xPos && t.Item2 == zPos)
                            shouldAdd = false;

                    lock (LoadedChunks)
                    {
                        foreach (var t in LoadedChunks)
                            if (t.Item1 == xPos && t.Item2 == zPos)
                                shouldAdd = false;
                    }

                    if (shouldAdd) ToLoad.Add(new Tuple<int, int>(xPos, zPos));
                }
            }
        }
    }
}