using Jitter2;
using Jitter2.Collision.Shapes;
using Jitter2.Dynamics;
using Jitter2.LinearMath;
using Silk.NET.Maths;

namespace Tamale.Behaviour.Collision
{
    internal class AABox : Component
    {
        public Vector3D<float> Scale = new Vector3D<float>(1, 1, 1);

        public bool IsStatic = false;
        public RigidBody body;

        private bool firstUpdate = true;

        public AABox()
        {
            SharedData.AABoxes.Add(this);
            body = SharedData.world.CreateRigidBody();
            //TODO: make sized boxes work
            body.AddShape(new BoxShape(1));
            body.IsStatic = IsStatic;
            body.SetMassInertia(JMatrix.Zero, 1e-3f, setAsInverse: true);
            body.Damping = (linear: 0.0f, angular: 0.0f);
        }



        public bool PointInAABox(Vector3D<float> point, AABox box)
        {
            return point.X >= box.gameObject.Position.X - box.Scale.X / 2 &&
                   point.X <= box.gameObject.Position.X + box.Scale.X / 2 &&
                   point.Y >= box.gameObject.Position.Y - box.Scale.Y / 2 &&
                   point.Y <= box.gameObject.Position.Y + box.Scale.Y / 2 &&
                   point.Z >= box.gameObject.Position.Z - box.Scale.Z / 2 &&
                   point.Z <= box.gameObject.Position.Z + box.Scale.Z / 2;
        }

        public override void Update(double delta)
        {
            body.Position = new JVector(gameObject.Position.X, gameObject.Position.Y, gameObject.Position.Z);

            if (firstUpdate)
            {
                body.BeginCollide += gameObject.CollideStart;
                body.EndCollide += gameObject.CollideEnd;
                firstUpdate = false;
            }
        }

        public override void Destroy()
        {
            body.RemoveShape(body.Shapes[0]);
            SharedData.world.Remove(body);
        }
    }
}
