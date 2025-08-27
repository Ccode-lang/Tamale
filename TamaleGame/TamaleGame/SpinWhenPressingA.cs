using Silk.NET.Input;
using Silk.NET.Maths;
using Tamale;
using Tamale.Behaviour;
using Tamale.Rendering;

namespace TamaleGame
{
    internal class SpinWhenPressingA : GameObject
    {
        public SpinWhenPressingA(Vector3D<float> position, Vector3D<float> rotation, Model model, Texture texture) : base(position, rotation, model, texture)
        {
        }

        public override void Update(double delta)
        {
            Console.WriteLine(Rotation.Y);
            if (Input.GetKey(Key.A))
            {
                Rotation += new Vector3D<float>(0, 60 * (float)delta, 0);
            }
        }
    }
}
