using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamale.Behaviour
{
    public abstract class Component
    {
        public GameObject gameObject = null;
        public abstract void Update(double delta);
        public virtual void Destroy()
        {
            gameObject.components.Remove(this);
        }
    }
}
