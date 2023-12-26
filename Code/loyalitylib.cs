using System;
using System.Threading;
using System.Reflection;
using NCMS;
using Unity;
using ReflectionUtility;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Figurebox.Utils;
using NCMS.Utils;


namespace Figurebox
{
    class LoyaltyLibrary
    {
      public static void init()
        {
        LoyaltyAsset tianming = new LoyaltyAsset();
		tianming.id = "tianming";
		tianming.translation_key = "tianming";
        addTraitToLocalizedLibrary("cz",tianming.id,"天命");
        addTraitToLocalizedLibrary("ch",tianming.id,"天命");
		addTraitToLocalizedLibrary("en",tianming.id,"Mandate Of Heaven");
		tianming.calc = delegate(City pCity)
		{
			int result = 0;
			Actor king = pCity.kingdom.king;
			if (king != null&&king.hasStatus("tianming0"))//if has tianming1 tianming2.....
			{
				result = (int)(king.stats[S.diplomacy] + king.stats[S.stewardship] * 1f);
			}
			 if (king != null&&king.hasStatus("tianmingm1"))//if has tianming1 tianming2.....
			{
				result = -(int)(king.stats[S.diplomacy] + king.stats[S.stewardship] * 2f);
			}
			return result;
		};
        AssetManager.loyalty_library.add(tianming);
       OpinionAsset tianminggo = new OpinionAsset();
		tianminggo.id = "tianminggo";
		tianminggo.translation_key = "tianminggo";
		 addTraitToLocalizedLibrary("cz",tianminggo.id,"天命");
        addTraitToLocalizedLibrary("ch",tianminggo.id,"天命");
		addTraitToLocalizedLibrary("en",tianminggo.id,"Mandate Of Heaven");
		AssetManager.opinion_library.add(tianminggo);
		tianminggo.calc = delegate(Kingdom pMain, Kingdom pTarget)
		{
			int result = 0;
			bool tianmingBoolValue;
        pMain.data.get("tianmingbool", out tianmingBoolValue);
            if (tianmingBoolValue)
            {
				result=-200;
			}
			return result;
		};

OpinionAsset vassels = new OpinionAsset();
vassels.id = "vassels";
vassels.translation_key = "vassels";
addTraitToLocalizedLibrary("cz", vassels.id, "附庸");
addTraitToLocalizedLibrary("ch", vassels.id, "附庸");
addTraitToLocalizedLibrary("en", vassels.id, "Vassels");
AssetManager.opinion_library.add(vassels);

vassels.calc = delegate(Kingdom pMain, Kingdom pTarget)
{
    int result = 0;
    string vassalId;
	string vassaltarget;
    pMain.data.get("vassels", out vassalId);
	pTarget.data.get("vassels", out vassaltarget);
    if (vassalId != pMain.data.id&&vassalId == pTarget.data.id)
    {
        result = 1000;
    }
	if (vassaltarget == pMain.data.id&&String.IsNullOrEmpty(vassalId))
    {
        result = 500;
    }
    if (vassalId == vassaltarget && !string.IsNullOrEmpty(vassalId))
    {
        result = 500;
    }
    return result;
};


        }
        private static void addTraitToLocalizedLibrary(string pLanguage,string id, string name)
        {
            string language = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "language") as string;
            if (language != "en" && language != "ch" && language != "cz")
            {
                pLanguage = "en";
            }
            
            if (pLanguage == language)
            {
                Dictionary<string, string> localizedText = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "localizedText") as Dictionary<string, string>;
                localizedText.Add( id, name);
               
            }
        }
    }
}