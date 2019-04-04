using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VertexWave
{
    public class Camera : Entity
    {
        private const float MaxHeigth = 25;
        private const float MinHeigth = 0;

        private readonly float _distanceToPlayer = 7;

        private readonly Entity _toFollow;

        private Vector3 _cameraDirection;

        private float _height;


        private int _oldMouseX = -1;
        private int _oldMouseY = -1;

        private float _rotation;

        //defines speed of camera movement
        private float _speed = 0.1F;

        public float Distance = 500;

        public Camera(Vector3 pos, Vector3 target, Vector3 up)
        {
            // TODO: Construct any child components here

            // Build camera view matrix
            position = pos;
            _cameraDirection = target - pos;
            _cameraDirection.Normalize();
            CreateLookAt();

            //projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)VoxeLand.game.Window.ClientBounds.Width / (float)VoxeLand.game.Window.ClientBounds.Height, 1f, 500f);

            //projection = Matrix.CreateOrthographic((float)VoxeLand.game.Window.ClientBounds.Width/40f , (float)VoxeLand.game.Window.ClientBounds.Height/40f, 1.0f, 100.0f);
        }

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

            //projection = Matrix.CreateOrthographic((float)VoxeLand.game.Window.ClientBounds.Width/40f , (float)VoxeLand.game.Window.ClientBounds.Height/40f, 1.0f, 1000.0f);
        }

        public Matrix View { get; protected set; }
        public Matrix Projection { get; protected set; }

        /// <summary>
        ///     Allows the game component to perform any initialization it needs to before starting
        ///     to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Ready()
        {
            // TODO: Add your initialization code here

            // Set mouse position and do initial get state
            Mouse.SetPosition(VoxeLand.game.Window.ClientBounds.Width / 2,
                VoxeLand.game.Window.ClientBounds.Height / 2);

            base.Ready();
        }

        /// <summary>
        ///     Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(float deltaTime)
        {
            // TODO: Add your update code here


            // Move forward and backward


            position = new Vector3();
            //cameraMovePosition.X = cameraDirection.X;
            //cameraMovePosition.Z = cameraDirection.Z;

            position.X = _toFollow.position.X;
            position.Z = _toFollow.position.Z;
            position.Y = _toFollow.position.Y + _height;

            /*
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                rotation += 0.1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                rotation -= 0.1f;
            }

*/

            var mouseState = Mouse.GetState();
            var mousePosition = new Point(mouseState.X, mouseState.Y);

            if (mouseState.RightButton == ButtonState.Pressed)
            {
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

            if (mouseState.RightButton == ButtonState.Released)
            {
                _oldMouseX = -1;
                _oldMouseY = -1;
            }

            var newDistance = _distanceToPlayer;

            if (_height < 0) newDistance = Math.Abs(1f / (_height + 0.01f));

            position.X = _toFollow.position.X + (float) Math.Sin(_rotation) * newDistance;
            position.Z = _toFollow.position.Z + (float) Math.Cos(_rotation) * newDistance;


            _cameraDirection = _toFollow.position - position;

            CreateLookAt();

            //base.Update(deltaTime);
        }

        private void CreateLookAt()
        {
            View = Matrix.CreateLookAt(position, position + _cameraDirection, Vector3.Up);
        }
    }
}