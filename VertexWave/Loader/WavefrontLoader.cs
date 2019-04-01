using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace VertexWave.Loader
{
    public class WavefrontLoader
    {
        
        List<int> indices = new List<int>();
        List<VertexPositionColorLine> verts = new List<VertexPositionColorLine>();
        
        public WavefrontLoader(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    String content = sr.ReadToEnd();
                    Load(content);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        private void Load(string content)
        {
            string[] lines = content.Split('\n');
            List<Vector3> vertices = new List<Vector3>();

            string numbers;
            foreach (var l in lines)
            {
                if (l.StartsWith("v "))
                {
                    numbers = l.Replace("v ", "");
                    var vs = numbers.Split(' ');

                    var newVector = new Vector3(float.Parse(vs[0]), float.Parse(vs[1]), float.Parse(vs[2]));

                    vertices.Add(newVector);

                }

                if (l.StartsWith("f "))
                {
                    numbers = l.Replace("f ", "");
                    var fs = numbers.Split(' ');

                    foreach (var plane in fs)
                    {
                        var inds = plane.Split('/');

                        foreach (var ind in inds)
                        {
                            indices.Add(int.Parse(ind));
                        }
                    }
                }
            }

            

            foreach (var v in vertices)
            {
                //verts.Add(new VertexPositionColorTexture(v, Color.White, new Vector2(0, 0)));
            }

        }

        public (VertexPositionColorLine[], int[]) GetData()
        {
            VertexPositionColorLine[] vert = verts.ToArray();
            int[] ind = indices.ToArray();

            return (vert, ind);
        }
    }
}