#if 一米_中文名
using Chinese_Name;
using Figurebox.core;
using Figurebox.Utils;
using HarmonyLib;

namespace Figurebox;

public class KingdomYearName
{
    public static void Make_New_YearName(AW_Kingdom kingdom)
    {
        string yearName;
        if (kingdom.king == null)
        {
            return;
        }

        if (kingdom.policy_data.Title == KingdomPolicyData.KingdomTitle.Emperor)
        {
            // 帝国级别的命名规则
            string KYN_F = WordLibraryManager.GetRandomWord("国号前");
            string KYN_R = WordLibraryManager.GetRandomWord("国号后");
            yearName = KYN_F + KYN_R;
        }
        else
        {
            // 其他等级的命名规则
            string singleCharTitle = AW_Kingdom.GetSingleCharacterTitle(kingdom.policy_data.Title); // 返回 "公"

            yearName = kingdom.name + singleCharTitle + kingdom.king.getNameCharacter();
        }

        kingdom.policy_data.year_name = yearName;
        kingdom.policy_data.year_start_timestamp = World.world.getCurWorldTime();
    }



    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapText), "showTextKingdom")]
    public static void ShowTextKingdom_Title(MapText __instance, ref Kingdom pKingdom)
    {
        AW_Kingdom awKingdom = pKingdom as AW_Kingdom;
        if (awKingdom != null)
        {
            // 获取爵位头衔
            string titleString = awKingdom.GetTitleString(awKingdom.policy_data.Title);
            string Text = pKingdom.name + titleString + "  " + pKingdom.getPopulationTotal().ToString();

            // 将爵位头衔添加到王国名称之后
            __instance.setText(Text, pKingdom.capital.cityCenter);
        }


    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapText), "showTextKingdom")]
    public static void ShowTextKingdom_Postfix(MapText __instance, Kingdom pKingdom)
    {
        AW_Kingdom awKingdom = pKingdom as AW_Kingdom;

        // 检查pKingdom是否为null，避免空引用异常
        if (awKingdom == null || string.IsNullOrEmpty(awKingdom.policy_data.year_name))
        {
            return;
        }

        // 获取年号信息
        int yearcal = World.world.mapStats.getYearsSince(awKingdom.policy_data.year_start_timestamp);
        string nianhao = awKingdom.policy_data.year_name;
        string date = (yearcal == 0) ? "元年" : yearcal + 1 + "年"; // 当年数为0时显示为“元年”
        string yearInfo = " | " + nianhao + date;

        // 获取现有的文本
        string existingText = __instance.text.text;
        string final_text = "";
        // 检查现有文本是否已包含年号信息
        if (!existingText.Contains(" | " + nianhao)) // 检查是否已包含该年号
        {
            // 将年号添加到现有文本中
            final_text = existingText + yearInfo;
        }
        else
        {
            // 如果已包含年号，只更新年数
            int startIndex = existingText.IndexOf(" | " + nianhao) + (" | " + nianhao).Length;
            int endIndex = existingText.IndexOf("年", startIndex);
            if (endIndex != -1)
            {
                final_text = existingText.Substring(0, startIndex) + date + existingText.Substring(endIndex + 1);
            }
        }

        __instance.setText(final_text, pKingdom.capital.cityCenter);
    }

    public static void changeYearname(AW_Kingdom __instance)
    {
        KingdomPolicyAsset policy = KingdomPolicyLibrary.Instance.get("kingdom_yearname");

        __instance.StartPolicy(policy, true);
    }
}
#endif