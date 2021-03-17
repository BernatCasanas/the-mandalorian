using System;
using DiamondEngine;

public class BoonInstance : DiamondComponent
{
    Boon whatever;
    public string myType;
    bool firstFrame = true;
    public void Update()
    {
        if (firstFrame)
        {
            firstFrame = !firstFrame;

            Type t = Boon_Data_Holder.boonType[myType];
            whatever = (Boon)Activator.CreateInstance(t);
            whatever.Use();
        }

    }

    //OnCollision whatever.use()


}