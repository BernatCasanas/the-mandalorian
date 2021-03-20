using DiamondEngine;


class MusicSourceLocate : DiamondComponent 
{
    public static MusicSourceLocate instance;

    bool started = false;

    public void Update()
    {
        if (!started)
        {
            instance = this;
            started = true;
        }
    }
}

