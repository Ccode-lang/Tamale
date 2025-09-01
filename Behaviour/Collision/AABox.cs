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

        // Does a box cast a certain distance away in a certain direction. If it hits something, return true and the object hit.
        public bool ForwardCast(Vector3D<float> direction, float distance, out GameObject ObjectHit)
        {
            // Set rigidbody position to target position.
            Vector3D<float> target = gameObject.Position + Vector3D.Normalize(direction) * distance;
            body.Position = new JVector(target.X, target.Y, target.Z);

            // Need this because access levels for local functions are weird.
            GameObject localObjectHit = null;

            // Call callback for each overlap in the dynamic tree.
            SharedData.world.DynamicTree.EnumerateOverlaps(Callback);

            // Local function to be called for each overlap in the dynamic tree.
            void Callback(IDynamicTreeProxy proxy, IDynamicTreeProxy proxy2)
            {
                IDynamicTreeProxy thisproxy = body.Shapes[0];
                if (thisproxy.NodePtr == proxy.NodePtr)
                {
                    SharedData.ProxyPtrToObjectTable.TryGetValue(proxy2.NodePtr, out localObjectHit);
                }
                if (thisproxy.NodePtr == proxy2.NodePtr)
                {
                    SharedData.ProxyPtrToObjectTable.TryGetValue(proxy.NodePtr, out localObjectHit);
                }
            }

            // Reset the position of the rigidbody to the gameobject's position.
            body.Position = new JVector(gameObject.Position.X, gameObject.Position.Y, gameObject.Position.Z);

            // Return stuff
            ObjectHit = localObjectHit;

            return localObjectHit != null;
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
