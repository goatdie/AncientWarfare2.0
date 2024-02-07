using Figurebox.abstracts;

namespace Figurebox.content
{
    internal class TraitGroupLibrary : ExtendedLibrary<ActorTraitGroupAsset>
    {
        public static readonly string aw2 = "aw2";
        public static readonly string social_identity_group = "aw_social_identity";

        protected override void init()
        {
            ActorTraitGroupAsset aw2 = new ActorTraitGroupAsset();
            aw2.id = "aw2";
            aw2.name = "trait_group_aw2";
            aw2.color = Toolbox.makeColor("#3BAFFF", -1f);
            add(aw2);

            var social_identity = new ActorTraitGroupAsset();
            social_identity.id = "aw_social_identity";
            social_identity.name = "trait_group_social_identity";
            social_identity.color = Toolbox.makeColor("#FF9300");
            add(social_identity);
        }
    }
}