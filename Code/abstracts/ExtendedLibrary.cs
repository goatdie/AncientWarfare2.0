using System.Collections.Generic;

namespace Figurebox.abstracts;

public abstract class ExtendedLibrary<T> where T : Asset
{
    public    List<T>         added_assets = new();
    private   AssetLibrary<T> cached_library;
    protected T               t;

    protected ExtendedLibrary()
    {
        Instance = this;
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
        return cached_library.add(pObj);
    }

    protected virtual T clone(string pNew, string pFrom)
    {
        _init();
        return cached_library.clone(pNew, pFrom);
    }

    protected virtual void replace(T pNew)
    {
        _init();
        if (cached_library.dict.TryGetValue(pNew.id, out T old)) cached_library.list.Remove(old);

        cached_library.list.Add(pNew);
        cached_library.dict[pNew.id] = pNew;
    }

    protected abstract void init();
}