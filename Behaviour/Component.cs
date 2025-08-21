using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamale.Behaviour
{
    internal abstract class Component
    {
        public GameObject gameObject = null;
        public abstract void Update(double delta);
        public abstract void Destroy();
    }
}
