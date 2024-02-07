using Figurebox.abstracts;
using Figurebox.utils.extensions;
using Figurebox.utils.MoH;

namespace Figurebox.content
{
    class LoyaltyLibrary : ExtendedLibrary<LoyaltyAsset>
    {
        protected override void init()
        {
            LoyaltyAsset tianming = new LoyaltyAsset();
            tianming.id = "tianming";
            tianming.translation_key = "tianming";
            tianming.calc = delegate (City pCity)
            {
                int result = 0;
                Actor king = pCity.kingdom.king;
                if (king != null && king.hasStatus("tianming0"))//if has tianming1 tianming2.....
                {
                    result = (int)(king.stats[S.diplomacy] + king.stats[S.stewardship] * 1f);
                }
                if (king != null && king.hasStatus("tianmingm1"))//if has tianming1 tianming2.....
                {
                    result = -(int)(king.stats[S.diplomacy] + king.stats[S.stewardship] * 2f);
                }
                return result;
            };
            AssetManager.loyalty_library.add(tianming);
            OpinionAsset tianminggo = new OpinionAsset();
            tianminggo.id = "tianminggo";
            tianminggo.translation_key = "tianminggo";
            AssetManager.opinion_library.add(tianminggo);
            tianminggo.calc = delegate (Kingdom pMain, Kingdom pTarget)
            {
                int result = 0;

                if (MoHTools.IsMoHKingdom(pMain.AW()))
                {
                    result = -200;
                }
                return result;
            };

            OpinionAsset vassels = new OpinionAsset();
            vassels.id = "vassels";
            vassels.translation_key = "vassels";
            AssetManager.opinion_library.add(vassels);

            vassels.calc = delegate (Kingdom pMain, Kingdom pTarget)
            {
                int result = 0;
                string vassalId = pMain.AW().addition_data.suzerain_id;
                string vassaltarget = pTarget.AW().addition_data.suzerain_id;

                // 如果pMain是pTarget的宗主国
                if (vassalId == pTarget.data.id)
                {
                    result = 1000;
                }
                // 如果pMain和pTarget有相同的宗主国
                else if (vassalId == vassaltarget && !string.IsNullOrEmpty(vassalId))
                {
                    result = 500;
                }
                // 如果pTarget是pMain的宗主国
                else if (vassaltarget == pMain.data.id)
                {
                    result = 500;
                }


                return result;
            };
            OpinionAsset vasselsrebel = new OpinionAsset();
            vasselsrebel.id = "vasselsrebel";
            vasselsrebel.translation_key = "vasselsrebel";
            AssetManager.opinion_library.add(vasselsrebel);
            vasselsrebel.calc = delegate (Kingdom pMain, Kingdom pTarget)
            {
                int result = 0;
                string mainSuzerainId = pMain.AW().addition_data.suzerain_id;
                string targetSuzerainId = pTarget.AW().addition_data.suzerain_id;

                // 如果pMain是附庸国并且宗主国是pTarget
                if (pMain.AW().IsVassal() && pTarget.data.id == mainSuzerainId)
                {
                    // 比较领土和军队数量
                    bool hasMoreTerritoryAndArmy = pMain.AW().countZones() > pTarget.AW().countZones() &&
                                                    pMain.AW().getArmy() > pTarget.AW().getArmy();

                    // 如果附庸国的领土和军队数量都比宗主国多，则减少好感度
                    if (hasMoreTerritoryAndArmy)
                    {
                        result -= 200;
                    }
                }

                return result;
            };
        }
    }
}