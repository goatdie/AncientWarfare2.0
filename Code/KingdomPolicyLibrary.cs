namespace Figurebox;

class KingdomPolicyLibrary : AssetLibrary<KingdomPolicyAsset>
{
    public static KingdomPolicyLibrary Instance { get; } = new();
    // 这里可以根据需要添加特定于 KingdomPolicyAsset 的方法
    public override void init()
    {
        base.init();
        add(new KingdomPolicyAsset
        {
            id = "hello_world"
        }).execute_policy = (pPolicy, pKingdom, pPolicyData, pState) => //这个玩意也可以独立在外边写
        {
            Main.LogInfo($"{(pKingdom.king == null ? "no king" : pKingdom.king.getName())}: Hello World! 当前政策id: {pPolicyData.current_policy_id}, 当前政策状态: {pPolicyData.p_status}, 当前政策剩余进度: {pPolicyData.p_progress}, 当前政治状态id: {pState.id}");
        };
    }
    // 例如，执行所有政策的方法
    public void ExecuteAllPolicies(Kingdom kingdom, KingdomPolicyStateAsset state)
    {

    }

    // 根据ID加载政策资产的方法
    public void LoadPolicyAssetsFromFile(string filePath)
    {
        // 从文件中加载政策资产的逻辑
        // 假设有一个方法从文件中读取政策数据，并转换为 KingdomPolicyAsset 对象
    }

    // 根据ID保存政策资产的方法
    public void SavePolicyAssetsToFile(string filePath)
    {
        // 将政策资产保存到文件的逻辑
        // 假设有一个方法将 KingdomPolicyAsset 对象转换为适合保存的数据格式
    }
}