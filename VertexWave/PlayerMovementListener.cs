using System.Collections.Generic;

namespace VertexWave
{
    public class PlayerMovementListener : Node, IPlayerMovement
    {
        private static readonly List<IPlayerMovement> Clients = new List<IPlayerMovement>();

        public void PlayerMovedX(float absX)
        {
            //GD.Print(absX);
            lock (Clients)
            {
                foreach (var c in Clients)
                    //GD.Print(absX);
                    c.PlayerMovedX(absX);
            }
        }

        public void PlayerMovedY(float absY)
        {
            lock (Clients)
            {
                foreach (var c in Clients) c.PlayerMovedY(absY);
            }
        }

        public void PlayerMovedZ(float absZ)
        {
            lock (Clients)
            {
                foreach (var c in Clients) c.PlayerMovedZ(absZ);
            }
        }

        public void Add(IPlayerMovement p)
        {
            lock (Clients)
            {
                if (!Clients.Contains(p)) Clients.Add(p);
            }
        }

        public void Remove(IPlayerMovement p)
        {
            lock (Clients)
            {
                if (Clients.Contains(p)) Clients.Remove(p);
            }
        }
    }
}