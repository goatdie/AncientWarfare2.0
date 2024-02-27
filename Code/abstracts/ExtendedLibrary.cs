using System.Collections.Generic;
using System.Reflection;

namespace Figurebox.abstracts;

public abstract class ExtendedLibrary<T> where T : Asset
{
    public    List<T>         added_assets = new();
    private   AssetLibrary<T> cached_library;
    protected T               t;
    private   Dictionary<string, FieldInfo> _fields = new();
    protected ExtendedLibrary()
    {
        Instance = this;

        var fields = GetType().GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
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
    private void _set_field(T pObj)
    {
        if (_fields.TryGetValue(pObj.id, out FieldInfo field))
        {
            field.SetValue(null, pObj);
        }
    }
    protected abstract void init();
}