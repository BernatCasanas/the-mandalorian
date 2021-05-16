using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace DiamondEngine 
{
    public class Counter
    {
        public enum CounterTypes
        {
            NONE,
            BOKATAN_RES, //Boon
            WRECKER_RES, //Boon
            CAD_BANE_SOH, //Boon
            CAD_BANE_BOOTS, //Boon
            ENEMY_BANTHA,
            ENEMY_STORMTROOPER,
            RANCOR,
            ENEMY_SKYTROOPER,
            ENEMY_LASER_TURRET,
            WAMPA,
            SKEL,
            ENEMY_DEATHTROOPER,
            ENEMY_HEAVYTROOPER,
            MOFFGIDEON,
            LEVELS,
            RUN_COINS,
            MAX,
        }

        public int place = 6;
        public int amount = 0;
        public CounterTypes type;
        public static int roomEnemies = 0;
        public static int maxCombo = 0;
        public static bool isFinalScene = false;
        public static bool allEnemiesDead = false;
        public static bool firstRun = true; // When we have save / load functionality, this should be in it

        public Counter()
        {
            place = 6;
            amount = 0;
            type = CounterTypes.NONE;
        }
        public Counter(int _place, int _amount, CounterTypes ty)
        {
            place = _place;
            amount = _amount;
            type = ty;
        }

        public static Dictionary<CounterTypes, Counter> GameCounters = new Dictionary<CounterTypes, Counter>();
        public static void SumToCounterType(CounterTypes type)
        {
            if (GameCounters.ContainsKey(type))
            {
                Debug.Log("Current counter count: " + type.ToString());
                GameCounters[type].amount += 1;
            }
            else 
            {
                Counter aux = new Counter(GameCounters.Count + 1, 1, type);
                GameCounters.Add(type, aux);
                Debug.Log(type.ToString() + ": added");
            }
        }

        public static void ResetCounters()
        {
            GameCounters.Clear();
            roomEnemies = 0;
            maxCombo = 0;
            isFinalScene = false;
        }

        public static void DebugAllCounters()
        {
            if(GameCounters.ContainsKey(CounterTypes.BOKATAN_RES))
                Debug.Log("Bokatan Res: " + GameCounters[CounterTypes.BOKATAN_RES].amount);
            
            if (GameCounters.ContainsKey(CounterTypes.WRECKER_RES))
                Debug.Log("Wrecker Res: " + GameCounters[CounterTypes.WRECKER_RES].amount);

            if (GameCounters.ContainsKey(CounterTypes.CAD_BANE_SOH))
                Debug.Log("Cad Bane Soh: " + GameCounters[CounterTypes.CAD_BANE_SOH].amount);

            if (GameCounters.ContainsKey(CounterTypes.CAD_BANE_BOOTS))
                Debug.Log("Cad Bane Boots: " + GameCounters[CounterTypes.CAD_BANE_BOOTS].amount);
        }

        public enum GameResult
        {
            NONE,
            VICTORY,
            DEFEAT,
        }

        public static GameResult gameResult = GameResult.NONE;
    }
}
