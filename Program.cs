using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Drawing;
using Tamale.Behaviour;
using Tamale.Behaviour.Collision;
using Tamale.Rendering;
using Texture = Tamale.Rendering.Texture;

namespace Tamale
{
    internal class Program
    {

        private static IWindow window;
        public static GL gl;
        public static uint program;

        static void Main(string[] args)
        {
            WindowOptions options = WindowOptions.Default with
            {
                Size = new Vector2D<int>(800, 600),
                Title = "TamaleEngine"
            };

            window = Window.Create(options);
            window.Load += OnLoad;
            window.Render += OnRender;
            window.Update += OnUpdate;
            window.ShouldSwapAutomatically = false;
            window.Run();
        }

        private static unsafe void OnLoad()
        {
            gl = window.CreateOpenGL();

            gl.Enable(EnableCap.DepthTest);

            const string vertexCode = @"
            #version 330 core

            layout (location = 0) in vec3 aPosition;
            layout (location = 1) in vec2 aTextureCoord;

            uniform mat4 uModel;
            uniform mat4 uView;
            uniform mat4 uProjection;

            out vec2 frag_texCoords;

            void main()
            {
                gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0); //uProjection * uView * uModel *

                frag_texCoords = aTextureCoord;
            }";

            const string fragmentCode = @"
            #version 330 core

            in vec2 frag_texCoords;

            out vec4 out_color;

            uniform sampler2D uTexture;
            uniform vec4 uAmbientColor;
            uniform float uAmbientStrength;

            void main()
            {
                out_color = uAmbientStrength * uAmbientColor * texture(uTexture, frag_texCoords);
            }";

            uint vertexShader = gl.CreateShader(ShaderType.VertexShader);
            gl.ShaderSource(vertexShader, vertexCode);

            gl.CompileShader(vertexShader);

            gl.GetShader(vertexShader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + gl.GetShaderInfoLog(vertexShader));

            uint fragmentShader = gl.CreateShader(ShaderType.FragmentShader);
            gl.ShaderSource(fragmentShader, fragmentCode);

            gl.CompileShader(fragmentShader);

            gl.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out int fStatus);
            if (fStatus != (int)GLEnum.True)
                throw new Exception("Fragment shader failed to compile: " + gl.GetShaderInfoLog(fragmentShader));

            program = gl.CreateProgram();

            gl.AttachShader(program, vertexShader);
            gl.AttachShader(program, fragmentShader);

            gl.LinkProgram(program);

            gl.GetProgram(program, ProgramPropertyARB.LinkStatus, out int lStatus);
            if (lStatus != (int)GLEnum.True)
                throw new Exception("Program failed to link: " + gl.GetProgramInfoLog(program));

            gl.DetachShader(program, vertexShader);
            gl.DetachShader(program, fragmentShader);
            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);

            SharedData.uModel = gl.GetUniformLocation(program, "uModel");
            SharedData.uView = gl.GetUniformLocation(program, "uView");
            SharedData.uProjection = gl.GetUniformLocation(program, "uProjection");
            SharedData.uTexture = gl.GetUniformLocation(program, "uTexture");
            SharedData.uAmbientColor = gl.GetUniformLocation(program, "uAmbientColor");
            SharedData.uAmbientStrength = gl.GetUniformLocation(program, "uAmbientStrength");

            gl.UseProgram(program);

            SharedData.viewMat = Matrix4X4.CreateTranslation(new Vector3D<float>(0, 0, -3f));

            SharedData.projectionMat = Matrix4X4.CreatePerspectiveFieldOfView(
                MathF.PI / 4, // 45 degrees
                window.Size.X / (float)window.Size.Y, // Aspect ratio
                0.1f, // Near plane
                100.0f // Far plane
            );

            Matrix4X4<float> viewMat = SharedData.viewMat;
            Matrix4X4<float> projectionMat = SharedData.projectionMat;

            gl.UniformMatrix4(SharedData.uView, 1, false, (float*)&viewMat);
            gl.UniformMatrix4(SharedData.uProjection, 1, false, (float*)&projectionMat);
            gl.Uniform1(SharedData.uTexture, 0);

            StartGame();
        }

        private static void StartGame()
        {
            float[] vertices =
            {
                -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
                 0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
                -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

                -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

                -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
            };

            float[] skullverts = Model.LoadCOF("./Assets/skull.cof");

            Model model = new Model(vertices);
            Model skullModel = new Model(skullverts);
            Texture texture1 = new Texture("./Assets/texture1.png");
            Texture skullTexture = new Texture("./Assets/skull.jpg");
            GameObject gameObject1 = new TestGameObject(new Vector3D<float>(-1.5f, 0, 0), new Vector3D<float>(0, 0, 0), model, texture1);
            GameObject gameObject2 = new GameObject(new Vector3D<float>(0, 0, 0), new Vector3D<float>(0, 0, 0), model, skullTexture);
            Component spin = new Spin();
            Component box1 = new AABox();
            Component box2 = new AABox();
            gameObject1.components.Add(spin);
            gameObject1.components.Add(box1);
            gameObject2.components.Add(box2);
            SharedData.gameObjects.Add(gameObject1);
            SharedData.gameObjects.Add(gameObject2);
        }

        private static unsafe void OnRender(double delta)
        {
            gl.BindVertexArray(0);
            gl.ClearColor(Color.Violet);
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (var gameObject in SharedData.gameObjects)
            {
                gameObject.Render();
            }

            window.SwapBuffers();
        }

        private static void OnUpdate(double delta)
        {
            // Update logic can be added here if needed
            foreach (var gameObject in SharedData.gameObjects)
            {
                gameObject.UpdateInternal(delta);
            }

            SharedData.world.Step((float)delta);
        }
    }
}
