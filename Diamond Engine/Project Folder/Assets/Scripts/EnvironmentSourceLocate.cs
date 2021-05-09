using System;
using DiamondEngine;

public class EnvironmentSourceLocate : DiamondComponent
{
    public static EnvironmentSourceLocate instance = null;

    public void Awake()
    {
        if (instance != null)
            InternalCalls.Destroy(gameObject);
        else
            instance = this;

    }

    public void OnApplicationQuit()
    {
        instance = null;
    }

}