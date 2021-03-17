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

    //Each time you kill an enemy heal +1 HP. - Bo Katan’s resilience
    public class LifeStealBoon : Boon
    {
        public override void Use()
        {
            int currentLifeSteal = Core.instance.gameObject.GetComponent<PlayerHealth>().IncrementHealingWhenKillingEnemy(1);
            Debug.Log("LifeSteal increased to: " + currentLifeSteal);

        }
    }

    //+20% max HP. - Wrecker’s resilience
    public class IncrementMaxHpBoon : Boon
    {
        public override void Use()
        {

            int currentMaxHp = Core.instance.gameObject.GetComponent<PlayerHealth>().IncrementMaxHpPercent(0.2f);
            Debug.Log("Current HP increased to: " + currentMaxHp);

        }
    }

    public class Boon_Data_Holder
    {
        public static Dictionary<string, Type> boonType = new Dictionary<string, Type>
        {
            {"LifeSteal", typeof(LifeStealBoon)},
            {"IncMaxHp", typeof(IncrementMaxHpBoon)},
            //TODO Add boons here
        };
    }
}
