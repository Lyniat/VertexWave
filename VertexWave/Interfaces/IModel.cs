using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertexWave.Interfaces
{
    interface IModel
    {
        int Rotation { get;}
        float Scale { get;}
        Model Model { get;}
    }
}
