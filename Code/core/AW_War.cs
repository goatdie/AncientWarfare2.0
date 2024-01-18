namespace Figurebox.core;
using Figurebox.attributes;
using System.Collections.Generic;
public class AW_War : War
{


    private readonly List<City> _list_attackers_city = new List<City>();
    private readonly List<City> _list_defenders_city = new List<City>();
    public City _attackerCapital;
    public City _defenderCapital;








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
    }

}