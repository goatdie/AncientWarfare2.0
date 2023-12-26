using System;
using System.Reflection;
using Unity;
using UnityEngine;
using NCMS.Utils;
using HarmonyLib;
using ReflectionUtility;
using System.Collections.Generic;
using Figurebox.Utils;
using ai.behaviours.conditions;
using System.Linq;

namespace Figurebox
{
	class Actionlib
	{
		public static Actionlib instance;
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

			if (a.GetData().profession != UnitProfession.King)
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
				if (a.hasTrait("first")) { a.removeTrait("first"); }

				Debug.Log("双重天命结束保险" + a.getName());
			}



			return true;
		}
		public static Dictionary<string, string> KingKingdomId = new Dictionary<string, string>();

		public static bool checkkingleft(BaseSimObject pTarget, WorldTile pTile = null)
		{
			if (pTarget == null || pTarget.a == null) return false;
			Actor a = (Actor)pTarget;
			if (a.GetData().profession == UnitProfession.King && !KingKingdomId.ContainsKey(a.data.id))
			{
				KingKingdomId[a.data.id] = a.kingdom.data.id;
			}

			if (a.GetData().profession != UnitProfession.King)
			{
				string kingId = a.data.id;
				string kingdomId;
				if (KingKingdomId.ContainsKey(a.data.id))
				{
					kingdomId = KingKingdomId[a.data.id];
				}
				else
				{
					kingdomId = a.kingdom.data.id;
				}
				int yeardata = World.world.mapStats.getCurrentYear();

				// Find the first available count for this king and kingdom.
				int count = 1;
				while (FunctionHelper.KingEndYearInKingdom.ContainsKey(kingId + "-" + kingdomId + "-" + count))
				{
					count++;
				}

				string kingAndKingdom = kingId + "-" + kingdomId + "-" + count;

				// Only update the dictionary if the king and kingdom combination is not already in it.
				if (!FunctionHelper.KingEndYearInKingdom.ContainsKey(kingAndKingdom))
				{
					FunctionHelper.KingEndYearInKingdom[kingAndKingdom] = yeardata;
					a.removeTrait("zhuhou");
					Debug.Log("不再是国王在字典上记住" + a.getName());
				}
			}

			return true;
		}

	}
}