using System.Collections.Generic;

namespace VertexWave
{
    public class MeshNotifier
    {
        private static readonly List<IMesh> List = new List<IMesh>();

        public static void Add(IMesh mesh)
        {
            lock (List)
            {
                List.Add(mesh);
            }
        }

        public static void Remove(IMesh mesh)
        {
            lock (List)
            {
                List.Remove(mesh);
            }
        }

        public static void OnModified(int x, int z)
        {
            lock (List)
            {
                var i = 0;
                while (i < List.Count)
                {
                    List[i].OnModified(x, z);
                    i++;
                }
            }
        }
    }
}