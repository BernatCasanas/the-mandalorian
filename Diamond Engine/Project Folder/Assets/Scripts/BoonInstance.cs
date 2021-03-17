using System;
using DiamondEngine;

public class BoonInstance : DiamondComponent
{
    Boon myBoon = null;
    BoonSpawn boonSpawner = null;
    public string myType;
    bool firstFrame = true;
    public void Update()
    {
        if (firstFrame)
        {
            firstFrame = !firstFrame;

            Type t = Boon_Data_Holder.boonType[myType];
            myBoon = (Boon)Activator.CreateInstance(t);
            boonSpawner = Core.instance.gameObject.GetComponent<BoonSpawn>();

        }

        //TODO animate boon here (UP-DOwN + rotation movement)

    }


    public void OnCollisionEnter()
    {
        myBoon.Use();
        if (boonSpawner != null)
        {
            //boonSpawner boon spawner.destroy instantiatet prefabs TODO
        }

    }
}