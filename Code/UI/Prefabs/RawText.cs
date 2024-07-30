using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace AncientWarfare.UI.Prefabs;

public class RawText : APrefab<RawText>
{
    private Text mText;

    public Text Text
    {
        get
        {
            Init();
            return mText;
        }
    }

    protected override void Init()
    {
        if (Initialized) return;
        base.Init();

        mText = GetComponent<Text>();
    }

    public void Setup(string text, Color color = default, TextAnchor alignment = TextAnchor.UpperLeft)
    {
        Text.text = text;
        if (color != default) Text.color = color;

        Text.alignment = alignment;
    }

    private static void _init()
    {
        var obj = new GameObject(nameof(RawText), typeof(Text));
        obj.transform.SetParent(Main.prefabs_library);
        var text = obj.GetComponent<Text>();
        text.color = Color.white;
        text.font = LocalizedTextManager.currentFont;
        text.resizeTextForBestFit = true;
        text.resizeTextMaxSize = 12;
        text.resizeTextMinSize = 8;
        text.alignment = TextAnchor.UpperLeft;
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 240);


        Prefab = obj.AddComponent<RawText>();
    }
}