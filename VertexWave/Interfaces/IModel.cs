using Microsoft.Xna.Framework.Graphics;

namespace VertexWave.Interfaces
{
    internal interface IModel
    {
        int Rotation { get; }
        float Scale { get; }
        Model Model { get; }
    }
}