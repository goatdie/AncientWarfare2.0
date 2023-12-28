using System;
using System.Collections.Generic;
using System.IO;
using Figurebox.Core.KingdomEvents;
using Figurebox.Utils;
using NeoModLoader.services;
using NeoModLoader.utils;
namespace Figurebox.core;

public class KingdomEventsManager
{
    /// <summary>
    ///     缓存的所有事件
    /// </summary>
    private readonly JumpList<BaseEvent, int> cached_events = new(1024, new EventStartTimeComparer());
    /// <summary>
    ///     缓存的国家-所有事件的词典
    /// </summary>
    private readonly Dictionary<string, JumpList<BaseEvent, int>> cached_kingdom_to_events_dict = new();
    /// <summary>
    ///     缓存的所有事件的时间范围, 用于读取硬盘
    /// </summary>
    private PriorityQueue<TimeBlock> cached_events_durations = new(64, new TimeBlock.TimeBlockComparer());

    private string FilePath;
    private bool OnlyRead;
    public static KingdomEventsManager Instance { get; } = new();

    /// <summary>
    ///     获取一段时间内开始的所有事件
    /// </summary>
    /// <param name="pStartYearInclusive">起始年（包含）</param>
    /// <param name="pEndYearExclusive">末年（不包含）</param>
    /// <returns>按起始时间升序排列的事件</returns>
    public List<BaseEvent> GetEventsDuring(int pStartYearInclusive, int pEndYearExclusive)
    {
        ReadEventsFromDisk(pStartYearInclusive, pEndYearExclusive);
        int start_index = cached_events.GetIndex(pStartYearInclusive);
        int end_index = cached_events.GetIndex(pEndYearExclusive);
        List<BaseEvent> ret = new(end_index - start_index);
        for (int i = start_index; i < end_index; i++)
        {
            ret.Add(cached_events[i]);
        }

        return ret;
    }
    /// <summary>
    ///     获取一段时间内开始的指定类型的所有事件
    /// </summary>
    /// <param name="pStartYearInclusive"></param>
    /// <param name="pEndYearExclusive"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns>按起始时间升序排列的事件</returns>
    public List<TEvent> GetEventsDuring<TEvent>(int pStartYearInclusive, int pEndYearExclusive) where TEvent : BaseEvent
    {
        ReadEventsFromDisk(pStartYearInclusive, pEndYearExclusive);
        int start_index = cached_events.GetIndex(pStartYearInclusive);
        int end_index = cached_events.GetIndex(pEndYearExclusive);
        List<TEvent> ret = new(end_index - start_index);
        for (int i = start_index; i < end_index; i++)
        {
            if (cached_events[i] is TEvent t_event)
            {
                ret.Add(t_event);
            }
        }

        return ret;
    }
    /// <summary>
    ///     获取一段时间内开始的指定国家的所有事件
    /// </summary>
    /// <param name="pKingdomID"></param>
    /// <param name="pStartYearInclusive"></param>
    /// <param name="pEndYearExclusive"></param>
    /// <returns>按起始时间升序排列的事件</returns>
    public List<BaseEvent> GetEventsDuringOnKingdom(string pKingdomID, int pStartYearInclusive, int pEndYearExclusive)
    {
        ReadEventsFromDisk(pStartYearInclusive, pEndYearExclusive);
        if (!cached_kingdom_to_events_dict.TryGetValue(pKingdomID, out var list))
        {
            return new List<BaseEvent>();
        }
        int start_index = list.GetIndex(pStartYearInclusive);
        int end_index = list.GetIndex(pEndYearExclusive);
        List<BaseEvent> ret = new(end_index - start_index);
        for (int i = start_index; i < end_index; i++)
        {
            ret.Add(list[i]);
        }

        return ret;
    }
    /// <summary>
    ///     获取一段时间内开始的指定国家的指定类型的所有事件
    /// </summary>
    /// <param name="pKingdomID"></param>
    /// <param name="pStartYearInclusive"></param>
    /// <param name="pEndYearExclusive"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns>按起始时间升序排列的事件</returns>
    public List<TEvent> GetEventsDuringOnKingdom<TEvent>(string pKingdomID, int pStartYearInclusive, int pEndYearExclusive) where TEvent : BaseEvent
    {
        ReadEventsFromDisk(pStartYearInclusive, pEndYearExclusive);
        if (!cached_kingdom_to_events_dict.TryGetValue(pKingdomID, out var list))
        {
            return new List<TEvent>();
        }
        int start_index = list.GetIndex(pStartYearInclusive);
        int end_index = list.GetIndex(pEndYearExclusive);
        List<TEvent> ret = new(end_index - start_index);
        for (int i = start_index; i < end_index; i++)
        {
            if (list[i] is TEvent t_event)
            {
                ret.Add(t_event);
            }
        }

        return ret;
    }
    /// <summary>
    ///     加入一个事件
    /// </summary>
    /// <param name="pEvent"></param>
    /// <param name="pKingdom"></param>
    public void AddEvent(BaseEvent pEvent, Kingdom pKingdom = null)
    {
        if (pKingdom != null)
        {
            if (!cached_kingdom_to_events_dict.TryGetValue(pKingdom.id, out var list))
            {
                list = new JumpList<BaseEvent, int>(256, new EventStartTimeComparer());
                cached_kingdom_to_events_dict.Add(pKingdom.id, list);
            }
            list.Enqueue(pEvent);
        }
        cached_events.Enqueue(pEvent);
    }
    /// <summary>
    ///     从硬盘读取一段时间内的所有事件，会自动跳过已经读取过的时间段，已经读取过的时间段会直接返回
    /// </summary>
    /// <param name="pStartYearInclusive"></param>
    /// <param name="pEndYearExclusive"></param>
    private void ReadEventsFromDisk(int pStartYearInclusive, int pEndYearExclusive)
    {
        if (OnlyRead) return;


    }
    public void SetSavePath(string pPath)
    {
        if (string.IsNullOrEmpty(pPath)) throw new ArgumentException($"{nameof(pPath)} is null or empty");
        if (!File.Exists(pPath))
        {
            OnlyRead = true;
            LogService.LogWarning($"[AW2]: {nameof(pPath)} does not exists");
        }

        FilePath = pPath;
    }

    private class EventStartTimeComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return x - y;
        }
    }

    private struct TimeBlock
    {
        public readonly int start;
        public readonly int end;

        public class TimeBlockComparer : IComparer<TimeBlock>
        {
            public int Compare(TimeBlock x, TimeBlock y)
            {
                int start_comparison = x.start.CompareTo(y.start);
                if (start_comparison != 0) return start_comparison;
                return x.end.CompareTo(y.end);
            }
        }
    }
}