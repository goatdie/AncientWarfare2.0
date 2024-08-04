using AncientWarfare.Abstracts;
using AncientWarfare.Core.Force;
using AncientWarfare.Core.MapModes;

namespace AncientWarfare.Core.Content
{
    public class NameplateExtendLibrary : ExtendedLibrary<NameplateAsset>, IManager
    {
        public static readonly NameplateAsset plate_tribe;

        public void Initialize()
        {
        }

        protected override void init()
        {
            add(new() { id = nameof(plate_tribe), map_mode = (MapMode)CustomMapMode.Tribe });
            t.path_sprite = "ui/nameplates/nameplate_culture";
            t.padding_left = 6;
            t.padding_right = 7;
            t.padding_top = -2;
            t.action_main = (manager, asset) =>
            {
                foreach (var tribe in ForceManager.I.tribes.All)
                {
                    MapText map_text = manager.prepareNext(plate_tribe);

                    string text = $"{tribe.GetName()} {tribe.Data.members.Count}";

                    map_text.setText(text, tribe.CenterTile.posV);
                    map_text.base_icon.sprite = tribe.Race.getRaceIconSprite();
                    map_text._show_base_icon = true;
                }
            };
        }
    }
}