using AncientWarfare.Const;
using AncientWarfare.Core.Force;
using NeoModLoader.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AncientWarfare.UI.Windows
{
    public class TribeInfoWindow : AbstractWideWindow<TribeInfoWindow>
    {
        public Text Title { get; private set; }
        public Text Info { get; private set; }
        public ScrollWindow mScrollWindow { get; private set; }
        public Tribe mTribe { get; private set; }
        protected override void Init()
        {
            mScrollWindow = GetComponent<ScrollWindow>();
            Title = mScrollWindow.titleText;
            Title.GetComponent<LocalizedText>().autoField = false;

            Info = new GameObject("Info", typeof(Text)).GetComponent<Text>();
            Info.transform.SetParent(BackgroundTransform);
            Info.color = Color.white;
            Info.resizeTextForBestFit = true;
            Info.resizeTextMaxSize = 12;
            Info.resizeTextMinSize = 8;
            Info.alignment = TextAnchor.UpperLeft;
            Info.transform.localPosition = Vector3.zero;
            Info.transform.localScale = Vector3.one;
            Info.GetComponent<RectTransform>().sizeDelta = new(200, 240);
        }
        public override void OnNormalDisable()
        {
            mTribe = null;
            AdditionConfig.selectedTribe = null;
        }
        public override void OnNormalEnable()
        {
            mTribe = AdditionConfig.selectedTribe;
            if (mTribe == null) return;

            Title.text = AdditionConfig.selectedTribe.GetName();
            StringBuilder info_builder = new(
$"""
ID: {mTribe.BaseData.id}
人数: {mTribe.Data.members.Count}
""");
            foreach(var actor_id in mTribe.Data.members)
            {
                info_builder.AppendLine($"{actor_id}: {World.world.units.get(actor_id).getName()}");
            }
            info_builder.AppendLine("资源:");
            foreach(var res in mTribe.Data.storage.resources)
            {
                info_builder.AppendLine($"{res.Key}: {res.Value}");
            }
            Info.text = info_builder.ToString();
        }
    }
}
