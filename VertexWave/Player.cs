using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VertexWave.BoardController;
using VertexWave.Interfaces;

namespace VertexWave
{
    public class Player : Entity, IMessageListener, IGameState
    {
        private const float ControllerSensitivity = 1.75f;
        private readonly GameStateListener _gameStateListener;

        private readonly PlayerMovementListener _playerMovementListener;


        private byte _b0;
        private byte _b1;

        private Vector3 _boardDirection = Vector3.Zero;

        private float _directionX;
        private float _directionZ;
        private float _drift;

        private int _isMoving;

        private int _iteration;

        private float _light = 0;
        private Entity _mCamera;

        private bool _started;

        public Player(Model model, Vector3 pos)
        {
            position = pos;
            Model = model;
            Scale = 1 / 24f;
            Rotation = 0;

            _playerMovementListener = new PlayerMovementListener();
            _playerMovementListener.Add(this);

            _gameStateListener = new GameStateListener();
            _gameStateListener.Add(this);
        }

        public float Rotation { get; }
        public float Scale { get; }
        public Model Model { get; }

        protected override Vector3[] BoundingBox => new[]
        {
            new Vector3(-0.5f, 0, -0.5f),
            new Vector3(-0.5f, 0, 0.5f),
            new Vector3(0.5f, 0, -0.5f),
            new Vector3(0.5f, 0, 0.5f)
        };

        public void LostGame()
        {
            _isMoving = 0;
        }

        public void StartedGame()
        {
            _isMoving = 1;
        }

        public void LoadedGame()
        {
        }

        public void OnConnectionEvent(bool status)
        {
            //throw new NotImplementedException();
        }

        public void OnInput(float axis)
        {
            _boardDirection = new Vector3(axis, 0, 0);
        }

        public void OnMessageArrived(string message)
        {
            try
            {
                byte value;
                Console.WriteLine("arrived");
                try
                {
                    value = byte.Parse(message);
                }
                catch (OverflowException)
                {
                    //might occur sometimes if serial had problems reading
                    return;
                }

                if (_iteration == 0) // first byte must be 255
                {
                    if (value == 255)
                    {
                        _iteration = 1;
                        _b0 = 0;
                        _b1 = 0;
                    }

                    return;
                }

                if (_iteration == 1 || _iteration == 2) // 1 byte
                {
                    _b0 = (byte) (_b0 | value);
                    _iteration++;
                    return;
                }

                if (_iteration == 3 || _iteration == 4) // 2 byte
                {
                    _b1 = (byte) (_b1 | value);
                    _iteration++;

                    if (_iteration == 5)
                        _iteration = 0;
                    else
                        return;
                }


                byte[] bytes = {_b1, _b0};

                var axis = BitConverter.ToInt16(bytes, 0) / 100f;

                if (!_started)
                {
                    _drift = axis;
                    _started = true;
                }

                axis -= _drift;

                _boardDirection = new Vector3(axis, 0, 0);
            }
            catch (FormatException)
            {
                //might occur sometimes if serial had problems reading
            }
        }

        public void SetCamera(Entity camera)
        {
            _mCamera = camera;
        }

        public override void Update(float deltaTime)
        {
            /*
            var speed = 0.01f;
            var direction = position - mCamera.position; 
            if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Move(direction * speed);
                playerMovementListener.PlayerMovedX(position.X);
                playerMovementListener.PlayerMovedZ(position.Z);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Move(-direction * speed/2);
                playerMovementListener.PlayerMovedX(position.X);
                playerMovementListener.PlayerMovedZ(position.Z);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Jump(0.3f);
            }

            var newDirection = Vector3.Normalize(direction);
            _rotation = (float)Math.Atan2(newDirection.X, newDirection.Z);
            light = World.GetLightAt((int)position.X, (int)position.Y, (int)position.Z);

    */

            var direction = new Vector3(0, 0, -1);
            direction += _boardDirection / -15f;
            var speed = 0.018f;
            var speedVector = new Vector3(speed * ControllerSensitivity, 0, speed);
            Move(direction * speedVector * deltaTime * _isMoving);
            _playerMovementListener.PlayerMovedX(position.X);
            _playerMovementListener.PlayerMovedZ(position.Z);

            if (position.Y < WorldGenerator.PathHeight) _gameStateListener.LostGame();

            base.Update(deltaTime);
        }

        public override void Draw(bool alpha)
        {
            if (!alpha) return;
            var world = Matrix.CreateTranslation(position + new Vector3(0, 0.1f,
                                                     0.5f)); // to fix player y-axis and z-axis
            DrawModel(Model, world, VoxeLand.View, VoxeLand.Projection, _boardDirection.X / 50f, Scale);
            base.Draw(alpha);
        }

        private void RotateHead(Model model)
        {
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection, float rotation, float scale)
        {
            for (var i = 0; i < model.Meshes.Count; i++)
            {
                var mesh = model.Meshes[i];
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //in the fields declaration 
                    Matrix[] modelTransforms;

                    //in the loadContent method after loading the model
                    modelTransforms = new Matrix[model.Bones.Count];
                    model.CopyAbsoluteBoneTransformsTo(modelTransforms);

                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = new Vector3(1, 1, 1);


                    effect.World = modelTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationZ(rotation) *
                                   Matrix.CreateScale(scale) *
                                   world; //where 'mesh' iterates through all objects in the model

                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }
    }
}