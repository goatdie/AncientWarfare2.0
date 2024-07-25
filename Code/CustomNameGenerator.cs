using System;
using Random = UnityEngine.Random;
#if 一米_中文名
using System.Collections.Generic;
using Chinese_Name;
#endif

namespace AncientWarfare;
#if 一米_中文名
/*
internal static class NameGeneratorInitialzier
{
    static void NewActorNameGeneratorDefaultParameterGetter(Actor pActor, Dictionary<string, string> pParameters)
    {
        AW_Kingdom awKingdom = pActor.kingdom as AW_Kingdom;
        if (awKingdom != null && awKingdom.NameIntegration)
        {
            string familyName;
            string clanName;
            string ChineseFamilyName;

            pActor.data.get("family_name", out familyName, "");
            pActor.data.get("clan_name", out clanName, "");
            pActor.data.get("chinese_family_name", out ChineseFamilyName, "");

            if (!string.IsNullOrEmpty(clanName) && clanName.Equals(familyName) &&
                string.IsNullOrEmpty(ChineseFamilyName))
            {
                pActor.data.set("family_name", clanName);
                pActor.data.set("chinese_family_name", clanName);
                if (pActor.hasClan())
                {
                    pActor.getClan().data.set("clan_chinese_family_name", clanName);
                }

                pActor.data.set("name_set", true);
            }

            // 当 clan_name 存在，且与 family_name 不同，同时 chinese_family_name 为空时，使用 clan_name 更新其他名称
            if (!string.IsNullOrEmpty(clanName) && !clanName.Equals(familyName) &&
                string.IsNullOrEmpty(ChineseFamilyName))
            {
                pActor.data.set("family_name", clanName);
                pActor.data.set("chinese_family_name", clanName);
                if (pActor.hasClan())
                {
                    pActor.getClan().data.set("clan_chinese_family_name", clanName);
                }

                pActor.data.set("name_set", true);
            }
            else if (string.IsNullOrEmpty(familyName) && string.IsNullOrEmpty(clanName) &&
                     string.IsNullOrEmpty(ChineseFamilyName))
            {
                familyName = WordLibraryManager.GetRandomWord("氏");
                clanName = familyName;
                pActor.data.set("chinese_family_name", familyName);
                pActor.data.set("family_name", familyName);
                pActor.data.set("clan_name", familyName);
            }

            if (string.IsNullOrEmpty(familyName) && string.IsNullOrEmpty(clanName) &&
                !string.IsNullOrEmpty(ChineseFamilyName))
            {
                familyName = ChineseFamilyName;
                pActor.data.set("chinese_family_name", familyName);
                pActor.data.set("family_name", familyName);
                pActor.data.set("clan_name", familyName);
            }

            pParameters["family_name"] = familyName;
            pParameters["clan_name"] = clanName;
            if (string.IsNullOrEmpty(pParameters["clan_name"]))
            {
                pParameters["clan_name"] = pParameters["family_name"];
            }

            pActor.data.set("clan_name", pParameters["clan_name"]);
        }
        else
        {
            pParameters["family_name"] = "";
        }
    }

    static void Culture_city_parameter_getter(Culture pCulture, Dictionary<string, string> pParameters)
    {
        if (pCulture != null)
        {
            pParameters["village_origin"] = pCulture.data.village_origin;
        }
    }

    public static void init()
    {
        NewActorNameGenerator generator = new NewActorNameGenerator("Xia_name", "default");
        generator.AddTemplate("$family_name${中文名字}", 1);
        generator.AddTemplate("$family_name${中文名字}{中文名字}", 1);
        generator.AddTemplate("{中文名字}{千字文}", 1);
        generator.AddTemplate("{千字文}", 1);
        CN_NameGeneratorLibrary.Submit(generator);

        ParameterGetters.PutActorParameterGetter("default",
            (ParameterGetter<Actor>)Delegate.Combine(
                ParameterGetters.GetActorParameterGetter("default"), NewActorNameGeneratorDefaultParameterGetter));
        ParameterGetters.PutCultureParameterGetter("default", Culture_city_parameter_getter);
    }

    class NewActorNameGenerator : CN_NameGeneratorAsset
    {
        public NewActorNameGenerator(string id, string parameter_getter)
        {
            this.id = id;
            this.parameter_getter = parameter_getter;
            templates ??= new();
        }

        public override string GenerateName(Dictionary<string, string> pParameters)
        {
            // 下标从0开始
            // 这里默认了第一个是有姓的, 第二个是无姓的, 你可以自己写逻辑
            if (pParameters.ContainsKey("family_name") && !string.IsNullOrEmpty(pParameters["family_name"]))
            {
                int index1 = Toolbox.randomInt(0, 2); // 生成0或1的随机数
                return templates[index1].GenerateName(pParameters);
            }

            int index = Random.Range(2, 4); // 生成2或3的随机数
            return templates[index].GenerateName(pParameters);
            // 从这里出去后, 中文名会自动把family_name参数的值送给Actor.data中的"chinese_family_name"
            // 如果要记录"氏", 你需要在`NewActorNameGeneratorDefaultParameterGetter`里自己生成并送给Actor
        }

        public void AddTemplate(string format, float weight)
        {
            templates.Add(CN_NameTemplate.Create(format, weight));
        }
    }
}
*/
#else
internal static class NameGeneratorInitialzier{


    public static void init(){
          
      
       
        // 当中文名不存在时的解决方案, 如果不想给的话, 推荐将中文名设置为硬依赖
        // 设置为硬依赖后, #if 和 #endif 块都可以去除
        // 但当中文名没安装时, 春秋就不会被编译

    }
}
#endif