using System.Collections.Concurrent;
namespace Figurebox.Utils;

public interface IReusable
{
    public void Setup();
    public void Recycle();
}

public class ObjectPool<T> where T : IReusable, new()
{
    /// <summary>
    ///     全局池
    /// </summary>
    public static ObjectPool<T> GlobalPool = new(64);
    private readonly int expected_capacity;
    private readonly ConcurrentBag<T> pool;

    public ObjectPool(int pCapacity)
    {
        pool = new ConcurrentBag<T>();
        expected_capacity = pCapacity;
    }
    public T GetNext()
    {
        if (!pool.TryTake(out T ret))
        {
            ret = new T();
        }
        ret.Setup();
        return ret;
    }

    public void Recycle(T obj)
    {
        obj.Recycle();
        if (pool.Count >= expected_capacity) return;
        pool.Add(obj);
    }

    public static void GlobalRecycle(T obj)
    {
        GlobalPool.Recycle(obj);
    }

    public static T GlobalGetNext()
    {
        return GlobalPool.GetNext();
    }
}