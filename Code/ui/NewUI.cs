using System.Collections.Generic;
using Figurebox.prefabs;
using NCMS.Utils;
using NeoModLoader.General;
using ReflectionUtility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace Figurebox
{
    internal class NewUI
    { // thanks to Dej
        private static GameObject textRef;
        private static readonly Dictionary<string, Object> _patched_resources = RF.GetStaticField<Dictionary<string, Object>, ResourcesPatch>("modsResources");
        /// <summary>
        ///     将资源添加到游戏中
        /// </summary>
        /// <param name="pPath"></param>
        /// <param name="pObject"></param>
        private static void PatchResourceToGame(string pPath, Object pObject)
        {
            // TODO: 等待下一版本NML将会使用新的API，这里的代码将会被移除
            _patched_resources[pPath.ToLower()] = pObject;
        }
        public static void PatchResources()
        {
            PatchResourceToGame("tooltips/tooltip_policy", KingdomPolicyTooltip.Prefab.GetComponent<Tooltip>());
        }

        public static UiUnitAvatarElement createActorUI(Actor actor, GameObject parent, Vector3 pos)
        {
            GameObject GO = Object.Instantiate(Main.backgroundAvatar);
            GO.transform.SetParent(parent.transform);
            var avatarElement = GO.GetComponent<UiUnitAvatarElement>();
            avatarElement.show_banner_clan = true;
            avatarElement.show_banner_kingdom = true;
            avatarElement.show(actor);
            RectTransform GORect = GO.GetComponent<RectTransform>();
            GORect.localPosition = pos;
            GORect.localScale = new Vector3(1, 1, 1);


            Button GOButton = GO.AddComponent<Button>();
            GOButton.OnHover(new UnityAction(() => actorTooltip(actor)));
            GOButton.OnHoverOut(Tooltip.hideTooltip);
            GOButton.onClick.AddListener(() => showActor(actor));
            GO.AddComponent<GraphicRaycaster>();
            return avatarElement;
        }

        private static void actorTooltip(Actor actor)
        {
            if (actor == null)
            {
                return;
            }
            string text = "actor";
            if (actor.isKing())
            {
                text = "actor_king";
            }
            else if (actor.isCityLeader())
            {
                text = "actor_leader";
            }
            Tooltip.show(actor, text, new TooltipData
            {
                actor = actor,
            });
            return;
        }

        public static void showActor(Actor pActor)
        {
            Config.selectedUnit = pActor;
            ScrollWindow.showWindow("inspect_unit");
        }

        public static Button createBGWindowButton(GameObject parent, int posY, string iconName, string buttonName, string buttonTitle,
            string buttonDesc, UnityAction call)
        {
            PowerButton button = PowerButtons.CreateButton(
                buttonName,
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.{iconName}.png"),
                LocalizedTextManager.stringExists(buttonName) ? LM.Get(buttonName) : buttonTitle, // 手动判断是否覆盖
                LocalizedTextManager.stringExists($"{buttonName} Description") ? LM.Get($"{buttonName} Description") : buttonDesc,
                new Vector2(118, posY),
                ButtonType.Click,
                parent.transform,
                call
            );

            Image buttonBG = button.gameObject.GetComponent<Image>();
            buttonBG.sprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.backgroundTabButton.png");
            Button buttonButton = button.gameObject.GetComponent<Button>();
            buttonBG.rectTransform.localScale = Vector3.one;

            return buttonButton;
        }

        public static Text addText(string textString, GameObject parent, int sizeFont, Vector3 pos, Vector2 addSize = default(Vector2))
        {
            textRef = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/leaderBoardWindow/Background/Title");
            GameObject textGo = Object.Instantiate(textRef, parent.transform);
            textGo.SetActive(true);

            var textComp = textGo.GetComponent<Text>();
            textComp.fontSize = sizeFont;
            textComp.resizeTextMaxSize = sizeFont;
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.position = new Vector3(0, 0, 0);
            textRect.localPosition = pos + new Vector3(0, -50, 0);
            textRect.sizeDelta = new Vector2(100, 100) + addSize;
            textGo.AddComponent<GraphicRaycaster>();
            textComp.text = textString;

            return textComp;
        }

        public static void loadTraitButton(string pID, Vector2 pos, GameObject parent)
        {
            WindowCreatureInfo info = GameObjects.FindEvenInactive("inspect_unit").GetComponent<WindowCreatureInfo>();
            TraitButton traitButton = Object.Instantiate(info.prefabTrait, parent.transform);
            Reflection.CallMethod(traitButton, "Awake");
            Reflection.CallMethod(traitButton, "load", pID);
            RectTransform component = traitButton.GetComponent<RectTransform>();
            component.localPosition = pos;
        }


        public static NameInput createInputOption(string objName, string title, string desc, int posY, GameObject parent, string textValue = "-1")
        {
            GameObject statHolder = new GameObject("OptionHolder");
            statHolder.transform.SetParent(parent.transform);
            Image statImage = statHolder.AddComponent<Image>();
            statImage.sprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.windowInnerSliced.png");
            RectTransform statHolderRect = statHolder.GetComponent<RectTransform>();
            statHolderRect.localPosition = new Vector3(130, posY, 0);
            statHolderRect.sizeDelta = new Vector2(400, 150);

            Text statText = addText(title, statHolder, 20, new Vector3(0, 110, 0), new Vector2(100, 0));
            RectTransform statTextRect = statText.gameObject.GetComponent<RectTransform>();
            statTextRect.sizeDelta = new Vector2(statTextRect.sizeDelta.x, 80);

            Text descText = addText(desc, statHolder, 20, new Vector3(0, 60, 0), new Vector2(300, 0));
            RectTransform descTextRect = descText.gameObject.GetComponent<RectTransform>();
            descTextRect.sizeDelta = new Vector2(descTextRect.sizeDelta.x, 80);

            GameObject inputRef = GameObjects.FindEvenInactive("NameInputElement");

            GameObject inputField = Object.Instantiate(inputRef, statHolder.transform);
            NameInput nameInputComp = inputField.GetComponent<NameInput>();
            nameInputComp.setText(textValue);
            RectTransform inputRect = inputField.GetComponent<RectTransform>();
            inputRect.localPosition = new Vector3(0, -40, 0);
            inputRect.sizeDelta += new Vector2(120, 40);

            GameObject inputChild = inputField.transform.Find("InputField").gameObject;
            RectTransform inputChildRect = inputChild.GetComponent<RectTransform>();
            inputChildRect.sizeDelta *= 2;
            Text inputChildText = inputChild.GetComponent<Text>();
            inputChildText.resizeTextMaxSize = 20;
            return nameInputComp;
        }

        public static string checkStatInput(NameInput pInput = null, string pText = null)
        {
            string text = pText;
            if (pInput != null)
            {
                text = pInput.inputField.text;
            }
            int num = -1;
            if (!int.TryParse(text, out num))
            {
                return "0";
            }
            if (num > 9999)
            {
                return "9999";
            }
            if (num < -9999)
            {
                return "-9999";
            }
            return text;
        }

        public static void createTextButtonWSize(string name, string title, Vector2 pos, Color color, Transform parent, UnityAction callback, Vector2 size)
        {
            Button textButton = PowerButtons.CreateTextButton(
                name,
                title,
                pos,
                color,
                parent,
                callback
            );
            if (title.Length > 7)
            {
                textButton.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta += new Vector2(0, 10);
            }
            textButton.gameObject.GetComponent<RectTransform>().sizeDelta = size;
        }

        public static GameObject createSubWindow(GameObject parent, Vector3 pos, Vector2 size, Vector2 infoSize)
        {
            GameObject parentScrollHolder = new GameObject("scrollHolder");
            parentScrollHolder.transform.SetParent(parent.transform);
            Image scrollImg = parentScrollHolder.AddComponent<Image>();
            scrollImg.sprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.windowInnerSliced.png");
            RectTransform scrollHolderRect = parentScrollHolder.GetComponent<RectTransform>();
            scrollHolderRect.localPosition = pos;
            scrollHolderRect.sizeDelta = size;

            GameObject infoHolder = new GameObject("titleInfoHolder");
            infoHolder.transform.SetParent(parentScrollHolder.transform);
            infoHolder.AddComponent<Image>().color = new Color(0, 0, 0, 0.01f);
            RectTransform infoRect = infoHolder.GetComponent<RectTransform>();
            infoRect.sizeDelta = infoSize;
            ScrollRect scroll = parentScrollHolder.AddComponent<ScrollRect>();
            scroll.scrollSensitivity = 13f;
            scroll.viewport = parentScrollHolder.GetComponent<RectTransform>();
            scroll.content = infoHolder.GetComponent<RectTransform>();
            parentScrollHolder.AddComponent<Mask>();
            infoRect.localPosition = new Vector3(25, 0, 0);

            return infoHolder;
        }

        public static GameObject createProgressBar(GameObject parent, Vector3 pos)
        {
            GameObject researchBar = GameObjects.FindEvenInactive("HealthBar");
            GameObject progressBar = Object.Instantiate(researchBar, parent.transform);
            progressBar.name = "ProgressBar";
            progressBar.SetActive(true);

            RectTransform progressRect = progressBar.GetComponent<RectTransform>();
            progressRect.localPosition = pos;

            StatBar statBar = progressBar.GetComponent<StatBar>();
            statBar.CallMethod("restartBar");

            TipButton tipButton = progressBar.GetComponent<TipButton>();
            tipButton.textOnClick = "Progress Bar";

            GameObject icon = progressBar.transform.Find("Icon").gameObject;
            icon.SetActive(false);

            return progressBar;
        }

        public static GameObject createCultureBanner(GameObject parent, Culture culture, Vector3 pos)
        {
            GameObject cultureHolder = new GameObject("cultureHolder");
            cultureHolder.transform.SetParent(parent.transform);
            Image cultureHolderImg = cultureHolder.AddComponent<Image>();
            cultureHolderImg.sprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.culture_background.png");

            GameObject partIcon = new GameObject("partIcon");
            partIcon.transform.SetParent(cultureHolder.transform);
            Image partIconImg = partIcon.AddComponent<Image>();
            partIcon.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            GameObject partIconDecoration = new GameObject("partIconDecoration");
            partIconDecoration.transform.SetParent(cultureHolder.transform);
            Image partIconDecorationImg = partIconDecoration.AddComponent<Image>();
            partIconDecoration.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

            cultureHolder.AddComponent<Button>();
            cultureHolder.AddComponent<TipButton>().type = "culture";
            BannerLoaderCulture loader = cultureHolder.AddComponent<BannerLoaderCulture>();
            loader.partIcon = partIconImg;
            loader.partIconDecoration = partIconDecorationImg;
            loader.load(culture);
            cultureHolder.GetComponent<RectTransform>().localPosition = pos;

            return cultureHolder;
        }
        public static GameObject createKingdomBanner(GameObject parent, Kingdom kingdom, Vector3 pos)
        {
            GameObject kingdomHolder = new("kingdomHolder");
            kingdomHolder.transform.SetParent(parent.transform);
            Image kingdomHolderImg = kingdomHolder.AddComponent<Image>();
            kingdomHolderImg.sprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.kingdombanner.png");

            GameObject backgroundGO = new("background");
            backgroundGO.transform.SetParent(kingdomHolder.transform);
            Image backgroundImage = backgroundGO.AddComponent<Image>();
            backgroundGO.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

            GameObject partIcon = new("partIcon");
            partIcon.transform.SetParent(kingdomHolder.transform);
            Image partIconImg = partIcon.AddComponent<Image>();
            partIcon.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

            //调整子对象在父对象的子对象列表中的顺序
            partIcon.transform.SetAsLastSibling();

            kingdomHolder.AddComponent<Button>();
            kingdomHolder.AddComponent<TipButton>().type = "kingdom";
            BannerLoader loader = kingdomHolder.AddComponent<BannerLoader>();
            loader.partIcon = partIconImg;
            loader.partBackround = backgroundImage;
            loader.load(kingdom);
            kingdomHolder.GetComponent<RectTransform>().localPosition = pos;

            return kingdomHolder;
        }
    }
}