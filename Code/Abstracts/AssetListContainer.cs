using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace AncientWarfare.Abstracts;

public class AssetListContainer<T> : IEnumerable<T> where T : Asset
{
    private readonly List<string>               _id_list = new();
    private readonly HashSet<string>            _id_set  = new();
    private readonly List<T>                    _list    = new();
    private readonly HashSet<T>                 _set     = new();
    public readonly  ReadOnlyCollection<string> id_list;
    public readonly  ReadOnlyCollection<T>      list;

    public AssetListContainer()
    {
        list = _list.AsReadOnly();
        id_list = _id_list.AsReadOnly();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Contains(T asset)
    {
        return _set.Contains(asset);
    }

    public bool Contains(string id)
    {
        return _id_set.Contains(id);
    }

    public void Add(params T[] assets)
    {
        _list.AddRange(assets);
        _set.UnionWith(assets);
        var ids = assets.Select(x => x.id);
        _id_list.AddRange(ids);
        _id_set.UnionWith(ids);
    }

    public void AddRange(IEnumerable<T> assets)
    {
        _list.AddRange(assets);
        _set.UnionWith(assets);
        var ids = assets.Select(x => x.id);
        _id_list.AddRange(ids);
        _id_set.UnionWith(ids);
    }

    public T GetRandom()
    {
        return _list[Random.Range(0, _list.Count)];
    }

    public string GetRandomId()
    {
        return _id_list[Random.Range(0, _id_list.Count)];
    }
}