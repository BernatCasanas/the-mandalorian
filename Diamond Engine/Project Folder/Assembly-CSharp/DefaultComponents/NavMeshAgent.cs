using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DiamondEngine
{
    public class NavMeshAgent : DiamondComponent
    {
        public extern float speed
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }

        public extern float angularSpeed
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }

        public extern float stoppingDistance
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }


        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void CalculateRandomPath(object go, object startPos, float radius);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void CalculatePath(object go, object startPos, object endPos);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern Vector3 GetDestination(object go);

        public void MoveToCalculatedPos(GameObject go, float speed)
        {
            Vector3 pos = go.transform.localPosition;
            Vector3 direction = GetDestination(go) - pos;

            pos += direction.normalized * speed * Time.deltaTime;
        }
    }
}


