using Jitter2;
using Jitter2.Collision;
using Jitter2.Collision.Shapes;
using Jitter2.Dynamics;
using Jitter2.LinearMath;
using Silk.NET.Maths;

namespace Tamale.Behaviour.Collision
{
    // Axis Aligned Box for collision detection. Is kinematic.
    public class AABox : Component
    {
        public Vector3D<float> Scale = new Vector3D<float>(1, 1, 1);

        public bool IsStatic = false;
        public RigidBody body;

        private bool firstUpdate = true;

        public AABox()
        {
            // Add to the list of AABoxes in the scene.
            SharedData.AABoxes.Add(this);

            // Create the rigidbody and add a box shape to it.
            body = SharedData.world.CreateRigidBody();
            body.AddShape(new BoxShape(Scale.X, Scale.Y, Scale.Z));
            body.IsStatic = IsStatic;
            body.SetMassInertia(JMatrix.Zero, 1e-3f, setAsInverse: true);
            body.Damping = (linear: 0.0f, angular: 0.0f);
        }


        // Check if a point is inside an AABox. I don't even know if this works but it might be useful in the future.
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
            // Set collider to the position of the game object.
            body.Position = new JVector(gameObject.Position.X, gameObject.Position.Y, gameObject.Position.Z);

            // Why tf is this so well hidden. I looked for hours and ended up finding this because I read the code of Jitter itself.
            BoxShape box = (BoxShape)body.Shapes[0];
            box.Size = new JVector(Scale.X, Scale.Y, Scale.Z);

            // Set stuff up the first time this is updated.
            if (firstUpdate)
            {
                body.BeginCollide += gameObject.CollideStart;
                body.EndCollide += gameObject.CollideEnd;
                firstUpdate = false;

                IDynamicTreeProxy proxy = body.Shapes[0];
                SharedData.ProxyPtrToObjectTable.Add(proxy.NodePtr, gameObject);
            }
        }

        public override void Destroy()
        {
            // Clean up the jitter objects and remove this from the list of AABoxes in the scene.
            base.Destroy();
            body.RemoveShape(body.Shapes[0]);
            SharedData.world.Remove(body);
            SharedData.AABoxes.Remove(this);
            IDynamicTreeProxy proxy = body.Shapes[0];
            SharedData.ProxyPtrToObjectTable.Remove(proxy.NodePtr);
        }
    }
}
