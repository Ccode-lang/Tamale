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


            GameObject floor = new GameObject(new Vector3D<float>(0, 0, 0), new Vector3D<float>(0, 0, 0), cube, skullTexture);
            floor.Scale = new Vector3D<float>(0.5f, 0.5f, 0.5f) * new Vector3D<float>(20, 0.1f, 20);
            AABox floorCollider = new AABox();
            floorCollider.Scale = floor.Scale * 2;
            floorCollider.IsStatic = true;
            floor.components.Add(floorCollider);
            SharedData.gameObjects.Add(floor);

            Player player = new Player(new Vector3D<float>(0, 5, 0), new Vector3D<float>(0, 0, 0), null, null);
            SharedData.gameObjects.Add(player);
        }
    }
}
