﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DiamondEngine
{
    public class NavMeshAgent : DiamondComponent
    {
        public NavMeshAgent()
        {
            type = ComponentType.NAVMESHAGENT;
        }

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
        public extern bool CalculateRandomPath(object startPos, float radius);


        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern bool CalculatePath(object startPos, object endPos);


        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern Vector3 GetDestination();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern Vector3 GetPointAt(int index);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern int GetPathSize();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void ClearPath();
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern Vector3 GetLastVector();

        public void MoveToCalculatedPos(float speed)
        {
            Vector3 pos = gameObject.transform.globalPosition;
            Vector3 direction = GetDestination() - pos;

            gameObject.transform.localPosition += direction.normalized * speed * Time.deltaTime;
        }
    }
}


