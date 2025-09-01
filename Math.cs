using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamale
{
    public static class Math
    {
        // Converts degrees to radians
        public static float DegToRad(float degrees)
        {
            return (float)System.Math.PI * (degrees / 180);
        }

        // Converts radians to degrees
        public static float RadToDeg(float radians)
        {
            return 180 * (radians / (float)System.Math.PI);
        }

        // Convert Euler angles to quaternion
        public static Quaternion<float> EulerToQuaternion(Vector3D<float> euler)
        {
            return Quaternion<float>.CreateFromYawPitchRoll(DegToRad(SharedData.cameraRot.Y), DegToRad(SharedData.cameraRot.X), DegToRad(SharedData.cameraRot.Z));
        }

        public static Vector3D<float> QuaternionToDirection(Vector3D<float> dir, Quaternion<float> rot)
        {
            return Vector3D.Transform(dir, rot);
        }
    }
}
