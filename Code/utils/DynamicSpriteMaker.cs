using System.Collections.Generic;
using UnityEngine;
namespace Figurebox.Utils;

public class DynamicSpriteMaker
{
    private static readonly Dictionary<Font, Texture2D> _font_texture_cache = new();
    private static Texture2D GetReadableTexture(Font pFont)
    {
        if (_font_texture_cache.TryGetValue(pFont, out Texture2D texture))
        {
            return texture;
        }
        var font_texture = (Texture2D)pFont.material.mainTexture;
        texture = new Texture2D(font_texture.width, font_texture.height, font_texture.format, font_texture.mipmapCount, true);
        Graphics.CopyTexture(font_texture, texture);
        texture.Apply();
        _font_texture_cache.Add(pFont, texture);
        return texture;
    }
    public static Sprite MakeCharacterSprite(char pChar, Font pFont, Vector2Int pSize, Color pColor)
    {
        Texture2D texture = new(pSize.x, pSize.y);
        texture.filterMode = FilterMode.Point;
        var pixels_to_set = new Color[pSize.x * pSize.y];
        pFont.GetCharacterInfo(pChar, out CharacterInfo info, pSize.y);
        var font_texture = GetReadableTexture(pFont);

        int char_width;
        int char_height;
        if (info.uvTopLeft.x < info.uvBottomRight.x)
        {
            char_width = info.glyphWidth;
            char_height = info.glyphHeight;

            var pixels = font_texture.GetPixels((int)(font_texture.width * info.uvTopLeft.x), (int)(font_texture.height * info.uvTopLeft.y), char_width, char_height);

            for (int j = 0; j < char_height; j++)
            {
                for (int i = 0; i < char_width; i++)
                {
                    if (pixels[j * char_width + i].a == 0)
                    {
                        pixels_to_set[(char_height - j) * pSize.x + i] = Color.clear;
                    }
                    else
                    {
                        pixels_to_set[(char_height - j) * pSize.x + i] = pColor;
                    }
                }
            }
        }
        else
        {
            char_width = info.glyphHeight;
            char_height = info.glyphWidth;

            var pixels = font_texture.GetPixels(
                (int)(font_texture.width * info.uvBottomRight.x),
                (int)(font_texture.height * info.uvBottomRight.y),
                char_width, char_height);

            for (int j = 0; j < char_height; j++)
            {
                for (int i = 0; i < char_width; i++)
                {
                    if (pixels[j * char_width + i].a == 0)
                    {
                        pixels_to_set[i * pSize.x + char_height - j] = Color.clear;
                    }
                    else
                    {
                        pixels_to_set[i * pSize.x + char_height - j] = pColor;
                    }
                }
            }
        }
        texture.SetPixels(pixels_to_set);
        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, pSize.x, pSize.y), new Vector2(0.5f, 0.5f), 1, 1);
        sprite.name = pChar.ToString();
        return sprite;
    }
}