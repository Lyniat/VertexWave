using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertexWave
{
    class Logo : Entity
    {

        Entity mCamera;


        float _rotation;
        float _scale;
        Model _model;

        public override bool Static { get => true; }


        public float Rotation { get => _rotation; }
        public float Scale { get => _scale; }
        public Model Model { get => _model; }

        public Logo(Model model, Vector3 pos)
        {
            position = pos;
            _model = model;
            _scale = 1;
            _rotation = 0;

        }

        public void SetCamera(Entity camera)
        {
            mCamera = camera;
        }


        public override void Draw(bool alpha)
        {
            if (!alpha)
            {
                return;
            }
            var world = Matrix.CreateTranslation(position);
            DrawModel(_model, world, VoxeLand.View, VoxeLand.Projection, 0, _scale);
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

                    effect.World = modelTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationZ(rotation) * Matrix.CreateScale(scale) * world;//where 'mesh' iterates through all objects in the model

                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }
    }
}
