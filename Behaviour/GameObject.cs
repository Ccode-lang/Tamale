using Jitter2.Dynamics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Tamale.Rendering;
using Texture = Tamale.Rendering.Texture;


namespace Tamale.Behaviour
{
    // Object in world which can run logic and have components attached to it. Can only have one model attatched to it.
    public class GameObject
    {
        // Transform properties
        public Vector3D<float> Position = new Vector3D<float>(0, 0, 0);
        public Vector3D<float> Rotation = new Vector3D<float>(0, 0, 0);
        public Vector3D<float> Scale = new Vector3D<float>(1, 1, 1);

        // Rendering properties
        public Model Model { get; set; }
        public Texture Texture { get; set; }

        public Vector4D<float> lightingColor = new Vector4D<float>(1, 1, 1, 1);
        public float ambientStrength = 1f;

        public bool render = true;

        // List of components attached to this GameObject
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
            // Don't render if not rendering this
            if (!render) return;

            // Set up model matrix
            Matrix4X4<float> rot = Matrix4X4.CreateFromYawPitchRoll(Math.DegToRad(Rotation.Y), Math.DegToRad(Rotation.X), Math.DegToRad(Rotation.Z));
            Matrix4X4<float> scale = Matrix4X4.CreateScale(Scale);
            Matrix4X4<float> modelMat = scale * rot * Matrix4X4.CreateTranslation(Position);

            // Set up view matrix
            Matrix4X4<float> viewMat = Matrix4X4.CreateTranslation(new Vector3D<float>(-SharedData.cameraPos.X, -SharedData.cameraPos.Y, -SharedData.cameraPos.Z)) * Matrix4X4.CreateFromYawPitchRoll(Math.DegToRad(-SharedData.cameraRot.Y), Math.DegToRad(-SharedData.cameraRot.X), Math.DegToRad(-SharedData.cameraRot.Z));
            SharedData.viewMat = viewMat;

            // Send matrices to shader
            Program.gl.UniformMatrix4(SharedData.uModel, 1, false, (float*)&modelMat);
            Program.gl.UniformMatrix4(SharedData.uView, 1, false, (float*)&viewMat);

            // Send lighting data to shader
            System.Numerics.Vector4 lightingColorSys = lightingColor.ToSystem();
            Program.gl.Uniform4(SharedData.uAmbientColor, ref lightingColorSys);

            Program.gl.Uniform1(SharedData.uAmbientStrength, ambientStrength);

            // Set the active texture and bind this GammeObject's texture
            Program.gl.ActiveTexture(TextureUnit.Texture0);
            Program.gl.BindTexture(TextureTarget.Texture2D, Texture.ID);

            // Draw the model
            Program.gl.BindVertexArray(Model.VAO);
            Program.gl.DrawArrays(PrimitiveType.Triangles, 0, Model.vertexCount);
        }

        // Find a reference to a Component of type T attached to this GameObject
        public Component GetComponent<T>() where T : Component
        {
            return components.OfType<T>().FirstOrDefault();
        }

        // Update all components attached to this GameObject and call this game object's Update
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

        // Called every frame by UpdateInternal
        public virtual void Update(double delta)
        {
        }

        // Called when a collision starts with this GameObject
        public virtual void CollideStart(Arbiter arb)
        {
        }

        // Called when a collision ends with this GameObject
        public virtual void CollideEnd(Arbiter arb)
        {
        }

        // Called when this GameObject is being destroyed. Used for cleanup.
        public virtual void Destroy()
        {
            foreach (Component component in components)
            {
                component.Destroy();
            }
        }
    }
}
