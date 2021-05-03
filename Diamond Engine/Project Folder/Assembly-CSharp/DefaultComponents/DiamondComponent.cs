using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace DiamondEngine
{
    public enum ComponentType
    {
        NONE,
        TRANSFORM,
        MESH_RENDERER,
        MATERIAL,
        CAMERA,
        SCRIPT,
        TRANSFORM_2D,
        BUTTON,
        CHECKBOX,
        TEXT_UI,
        CANVAS,
        IMAGE_2D,
        AUDIO_LISTENER,
        AUDIO_SOURCE,
        RIGIDBODY,
        COLLIDER,
        ANIMATOR,
        NAVIGATION,
        BOXCOLLIDER,
        MESHCOLLIDER,
        PARTICLE_SYSTEM,
        BILLBOARD,
        SPHERECOLLIDER,
        DIRECTIONAL_LIGHT,
        NAVMESHAGENT,
        CAPSULECOLLIDER,
        STENCIL_MATERIAL,
        COUNT
    }


    public class DiamondComponent
    {
        public UIntPtr pointer;
        public ComponentType type;
        public static Dictionary<System.Type, ComponentType> componentTable = new Dictionary<Type, ComponentType> {
            { typeof(Transform), ComponentType.TRANSFORM},
            { typeof(Text), ComponentType.TEXT_UI  },
            { typeof(Material), ComponentType.MATERIAL  },
            { typeof(Image2D), ComponentType.IMAGE_2D  },
            { typeof(Navigation), ComponentType.NAVIGATION  },
            { typeof(ParticleSystem), ComponentType.PARTICLE_SYSTEM  },
            { typeof(Transform2D), ComponentType.TRANSFORM_2D  },
            { typeof(NavMeshAgent), ComponentType.NAVMESHAGENT  },
            { typeof(Button), ComponentType.BUTTON  },
            { typeof(Collider), ComponentType.COLLIDER},
            { typeof(BoxCollider), ComponentType.BOXCOLLIDER},
            { typeof(MeshCollider), ComponentType.MESHCOLLIDER},
            { typeof(SphereCollider), ComponentType.SPHERECOLLIDER},
            { typeof(MeshRenderer), ComponentType.MESH_RENDERER},
            { typeof(Rigidbody), ComponentType.RIGIDBODY},
        };

        public DiamondComponent()
        {
            this.type = ComponentType.SCRIPT;
        }

        public extern GameObject gameObject
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }

        public ComponentType GetComponentType()
        {
            return type;
        }

        public extern bool active
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }
    }
}
