using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamale;
using Tamale.Behaviour;
using Tamale.Behaviour.Collision;
using Tamale.Rendering;
using Math = Tamale.Math;

namespace TamaleGame
{
    internal class Player : GameObject
    {
        AABox collider;
        public Player(Vector3D<float> position, Vector3D<float> rotation, Model model, Texture texture) : base(position, rotation, model, texture)
        {
            render = false;
            collider = new AABox();
            collider.Scale = new Vector3D<float>(1, 2, 1);
            components.Add(collider);
        }

        public override void Update(double delta)
        {
            if (!collider.ForwardCast(-Vector3D<float>.UnitY, 2 * (float)delta, out GameObject hit))
            {
                Position += -Vector3D<float>.UnitY * 2 * (float)delta;
            }

            if (Input.GetKey(Silk.NET.Input.Key.Right))
            {
                Rotation += new Vector3D<float>(0, -90 * (float)delta, 0);
            }
            if (Input.GetKey(Silk.NET.Input.Key.Left))
            {
                Rotation += new Vector3D<float>(0, 90 * (float)delta, 0);
            }

            Vector3D<float> forward = Math.QuaternionToDirection(-Vector3D<float>.UnitZ, Math.EulerToQuaternion(Rotation));
            Console.WriteLine(forward);
            Console.WriteLine(Quaternion<float>.Normalize(Math.EulerToQuaternion(Rotation)));
            if (Input.GetKey(Silk.NET.Input.Key.W))
            {
                if (!collider.ForwardCast(forward, 2 * (float)delta, out GameObject hit1))
                    Position += forward * 2 * (float)delta;
            }

            SharedData.cameraPos = Position;
            SharedData.cameraRot = Rotation;
        }
    }
}
