using System.Collections.Generic;

namespace Figurebox.abstracts;

public abstract class ExtendedLibrary<T> where T : Asset
{
    public List<T> added_assets = new();
    private AssetLibrary<T> cached_library;
    protected T t;

    public ExtendedLibrary()
    {
        Instance = this;
        init();
    }

    public static ExtendedLibrary<T> Instance { get; private set; }

    protected virtual T add(T pObj)
    {
        cached_library ??= (AssetLibrary<T>)AssetManager.instance.list.Find(l => l is AssetLibrary<T>);
        t = pObj;
        added_assets.Add(pObj);
        return cached_library.add(pObj);
    }

    protected abstract void init();
}