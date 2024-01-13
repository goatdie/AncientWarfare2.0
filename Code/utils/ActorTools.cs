using Figurebox.core;
using System.Collections.Generic;
namespace Figurebox.Utils;

public static class ActorTools
{
    /// <summary>
    ///     改变生物所属城市
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="ncity"></param>
    public static void ChangeCity(this Actor actor, City ncity)
    {
        actor.city?.removeUnit(actor, false);
        ncity.addNewUnit(actor);
    }
    public static bool IsHeir(this Actor actor)
    {
        Clan pClan = actor.getClan();
        if (pClan == null)
        {
            return false;
        }
        if (pClan.units.Count == 0)
        {
            return false;
        }
        foreach (Actor value in pClan.units.Values)
        {
            if (value.isKing())
            {
                AW_Kingdom actorKingdom = value.kingdom as AW_Kingdom;
                return actorKingdom.heir == actor;
            }
        }

        return false;
    }
    public static string getNameCharacter(this Actor actor)
    {
    
        string existingName = actor.getName();
      
            string familyName;
            string clanName;
            string chineseFamilyName;

            actor.data.get("family_name", out familyName, "");
            actor.data.get("clan_name", out clanName, "");
            actor.data.get("chinese_family_name", out chineseFamilyName, "");

            // 移除姓氏
            if (!string.IsNullOrEmpty(familyName) && existingName.EndsWith(familyName))
            {
                // 计算应去除的姓氏长度
                int nameLengthToRemove = familyName.Length;
                // 移除姓氏部分
                existingName = existingName.Substring(0, existingName.Length - nameLengthToRemove);
            }
            else if (!string.IsNullOrEmpty(clanName) && existingName.StartsWith(clanName))
            {
                existingName = existingName.Substring(clanName.Length);
            }
            else if (!string.IsNullOrEmpty(chineseFamilyName) && existingName.StartsWith(chineseFamilyName))
            {
                existingName = existingName.Substring(chineseFamilyName.Length);
            }

            // 返回处理后的名字
            return existingName.Trim(); // 使用 Trim() 移除名字前后可能的空白字符
    

    }

}