using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamale.Behaviour
{
    // Base class for components that can be attached to GameObjects.
    public abstract class Component
    {
        // Refernce to the GameObject is null until the first update frame of this component.
        public GameObject gameObject = null;
        public abstract void Update(double delta);
        public virtual void Destroy()
        {
            gameObject.components.Remove(this);
        }
    }
}
