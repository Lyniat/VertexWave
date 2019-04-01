using System;
using Microsoft.Xna.Framework;

namespace VertexWave
{
    public class Entity : Node
    {
        public Vector3 position;
        const int CameraHeight = 2;

        public float currentGravivty = -0.3f;
        public Vector2 lastPosition = new Vector2();

        bool wasOnGroundAgain = true;

        protected virtual Vector3[] BoundingBox => new Vector3[]{new Vector3(0,0,0)};

        public Entity()
        {
            
        }

        public override void Update(float deltaTime)
        {
            currentGravivty -= 0.01f;
            if(currentGravivty < -0.3f){
                currentGravivty = -0.3f;
            }
            //System.Console.WriteLine("delat " + deltaTime);
            var oldY = position.Y;
            position.Y = position.Y + currentGravivty;

            var collided = false;
            foreach (var b in BoundingBox)
            {
                if (World.IsCollisionAt(position.X + b.X, position.Y + b.Y, position.Z + b.Z))
                {
                    collided = true;
                    break;
                }
            }

            if (collided)
            {
                position.Y = (float)Math.Ceiling(position.Y);
                wasOnGroundAgain = true;
            }
        }


        public void Jump(float newGravity){
            if(!wasOnGroundAgain){
                return;
            }
            wasOnGroundAgain = false;

            currentGravivty = newGravity;
        }

        public bool Move(Vector3 direction){
            position.X = position.X + direction.X;
            position.Z = position.Z + direction.Z;

            var collided = false;


            foreach (var b in BoundingBox)
            {
                if (World.IsCollisionAt(position.X + b.X, position.Y + b.Y, position.Z + b.Z))
                {
                    collided = true;
                    break;
                }
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
