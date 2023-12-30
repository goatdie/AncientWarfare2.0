using Figurebox.core.map_modes;
using UnityEngine;
namespace Figurebox.core;

public class MapModeManager
{
    public static CustomMapLayer map_layer { get; private set; }
    public static CustomMapMode map_mode { get; private set; }
    internal static void CreateMapLayer()
    {
        GameObject custom_map_layer_obj = new("[layer]Ancient Warfare Layer", typeof(CustomMapLayer), typeof(SpriteRenderer));
        custom_map_layer_obj.transform.SetParent(World.world.transform);
        custom_map_layer_obj.transform.localPosition = Vector3.zero;
        custom_map_layer_obj.transform.localScale = Vector3.one;
        custom_map_layer_obj.GetComponent<SpriteRenderer>().sortingOrder = 1;
        map_layer = custom_map_layer_obj.GetComponent<CustomMapLayer>();
        World.world.mapLayers.Add(map_layer);
    }
    internal static void SetMapMode(CustomMapMode pMode)
    {
        if (map_mode == pMode) return;
        map_mode = pMode;
        switch (map_mode)
        {
            case CustomMapMode.Hidden:
                map_layer.Hide();
                break;
            default:
                map_layer.Show();
                map_layer.SetAllDirty();
                break;
        }
    }
}