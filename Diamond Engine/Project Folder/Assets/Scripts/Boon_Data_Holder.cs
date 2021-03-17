using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondEngine
{
    public class Boon
    {
        public virtual void Use()
        {

        }
    }

    public class MaxHpBoon : Boon
    {
        public override void Use()
        {
            //actual functionallity
            Debug.Log("Max HP increased");
            //Core.instance.adadsa
        }
    }


    public class IncrementHpBoon : Boon
    {
        public override void Use()
        {
            //actual functionallity
            Debug.Log("Current HP increased");
        }
    }

    public class Boon_Data_Holder
    {
        public static Dictionary<string, Type> boonType = new Dictionary<string, Type>
        {
            {"MaxHP", typeof(MaxHpBoon)},
            {"IncHP", typeof(IncrementHpBoon)},
            //TODO Add boons here
        };
    }
}
