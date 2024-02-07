using System.Collections.Generic;

namespace Figurebox.abstracts;

public abstract class ExtendedLibrary<T> where T : Asset
{
    public List<T> added_assets = new();
    private AssetLibrary<T> cached_library;
    protected T t;

    protected ExtendedLibrary()
    {
        Instance = this;
        init();
    }
    private void _init()
    {
        cached_library ??= (AssetLibrary<T>)AssetManager.instance.list.Find(l => l is AssetLibrary<T>);
    }
    public static ExtendedLibrary<T> Instance { get; private set; }

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

    protected abstract void init();
}