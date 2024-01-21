using System.Linq;
using ai.behaviours;
using Figurebox.attributes;
using Figurebox.constants;
using Figurebox.Utils;
using NeoModLoader.api.attributes;
using System;
using System.Collections.Generic;
using EpPathFinding.cs;
using UnityEngine;
using C5;
namespace Figurebox;

internal class PathFinderPatch
{

    [Hotfixable]
    [MethodReplace(typeof(PathfinderTools), nameof(PathfinderTools.tryToGetSimplePath))]
    public static bool tryToGetSimplePath(WorldTile pFrom, WorldTile pTargetTile, List<WorldTile> pPathToFill, ActorAsset pAsset, AStarParam pParam)
    {
        WorldTile worldTile = pFrom;
        float num2 = 0f;
        WorldTile worldTile2 = null;
        for (; ; )
        {
            foreach (WorldTile worldTile3 in worldTile.neighboursAll)
            {
                float num3 = Toolbox.DistTile(worldTile3, pTargetTile);
                if (worldTile2 == null || num3 < num2)
                {
                    worldTile2 = worldTile3;
                    num2 = num3;
                }
            }
            if (worldTile2 == null || worldTile2 == worldTile)
            {
                return true;
            }
            worldTile = worldTile2;
            if (!pParam.block && worldTile.Type.block)
            {
                break;
            }
            if (!pParam.lava && worldTile.Type.lava)
            {
                return false;
            }
            if (!pParam.ocean && worldTile.Type.ocean && worldTile.Type != TileLibrary.shallow_waters)
            {
                return false;
            }
            if (!pParam.ground && worldTile.Type.ground)
            {
                return false;
            }
            if (pParam.boat && !worldTile.isGoodForBoat())
            {
                return false;
            }
            pPathToFill.Add(worldTile2);
            if (worldTile2 == pTargetTile)
            {
                return true;
            }
            worldTile2 = null;
        }
        return false;
    }


    [Hotfixable]
    [MethodReplace(typeof(AStarFinder), nameof(AStarFinder.FindPath))]
    public static List<WorldTile> FindPath(AStarParam iParam, List<WorldTile> pSavePath)
    {
        IntervalHeap<Node> intervalHeap = new IntervalHeap<Node>();
        Node node;
        Node node2;
        if (iParam.endToStartPath)
        {
            node = iParam.EndNode;
            node2 = iParam.StartNode;
        }
        else
        {
            node = iParam.StartNode;
            node2 = iParam.EndNode;
        }
        HeuristicDelegate heuristicFunc = iParam.HeuristicFunc;
        BaseGrid searchGrid = iParam.SearchGrid;
        StaticGrid staticGrid = (StaticGrid)searchGrid;
        DiagonalMovement diagonalMovement = iParam.DiagonalMovement;
        float weight = iParam.Weight;
        bool boat = iParam.boat;
        node.startToCurNodeLen = 0f;
        node.heuristicStartToEndLen = 0f;
        intervalHeap.Add(node);
        node.isOpened = true;
        AStarFinder.result_split_path = false;
        AStarFinder.lastGlobalRegionID = 0;
        while (intervalHeap.Count != 0)
        {
            if (iParam.maxOpenList != -1 && intervalHeap.Count > iParam.maxOpenList)
            {
                return pSavePath;
            }
            Node node3 = intervalHeap.DeleteMin();
            node3.isClosed = true;
            searchGrid.addToClosed(node3);
            WorldTile tile = node3.tile;
            if (node3 == node2)
            {
                return AStarFinder.backTracePath(node2, pSavePath, iParam.endToStartPath);
            }
            if (iParam.useGlobalPathLock)
            {
                if (tile.region.regionPathID < AStarFinder.lastGlobalRegionID && tile.region.regionPathID != -1)
                {
                    node3.isClosed = true;
                    searchGrid.addToClosed(node3);
                    continue;
                }
                if (tile.region.regionPathID > AStarFinder.lastGlobalRegionID)
                {
                    AStarFinder.lastGlobalRegionID = tile.region.regionPathID;
                }
            }
            WorldTile[] array;
            if (diagonalMovement == DiagonalMovement.Never)
            {
                array = tile.neighbours;
            }
            else
            {
                array = tile.neighboursAll;
            }
            foreach (WorldTile worldTile in array)
            {
                Node node4 = staticGrid.m_nodes[worldTile.x][worldTile.y];
                if (!node4.isClosed && (!worldTile.Type.block || iParam.block))
                {
                    if (iParam.useGlobalPathLock)
                    {
                        if (worldTile.region.regionPathID < AStarFinder.lastGlobalRegionID && worldTile.region.regionPathID != -1)
                        {
                            node3.isClosed = true;
                            searchGrid.addToClosed(node3);
                            goto IL_445;
                        }
                        if (worldTile.region.regionPathID > AStarFinder.lastGlobalRegionID)
                        {
                            AStarFinder.lastGlobalRegionID = worldTile.region.regionPathID;
                        }
                        if (!worldTile.region.usedByPathLock)
                        {
                            node3.isClosed = true;
                            searchGrid.addToClosed(node3);
                            goto IL_445;
                        }
                    }
                    if (boat)
                    {
                        if (!worldTile.isGoodForBoat())
                        {
                            goto IL_445;
                        }
                    }
                    else
                    {
                        if (worldTile.Type.lava && !iParam.lava)
                        {
                            goto IL_445;
                        }
                        if ((!iParam.block || !worldTile.Type.block) && (!iParam.lava || !worldTile.Type.lava))
                        {
                            if (iParam.ground && iParam.ocean)
                            {
                                if (!worldTile.Type.ground && !worldTile.Type.ocean)
                                {
                                    goto IL_445;
                                }
                            }
                            else if ((!worldTile.Type.ground && iParam.ground) || (!worldTile.Type.ocean && iParam.ocean))
                            {
                                if (worldTile.Type != TileLibrary.shallow_waters) // 浅水被视为陆地
                                {
                                    goto IL_445;
                                }
                            }
                        }
                    }
                    float num = 1f;
                    if (iParam.roads && World.world.GetTileSimple(node4.x, node4.y).Type.road)
                    {
                        num = 0.01f;
                    }
                    if (AStarFinder.lastGlobalRegionID >= 4 && searchGrid.closed_list_count > 10)
                    {
                        AStarFinder.result_split_path = true;
                        return AStarFinder.backTracePath(node3, pSavePath, iParam.endToStartPath);
                    }
                    float num2 = node3.startToCurNodeLen + num * ((node4.x - node3.x == 0 || node4.y - node3.y == 0) ? 1f : 1.414f);
                    if (!node4.isOpened || num2 < node4.startToCurNodeLen)
                    {
                        node4.startToCurNodeLen = num2;
                        if (node4.heuristicCurNodeToEndLen == null)
                        {
                            node4.heuristicCurNodeToEndLen = new float?(weight * heuristicFunc(Math.Abs(node4.x - node2.x), Math.Abs(node4.y - node2.y)));
                        }
                        node4.heuristicStartToEndLen = node4.startToCurNodeLen + node4.heuristicCurNodeToEndLen.Value;
                        node4.parent = node3;
                        if (!node4.isOpened)
                        {
                            intervalHeap.Add(node4);
                            node4.isOpened = true;
                            searchGrid.addToClosed(node4);
                        }
                    }
                }
            IL_445:;
            }
        }
        return pSavePath;
    }

}