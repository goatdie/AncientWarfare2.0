namespace Figurebox.abstracts;

public abstract class ExtendedLibrary<T> where T : Asset
{
    private AssetLibrary<T> cached_library;
    public ExtendedLibrary<T> Instance;
    protected T t;

    public ExtendedLibrary()
    {
        Instance = this;
        init();
    }

    protected virtual T add(T pObj)
    {
        cached_library ??= (AssetLibrary<T>)AssetManager.instance.list.Find(l => l is AssetLibrary<T>);
        t = pObj;
        return cached_library.add(pObj);
    }

    protected abstract void init();
}