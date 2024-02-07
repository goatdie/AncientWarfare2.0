using NCMS.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Figurebox.utils
{
    class ResourcesHelper
    {
        //全图加载
        public static Sprite[] loadAllSprite(string path, int resizeX = 0, int resizeY = 0, float offsetX = 0f, float offsetY = 0f, bool withFolders = false)//路径
        {
            string p = $"{Main.mainPath}/EmbededResources/{path}";
            DirectoryInfo folder = new DirectoryInfo(p);
            List<Sprite> res = new List<Sprite>();
            foreach (FileInfo file in folder.GetFiles("*.png"))
            {
                Sprite sprite = LoadSpriteFUBEN($"{file.FullName}", resizeX, resizeY, offsetX, offsetY);
                sprite.name = file.Name.Replace(".png", "");
                res.Add(sprite);
            }
            foreach (DirectoryInfo cFolder in folder.GetDirectories())
            {
                foreach (FileInfo file in cFolder.GetFiles("*.png"))
                {
                    Sprite sprite = LoadSpriteFUBEN($"{file.FullName}", resizeX, resizeY, offsetX, offsetY);
                    sprite.name = file.Name.Replace(".png", "");
                    res.Add(sprite);
                }
            }
            return res.ToArray();
        }
        //重载
        public static Sprite[] loadAllSprite(string path, float offsetX = 0f, float offsetY = 0f, bool withFolders = false)//路径
        {
            string p = $"{Main.mainPath}/EmbededResources/{path}";
            DirectoryInfo folder = new DirectoryInfo(p);
            List<Sprite> res = new List<Sprite>();
            foreach (FileInfo file in folder.GetFiles("*.png"))
            {
                Sprite sprite = Sprites.LoadSprite($"{file.FullName}", offsetX, offsetY);
                sprite.name = file.Name.Replace(".png", "");
                res.Add(sprite);
            }
            foreach (DirectoryInfo cFolder in folder.GetDirectories())
            {
                foreach (FileInfo file in cFolder.GetFiles("*.png"))
                {
                    Sprite sprite = Sprites.LoadSprite($"{file.FullName}", offsetX, offsetY);
                    sprite.name = file.Name.Replace(".png", "");
                    res.Add(sprite);
                }
            }
            return res.ToArray();
        }
        public static string LoadTextAsset(string path)
        {
            string result = File.ReadAllText($"{Main.mainPath}/EmbededResources/" + path);
            return result;
        }

        public static Sprite LoadSpriteFUBEN(string path, int resizeX, int resizeY, float offsetx, float offsety)

        {

            if (string.IsNullOrEmpty(path)) return null;


            if (!File.Exists(path)) return null;


            byte[] data = File.ReadAllBytes(path);

            Texture2D texture2D = new Texture2D(1, 1);

            texture2D.anisoLevel = 0;

            texture2D.LoadImage(data);

            texture2D.filterMode = FilterMode.Point;

            TextureScale.Point(texture2D, resizeX, resizeY);

            return Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(offsetx, offsety), 1f);

        }
    }
}
