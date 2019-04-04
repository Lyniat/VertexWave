using System.Collections.Generic;

namespace VertexWave
{
    public class Node : IDrawable
    {
        private readonly List<Node> _children = new List<Node>();
        private readonly List<Node> _toAdd = new List<Node>();
        private bool _loaded;

        public Node()
        {
            lock (VoxeLand.nodes)
            {
                VoxeLand.nodes.Add(this);
            }
        }

        public bool ShouldRemove { get; private set; }

        public virtual void Draw(bool alpha)
        {
            var length = _children.Count;
            for (var i = 0; i < length; i++)
                lock (_children)
                {
                    if (i < _children.Count)
                        _children[i].Draw(alpha);
                    else
                        break;
                }
        }

        public virtual void Ready()
        {
        }

        public void Add(Node n)
        {
            lock (_toAdd)
            {
                _toAdd.Add(n);
            }
        }

        public virtual void Destroy()
        {
            lock (_children)
            {
                foreach (var c in _children) c.Destroy();
            }

            ShouldRemove = true;
        }

        public virtual void Update(float deltaTime)
        {
            if (!_loaded)
            {
                Ready();
                _loaded = true;
            }

            lock (_children)
            {
                foreach (var c in _children) c.Update(deltaTime);

                lock (_toAdd)
                {
                    _children.AddRange(_toAdd);
                    _toAdd.Clear();
                }

                var again = true;
                while (again)
                {
                    again = false;
                    foreach (var c in _children)
                        if (c.ShouldRemove)
                        {
                            _children.Remove(c);
                            again = true;
                            break;
                        }
                }
            }
        }
    }
}