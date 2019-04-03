using System;
using VertexWave.BoardController;
using VertexWave.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace VertexWave
{
	public class Player : Entity, IMessageListener, IGameState
    {
        Entity mCamera;


        float _rotation;
        float _scale;
        Model _model;

        float directionX;
        float directionZ;

        float light = 0;

        int isMoving = 0;

        public float Rotation { get => _rotation; }
        public float Scale { get => _scale; }
        public Model Model { get => _model; }

        PlayerMovementListener playerMovementListener;
        GameStateListener gameStateListener;

        int iteration = 0;
        byte b_0 = 0;
        byte b_1 = 0;

        bool started = false;
        float drift = 0;

        const float ControllerSensitivity = 1.75f;

        Vector3 boardDirection = Vector3.Zero;

        protected override Vector3[] BoundingBox => new Vector3[]
        {
            new Vector3(-0.5f,0,-0.5f),
            new Vector3(-0.5f,0,0.5f),
            new Vector3(0.5f,0,-0.5f),
            new Vector3(0.5f,0,0.5f)
        };

        public Player(Model model, Vector3 pos)
        {
			position = pos;
            _model = model;
            _scale = 1 / 24f;
            _rotation = 0;

            playerMovementListener = new PlayerMovementListener();
            playerMovementListener.Add(this);

            gameStateListener = new GameStateListener();
            gameStateListener.Add(this);

        }

        public void SetCamera(Entity camera){
            mCamera = camera;
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
            direction += boardDirection/ -15f;
            var speed = 0.018f;
            var speedVector = new Vector3(speed * ControllerSensitivity, 0, speed);
            Move(direction * speedVector * deltaTime * isMoving);
            playerMovementListener.PlayerMovedX(position.X);
            playerMovementListener.PlayerMovedZ(position.Z);

            if(position.Y < WorldGenerator.PathHeight)
            {
                gameStateListener.LostGame();
            }

            base.Update(deltaTime);

        }

        public override void Draw(bool alpha)
        {
            if (!alpha)
            {
                return;
            }
            var world = Matrix.CreateTranslation(position +  new Vector3(0,0.1f,0.5f)); // to fix player y-axis and z-axis
            DrawModel(_model, world, VoxeLand.View, VoxeLand.Projection, boardDirection.X/50f, _scale);
            base.Draw(alpha);
        }

        private void RotateHead(Model model){

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
                    effect.AmbientLightColor = new Vector3(1,1,1);


                    effect.World = modelTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationZ(rotation) * Matrix.CreateScale(scale) * world;//where 'mesh' iterates through all objects in the model

                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }

        public void OnConnectionEvent(bool status)
        {
            //throw new NotImplementedException();
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

                if (iteration == 0)// first byte must be 255
                {
                    if (value == 255)
                    {
                        iteration = 1;
                        b_0 = 0;
                        b_1 = 0;
                    }
                    return;
                }

                if (iteration == 1 || iteration == 2)// 1 byte
                {
                    b_0 = (byte)(b_0 | value);
                    iteration++;
                    return;
                }

                if (iteration == 3 || iteration == 4)// 2 byte
                {
                    b_1 = (byte)(b_1 | value);
                    iteration++;

                    if (iteration == 5)
                    {
                        iteration = 0;
                    }
                    else
                    {
                        return;
                    }

                }


                byte[] bytes = { b_1, b_0 };

                float axis = BitConverter.ToInt16(bytes, 0) / 100f;

                if (!started)
                {
                    drift = axis;
                    started = true;
                }

                axis -= drift;

                boardDirection = new Vector3(axis, 0, 0);

            }
            catch (FormatException)
            {
                //might occur sometimes if serial had problems reading
            }

        }

        public void LostGame()
        {
            isMoving = 0;
        }

        public void StartedGame()
        {
            isMoving = 1;
        }

        public void LoadedGame()
        {
            
        }
    }
}
