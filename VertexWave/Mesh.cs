using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using Voxeland.Generators.BlockTypes;

namespace VertexWave
{
    public class Mesh : Node, IMesh
    {
        private readonly int _x;
        private readonly int _z;
        private Block[] _blocks;

        private bool _dirty = false;
        private GraphicsDevice _graphicsDevice;

        private DynamicIndexBuffer _ib;
        private IndexBuffer _iBuffer;
        private int[] _ind;
        private bool _isDrawing;

        private bool _isMeshing;

        private byte _light = Enviroment.Light;

        private DynamicVertexBuffer _vb;
        private VertexBuffer _vBuffer;
        private VertexPositionColorLine[] _vert;

        public Mesh(Block[] blocks, int x, int z)
        {
            MeshNotifier.Add(this);
            _graphicsDevice = VoxeLand.game.GraphicsDevice;
            _x = x / WorldGenerator.ChunkSize;
            _z = z / WorldGenerator.ChunkSize;
            new Thread(() =>
            {
                _isMeshing = true;
                //_blocks = blocks;
                //_blocks = WorldLighter.LightBlocks(_x, _z, _blocks);
                var chunk = new Chunk(x, 0, z, blocks);
                var data = chunk.GetData();
                _ind = data.Item2;
                _vert = data.Item1;
                _isMeshing = false;
            }).Start();
        }

        public long LastUpdate { get; } = 0;

        public void OnModified(int x, int z)
        {
            if (x != _x || z != _z) return;

            var newBlocks = World.GetChunkAt(x, z);
            var newChunk = new Chunk(x, 0, z, newBlocks);
            var data = newChunk.GetData();
            new Thread(() =>
            {
                _isMeshing = true;
                while (_isDrawing) ;
                /*
                if(ib != null){
                    ib.Dispose();
                }
                if(vb != null){
                    vb.Dispose();
                }
*/

                //ind = data.Item2;
                //vert = data.Item1;

                _ib = new DynamicIndexBuffer(VoxeLand.game.GraphicsDevice, typeof(int), _ind.Length, BufferUsage.None);

                _vb = new DynamicVertexBuffer(VoxeLand.game.GraphicsDevice, typeof(VertexPositionColorTexture),
                    _vert.Length, BufferUsage.None);

                _vb.SetData(_vert);
                _ib.SetData(_ind);

                _isMeshing = false;
            }).Start();
        }

        public override void Draw(bool alpha)
        {
            if (alpha) return;
            _isDrawing = true;

            if (_isMeshing)
            {
                _isDrawing = false;

                base.Draw(alpha);
                return;
            }

            if (_vb == null)
                if (_ind != null && _vert != null)
                {
                    _ib = new DynamicIndexBuffer(VoxeLand.game.GraphicsDevice, typeof(int), _ind.Length,
                        BufferUsage.None);

                    _vb = new DynamicVertexBuffer(VoxeLand.game.GraphicsDevice, typeof(VertexPositionColorLine),
                        _vert.Length, BufferUsage.None);

                    _vb.SetData(_vert);
                    _ib.SetData(_ind);
                }

            if (_vb != null && _ib != null && !alpha)
            {
                VoxeLand.game.GraphicsDevice.Indices = _ib;
                VoxeLand.game.GraphicsDevice.SetVertexBuffer(_vb);

                VoxeLand.game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, _vb.VertexCount, 0,
                    _ib.IndexCount / 2);
            }

            _isDrawing = false;

            base.Draw(alpha);
        }

        public override void Destroy()
        {
            new Thread(() =>
            {
                while (_isDrawing) ;
                _isMeshing = true;
                if (_ib != null) _ib.Dispose();
                if (_vb != null) _vb.Dispose();
            }).Start();
            MeshNotifier.Remove(this);
            base.Destroy();
        }
    }
}