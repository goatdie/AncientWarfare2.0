namespace Figurebox
{
    class warTypeLibrary
    {


        public static void init()
        {
            WarTypeAsset tianming = new WarTypeAsset();
            tianming.id = "tianming";
            tianming.name_template = "war_conquest";
            tianming.localized_type = "war_type_tianming";
            tianming.path_icon = "ui/wars/war_tianming";
            tianming.kingdom_for_name_attacker = true;
            tianming.forced_war = false;
            tianming.total_war = false;
            tianming.alliance_join = true;
            AssetManager.war_types_library.add(tianming);
            WarTypeAsset tianmingrebel = new WarTypeAsset();
            tianmingrebel.id = "tianmingrebel";
            tianmingrebel.name_template = "war_rebellion";
            tianmingrebel.localized_type = "war_type_tianmingrebel";
            tianmingrebel.path_icon = "ui/wars/war_tianmingrebel";
            tianmingrebel.kingdom_for_name_attacker = true;
            tianmingrebel.forced_war = false;
            tianmingrebel.total_war = false;
            tianmingrebel.alliance_join = false;
            AssetManager.war_types_library.add(tianmingrebel);

            WarTypeAsset reclaim = new WarTypeAsset();
            reclaim.id = "reclaim";
            reclaim.name_template = "war_reclaim";
            reclaim.localized_type = "war_type_reclaim";
            reclaim.path_icon = "ui/wars/war_reclaim";
            reclaim.kingdom_for_name_attacker = true;
            reclaim.forced_war = false;
            reclaim.total_war = false;
            reclaim.alliance_join = true;
            AssetManager.war_types_library.add(reclaim);
            WarTypeAsset vassalWar = new WarTypeAsset();
            vassalWar.id = "vassal_war";
            vassalWar.name_template = "war_conquest";
            vassalWar.localized_type = "war_type_vassal_war";
            vassalWar.path_icon = "ui/wars/war_vassal";
            vassalWar.kingdom_for_name_attacker = true;
            vassalWar.forced_war = false;
            vassalWar.total_war = false;
            vassalWar.alliance_join = false;
            AssetManager.war_types_library.add(vassalWar);
            WarTypeAsset IndependenceWar = new WarTypeAsset();
            IndependenceWar.id = "independence_war";
            IndependenceWar.name_template = "war_conquest";
            IndependenceWar.localized_type = "war_type_independence_war";
            IndependenceWar.path_icon = "ui/wars/war_independent";
            IndependenceWar.kingdom_for_name_attacker = true;
            IndependenceWar.forced_war = false;
            IndependenceWar.total_war = false;
            IndependenceWar.alliance_join = false;
            AssetManager.war_types_library.add(IndependenceWar);



        }
    }

}