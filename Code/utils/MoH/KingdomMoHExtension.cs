namespace Figurebox.core;
using System.Collections.Generic;

public partial class AW_Kingdom
{
    public static void GiveAllCityFood(AW_Kingdom kingdom, string foodid, int number)
    {
        foreach (City city in kingdom.cities)
        {

            city.data.storage.change(foodid, number);
        }
    }
    public static void GiveCityFood(City city, string foodid, int number)
    {

        city.data.storage.change(foodid, number);

    }
    public static void BoostSolider(AW_Kingdom kingdom)
    {

        foreach (City city in kingdom.cities)
        {
            List<Actor> simpleList = city.professionsDict[UnitProfession.Warrior];
            foreach (Actor a in simpleList)
            {

                a.addStatusEffect("powerup", 50f);




            }
        }

    }
}