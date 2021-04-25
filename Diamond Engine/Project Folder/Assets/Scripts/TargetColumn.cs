using System;
using DiamondEngine;

public class TargetColumn : DiamondComponent
{
	public GameObject target01, target02;

	public GameObject GetTarget(Vector3 pos)
    {
        if(target02 == null)
        {
            return target01;
        }

        float distance = pos.x - gameObject.transform.globalPosition.x;

        if(distance >= 0)
        {
            return target01;
        }
        else
        {
            return target02;
        }
    }
}