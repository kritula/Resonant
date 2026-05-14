using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class MainMenuWindow : Window
    {
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button arsenalButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitGameButton;

        private WindowsService _windowsService;

        public override void Initialize()
        {
            _windowsService = GetComponentInParent<WindowsService>();

            startGameButton?.onClick.AddListener(StartGameHandler);
            arsenalButton?.onClick.AddListener(ArsenalHandler);
            settingsButton?.onClick.AddListener(OpenSettingsHandler);
            exitGameButton?.onClick.AddListener(ExitGameHandler);
        }

        protected override void OpenEnd()
        {
            base.OpenEnd();
            SetButtonsInteractable(true);
        }

        protected override void CloseStart()
        {
            base.CloseStart();
            SetButtonsInteractable(false);
        }

        private void StartGameHandler()
        {
            SceneLoader.LoadGameLevel();
        }

        private void ArsenalHandler()
        {
            Debug.Log("Arsenal button clicked. Arsenal window is not implemented yet.");
        }

        private void OpenSettingsHandler()
        {
            if (_windowsService == null)
                _windowsService = GetComponentInParent<WindowsService>();

            if (_windowsService == null || _windowsService.HasWindow<SettingsWindow>() == false)
            {
                CreateFallbackSettingsWindow();
            }

            if (_windowsService == null)
            {
                Debug.LogError("WindowsService not found for MainMenuWindow.");
                return;
            }

            Hide(true);
            _windowsService.ShowWindow<SettingsWindow>(false);
        }

        private void ExitGameHandler()
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private void SetButtonsInteractable(bool isInteractable)
        {
            if (startGameButton != null)
                startGameButton.interactable = isInteractable;

            if (arsenalButton != null)
                arsenalButton.interactable = isInteractable;

            if (settingsButton != null)
                settingsButton.interactable = isInteractable;

            if (exitGameButton != null)
                exitGameButton.interactable = isInteractable;
        }

        private void CreateFallbackSettingsWindow()
        {
            if (_windowsService == null || _windowsService.HasWindow<SettingsWindow>())
                return;

            Transform parent = _windowsService.transform;
            GameObject windowObject = new GameObject(
                "SettingsWindow",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(SettingsWindow));

            windowObject.transform.SetParent(parent, false);

            RectTransform windowRect = windowObject.GetComponent<RectTransform>();
            windowRect.anchorMin = Vector2.zero;
            windowRect.anchorMax = Vector2.one;
            windowRect.offsetMin = Vector2.zero;
            windowRect.offsetMax = Vector2.zero;

            Image background = windowObject.GetComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.85f);

            VerticalLayoutGroup layout = windowObject.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            layout.spacing = 24f;

            SettingsWindow settingsWindow = windowObject.GetComponent<SettingsWindow>();
            Toggle sound = CreateToggle(windowRect, "Sound");
            Toggle music = CreateToggle(windowRect, "Music");
            Toggle vibration = CreateToggle(windowRect, "Vibration");
            Toggle notifications = CreateToggle(windowRect, "Notifications");
            Button quit = CreateButton(windowRect, "QUIT");

            settingsWindow.BindControls(sound, music, vibration, notifications, quit);
            _windowsService.RegisterWindow(settingsWindow);
        }

        private Toggle CreateToggle(RectTransform parent, string label)
        {
            GameObject toggleObject = new GameObject(
                label + "Toggle",
                typeof(RectTransform),
                typeof(Toggle),
                typeof(HorizontalLayoutGroup));

            toggleObject.transform.SetParent(parent, false);

            RectTransform rectTransform = toggleObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(520f, 72f);

            HorizontalLayoutGroup layout = toggleObject.GetComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.spacing = 24f;

            Toggle toggle = toggleObject.GetComponent<Toggle>();
            Image box = CreateImage(rectTransform, "Box", new Vector2(56f, 56f), Color.white);
            Image checkmark = CreateImage(box.rectTransform, "Checkmark", new Vector2(36f, 36f), Color.cyan);
            Text text = CreateText(rectTransform, label, 36, TextAnchor.MiddleLeft);

            toggle.targetGraphic = box;
            toggle.graphic = checkmark;
            toggle.isOn = true;

            LayoutElement textLayout = text.gameObject.AddComponent<LayoutElement>();
            textLayout.preferredWidth = 420f;

            return toggle;
        }

        private Button CreateButton(RectTransform parent, string label)
        {
            GameObject buttonObject = new GameObject(
                label + "Button",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(Button));

            buttonObject.transform.SetParent(parent, false);

            RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(320f, 82f);

            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.24f, 0.27f, 0.56f, 1f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;

            RectTransform textParent = rectTransform;
            Text text = CreateText(textParent, label, 38, TextAnchor.MiddleCenter);
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            return button;
        }

        private Image CreateImage(RectTransform parent, string name, Vector2 size, Color color)
        {
            GameObject imageObject = new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));

            imageObject.transform.SetParent(parent, false);

            RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = size;

            Image image = imageObject.GetComponent<Image>();
            image.color = color;

            return image;
        }

        private Text CreateText(RectTransform parent, string value, int fontSize, TextAnchor alignment)
        {
            GameObject textObject = new GameObject(
                value + "Text",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Text));

            textObject.transform.SetParent(parent, false);

            RectTransform rectTransform = textObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(420f, 72f);

            Text text = textObject.GetComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;

            return text;
        }
    }
}
