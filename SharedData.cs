using Silk.NET.Maths;
using Tamale.Behaviour;

namespace Tamale
{
    internal static class SharedData
    {
        public static int uModel;
        public static int uView;
        public static int uProjection;
        public static int uTexture;
        public static int uAmbientColor;
        public static int uAmbientStrength;
        public static Matrix4X4<float> viewMat;
        public static Matrix4X4<float> projectionMat;
        public static Vector3D<float> cameraPos = new Vector3D<float>(0, 0, 3);
        public static Vector3D<float> cameraRot = new Vector3D<float>(0, 0, 0);


        public static List<GameObject> gameObjects = new List<GameObject>();
    }
}
