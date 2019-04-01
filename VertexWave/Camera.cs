using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VertexWave
{
    
    public class Camera : Entity
    {
        public Matrix view {get; protected set;}
        public Matrix projection { get; protected set; }

        Vector3 cameraDirection;
        Vector3 cameraUp;

        public float Distance = 500;


        int midX = GraphicsDeviceManager.DefaultBackBufferWidth / 2;
        int midY = GraphicsDeviceManager.DefaultBackBufferHeight / 2;

        //defines speed of camera movement
        float speed = 0.1F;

        MouseState prevMouseState;

        PlayerMovementListener playerMovementListener;

        Entity toFollow;

        float rotation = 0;

        float distanceToPlayer = 7;
        
        float MaxDistance = 25;

        float height = 0;

        const float MaxHeigth = 25;
        const float MinHeigth = 0;


        int oldMouseX = -1;
        int oldMouseY = -1;

        public Camera(Vector3 pos, Vector3 target, Vector3 up)
        {
            // TODO: Construct any child components here

            // Build camera view matrix
            position = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();

            //projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)VoxeLand.game.Window.ClientBounds.Width / (float)VoxeLand.game.Window.ClientBounds.Height, 1f, 500f);

            //projection = Matrix.CreateOrthographic((float)VoxeLand.game.Window.ClientBounds.Width/40f , (float)VoxeLand.game.Window.ClientBounds.Height/40f, 1.0f, 100.0f);

            playerMovementListener = new PlayerMovementListener();
        }

        public Camera(Entity follow)
        {
            // TODO: Construct any child components here
            var target = new Vector3(0, 0, 0);
            var up = Vector3.UnitZ;
            position = follow.position;
            cameraDirection = target - position;
            cameraDirection.Normalize();
            cameraUp = up;
            // Build camera view matrix
            toFollow = follow;
            CreateLookAt();

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)VoxeLand.game.Window.ClientBounds.Width / (float)VoxeLand.game.Window.ClientBounds.Height, 0.1f, Distance);

            //projection = Matrix.CreateOrthographic((float)VoxeLand.game.Window.ClientBounds.Width/40f , (float)VoxeLand.game.Window.ClientBounds.Height/40f, 1.0f, 1000.0f);

            playerMovementListener = new PlayerMovementListener();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Ready()
        {
            // TODO: Add your initialization code here

            // Set mouse position and do initial get state
            Mouse.SetPosition(VoxeLand.game.Window.ClientBounds.Width / 2, VoxeLand.game.Window.ClientBounds.Height / 2); 
            
            prevMouseState = Mouse.GetState();
            
            base.Ready();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(float deltaTime)
        {
            // TODO: Add your update code here


            // Move forward and backward



            position= new Vector3();
            //cameraMovePosition.X = cameraDirection.X;
            //cameraMovePosition.Z = cameraDirection.Z;

            position.X = toFollow.position.X;
            position.Z = toFollow.position.Z;
            position.Y = toFollow.position.Y + height;

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

            if(mouseState.RightButton == ButtonState.Pressed){
                if(oldMouseX == -1 && oldMouseY == -1){
                    oldMouseX = mousePosition.X;
                    oldMouseY = mousePosition.Y;
                }else{
                    var deltaX = oldMouseX - mousePosition.X;
                    var deltaY = oldMouseY - mousePosition.Y;
                    oldMouseX = mousePosition.X;
                    oldMouseY = mousePosition.Y;

                    rotation += deltaX * 0.005f;

                    height -= deltaY * 0.1f;

                    if(height > MaxHeigth){
                        height = MaxHeigth;
                    }
                    if(height < MinHeigth){
                        height = MinHeigth;
                    }
                }
            }

            if (mouseState.RightButton == ButtonState.Released){
                oldMouseX = -1;
                oldMouseY = -1;
            }

            var newDistance = distanceToPlayer;

            if (height < 0)
            {
                newDistance = (float) Math.Abs(1f/(height+0.01f));
            }

                position.X = toFollow.position.X + (float) Math.Sin(rotation) * newDistance;
                position.Z = toFollow.position.Z + (float) Math.Cos(rotation) * newDistance;


                cameraDirection = toFollow.position - position;


            /*
            if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Move(Vector3.Normalize(cameraMovePosition)* speed);
                playerMovementListener.PlayerMovedX(position.X);
                playerMovementListener.PlayerMovedZ(position.Z);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Move(-Vector3.Normalize(cameraMovePosition) * speed);
                playerMovementListener.PlayerMovedX(position.X);
                playerMovementListener.PlayerMovedZ(position.Z);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Move(Vector3.Cross(cameraUp, Vector3.Normalize(cameraDirection)) * speed);
                playerMovementListener.PlayerMovedX(position.X);
                playerMovementListener.PlayerMovedZ(position.Z);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Move(-Vector3.Cross(cameraUp, Vector3.Normalize(cameraDirection)) * speed);
                playerMovementListener.PlayerMovedX(position.X);
                playerMovementListener.PlayerMovedZ(position.Z);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Jump(0.5f);
            }

            var x = Mouse.GetState().X;
            var y = Mouse.GetState().Y;

            if(x < 1){
                x = 1;
            }
            if (y < 1)
            {
                y = 1;
            }

            if (x > VoxeLand.game.Window.ClientBounds.Width-1)
            {
                x = VoxeLand.game.Window.ClientBounds.Width - 1;
            }

            if (y > VoxeLand.game.Window.ClientBounds.Height - 1)
            {
                y = VoxeLand.game.Window.ClientBounds.Height - 1;
            }

            var a = (-MathHelper.PiOver4 / 150) * (x - midX);
            var b = (MathHelper.PiOver4 / 100) * (y - midY);

            // Rotation in the world
            cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(cameraUp, a));

     
            cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection), b));

            cameraUp = Vector3.Up;

            // Reset prevMouseState
            //prevMouseState = Mouse.GetState( );

            Mouse.SetPosition(midX,midY);

            */
            
            /*

            var moved = false;

            while (World.IsCollisionAt(position.X, position.Y, position.Z))
            {
                moved = true;
                distanceToPlayer -= 0.25f;
                if (distanceToPlayer < 0.2f)
                {
                    distanceToPlayer = 0.2f;
                    break;
                }
                position.X = toFollow.position.X + (float) Math.Sin(rotation) * distanceToPlayer;
                position.Z = toFollow.position.Z + (float) Math.Cos(rotation) * distanceToPlayer;
            }
            
            
            if (distanceToPlayer < MaxDistance && !moved)
            {
                distanceToPlayer += 0.25f;
                position.X = toFollow.position.X + (float) Math.Sin(rotation) * distanceToPlayer;
                position.Z = toFollow.position.Z + (float) Math.Cos(rotation) * distanceToPlayer;
                if (World.IsCollisionAt(position.X, position.Y, position.Z))
                {
                    distanceToPlayer -= 0.25f;
                    position.X = toFollow.position.X + (float) Math.Sin(rotation) * distanceToPlayer;
                    position.Z = toFollow.position.Z + (float) Math.Cos(rotation) * distanceToPlayer;
                }
            }
            
            */

            CreateLookAt();

            //base.Update(deltaTime);
        }

        private void CreateLookAt()
        {
            view =  Matrix.CreateLookAt(position, position + cameraDirection, Vector3.Up);
        }

    }
}