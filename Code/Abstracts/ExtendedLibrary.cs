using System.Collections.Generic;
using System.Reflection;

namespace AncientWarfare.Abstracts;

public abstract class ExtendedLibrary<T> where T : Asset
{
    protected Dictionary<string, FieldInfo> _fields      = new();
    public    List<T>                       added_assets = new();
    private   AssetLibrary<T>               cached_library;
    protected T                             t;

    protected ExtendedLibrary()
    {
        I = this;

        var fields = GetType().GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(T))
            {
                _fields.Add(field.Name, field);
            }
        }

        cached_library ??= find_target_library();

        init();
    }

    public static ExtendedLibrary<T> I { get; private set; }

    protected virtual AssetLibrary<T> find_target_library()
    {
        return (AssetLibrary<T>)AssetManager.instance.list.Find(l => l is AssetLibrary<T>);
    }

    protected virtual T get(string pId)
    {
        return cached_library.get(pId);
    }

    protected void init_fields()
    {
        if (cached_library == null) return;
        foreach (var asset in cached_library.list)
        {
            set_field(asset);
        }
    }

    protected virtual T add(T pObj)
    {
        t = pObj;
        added_assets.Add(pObj);

        set_field(pObj);

        return cached_library.add(pObj);
    }

    protected virtual T clone(string pNew, string pFrom)
    {
        var pObj = cached_library.clone(pNew, pFrom);

        set_field(pObj);
        return pObj;
    }

    protected virtual void replace(T pNew)
    {
        if (cached_library.dict.TryGetValue(pNew.id, out T old)) cached_library.list.Remove(old);

        cached_library.list.Add(pNew);
        cached_library.dict[pNew.id] = pNew;

        set_field(pNew);
    }

    protected virtual void set_field(T pObj)
    {
        if (_fields.TryGetValue(pObj.id, out FieldInfo field))
        {
            field.SetValue(null, pObj);
        }
    }

    protected abstract void init();
}