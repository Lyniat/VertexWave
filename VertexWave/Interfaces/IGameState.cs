using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertexWave.Interfaces
{
    public interface IGameState
    {
        void LostGame();

        void StartedGame();

        void LoadedGame();
    }
}
