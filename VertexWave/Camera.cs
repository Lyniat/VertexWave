using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VertexWave
{
    public class Camera : Entity
    {
        private const float MaxHeigth = 3;
        private const float MinHeigth = 1;

        private readonly float _distanceToPlayer = 4;

        private readonly Entity _toFollow;

        private Vector3 _cameraDirection;

        private float _height;


        private int _oldMouseX = -1;
        private int _oldMouseY = -1;

        private float _rotation;

        private float _speed = 0.1F;

        public float Distance = 500;

        public Camera(Entity follow)
        {
            // TODO: Construct any child components here
            var target = new Vector3(0, 0, 0);
            var up = Vector3.UnitZ;
            position = follow.position;
            _cameraDirection = target - position;
            _cameraDirection.Normalize();
            // Build camera view matrix
            _toFollow = follow;
            CreateLookAt();

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                VoxeLand.game.Window.ClientBounds.Width / (float) VoxeLand.game.Window.ClientBounds.Height, 0.1f,
                Distance);

        }

        public Matrix View { get; protected set; }
        public Matrix Projection { get; protected set; }

        public override void Ready()
        {

            Mouse.SetPosition(VoxeLand.game.Window.ClientBounds.Width / 2,
                VoxeLand.game.Window.ClientBounds.Height / 2);

            base.Ready();
        }

        public override void Update(float deltaTime)
        {

            position = new Vector3();

            position.X = _toFollow.position.X;
            position.Z = _toFollow.position.Z;
            position.Y = _toFollow.position.Y + _height;

            var mouseState = Mouse.GetState();

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                ResetMouse();
            }

            
            if (mouseState.RightButton == ButtonState.Released)
            {
                _oldMouseX = -1;
                _oldMouseY = -1;
            }
            

            var newDistance = _distanceToPlayer;

            if (_height < 0) newDistance = Math.Abs(1f / (_height + 0.01f));

            position.X = _toFollow.position.X + (float) Math.Sin(_rotation) * newDistance;
            position.Z = _toFollow.position.Z + (float) Math.Cos(_rotation) * newDistance;


            _cameraDirection = _toFollow.position - (position - new Vector3(0,1,0));

            CreateLookAt();
        }

        public void ResetMouse()
        {
            var mouseState = Mouse.GetState();
            var mousePosition = new Point(mouseState.X, mouseState.Y);
            if (_oldMouseX == -1 && _oldMouseY == -1)
            {
                _oldMouseX = mousePosition.X;
                _oldMouseY = mousePosition.Y;
            }
            else
            {
                var deltaX = _oldMouseX - mousePosition.X;
                var deltaY = _oldMouseY - mousePosition.Y;
                _oldMouseX = mousePosition.X;
                _oldMouseY = mousePosition.Y;

                _rotation += deltaX * 0.005f;

                _height -= deltaY * 0.1f;

                if (_height > MaxHeigth) _height = MaxHeigth;
                if (_height < MinHeigth) _height = MinHeigth;
            }
        }

        private void CreateLookAt()
        {
            View = Matrix.CreateLookAt(position, position + _cameraDirection, Vector3.Up);
        }
    }
}