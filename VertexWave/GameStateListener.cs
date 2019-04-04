using System.Collections.Generic;
using VertexWave.Interfaces;

namespace VertexWave
{
    public class GameStateListener
    {
        private static readonly List<IGameState> Clients = new List<IGameState>();

        public void Add(IGameState e)
        {
            lock (Clients)
            {
                if (!Clients.Contains(e)) Clients.Add(e);
            }
        }

        public void Remove(IGameState e)
        {
            lock (Clients)
            {
                if (Clients.Contains(e)) Clients.Remove(e);
            }
        }

        public void LostGame()
        {
            lock (Clients)
            {
                foreach (var c in Clients) c.LostGame();
            }
        }

        public void StartedGame()
        {
            lock (Clients)
            {
                foreach (var c in Clients) c.StartedGame();
            }
        }

        public void LoadedGame()
        {
            lock (Clients)
            {
                foreach (var c in Clients) c.LoadedGame();
            }
        }
    }
}