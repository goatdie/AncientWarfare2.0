using UnityEngine;
using HarmonyLib;
using ReflectionUtility;
using Figurebox.utils;
using NCMS.Utils;
using ai;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Reflection;
using Figurebox;
using System.Linq;
using System.IO;

namespace Figurebox
{
	class NameGeneratorAssets
	{


		public static void init()
		{

			NameGeneratorAsset reclaimnam = new NameGeneratorAsset();

			reclaimnam.id = "war_reclaim";
			reclaimnam.use_dictionary = true;
			reclaimnam.replacer_kingdom = new NameGeneratorReplacerKingdom(NameGeneratorReplacers.replaceKingdom);
			AssetManager.nameGenerator.add(reclaimnam);
			AssetManager.nameGenerator.addDictPart("space", " ");
			AssetManager.nameGenerator.addDictPart("kingdom_name", "$kingdom$");
			AssetManager.nameGenerator.addDictPart("'s", "'s");
			AssetManager.nameGenerator.addDictPart("of", "of");
			AssetManager.nameGenerator.addDictPart("restoration", "Restoration,Reclaim,Recovery,Retrieval,Retribution,Return,Rebirth,Resurgence");
			AssetManager.nameGenerator.addDictPart("dominance", "Dominance,Supremacy,Preeminence,Ascendancy,Superiority");
			AssetManager.nameGenerator.addDictPart("territory", "Territory,Land,Region,Domain,Province");
			AssetManager.nameGenerator.addDictPart("victory", "Victory,Triumph,Conquest,Success,Domination");
			AssetManager.nameGenerator.addDictPart("justice", "Justice,Righteousness,Equity,Integrity,Honor");
			reclaimnam.templates.Add("restoration,space,of,space,kingdom_name");
			//reclaimnam.templates.Add("war,space,of,space,dominance");
			//reclaimnam.templates.Add("victory,space,in,space,territory");
			reclaimnam.templates.Add("restoration,space,of,space,territory");
			Debug.Log("namegeneratorasset添加成功");

		}















	}

}