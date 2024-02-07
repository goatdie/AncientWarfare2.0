using System.Collections.Generic;
using UnityEngine;

namespace Figurebox
{
    class Actionlib
    {
        public static Actionlib instance;

        public static Dictionary<string, string> KingKingdomId = new Dictionary<string, string>();
        //public static int year = 1;


        public static bool rebelkingdom(BaseSimObject pTarget, WorldTile pTile = null)
        {
            Actor a = (Actor)pTarget;

            if (a.kingdom.getEnemiesKingdoms().Count == 0)
            {
                a.kingdom.setKing(a);
                a.removeTrait("rebel");
                Debug.Log("叛乱国王结束");
            }


            return true;
        }

        public static bool checkP(BaseSimObject pTarget, WorldTile pTile = null)
        {
            Actor a = (Actor)pTarget;

            if (a.data.profession != UnitProfession.King)
            {
                a.removeTrait("天命");
                Debug.Log("结束天命特质" + a.getName());
            }


            return true;
        }

        public static bool former(BaseSimObject pTarget, WorldTile pTile = null)
        {
            Actor a = (Actor)pTarget;
            bool tianmingBoolValue;
            a.kingdom.data.get("tianmingbool", out tianmingBoolValue);
            if (tianmingBoolValue)
            {
                a.kingdom.data.set("tianmingbool", false);
                if (a.hasTrait("first"))
                {
                    a.removeTrait("first");
                }

                Debug.Log("双重天命结束保险" + a.getName());
            }


            return true;
        }
    }
}