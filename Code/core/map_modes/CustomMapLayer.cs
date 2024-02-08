using System;
using System.Threading;
using UnityEngine;

namespace Figurebox.core.map_modes;

public class CustomMapLayer : MapLayer
{
    private readonly object    lock_pixels = new();
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

    public void SetAllDirty()
    {
    }

    public override void update(float pElapsed)
    {
        if (pixels == null)
            createTextureNew();

        if (sprRnd == null) sprRnd = GetComponent<SpriteRenderer>();
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

        Array.Copy(pixels, mirror_pixels, pixels.Length);

        var dirty = false;
        // Update mirror_pixels
        switch (MapModeManager.map_mode)
        {
            case CustomMapMode.Idealogy:
                dirty |= CustomMapModePainters.DrawIdealogy(mirror_pixels);
                break;
            case CustomMapMode.Vassals:
                dirty |= CustomMapModePainters.DrawVassals(mirror_pixels);
                break;
        }

        if (!dirty) return;

        Monitor.Enter(lock_pixels);
        (pixels, mirror_pixels) = (mirror_pixels, pixels);
        need_update = true;
        Monitor.Exit(lock_pixels);
    }
}