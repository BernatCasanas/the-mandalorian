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
            if (Core.instance.gameObject.GetComponent<PlayerHealth>() != null)
            {
                int currentLifeSteal = Core.instance.gameObject.GetComponent<PlayerHealth>().IncrementHealingWhenKillingEnemy(1);
                Debug.Log("LifeSteal increased to: " + currentLifeSteal);
            }
            else
            {
                Debug.Log("ERROR!! Din Djarin has no player health component");

            }
        }
    }

    //+20% max HP. - Wrecker’s resilience
    public class IncrementMaxHpBoon : Boon
    {
        public override void Use()
        {
            if (Core.instance.gameObject.GetComponent<PlayerHealth>() != null)
            {
                int toIncrement = 20;
                int currentMaxHp = Core.instance.gameObject.GetComponent<PlayerHealth>().IncrementMaxHpValue(toIncrement);
                int currentHp = Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(-toIncrement);
                Debug.Log("Max HP increased to: " + currentMaxHp);
                Debug.Log("Curr HP increased to: " + currentHp);

            }
            else
            {
                Debug.Log("ERROR!! Din Djarin has no player health component");
            }

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
