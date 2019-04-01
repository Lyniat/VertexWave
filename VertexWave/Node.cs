using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VertexWave
{
    public class Node : IDrawable
    {
        private bool loaded = false;
        private List<Node> children = new List<Node>();
        private List<Node> toAdd = new List<Node>();
        
        public bool shouldRemove { get; private set; }
        public Node()
        {
            lock (VoxeLand.nodes)
            {
                VoxeLand.nodes.Add(this);
            }
        }
        
        public virtual void Ready()
        {
            
        }

        public void Add(Node n)
        {
            lock (toAdd)
            {
                toAdd.Add(n);
            }
        }
        
        public virtual void Destroy()
        {
            lock (children)
            {
                foreach (var c in children)
                {
                    c.Destroy();
                }
            }

            shouldRemove = true;

        }

        public virtual void Draw(bool alpha)
        {
            var length = children.Count;
            for (var i = 0; i < length; i++){
                lock (children)
                {
                    if (i < children.Count)
                    {
                        children[i].Draw(alpha);
                    }else{
                        break;
                    }
                }
            }
        }

        public virtual void Update(float deltaTime)
        {
            if (!loaded)
            {
                Ready();
                loaded = true;
            }
            lock (children)
            {
                foreach (var c in children)
                {
                    c.Update(deltaTime);
                }

                lock (toAdd)
                {
                    children.AddRange(toAdd);
                    toAdd.Clear();
                }

                var again = true;
                while (again)
                {
                    again = false;
                    foreach (var c in children)
                    {
                        if (c.shouldRemove)
                        {
                            children.Remove(c);
                            again = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}