using Silk.NET.Maths;
using Tamale.Behaviour;

namespace Tamale
{
    internal class Spin : Component
    {
        public override void Update(double delta)
        {
            gameObject.Rotation += new Vector3D<float>(0, 60 * (float)delta, 0);
            //Position += new Vector3D<float>(0.5f * (float)delta, 0, 0);
            //SharedData.cameraRot += new Vector3D<float>(0, 60 * (float)delta, 0);
            //SharedData.cameraPos += new Vector3D<float>(0.5f * (float)delta, 0, 0);
        }
    }
}
