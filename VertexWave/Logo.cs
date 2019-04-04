using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VertexWave
{
    internal class Logo : Entity
    {
        private Entity _mCamera;

        public Logo(Model model, Vector3 pos)
        {
            position = pos;
            Model = model;
            Scale = 1;
            Rotation = 0;
        }

        public override bool Static => true;


        public float Rotation { get; }
        public float Scale { get; }
        public Model Model { get; }

        public void SetCamera(Entity camera)
        {
            _mCamera = camera;
        }


        public override void Draw(bool alpha)
        {
            if (!alpha) return;
            var world = Matrix.CreateTranslation(position);
            DrawModel(Model, world, VoxeLand.View, VoxeLand.Projection, 0, Scale);
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
                foreach (AlphaTestEffect effect in mesh.Effects)
                {
                    //in the fields declaration 
                    Matrix[] modelTransforms;

                    //in the loadContent method after loading the model
                    modelTransforms = new Matrix[model.Bones.Count];
                    model.CopyAbsoluteBoneTransformsTo(modelTransforms);

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