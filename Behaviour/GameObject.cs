using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Tamale.Rendering;
using Texture = Tamale.Rendering.Texture;


namespace Tamale.Behaviour
{
    internal class GameObject
    {
        public Vector3D<float> Position = new Vector3D<float>(0, 0, 0);
        public Vector3D<float> Rotation = new Vector3D<float>(0, 0, 0);

        public Model Model { get; set; }
        public Texture Texture { get; set; }

        public Vector4D<float> lightingColor = new Vector4D<float>(1, 1, 1, 1);
        public float ambientStrength = 1f;

        public List<Component> components = new List<Component>();

        public GameObject(Vector3D<float> position, Vector3D<float> rotation, Model model, Texture texture)
        {
            Position = position;
            Rotation = rotation;
            Model = model;
            Texture = texture;
        }

        public unsafe void Render()
        {
            Matrix4X4<float> rot = Matrix4X4.CreateFromYawPitchRoll((float)Math.PI * (Rotation.Y/180), (float)Math.PI * (Rotation.X / 180), (float)Math.PI * (Rotation.Z / 180));
            Matrix4X4<float> modelMat = rot * Matrix4X4.CreateTranslation(Position);

            Matrix4X4<float> viewMat = Matrix4X4.CreateTranslation(new Vector3D<float>(-SharedData.cameraPos.X, -SharedData.cameraPos.Y, -SharedData.cameraPos.Z)) * Matrix4X4.CreateFromYawPitchRoll((float)-Math.PI * (SharedData.cameraRot.Y / 180), (float)-Math.PI * (SharedData.cameraRot.X / 180), (float)-Math.PI * (SharedData.cameraRot.Z / 180));
            SharedData.viewMat = viewMat;

            Program.gl.UniformMatrix4(SharedData.uModel, 1, false, (float*)&modelMat);
            Program.gl.UniformMatrix4(SharedData.uView, 1, false, (float*)&viewMat);

            System.Numerics.Vector4 lightingColorSys = lightingColor.ToSystem();
            Program.gl.Uniform4(SharedData.uAmbientColor, ref lightingColorSys);

            Program.gl.Uniform1(SharedData.uAmbientStrength, ambientStrength);

            Program.gl.ActiveTexture(TextureUnit.Texture0);
            Program.gl.BindTexture(TextureTarget.Texture2D, Texture.ID);

            Program.gl.BindVertexArray(Model.VAO);
            Program.gl.DrawArrays(PrimitiveType.Triangles, 0, Model.vertexCount);
        }

        public void UpdateInternal(double delta)
        {
            foreach (Component component in components)
            {
                if (component.gameObject != this)
                    component.gameObject = this;
                component.Update(delta);
            }

            Update(delta);
        }
        public virtual void Update(double delta)
        {
        }
    }
}
