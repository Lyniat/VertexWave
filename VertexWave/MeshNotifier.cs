using System.Collections.Generic;

namespace VertexWave
{
    public class MeshNotifier
    {
        private static List<IMesh> list = new List<IMesh>();

        public static void Add(IMesh mesh)
        {
            lock (list)
            {
                list.Add(mesh);
            }
        }
        
        public static void Remove(IMesh mesh)
        {
            lock (list)
            {
                list.Remove(mesh);
            }
        }

        public static void OnModified(int x, int z)
        {
            lock (list)
            {
                var i = 0;
                while (i < list.Count)
                {
                    list[i].OnModified(x,z);
                    i++;
                }
            } 
        }
    }
}