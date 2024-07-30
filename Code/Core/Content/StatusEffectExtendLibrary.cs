using AncientWarfare.Abstracts;
using AncientWarfare.LocaleKeys;

namespace AncientWarfare.Core.Content;

public class StatusEffectExtendLibrary : ExtendedLibrary<StatusEffect>, IManager
{
    public static readonly StatusEffect pregnant;

    public void Initialize()
    {
    }

    protected override void init()
    {
        add(new StatusEffect { id = nameof(pregnant) });
        t.path_icon = "ui/icons/worldrules/icon_lastofus";
        t.action_interval = 1;
        t.duration = 60;
        t.name = StatusEffectKeys.pregnant;
        t.description = StatusEffectKeys.pregnant_desc;
        t.action = WorldActionLibrary.CheckChildBirth;
    }
}