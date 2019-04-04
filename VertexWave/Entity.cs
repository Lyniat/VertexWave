using System;
using Microsoft.Xna.Framework;

namespace VertexWave
{
    public class Entity : Node
    {
        private const int CameraHeight = 2;

        private bool _wasOnGroundAgain = true;

        public float currentGravivty = -0.3f;
        public Vector2 lastPosition;
        public Vector3 position;

        public virtual bool Static => false;

        protected virtual Vector3[] BoundingBox => new[] {new Vector3(0, 0, 0)};

        public override void Update(float deltaTime)
        {
            if (Static) return;
            currentGravivty -= 0.01f;
            if (currentGravivty < -0.3f) currentGravivty = -0.3f;
            //System.Console.WriteLine("delat " + deltaTime);
            var oldY = position.Y;
            position.Y = position.Y + currentGravivty;

            var collided = false;
            foreach (var b in BoundingBox)
                if (World.IsCollisionAt(position.X + b.X, position.Y + b.Y, position.Z + b.Z))
                {
                    collided = true;
                    break;
                }

            if (collided)
            {
                position.Y = (float) Math.Ceiling(position.Y);
                _wasOnGroundAgain = true;
            }
        }


        public void Jump(float newGravity)
        {
            if (!_wasOnGroundAgain) return;
            _wasOnGroundAgain = false;

            currentGravivty = newGravity;
        }

        public bool Move(Vector3 direction)
        {
            position.X = position.X + direction.X;
            position.Z = position.Z + direction.Z;

            var collided = false;


            foreach (var b in BoundingBox)
                if (World.IsCollisionAt(position.X + b.X, position.Y + b.Y, position.Z + b.Z))
                {
                    collided = true;
                    break;
                }

            return true;

            if (collided)
            {
                position.X = lastPosition.X;
                position.Z = lastPosition.Y; //Vector 2
                return false;
            }

            lastPosition.X = position.X;
            lastPosition.Y = position.Z; //Vector 2

            return true;
        }
    }
}