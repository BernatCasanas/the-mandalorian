using DiamondEngine;

class MusicSourceLocate : DiamondComponent 
{
    public static MusicSourceLocate instance = null;

    bool started = false;

    public void Update()
    {
        if (!started)
        {
            if (instance != null)
            {
                InternalCalls.Destroy(instance);
            }
            
            instance = this;
            started = true;

        }

    }
}

