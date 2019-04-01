using System;
using System.Collections.Generic;

namespace VertexWave
{
    public class EnviromentListener
    {
        private static List<IEnviroment> clients = new List<IEnviroment>();

        public void Add(IEnviroment e)
        {
            lock (clients)
            {
                if (!clients.Contains(e))
                {
                    clients.Add(e);
                }
            }
        }

        public void Remove(IEnviroment e)
        {
            lock (clients)
            {
                if (clients.Contains(e))
                {
                    clients.Remove(e);
                }
            }
        }

        public void UpdatedLight(byte light, int id)
        {
            //GD.Print(absX);
            lock (clients)
            {
                foreach (var c in clients)
                {
                    c.UpdatedLight(light , id);
                }
            }
        }

    }
}
