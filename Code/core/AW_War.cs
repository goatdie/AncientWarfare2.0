namespace Figurebox.core;
using Figurebox.attributes;
using System.Collections.Generic;
using Figurebox.Utils.MoH;
public class AW_War : War
{


    private readonly List<City> _list_attackers_city = new List<City>();
    private readonly List<City> _list_defenders_city = new List<City>();
    public City _attackerCapital;
    public City _defenderCapital;
    public List<City> PreCaptureNeighbourCities { get; private set; }

    public void ResolveTianmingWar()
    {
        AW_Kingdom attacker = main_attacker as AW_Kingdom;
        AW_Kingdom defender = main_defender as AW_Kingdom;
        // 确定战争的胜利者
        AW_Kingdom winner = DetermineWarWinner();
        if (winner == null)
        {
            return; // 如果无法确定胜利者，则不进行进一步操作
        }

        // 确定战争的失败者
        AW_Kingdom loser = (winner == attacker) ? defender : attacker;

        if (!MoHTools.IsMoHKingdom(winner))
        {
            // 清除当前的天命国家
            MoHTools.Clear_MoHKingdom();

            // 设置新的天命国家
            MoHTools.SetMoHKingdom(winner);
        }

        // 进行其他必要的结算操作，例如城市转移、资源分配等
        if (PreCaptureNeighbourCities != null)
        {
            foreach (City city in PreCaptureNeighbourCities)
            {
                city.joinAnotherKingdom(winner);
            }
        }

        // 结束战争
        World.world.wars.endWar(this, true);
    }


    private AW_Kingdom DetermineWarWinner()
    {
        // 检查攻击方和防守方的首都是否属于同一个国家
        if (_attackerCapital != null && _defenderCapital != null)
        {
            if (_attackerCapital.kingdom == _defenderCapital.kingdom)
            {
                // 如果两个首都现在属于同一个国家，则该国家是战争的赢家
                AW_Kingdom kingdom = _attackerCapital.kingdom as AW_Kingdom;
                return kingdom;
            }
        }

        // 如果首都不属于同一个国家或者某个首都为空，则无法确定赢家
        return null;
    }








    [MethodReplace]
    public new void initWar(Kingdom pAttacker, Kingdom pDefender, string pType)
    {
        #region 原版代码
        this.data.main_attacker = pAttacker.id;
        if (pDefender != null)
        {
            this.data.main_defender = pDefender.id;
        }
        this.data.war_type = pType;
        this.joinAttackers(pAttacker);
        if (pDefender != null)
        {
            this.joinDefenders(pDefender);
        }
        this.prepare();
        #endregion

        _list_attackers_city.AddRange(pAttacker.cities);
        _list_defenders_city.AddRange(pDefender?.cities ?? new List<City>());

        _attackerCapital = pAttacker.capital;
        _defenderCapital = pDefender?.capital;
        if (_defenderCapital != null)
        {
            PreCaptureNeighbourCities = new List<City>(_defenderCapital.neighbours_cities);
        }
        AW_Kingdom attacker = pAttacker as AW_Kingdom;
        AW_Kingdom defender = pDefender as AW_Kingdom;

        if (pType == "whisper_of_war" && MoHTools.IsMoHKingdom(defender))
        {
            _asset = AssetManager.war_types_library.get("tianming");
        }
    }

}