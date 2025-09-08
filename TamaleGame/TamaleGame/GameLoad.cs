using Silk.NET.Maths;
using Tamale;
using Tamale.Behaviour;
using Tamale.Behaviour.Collision;
using Tamale.Rendering;

namespace TamaleGame
{
    public class GameLoad
    {
        public static void OnGameLoad()
        {
            Console.WriteLine("Game Loaded!");

            Model cube = new Model(Model.LoadCOF("./Assets/cube.cof"));
            Texture skullTexture = new Texture("./Assets/texture1.png");

            // Scale the cube to 1 unit
            cube.ModelScale = new Vector3D<float>(0.5f, 0.5f, 0.5f);


            GameObject.MakeAABoxWithModel(new Vector3D<float>(0, 0, 0), cube, skullTexture, new Vector3D<float>(20, 0.1f, 20), true).tags.Add("floor");
            GameObject.MakeAABoxWithModel(new Vector3D<float>(0, 0, -3), cube, skullTexture, new Vector3D<float>(10, 10, 0.1f), true).tags.Add("wall");

            Player player = new Player(new Vector3D<float>(0, 5, 0), new Vector3D<float>(0, 0, 0), null, null);
            SharedData.gameObjects.Add(player);
        }
    }
}
