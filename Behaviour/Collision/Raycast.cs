using Jitter2.Collision;
using Jitter2.LinearMath;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Jitter2.Collision.DynamicTree;

namespace Tamale.Behaviour.Collision
{
    public static class Raycast
    {
        public static bool RaycastAll(Vector3D<float> point, Vector3D<float> direction, float range, out GameObject gameObject)
        {
            bool hit = SharedData.world.DynamicTree.RayCast(new JVector(point.X, point.Y, point.Z), new JVector(direction.X, direction.Y, direction.Z), 10, null, FilterSelf, out IDynamicTreeProxy? proxy, out JVector normal, out float lambda);
            gameObject = null;
            if (proxy != null && SharedData.ProxyPtrToObjectTable.ContainsKey(proxy.NodePtr))
            {
                gameObject = SharedData.ProxyPtrToObjectTable[proxy.NodePtr];
            }
            return hit;
        }

        public static bool FilterSelf(RayCastResult result)
        {
            return result.Lambda != 0;
        }
    }
}
