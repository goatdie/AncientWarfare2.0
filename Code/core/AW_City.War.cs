using System;
using System.Collections.Generic;
using System.Linq;
using Figurebox.attributes;
using Figurebox.constants;
using Figurebox.core.dbs;
using Figurebox.Utils;
using Figurebox.Utils.extensions;
using Figurebox.Utils.MoH;

namespace Figurebox.core;

public partial class AW_City : City
{
    public Dictionary<AW_Kingdom, double> cityOccupationHistory = new Dictionary<AW_Kingdom, double>();
    public void RecordOccupation(AW_Kingdom kingdom)
    {
        double daysOccupied = World.world.getCreationTime();
        // 检查该王国是否已存在于字典中
        if (cityOccupationHistory.ContainsKey(kingdom))
        {
            // 如果已存在，更新占领时间（可以是累加，或者是设置新值，根据需求定）
            cityOccupationHistory[kingdom] += daysOccupied; // 累加占领时间
        }
        else
        {
            // 如果不存在，添加王国和对应的占领时间
            cityOccupationHistory.Add(kingdom, daysOccupied);
        }
    }
    public int GetYearsSinceOccupation(AW_Kingdom kingdom)
    {
        if (cityOccupationHistory.ContainsKey(kingdom))
        {
            // 获取该王国占领的天数
            double daysOccupied = cityOccupationHistory[kingdom];
            // 调用现有逻辑计算从占领到现在经过的年数
            return World.world.mapStats.getYearsSince(daysOccupied);
        }
        else
        {
            // 如果字典中没有该王国的记录，可以返回-1表示没有找到记录
            return -1;
        }
    }
    public static bool HasKingdomRecoveredAllCities(AW_War aww)
    {

        AW_Kingdom attacker = aww.main_attacker as AW_Kingdom;
        int currentYear = World.world.mapStats.getCurrentYear();

        // 对于每个城市，检查是否被指定王国占领过
        foreach (var cityObj in aww._list_defenders_city)
        {
            AW_City city = cityObj as AW_City; // 确保城市对象是AW_City类型

            // 检查这个城市是否曾经被指定的王国占领
            if (city.cityOccupationHistory.ContainsKey(attacker))
            {
                double daysOccupied = city.cityOccupationHistory[attacker];
                int yearsSinceOccupation = World.world.mapStats.getYearsSince(daysOccupied);

                // 检查占领是否在过去100年内
                if (yearsSinceOccupation <= 100)
                {
                    // 如果城市曾经在过去100年内被占领，检查当前所有者是否是指定的王国
                    if (city.kingdom != aww.main_attacker)
                    {
                        // 如果发现至少有一个城市当前不属于该王国，且占领时间在100年内，返回false
                        return false;
                    }
                }
            }
        }

        return true;
    }


    private static void TransferCities(Kingdom winner, Kingdom loser, AW_City defenderCapital)
    {
        List<City> loserCitiesCopy = new List<City>(loser.cities);
        foreach (var city in loserCitiesCopy)
        {
            if (city != defenderCapital && city.getLoyalty() < 100)
            {
                city.joinAnotherKingdom(winner);
            }
        }
    }



    private static void SetLoserAsVassal(Kingdom loser, Kingdom winner)
    {
        // 这里应该实现将loser设置为winner的附庸的逻辑
        CityTools.LogVassalWarEnd(loser, winner);
        // 假设SetKingdom方法用于设置附庸关系
        loser.AW().SetVassal(winner.AW());
    }


}