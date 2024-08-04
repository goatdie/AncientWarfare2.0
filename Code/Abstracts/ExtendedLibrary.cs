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
        Instance = this;

        var fields = GetType().GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(T))
            {
                _fields.Add(field.Name, field);
            }
        }

        init();
    }

    public static ExtendedLibrary<T> Instance { get; private set; }

    private void _init()
    {
        cached_library ??= (AssetLibrary<T>)AssetManager.instance.list.Find(l => l is AssetLibrary<T>);
    }

    protected virtual T get(string pId)
    {
        _init();
        return cached_library.get(pId);
    }

    protected void init_fields()
    {
        _init();
        if (cached_library == null) return;
        foreach (var asset in cached_library.list)
        {
            _set_field(asset);
        }
    }

    protected virtual T add(T pObj)
    {
        _init();
        t = pObj;
        added_assets.Add(pObj);

        _set_field(pObj);

        return cached_library.add(pObj);
    }

    protected virtual T clone(string pNew, string pFrom)
    {
        _init();

        var pObj = cached_library.clone(pNew, pFrom);

        _set_field(pObj);
        return pObj;
    }

    protected virtual void replace(T pNew)
    {
        _init();
        if (cached_library.dict.TryGetValue(pNew.id, out T old)) cached_library.list.Remove(old);

        cached_library.list.Add(pNew);
        cached_library.dict[pNew.id] = pNew;

        _set_field(pNew);
    }

    protected virtual void _set_field(T pObj)
    {
        if (_fields.TryGetValue(pObj.id, out FieldInfo field))
        {
            field.SetValue(null, pObj);
        }
    }

    protected abstract void init();
}