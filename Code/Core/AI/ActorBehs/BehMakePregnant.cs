using System.Text;
using ai;
using ai.behaviours;
using AncientWarfare.Const;
using AncientWarfare.Core.Content;
using AncientWarfare.Core.Extensions;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehMakePregnant : BehaviourActionActor
{
    public override BehResult execute(Actor pObject)
    {
        Actor couple = pObject.beh_actor_target?.a;
        if (!couple.IsValid() || couple == pObject) return BehResult.Stop;

        (Actor male, Actor female) = pObject.data.gender == ActorGender.Male ? (pObject, couple) : (couple, pObject);
        if (male.data.gender != ActorGender.Male || female.data.gender != ActorGender.Female) return BehResult.Stop;

        // TODO: 检查距离
        // TODO: 检查身孕
        if (female.hasStatus(StatusEffectExtendLibrary.pregnant.id)) return BehResult.Stop;
        female.addStatusEffect(nameof(StatusEffectExtendLibrary.pregnant));

        var child_data = new ActorData();
        // TODO: 生成孩子数据
        // 仅记录asset_id，继承特质，继承皮肤, 其余可以在实际生成孩子的时候再确定
        child_data.name =
            $"{male.GetAdditionData().family_name}{ComnConst.COMN_SEPERATOR}{female.GetAdditionData().clan_name}";
        child_data.asset_id = couple.asset.id;
        child_data.generateTraits(couple.asset, couple.race);
        child_data.inheritTraits(couple.data.traits);
        child_data.inheritTraits(pObject.data.traits);
        // 仅完全同生物时考虑肤色变化
        if (couple.asset == pObject.asset)
        {
            child_data.skin = ActorTool.getBabyColor(couple, pObject);
            child_data.skin_set = couple.data.skin_set;
        }
        else
        {
            child_data.skin = couple.data.skin;
            child_data.skin_set = couple.data.skin_set;
        }

        male.data.makeChild(World.world.getCurWorldTime());
        female.data.set(ActorDataKeys.aw_pregnant_child_data, ActorDataForPregnantToJSON(child_data));


        return BehResult.Continue;
    }

    private static string ActorDataForPregnantToJSON(ActorData data)
    {
        StringBuilder sb = new();
        sb.Append('{');

        sb.Append("asset_id:\"");
        sb.Append(data.asset_id);

        sb.Append("\",traits:[");
        var trait_count = data.traits.Count;
        if (trait_count > 0)
        {
            sb.Append('\"');
            sb.Append(data.traits[0]);
            sb.Append('\"');
            for (var i = 1; i < trait_count; i++)
            {
                sb.Append(',');
                sb.Append('\"');
                sb.Append(data.traits[i]);
                sb.Append('\"');
            }
        }

        sb.Append("],skin:");
        sb.Append(data.skin);
        sb.Append(",skin_set:");
        sb.Append(data.skin_set);

        sb.Append('}');
        return sb.ToString();
    }
}