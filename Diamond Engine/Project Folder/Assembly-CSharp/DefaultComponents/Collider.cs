using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DiamondEngine
{
    public class Collider : DiamondComponent
    {
        public Collider()
        {
            type = ComponentType.COLLIDER;
        }
    }

    public class BoxCollider : DiamondComponent
    {
        public BoxCollider()
        {
            type = ComponentType.BOXCOLLIDER;
        }
    }

    public class MeshCollider : DiamondComponent
    {
        public MeshCollider()
        {
            type = ComponentType.MESHCOLLIDER;
        }
    }

    public class SphereCollider : DiamondComponent
    {
        public SphereCollider()
        {
            type = ComponentType.SPHERECOLLIDER;
        }
    }
}
