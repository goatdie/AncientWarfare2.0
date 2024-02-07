using System;
using System.Collections.Generic;
using NeoModLoader.utils;
namespace Figurebox.utils;

public interface IHasKey<TKey>
{
    public TKey GetKey();
}

public class JumpList<TObject, TKey> where TObject : IHasKey<TKey>
{
    private readonly IComparer<TKey> comparer;
    private readonly Dictionary<TKey, int> jump_dict = new();
    private readonly PriorityQueue<TKey> sorted_keys;
    private readonly PriorityQueue<TObject> sorted_list;

    public JumpList(int capacity, IComparer<TKey> pKeyComparer)
    {
        comparer = pKeyComparer;
        sorted_keys = new PriorityQueue<TKey>(capacity, pKeyComparer);
        sorted_list = new PriorityQueue<TObject>(capacity, new ObjectComparer(pKeyComparer));
    }

    public TObject this[int index] => sorted_list[index];

    public void Enqueue(TObject pObject)
    {
        sorted_list.Enqueue(pObject);

        TKey key = pObject.GetKey();
        int left = 0;
        int right = sorted_keys.Count - 1;
        int i = 0;
        while (left <= right)
        {
            i = (left + right) / 2;
            TKey key_i = sorted_keys[i];
            int compare_result = comparer.Compare(key_i, key);
            if (compare_result < 0)
            {
                right = i;
            }
            else if (compare_result == 0)
            {
                i++;
                break;
            }
            else
            {
                left = i;
            }
        }
        for (; i < sorted_keys.Count; i++)
        {
            TKey key_i = sorted_keys[i];
            jump_dict[key_i]++;
        }
    }
    public int GetIndex(TKey pIndexKey)
    {
        if (jump_dict.TryGetValue(pIndexKey, out var idx)) return idx;

        InsertKey(pIndexKey, out var last_key, out var next_key);

        int left = last_key == null ? 0 : jump_dict[last_key];
        int right = next_key == null ? sorted_list.Count - 1 : jump_dict[next_key];
        int i = 0;
        while (left <= right)
        {
            i = (left + right) / 2;
            TObject obj = sorted_list[i];
            int compare_result = comparer.Compare(obj.GetKey(), pIndexKey);
            if (compare_result < 0)
            {
                right = i;
            }
            else if (compare_result == 0)
            {
                break;
            }
            else
            {
                left = i;
            }
        }
        jump_dict[pIndexKey] = left;

        return i;
    }

    /// <summary>
    ///     将<paramref name="pKey" />加入排序后的key列表
    /// </summary>
    /// <param name="pKey"></param>
    /// <param name="pLastKey"></param>
    /// <param name="pNextKey"></param>
    /// <returns></returns>
    private void InsertKey(TKey pKey, out TKey pLastKey, out TKey pNextKey)
    {
        pLastKey = default;
        pNextKey = default;
        int index = sorted_keys.Enqueue(pKey);
        if (index > 0)
        {
            pLastKey = sorted_keys[index - 1];
        }
        if (index < sorted_keys.Count - 1)
        {
            pNextKey = sorted_keys[index + 1];
        }
    }

    private int FindIndexInObjectList(TKey pKey)
    {
        throw new NotImplementedException();
    }

    private int FindIndexInKeyList(TKey pKey)
    {
        throw new NotImplementedException();
    }

    private class ObjectComparer : IComparer<TObject>
    {
        private readonly IComparer<TKey> key_comparer;
        public ObjectComparer(IComparer<TKey> key_comparer)
        {
            this.key_comparer = key_comparer;
        }
        public int Compare(TObject x, TObject y)
        {
            if (x == null) return -1;
            if (y == null) return 1;
            return key_comparer.Compare(x.GetKey(), y.GetKey());
        }
    }
}