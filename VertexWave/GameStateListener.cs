using VertexWave.Interfaces;
using System;
using System.Collections.Generic;

namespace VertexWave
{
    public class GameStateListener
    {
        private static List<IGameState> clients = new List<IGameState>();

        public void Add(IGameState e)
        {
            lock (clients)
            {
                if (!clients.Contains(e))
                {
                    clients.Add(e);
                }
            }
        }

        public void Remove(IGameState e)
        {
            lock (clients)
            {
                if (clients.Contains(e))
                {
                    clients.Remove(e);
                }
            }
        }

        public void LostGame()
        {
            lock (clients)
            {
                foreach (var c in clients)
                {
                    c.LostGame();
                }
            }
        }

        public void StartedGame()
        {
            lock (clients)
            {
                foreach (var c in clients)
                {
                    c.StartedGame();
                }
            }
        }

        public void LoadedGame()
        {
            lock (clients)
            {
                foreach (var c in clients)
                {
                    c.LoadedGame();
                }
            }
        }

    }
}
