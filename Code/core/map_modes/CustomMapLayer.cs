using System;
using System.Threading;
using NeoModLoader.api.attributes;
using NeoModLoader.services;
using UnityEngine;

namespace Figurebox.core.map_modes;

public class CustomMapLayer : MapLayer
{
    private readonly object    lock_all_dirty = new();
    private readonly object    lock_pixels    = new();
    private          bool      all_dirty      = true;
    private          Color32[] mirror_pixels;

    private bool need_update = true;

    private Color spr_color = Color.white;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }

    internal void SetAllDirty()
    {
        Monitor.Enter(lock_all_dirty);
        all_dirty = true;
        Monitor.Exit(lock_all_dirty);
    }

    [Hotfixable]
    public override void update(float pElapsed)
    {
        if (pixels == null)
            createTextureNew();

        if (sprRnd == null) sprRnd = GetComponent<SpriteRenderer>();

        if (MapModeManager.map_mode == CustomMapMode.Hidden)
        {
            Hide();
            return;
        }

        Show();

        spr_color.a = World.world.zoneCalculator._night_mod * (MapBox.isRenderMiniMap()
            ? World.world.zoneCalculator.minimap_opacity
            : Mathf.Clamp(ZoneCalculator.getCameraScaleZoom() * 0.3f, 0f, 0.7f));

        sprRnd.enabled = true;
        sprRnd.color = spr_color;

        if (!need_update) return;

        Monitor.Enter(lock_pixels);
        updatePixels();
        need_update = false;
        Monitor.Exit(lock_pixels);

        base.update(pElapsed);
    }

    internal void PreparePixels()
    {
        if (pixels        == null || !sprRnd.enabled) return;
        if (mirror_pixels == null || mirror_pixels.Length != pixels.Length) mirror_pixels = new Color32[pixels.Length];

        CustomMapMode map_mode = MapModeManager.map_mode;
        if (map_mode == CustomMapMode.Hidden) return;

        Array.Copy(pixels, mirror_pixels, pixels.Length);

        var dirty = false;
        if (all_dirty)
        {
            dirty = true;
            Monitor.Enter(lock_all_dirty);
            all_dirty = false;
            Monitor.Exit(lock_all_dirty);

            CustomMapModePainters.ClearAll(mirror_pixels);
        }

        // Update mirror_pixels
        switch (map_mode)
        {
            case CustomMapMode.Idealogy:
                dirty |= CustomMapModePainters.DrawIdealogy(mirror_pixels);
                break;
            case CustomMapMode.Vassals:
                dirty |= CustomMapModePainters.DrawVassals(mirror_pixels);
                break;
            default:
                LogService.LogInfoConcurrent($"No painter for map mode {map_mode}");
                break;
        }

        if (!dirty) return;

        Monitor.Enter(lock_pixels);
        (pixels, mirror_pixels) = (mirror_pixels, pixels);
        need_update = true;
        Monitor.Exit(lock_pixels);
    }
}