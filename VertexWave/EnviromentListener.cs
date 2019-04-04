using System.Collections.Generic;

namespace VertexWave
{
    public class EnviromentListener
    {
        private static readonly List<IEnviroment> Clients = new List<IEnviroment>();

        public void Add(IEnviroment e)
        {
            lock (Clients)
            {
                if (!Clients.Contains(e)) Clients.Add(e);
            }
        }

        public void Remove(IEnviroment e)
        {
            lock (Clients)
            {
                if (Clients.Contains(e)) Clients.Remove(e);
            }
        }

        public void UpdatedLight(byte light, int id)
        {
            //GD.Print(absX);
            lock (Clients)
            {
                foreach (var c in Clients) c.UpdatedLight(light, id);
            }
        }
    }
}