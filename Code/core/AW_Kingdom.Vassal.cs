using System;
using System.Collections.Generic;
using System.Linq;
using Figurebox.attributes;
using Figurebox.constants;
using Figurebox.core.dbs;
using Figurebox.core.events;
using Figurebox.Utils;
using Figurebox.Utils.MoH;

namespace Figurebox.core;

public partial class AW_Kingdom
{
    /// <summary>
    ///   宗主国
    /// </summary>
    public AW_Kingdom suzerain { get; private set; }
    /// <summary>
    ///   吞并的时间戳
    /// </summary>
    public double absorb_timestamp;

    /// <summary>
    ///   附庸
    /// </summary>
    public List<AW_Kingdom> vassals { get; private set; } = new List<AW_Kingdom>();

    public bool IsVassal()
    {
        return suzerain != null;
    }
    /// <summary>
    ///   应对特殊清空的清空宗主国
    /// </summary>
    public void ClearSuzerain()
    {
        suzerain = null;
    }
    public AW_Kingdom GetSuzerain()
    {
        return IsVassal() ? suzerain : null;
    }
    public List<AW_Kingdom> GetVassals()
    {
        return IsSuzerain() ? vassals : null;
    }
    public bool IsSuzerain()
    {
        return vassals.Count > 0;
    }
    public int getSuzerainArmy()
    {
        int armyCount = suzerain.getArmy();

        if (IsSuzerain())
        {
            List<AW_Kingdom> vassals = GetVassals();

            foreach (Kingdom vassal in vassals)
            {
                armyCount += vassal.getArmy();
            }
        }


        return armyCount;
    }

    public void SetVassal(AW_Kingdom lord)
    {
        if (IsVassal())
        {
            // 如果是，那么它不能成为宗主国，所以我们直接返回

            return;
        }
        if (IsSuzerain())
        {
            // If it is, it can't become a vassal, so we simply return

            return;
        }
        if (king == null)
        {

            return;
        }

        string kingdomId = lord.data.id;
        data.set("originalColorID", data.colorID);
        ColorAsset originalColor = getColor();
        string serializedOriginalColor = Serialize(originalColor);

        // 将序列化后的字符串存储在kingdom的data字典中
        data.set("originalColor", serializedOriginalColor);

        addition_data.suzerain_id = kingdomId;
        suzerain = lord;
        Main.LogInfo($"Before adding, lord has {lord.vassals.Count} vassals.");
        lord.vassals.Add(this);
        Main.LogInfo($"After adding, lord has {lord.vassals.Count} vassals.");
        if (this.Rebel) { Rebel = false; }
        EventsManager.Instance.StartVassal(this, lord);
        data.colorID = lord.data.colorID;
        ColorAsset lordcolor = lord.getColor();
        updateColor(lordcolor);
        World.world.zoneCalculator.setDrawnZonesDirty();
        World.world.zoneCalculator.clearCurrentDrawnZones();
        if (World.world.wars.isInWarWith(this, lord))
        {
            // 如果存在，结束它
            War ongoingWar = World.world.wars.getWar(this, lord, false);
            if (ongoingWar != null)
            {
                World.world.wars.endWar(ongoingWar);
            }
        }
        if (lord.hasAlliance())
        {
            lord.getAlliance().join(this, true);
        }
    }
    public void UpdateToKingdomColor()
    {


        // 更新王国的颜色ID和颜色资产
        data.colorID = suzerain.data.colorID;
        ColorAsset lordcolor = suzerain.getColor();
        updateColor(lordcolor);

        // 通知世界区域计算器，颜色变更了
        World.world.zoneCalculator.setDrawnZonesDirty();
        World.world.zoneCalculator.clearCurrentDrawnZones(true);
        World.world.zoneCalculator.redrawZones();
    }


    public void RemoveSuzerain()
    {
        // 如果是vassal，清除附庸的数据
        if (IsVassal())
        {

            EventsManager.Instance.ENDVassal(this, suzerain, false);
            suzerain.vassals.Remove(this);
            suzerain = null;

            // 更新颜色
            UpdateColor(this);
        }

        // 清除旗帜

    }
    public void RemoveVassals()
    {
        // 清除附庸的数据
        if (IsSuzerain())
        {
            // 使用ToList()创建一个临时列表的副本进行遍历
            var tempVassals = GetVassals().ToList();
            foreach (var vassal in tempVassals)
            {
                vassal.RemoveSuzerain();
            }
            this.vassals.Clear();
        }
    }

    public static string Serialize(ColorAsset colorAsset)
    {
        return $"{colorAsset.color_main},{colorAsset.color_main_2},{colorAsset.color_banner},{colorAsset.index_id}";
    }
    public static void UpdateColor(Kingdom kingdom)
    {
        // 获取并检查原始颜色
        string serializedOriginalColor = "";
        kingdom.data.get("originalColor", out serializedOriginalColor);
        if (!string.IsNullOrEmpty(serializedOriginalColor))
        {
            ColorAsset originalColor = Deserialize(serializedOriginalColor);
            kingdom.updateColor(originalColor);
            World.world.zoneCalculator.setDrawnZonesDirty();
            World.world.zoneCalculator.clearCurrentDrawnZones(true);
            World.world.zoneCalculator.redrawZones();
        }

        // 获取并检查原始颜色ID
        int originalColorID = -1;
        kingdom.data.get("originalColorID", out originalColorID);
        if (originalColorID != -1)
        {
            kingdom.data.colorID = originalColorID;
        }
    }
    public static ColorAsset Deserialize(string s)
    {
        var parts = s.Split(',');
        if (parts.Length != 4)
        {
            throw new ArgumentException("Invalid input string");
        }
        var pColorMain = parts[0];
        var pColorMain2 = parts[1];
        var pColorBanner = parts[2];
        var indexId = int.Parse(parts[3]);

        var result = new ColorAsset(pColorMain, pColorMain2, pColorBanner)
        {
            index_id = indexId
        };

        result.initColor(); // 重新计算颜色值
        return result;
    }

}