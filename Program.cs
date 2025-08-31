using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.OpenAL;
using System.Drawing;
using System.Reflection;
using Tamale.Behaviour;
using Tamale.Behaviour.Collision;
using Tamale.Rendering;

using Texture = Tamale.Rendering.Texture;
using Tamale.Audio;
using Tamale.Testing;

namespace Tamale
{
    internal unsafe class Program
    {

        private static IWindow window;
        public static GL gl;
        public static uint program;

        public static IInputContext input;

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

        private static void OnLoad()
        {
            // Initialize APIs
            gl = window.CreateOpenGL();
            input = window.CreateInput();
            AudioVars.al = AL.GetApi();
            AudioVars.alc = ALContext.GetApi();

            // Initialize OpenAL
            AudioVars.device = AudioVars.alc.OpenDevice(null);
            AudioVars.audioContext = AudioVars.alc.CreateContext(AudioVars.device, null);
            AudioVars.alc.MakeContextCurrent(AudioVars.audioContext);

            // Set up default sounds source
            AudioVars.source = AudioVars.al.GenSource();
            AudioVars.al.SetSourceProperty(AudioVars.source, SourceBoolean.Looping, false);

            // Initialize input
            foreach (IKeyboard keyboard in input.Keyboards)
            {
                keyboard.KeyDown += Input.KeyDown;
                keyboard.KeyUp += Input.KeyUp;
            }

            // Enable depth testing
            gl.Enable(EnableCap.DepthTest);

            // Compile shaders
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


            // initialize OpenGL program
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

            // Get uniform locations
            SharedData.uModel = gl.GetUniformLocation(program, "uModel");
            SharedData.uView = gl.GetUniformLocation(program, "uView");
            SharedData.uProjection = gl.GetUniformLocation(program, "uProjection");
            SharedData.uTexture = gl.GetUniformLocation(program, "uTexture");
            SharedData.uAmbientColor = gl.GetUniformLocation(program, "uAmbientColor");
            SharedData.uAmbientStrength = gl.GetUniformLocation(program, "uAmbientStrength");

            gl.UseProgram(program);

            // Set default view and projection matrices
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

            // Initialize game
            StartGame();
        }

        private static void StartGame()
        {

            // Try to load TamaleGame.dll and call its OnGameLoad method if it exists
            Assembly asm;

            try
            {
                asm = Assembly.LoadFile(Path.GetFullPath("./TamaleGame.dll"));
            } catch {
                Console.WriteLine("No TamaleGame assembly found, loading default scene.");
                asm = null;
            }

            Type type = null;

            if (asm != null)
            {
                type = asm.GetType("TamaleGame.GameLoad");
            }

            if (type != null)
            {
                MethodInfo method = type.GetMethod("OnGameLoad", BindingFlags.Public | BindingFlags.Static);
                if (method != null)
                {
                    object obj = Activator.CreateInstance(type);
                    method.Invoke(obj, null);
                    return;
                }
            }

            // Fallthrough to testing scene
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
            Sound sound = new Sound("Assets/test.wav");
            //sound.Play();

            Model model = new Model(vertices);
            Texture texture1 = new Texture("./Assets/texture1.png");

            GameObject gameObject1 = new TestGameObject(new Vector3D<float>(-1.5f, 0, 0), new Vector3D<float>(0, 0, 0), model, texture1);
            GameObject gameObject2 = new GameObject(new Vector3D<float>(0.1f, 0, 0), new Vector3D<float>(0, 0, 0), model, texture1);

            Component audioSource = new AudioSource();
            Component playSound = new PlaySoundEverySecond(sound, (AudioSource)audioSource);
            Component spin = new Spin();
            Component box1 = new AABox();
            Component box2 = new AABox();

            gameObject1.components.Add(spin);
            gameObject1.components.Add(box1);
            gameObject1.components.Add(audioSource);
            gameObject1.components.Add(playSound);

            gameObject2.components.Add(box2);

            SharedData.gameObjects.Add(gameObject1);
            SharedData.gameObjects.Add(gameObject2);
        }

        private static unsafe void OnRender(double delta)
        {
            // Unbind vertex array and clear screen
            gl.BindVertexArray(0);
            gl.ClearColor(Color.Violet);
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Render all game objects
            foreach (var gameObject in SharedData.gameObjects)
            {
                gameObject.Render();
            }

            window.SwapBuffers();
        }

        private static void OnUpdate(double delta)
        {
            // Set audio listener position to the camera position
            AudioVars.al.SetListenerProperty(ListenerVector3.Position, SharedData.cameraPos.ToSystem());
            AudioVars.al.SetSourceProperty(AudioVars.source, SourceVector3.Position, SharedData.cameraPos.ToSystem());

            Quaternion<float> rot = Quaternion<float>.CreateFromYawPitchRoll((float)Math.PI * (SharedData.cameraRot.Y / 180), (float)Math.PI * (SharedData.cameraRot.X / 180), (float)Math.PI * (SharedData.cameraRot.Z / 180));
            Vector3D<float> forward = Vector3D.Transform(-Vector3D<float>.UnitZ, rot);
            Vector3D<float> up = Vector3D.Transform(Vector3D<float>.UnitY, rot);
            float[] atandup =
            [
                forward.X, forward.Y, forward.Z,
                up.X, up.Y, up.Z
            ];

            fixed (float* ptr = atandup)
                AudioVars.al.SetListenerProperty(ListenerFloatArray.Orientation, ptr);


            // Update logic can be added here if needed
            foreach (var gameObject in SharedData.gameObjects)
            {
                gameObject.UpdateInternal(delta);
            }

            // Step physics world
            SharedData.world.Step((float)delta);
        }
    }
}
