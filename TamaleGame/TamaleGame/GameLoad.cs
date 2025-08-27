using Silk.NET.Maths;
using Tamale;
using Tamale.Behaviour;
using Tamale.Rendering;

namespace TamaleGame
{
    public class GameLoad
    {
        public static void OnGameLoad()
        {
            Console.WriteLine("Game Loaded!");

            float[] skullverts = Model.LoadCOF("./Assets/skull.cof");

            Model skullModel = new Model(skullverts);
            Texture skullTexture = new Texture("./Assets/skull.jpg");
            GameObject gameObject3 = new SpinWhenPressingA(new Vector3D<float>(0, 0, 0), new Vector3D<float>(0, 0, 0), skullModel, skullTexture);
            SharedData.gameObjects.Add(gameObject3);
        }
    }
}
