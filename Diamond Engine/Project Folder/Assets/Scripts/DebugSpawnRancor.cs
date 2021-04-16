using System;
using DiamondEngine;

public class DebugSpawnRancor : DiamondComponent
{
    private float offSet = 5.0f;

    public void OnExecuteButton()
    {
        Vector3 pos = new Vector3(0, 0, 0);

        if (Core.instance != null)
            pos = new Vector3(Core.instance.gameObject.transform.globalPosition.x + offSet, Core.instance.gameObject.transform.globalPosition.y, Core.instance.gameObject.transform.globalPosition.z);

        InternalCalls.CreatePrefab("Library/Prefabs/1595610549.prefab", pos, new Quaternion(0, 0, 0), new Vector3(1, 1, 1));
    }
}