using System.Collections.Generic;
using System.Reflection;

namespace Figurebox.abstracts;

public class AW_AssetLibrary<TAsset, TLibrary> : AssetLibrary<TAsset> where TAsset : Asset
    where TLibrary : AW_AssetLibrary<TAsset, TLibrary>, new()
{
    public static TLibrary Instance { get; } = new TLibrary();

    private Dictionary<string, FieldInfo> _fields = new();
    public AW_AssetLibrary() : base()
    {
        var fields = GetType().GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(TAsset))
            {
                _fields.Add(field.Name, field);
            }
        }
    }

    public override TAsset get(string pID)
    {
        if (string.IsNullOrEmpty(pID))
        {
            Main.LogDebug($"{id}.get: pID is null or empty.", true, true);
            return null;
        }

        return base.get(pID);
    }
    public override TAsset add(TAsset pAsset)
    {
        _set_field(pAsset);
        return base.add(pAsset);
    }
    private void _set_field(TAsset pObj)
    {
        if (_fields.TryGetValue(pObj.id, out FieldInfo field))
        {
            field.SetValue(null, pObj);
        }
    }
}