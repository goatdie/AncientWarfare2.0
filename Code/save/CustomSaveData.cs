using System;
using System.Collections.Generic;
namespace Figurebox.Save;

internal class CustomSaveData
{
    public Dictionary<string, string> DkingdomCityData = new();
    public Dictionary<string, string> DkingdomCityNameyData = new();
    public List<string> Dkingdomids = new();
    public Dictionary<string, int> DkingdomVassalEndTime = new();
    public Dictionary<string, int> DkingdomVassalEstablishmentTime = new();
    public Dictionary<string, string> DkingdomYearNameData = new();
    public Dictionary<string, int> DKingEndYearInKingdom = new();
    public Dictionary<string, List<string>> DKingKingdomName = new();
    public Dictionary<string, List<string>> DKingKingdoms = new();
    public Dictionary<string, int> DKingStartYearInKingdom = new();
    public Dictionary<string, int> DkingYearData = new();


    public int Dtianmingvalue = 0;
    public Dictionary<string, string> DTmkingData = new();
    public Dictionary<string, int> DYearData = new();
    //[JsonConverter(typeof(CityYearKeyDictionaryConverter))]
    public Dictionary<string, Dictionary<string, Tuple<int, int, int>>> DCityYearData { get; set; }
    public Dictionary<string, string> DKingName { get; set; }
}

internal class WarDataSave
{
    public Dictionary<string, List<string>> DAttackers = new();
    public Dictionary<string, List<string>> DDefenders = new();
    public Dictionary<string, int> DWarEndDate = new();
    public Dictionary<string, double> DWarEndDateFloat = new();
    public Dictionary<string, string> DwarIdNameDict = new();
    public Dictionary<string, double> DWarStartDate = new();
}