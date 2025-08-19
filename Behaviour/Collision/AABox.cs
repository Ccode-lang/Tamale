using Silk.NET.Maths;

namespace Tamale.Behaviour.Collision
{
    internal class AABox : Component
    {
        public Vector3D<float> Scale = new Vector3D<float>(1, 1, 1);

        public bool Collided = false;

        public AABox()
        {
            SharedData.AABoxes.Add(this);
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
            float xs = Scale.X / 2;
            float ys = Scale.Y / 2;
            float zs = Scale.Z / 2;
            // z+
            Vector3D<float> point1 = new Vector3D<float>(-xs, -ys, zs);
            Vector3D<float> point2 = new Vector3D<float>(xs, -ys, zs);
            Vector3D<float> point3 = new Vector3D<float>(-xs, ys, zs);
            Vector3D<float> point4 = new Vector3D<float>(xs, ys, zs);
            // z-
            Vector3D<float> point5 = new Vector3D<float>(-xs, -ys, -zs);
            Vector3D<float> point6 = new Vector3D<float>(xs, -ys, -zs);
            Vector3D<float> point7 = new Vector3D<float>(-xs, ys, -zs);
            Vector3D<float> point8 = new Vector3D<float>(xs, ys, -zs);

            if (gameObject == null) return;

            foreach (AABox box in SharedData.AABoxes)
            {
                if (box == this) continue;
                if (box.gameObject == null) continue;
                bool collided = PointInAABox(gameObject.Position + point1, box) ||
                                PointInAABox(gameObject.Position + point2, box) ||
                                PointInAABox(gameObject.Position + point3, box) ||
                                PointInAABox(gameObject.Position + point4, box) ||
                                PointInAABox(gameObject.Position + point5, box) ||
                                PointInAABox(gameObject.Position + point6, box) ||
                                PointInAABox(gameObject.Position + point7, box) ||
                                PointInAABox(gameObject.Position + point8, box);
                if (collided)
                {
                    Collided = true;
                    return;
                }
            }
            Collided = false;
        }
    }
}
