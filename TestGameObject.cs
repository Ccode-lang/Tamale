using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamale.Behaviour;
using Tamale.Behaviour.Collision;
using Tamale.Rendering;

namespace Tamale
{
    internal class TestGameObject : GameObject
    {
        public TestGameObject(Vector3D<float> position, Vector3D<float> rotation, Model model, Texture texture) : base(position, rotation, model, texture)
        {
        }

        AABox box = null;

        public override void Update(double delta)
        {
            if (box == null) box = (AABox)GetComponent<AABox>();
            Console.WriteLine(box.Collided);
        }
    }
}
