namespace AncientWarfare.Core.Extensions;

public static class BaseSystemDataExtension
{
    public static string GetString(this BaseSystemData data, string key)
    {
        data.get(key, out string result);
        return result;
    }

    public static int GetInt(this BaseSystemData data, string key)
    {
        data.get(key, out int result);
        return result;
    }

    public static float GetFloat(this BaseSystemData data, string key)
    {
        data.get(key, out float result);
        return result;
    }

    public static bool GetBool(this BaseSystemData data, string key)
    {
        data.get(key, out bool result);
        return result;
    }
}