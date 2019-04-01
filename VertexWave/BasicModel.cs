using Microsoft.Xna.Framework.Graphics;
using VertexWave.Loader;

namespace VertexWave
{
    public class BasicModel : Node
    {
        private VertexPositionColorLine[] _vertices;
        private int[] _indices;

        private DynamicVertexBuffer vb;
        private DynamicIndexBuffer ib;

        public BasicModel(WavefrontLoader wavefrontLoader)
        {
            var data = wavefrontLoader.GetData();
            _vertices = data.Item1;
            _indices = data.Item2;
            
            ib = new DynamicIndexBuffer(VoxeLand.game.GraphicsDevice, typeof(int), _indices.Length, BufferUsage.None);

            vb = new DynamicVertexBuffer(VoxeLand.game.GraphicsDevice, typeof(VertexPositionColorLine), _vertices.Length, BufferUsage.None);

            vb.SetData(_vertices);
            ib.SetData(_indices);
        }
        
        public override void Draw(bool alpha)
        {
            if (!alpha)
            {
                return;
            }
            
            VoxeLand.game.GraphicsDevice.Indices = ib;
            VoxeLand.game.GraphicsDevice.SetVertexBuffer(vb);

            VoxeLand.game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vb.VertexCount, 0, ib.IndexCount / 3);

            
            base.Draw(alpha);
        }
        
    }
}