using Figurebox.abstracts;

namespace Figurebox.content
{
    internal class KingdomAssetLibrary : ExtendedLibrary<KingdomAsset>
    {
        protected override void init()
        {
            var kingdomAsset = AssetManager.kingdoms.get("human");
            kingdomAsset.addFriendlyTag("Xia");


            var kingdomNomadsAsset = AssetManager.kingdoms.get("nomads_human");
            kingdomNomadsAsset.addFriendlyTag("Xia");
            
            AssetManager.kingdoms.add(new KingdomAsset
            {
                id = "empty",
                civ = true
            });
            AssetManager.kingdoms.add(new KingdomAsset
            {
                id = "nomads_empty",
                nomads = true,
                mobs = true
            });

            //主要国家
            KingdomAsset addKingdom7 = AssetManager.kingdoms.clone("Xia", "empty");
            addKingdom7.addTag("civ");
            addKingdom7.addTag("Xia");
            addKingdom7.addFriendlyTag("human");
            addKingdom7.addFriendlyTag("Xia");
            addKingdom7.addFriendlyTag("neutral");
            addKingdom7.addFriendlyTag("good");
            addKingdom7.addEnemyTag("bandits");
            World.world.kingdoms.newHiddenKingdom(addKingdom7);
            //临时用的国家
            KingdomAsset addKingdom8 = AssetManager.kingdoms.clone("nomads_Xia", "nomads_empty");
            addKingdom8.addTag("civ");
            addKingdom8.addTag("Xia");
            addKingdom8.addFriendlyTag("Xia");
            addKingdom8.addFriendlyTag("human");
            addKingdom8.addFriendlyTag("neutral");
            addKingdom8.addFriendlyTag("good");
            addKingdom8.addEnemyTag("bandits");
            World.world.kingdoms.newHiddenKingdom(addKingdom8);

        }
    }
}