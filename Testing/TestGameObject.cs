using Jitter2.Collision;
using Jitter2.Collision.Shapes;
using Jitter2.Dynamics;
using Jitter2.LinearMath;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamale.Behaviour;
using Tamale.Behaviour.Collision;
using Tamale.Rendering;
using static Jitter2.Collision.DynamicTree;

namespace Tamale.Testing
{
    internal class TestGameObject : GameObject
    {
        public TestGameObject(Vector3D<float> position, Vector3D<float> rotation, Model model, Texture texture) : base(position, rotation, model, texture)
        {
        }

        public override void Update(double delta)
        {
            AABox box = (AABox)GetComponent<AABox>();
            Spin spin = (Spin)GetComponent<Spin>();
            if (box.ForwardCast(Math.QuaternionToDirection(Vector3D<float>.UnitX, Math.EulerToQuaternion(Rotation)), 0.5f * (float)delta, out GameObject hit) && spin != null)
            {
                Console.WriteLine($"Ran into somthing with position {hit.Position}");
                spin.Destroy();
            }
        }

        public override void CollideStart(Arbiter arb)
        {
            Console.WriteLine("Collsion with AABB");
        }

        public override void CollideEnd(Arbiter arb)
        {
            Console.WriteLine("Collsion ended with AABB");
        }
    }
}
