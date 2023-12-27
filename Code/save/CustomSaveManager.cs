using System.IO;
using System.Linq;
using HarmonyLib;
using NCMS.Utils;
using Newtonsoft.Json;
using UnityEngine;
namespace Figurebox.Save;

public class CustomSaveManager
{

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SaveManager), "saveToCurrentPath")]
    public static void SaveMapDataPostfix()
    {
        Debug.Log("保存自定义数据");
        string savePath = SaveManager.currentSavePath;
        string customDataPath = Path.Combine(savePath, "customData.json");
        string warDataSavePath = Path.Combine(savePath, "WarDataSave.json");


        CustomSaveData customData = new()
        {
            DkingYearData = FunctionHelper.kingYearData,
            DYearData = FunctionHelper.YearData,
            DTmkingData = FunctionHelper.TmkingData,
            Dtianmingvalue = FunctionHelper.tianmingvalue,
            DkingdomYearNameData = FunctionHelper.kingdomYearNameData,
            DkingdomCityData = FunctionHelper.kingdomCityData,
            DkingdomCityNameyData = FunctionHelper.kingdomCityNameyData,
            DCityYearData = FunctionHelper.CityYearData,
            DKingStartYearInKingdom = FunctionHelper.KingStartYearInKingdom,
            DKingName = FunctionHelper.KingName,
            DKingKingdomName = FunctionHelper.KingKingdomName,
            DKingKingdoms = FunctionHelper.KingKingdoms,
            Dkingdomids = FunctionHelper.kingdomids,
            DKingEndYearInKingdom = FunctionHelper.KingEndYearInKingdom,
            DkingdomVassalEstablishmentTime = FunctionHelper.kingdomVassalEstablishmentTime,
            DkingdomVassalEndTime = FunctionHelper.kingdomVassalEndTime
        };
        WarDataSave warDataSave = new()
        {
            DwarIdNameDict = FunctionHelper.warIdNameDict,
            DWarStartDate = FunctionHelper.WarStartDate, // 将 war 的开始日期数据赋值到 WarStartDate，
            DAttackers = FunctionHelper.Attackers, // 将攻击者数据赋值到 Attackers，
            DDefenders = FunctionHelper.Defenders,
            DWarEndDate = FunctionHelper.WarEndDate,
            DWarEndDateFloat = FunctionHelper.WarEndDateFloat

        };
        File.WriteAllText(warDataSavePath, JsonConvert.SerializeObject(warDataSave, Formatting.Indented));
        //settings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii; // 添加这一行
        File.WriteAllText(customDataPath, JsonConvert.SerializeObject(customData, Formatting.Indented));

        string backupFilePath = Path.Combine(savePath, "customData_backup.json");
        File.Copy(customDataPath, backupFilePath, true);
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapBox), "generateNewMap")]
    public static void createMapPostfix()
    {
        cleanData();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SaveManager), "loadData")]
    public static void LoadDataPostfix(SavedMap pData)
    {
        Debug.Log("加载自定义数据");

        int curslot = SaveManager.currentSlot + 1;
        string savePath = SaveManager.currentSavePath;
        Debug.Log(savePath);

        string customDataPath = Path.Combine(savePath, "customData.json");
        string warDataSavePath = Path.Combine(savePath, "WarDataSave.json");


        if (File.Exists(warDataSavePath))
        {
            string jsonData = File.ReadAllText(warDataSavePath);

            // 从 JSON 数据反序列化出 warDataSave 对象
            WarDataSave warDataSave = JsonConvert.DeserializeObject<WarDataSave>(jsonData);

            FunctionHelper.WarStartDate = warDataSave.DWarStartDate;
            FunctionHelper.Attackers = warDataSave.DAttackers;
            FunctionHelper.Defenders = warDataSave.DDefenders;
            FunctionHelper.warIdNameDict = warDataSave.DwarIdNameDict;
            FunctionHelper.WarEndDateFloat = warDataSave.DWarEndDateFloat;
            FunctionHelper.WarEndDate = warDataSave.DWarEndDate;
        }

        if (File.Exists(customDataPath))
        {
            string jsonData = File.ReadAllText(customDataPath);


            // settings.Converters.Add(new CityYearKeyTypeConverter());

            CustomSaveData customData = JsonConvert.DeserializeObject<CustomSaveData>(jsonData);
            cleanData();
            FunctionHelper.kingYearData = customData.DkingYearData;
            FunctionHelper.YearData = customData.DYearData;
            FunctionHelper.TmkingData = customData.DTmkingData;
            FunctionHelper.tianmingvalue = customData.Dtianmingvalue;
            FunctionHelper.kingdomYearNameData = customData.DkingdomYearNameData;
            FunctionHelper.kingdomCityData = customData.DkingdomCityData;
            FunctionHelper.kingdomCityNameyData = customData.DkingdomCityNameyData;
            FunctionHelper.CityYearData = customData.DCityYearData;
            FunctionHelper.KingStartYearInKingdom = customData.DKingStartYearInKingdom;
            FunctionHelper.KingName = customData.DKingName;
            FunctionHelper.KingKingdomName = customData.DKingKingdomName;
            FunctionHelper.KingKingdoms = customData.DKingKingdoms;
            FunctionHelper.kingdomids = customData.Dkingdomids;
            FunctionHelper.KingEndYearInKingdom = customData.DKingEndYearInKingdom;
            FunctionHelper.kingdomVassalEstablishmentTime = customData.DkingdomVassalEstablishmentTime;
            FunctionHelper.kingdomVassalEndTime = customData.DkingdomVassalEndTime;
        }

    }


    public static void cleanData()
    {
        Debug.Log("清理数据");
        FunctionHelper.kingYearData.Clear();
        FunctionHelper.YearData.Clear();
        FunctionHelper.TmkingData.Clear();
        FunctionHelper.tianmingvalue = 0;
        FunctionHelper.kingdomYearNameData.Clear();
        FunctionHelper.kingdomCityData.Clear();
        FunctionHelper.kingdomCityNameyData.Clear();
        FunctionHelper.CityYearData.Clear();
        FunctionHelper.KingName.Clear();
        FunctionHelper.KingStartYearInKingdom.Clear();
        FunctionHelper.KingKingdomName.Clear();
        FunctionHelper.KingKingdoms.Clear();
        FunctionHelper.kingdomids.Clear();
        FunctionHelper.warIdNameDict.Clear();
        FunctionHelper.WarStartDate.Clear();
        FunctionHelper.Attackers.Clear();
        FunctionHelper.Defenders.Clear();
        FunctionHelper.WarEndDateFloat.Clear();
        FunctionHelper.WarEndDate.Clear();
        FunctionHelper.KingEndYearInKingdom.Clear();
        FunctionHelper.kingdomVassalEstablishmentTime.Clear();
        FunctionHelper.kingdomVassalEndTime.Clear();
        KingdomVassals.bannerLoaders.Clear();

        foreach (var Toggleitem in PowerButtons.ToggleValues.ToList())
        {
            if (PowerButtons.GetToggleValue(Toggleitem.Key))
            {

                PowerButtons.ToggleButton(Toggleitem.Key);
            }
        }
    }
}