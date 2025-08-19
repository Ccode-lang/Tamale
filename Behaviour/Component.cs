using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamale.Behaviour
{
    internal abstract class Component
    {
        public GameObject gameObject;
        public abstract void Update(double delta);
    }
}
