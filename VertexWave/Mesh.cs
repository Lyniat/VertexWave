using System;
using System.Net;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxeland.Generators.BlockTypes;

namespace VertexWave
{
    public class Mesh : Node
    {
        private VertexBuffer _vBuffer;
        private IndexBuffer _iBuffer;
        private int[] ind;
        private VertexPositionColorLine[] vert;
        private GraphicsDevice _graphicsDevice;

        private DynamicIndexBuffer ib;

        private DynamicVertexBuffer vb;

        private bool isMeshing = false;
        bool isDrawing = false;

        private Block[] _blocks;

        private byte _light = Enviroment.Light;

        int _x;
        int _z;

        private long lastUpdate = 0;

        public long LastUpdate => lastUpdate;

        private bool dirty = false;
        
        public Mesh(Block[] blocks,int x, int z){
            _graphicsDevice = VoxeLand.game.GraphicsDevice;
            _x = x/WorldGenerator.ChunkSize;
            _z = z/WorldGenerator.ChunkSize;
            new Thread(() =>
            {
                isMeshing = true;
                //_blocks = blocks;
                //_blocks = WorldLighter.LightBlocks(_x, _z, _blocks);
                //Chunk chunk = new Chunk(x, 0, z, blocks);
                var data = Chunk.CreateFromArray(blocks, _x, 0, _z);
                //chunk.GetData();
                ind = data.Item2;
                vert = data.Item1;
                isMeshing = false;
                ib = new DynamicIndexBuffer(VoxeLand.game.GraphicsDevice, typeof(int), ind.Length, BufferUsage.None);

                vb = new DynamicVertexBuffer(VoxeLand.game.GraphicsDevice, typeof(VertexPositionColorLine), vert.Length, BufferUsage.None);

                vb.SetData(vert);
                ib.SetData(ind);

            }).Start();
        }

        public override void Draw(bool alpha)
        {

            if (vb != null && ib != null)
            {

                lock (vb)
                {
                    lock (ib)
                    {
                        if (!vb.IsDisposed && !ib.IsDisposed)
                        {
                            VoxeLand.game.GraphicsDevice.Indices = ib;
                            VoxeLand.game.GraphicsDevice.SetVertexBuffer(vb);

                            VoxeLand.game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, vb.VertexCount, 0, ib.IndexCount / 2);
                        }
                    }
                }
            }

            base.Draw(alpha);
        }

        public override void Destroy()
        {
            new Thread(() =>
            {
                lock (vb)
                {
                    lock (ib)
                    {
                        if (ib != null)
                        {
                            ib.Dispose();
                        }
                        if (vb != null)
                        {
                            vb.Dispose();
                        }

                    }
                }

            }).Start();
            base.Destroy();
        }
    }
}