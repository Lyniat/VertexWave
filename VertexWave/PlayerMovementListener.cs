using System.Collections.Generic;

namespace VertexWave
{
    public class PlayerMovementListener : Node, IPlayerMovement
    {
        private static List<IPlayerMovement> clients = new List<IPlayerMovement>();

        public PlayerMovementListener()
        {
            
        }

        public void Add(IPlayerMovement p)
        {
            lock (clients)
            {
                if (!clients.Contains(p))
                {
                    clients.Add(p);
                    //GD.Print("adding");
                }
            }
        }

        public void Remove(IPlayerMovement p)
        {
            lock (clients)
            {
                if (clients.Contains(p))
                {
                    clients.Remove(p);
                }
            }
        }

        public void PlayerMovedX(float absX)
        {
            //GD.Print(absX);
            lock (clients)
            {
                foreach (var c in clients)
                {
                    //GD.Print(absX);
                    c.PlayerMovedX(absX);
                }
            }
        }

        public void PlayerMovedY(float absY)
        {
            lock (clients)
            {
                foreach (var c in clients)
                {
                    c.PlayerMovedY(absY);
                }
            }
        }

        public void PlayerMovedZ(float absZ)
        {
            lock (clients)
            {
                foreach (var c in clients)
                {
                    c.PlayerMovedZ(absZ);
                }
            }
        }
    }
}