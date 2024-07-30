using System.Text;
using AncientWarfare.Const;
using AncientWarfare.Core.Force;
using AncientWarfare.Core.Quest;
using AncientWarfare.UI.Prefabs;
using NeoModLoader.api;
using UnityEngine;
using UnityEngine.UI;

namespace AncientWarfare.UI.Windows
{
    public class TribeInfoWindow : AbstractWideWindow<TribeInfoWindow>
    {
        public Text Title     { get; private set; }
        public Text BaseInfo  { get; private set; }
        public Text QuestInfo { get; private set; }
        public ScrollWindow mScrollWindow { get; private set; }
        public Tribe mTribe { get; private set; }

        protected override void Init()
        {
            mScrollWindow = GetComponent<ScrollWindow>();
            Title = mScrollWindow.titleText;
            Title.GetComponent<LocalizedText>().autoField = false;

            RawText baseinfo_component = Instantiate(RawText.Prefab, BackgroundTransform);
            baseinfo_component.transform.localPosition = new Vector3(-100, 0);
            baseinfo_component.transform.localScale = Vector3.one;
            baseinfo_component.SetSize(new Vector2(200, 240));
            baseinfo_component.name = nameof(BaseInfo);
            BaseInfo = baseinfo_component.Text;

            RawText questinfo_component = Instantiate(RawText.Prefab, BackgroundTransform);
            questinfo_component.transform.localPosition = new Vector3(100, 0);
            questinfo_component.transform.localScale = Vector3.one;
            questinfo_component.SetSize(new Vector2(200, 240));
            questinfo_component.name = nameof(BaseInfo);
            QuestInfo = questinfo_component.Text;
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
            foreach (var actor_id in mTribe.Data.members)
            {
                info_builder.AppendLine($"{actor_id}: {World.world.units.get(actor_id).getName()}");
            }

            info_builder.AppendLine("资源:");
            foreach (var res in mTribe.Data.storage.GetResDict())
            {
                info_builder.AppendLine($"{res.Key}: {res.Value}");
            }

            BaseInfo.text = info_builder.ToString();

            StringBuilder quest_builder = new("任务:\n");
            var idx = 0;
            foreach (QuestInst quest in mTribe.quests)
                quest_builder.AppendLine($"[{idx++}]: {quest.asset.id}(Active?{quest.Active})");

            QuestInfo.text = quest_builder.ToString();
        }
    }
}