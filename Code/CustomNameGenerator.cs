#if 一米_中文名
using System.Collections.Generic;
using Chinese_Name;
#endif
namespace Figurebox;
#if 一米_中文名
internal static class NameGeneratorInitialzier
{
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
            int index = UnityEngine.Random.Range(2, 4); // 生成2或3的随机数
            return templates[index].GenerateName(pParameters);
            // 从这里出去后, 中文名会自动把family_name参数的值送给Actor.data中的"chinese_family_name"
            // 如果要记录"氏", 你需要在`NewActorNameGeneratorDefaultParameterGetter`里自己生成并送给Actor
        }
        public void AddTemplate(string format, float weight)
        {
            templates.Add(CN_NameTemplate.Create(format, weight));
        }
    }

    static void NewActorNameGeneratorDefaultParameterGetter(Actor pActor, Dictionary<string, string> pParameters)
    {
        if (pActor == null)
        {                              // 自己的逻辑pActor.kingdom.canHasFamilyName()
            string familyName;
            string clanName;

            pActor.data.get("family_name", out familyName, "");
            pActor.data.get("clan_name", out clanName, "");
            if (string.IsNullOrEmpty(familyName) || string.IsNullOrEmpty(clanName))
            {
                familyName = WordLibraryManager.GetRandomWord("氏");
                clanName = familyName;
                pActor.data.set("chinese_family_name", familyName);
                pActor.data.set("family_name", familyName);
                pActor.data.set("clan_name", familyName);

            }
            pParameters["family_name"] = familyName; // 从data 里取
            pParameters["clan_name"] = clanName;          // 从data 里取
            if (string.IsNullOrEmpty(pParameters["clan_name"]))
            {
                // 当氏不存在时, 直接拿姓来用, 当然也可以写别的自己的获取什么的
                pParameters["clan_name"] = pParameters["family_name"];
            }
            // 将氏存到data里, 这样可以方便传承以及其他地方的使用
            pActor.data.set("clan_name", pParameters["clan_name"]);            // 以"AW_"作为key的前缀, 可以一定程度上避免和其他模组冲突
        }
        else
        {
            pParameters["family_name"] = "";
        }
    }
    public static void init()
    {
        NewActorNameGenerator generator = new NewActorNameGenerator("Xia_name", "default");
        generator.AddTemplate("$family_name${千字文}", 1);
        generator.AddTemplate("$family_name${千字文}{千字文}", 1);
        generator.AddTemplate("{千字文}{千字文}", 1);
        generator.AddTemplate("{千字文}", 1);
        CN_NameGeneratorLibrary.Submit(generator);

        ParameterGetters.PutActorParameterGetter("default", NewActorNameGeneratorDefaultParameterGetter);
    }
}
#else
internal static class NameGeneratorInitialzier{


    public static void init(){

        // 当中文名不存在时的解决方案, 如果不想给的话, 推荐将中文名设置为硬依赖
        // 设置为硬依赖后, #if 和 #endif 块都可以去除
        // 但当中文名没安装时, 春秋就不会被编译
    }
}
#endif