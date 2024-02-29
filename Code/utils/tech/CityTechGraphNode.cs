using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Figurebox.core.city_techs;
using UnityEngine;

namespace Figurebox.utils.tech;

public class CityTechGraphNode
{
    public readonly AW_CityTechAsset asset;
    public Vector2 position;
    public readonly HashSet<CityTechGraphNode> next = new();

    public CityTechGraphNode(AW_CityTechAsset pAsset)
    {
        asset = pAsset;
    }
}