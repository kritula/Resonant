using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class SkillsWindow : Window
    {
        [SerializeField] private Transform container;
        [SerializeField] private UpgradeButton buttonPrefab;
        [SerializeField] private float screenWidthUsage = 0.9f;
        [SerializeField] private float screenHeightUsage = 0.9f;
        [SerializeField] private float preferredCardSpacing = 24f;
        [SerializeField] private float minCardSpacing = 8f;

        private readonly List<UpgradeButton> _buttons = new List<UpgradeButton>();
        private RectTransform _containerRect;
        private HorizontalLayoutGroup _layoutGroup;

        public override void Initialize()
        {
            base.Initialize();
            CacheLayout();
            ConfigureContainer();
        }

        public void ShowUpgrades(List<UpgradeOfferData> upgradeOffers)
        {
            ClearButtons();
            ConfigureContainer();

            if (upgradeOffers == null || upgradeOffers.Count == 0)
                return;

            for (int i = 0; i < upgradeOffers.Count; i++)
            {
                UpgradeOfferData offer = upgradeOffers[i];

                if (offer == null || offer.UpgradeData == null)
                    continue;

                UpgradeButton button = Instantiate(buttonPrefab, container);
                button.Setup(offer);
                _buttons.Add(button);
            }

            FitContainerToScreen(_buttons.Count);
        }

        private void ClearButtons()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i] != null)
                {
                    Destroy(_buttons[i].gameObject);
                }
            }

            _buttons.Clear();
        }

        public override void Hide(bool isImmediately = false)
        {
            base.Hide(isImmediately);
            ClearButtons();
        }

        private void CacheLayout()
        {
            if (container == null)
                return;

            _containerRect = container as RectTransform;
            _layoutGroup = container.GetComponent<HorizontalLayoutGroup>();
        }

        private void ConfigureContainer()
        {
            if (_containerRect == null)
                CacheLayout();

            if (_containerRect != null)
            {
                _containerRect.anchorMin = Vector2.zero;
                _containerRect.anchorMax = Vector2.one;
                _containerRect.offsetMin = Vector2.zero;
                _containerRect.offsetMax = Vector2.zero;
                _containerRect.anchoredPosition = Vector2.zero;
                _containerRect.localScale = Vector3.one;
            }

            if (_layoutGroup != null)
            {
                _layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                _layoutGroup.childForceExpandWidth = false;
                _layoutGroup.childForceExpandHeight = false;
                _layoutGroup.childControlWidth = false;
                _layoutGroup.childControlHeight = false;
                _layoutGroup.spacing = preferredCardSpacing;
            }
        }

        private void FitContainerToScreen(int choicesCount)
        {
            if (_containerRect == null || choicesCount <= 0)
                return;

            Canvas.ForceUpdateCanvases();

            float availableWidth = GetWindowSize().x * screenWidthUsage;
            float availableHeight = GetWindowSize().y * screenHeightUsage;
            float cardWidth = GetButtonPreferredWidth();
            float cardHeight = GetButtonPreferredHeight();
            float spacing = GetSpacingForWidth(availableWidth, cardWidth, choicesCount);
            float contentWidth = cardWidth * choicesCount + spacing * (choicesCount - 1);

            if (_layoutGroup != null)
                _layoutGroup.spacing = spacing;

            float widthScale = contentWidth > 0f ? availableWidth / contentWidth : 1f;
            float heightScale = cardHeight > 0f ? availableHeight / cardHeight : 1f;
            float targetScale = Mathf.Min(1f, widthScale, heightScale);

            _containerRect.localScale = Vector3.one * Mathf.Max(0.1f, targetScale);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_containerRect);
        }

        private float GetSpacingForWidth(float availableWidth, float cardWidth, int choicesCount)
        {
            if (_layoutGroup == null || choicesCount <= 1)
                return 0f;

            float maxSpacingThatFits =
                (availableWidth - cardWidth * choicesCount) / (choicesCount - 1);

            return Mathf.Clamp(maxSpacingThatFits, minCardSpacing, preferredCardSpacing);
        }

        private Vector2 GetWindowSize()
        {
            RectTransform windowRect = transform as RectTransform;

            if (windowRect != null && windowRect.rect.width > 0f && windowRect.rect.height > 0f)
                return windowRect.rect.size;

            return new Vector2(Screen.width, Screen.height);
        }

        private float GetButtonPreferredWidth()
        {
            LayoutElement layoutElement = buttonPrefab != null
                ? buttonPrefab.GetComponent<LayoutElement>()
                : null;

            RectTransform buttonRect = buttonPrefab != null
                ? buttonPrefab.transform as RectTransform
                : null;

            float rectWidth = buttonRect != null && buttonRect.rect.width > 0f
                ? buttonRect.rect.width
                : 0f;

            float layoutWidth = layoutElement != null && layoutElement.preferredWidth > 0f
                ? layoutElement.preferredWidth
                : 0f;

            return Mathf.Max(rectWidth, layoutWidth, 300f);
        }

        private float GetButtonPreferredHeight()
        {
            LayoutElement layoutElement = buttonPrefab != null
                ? buttonPrefab.GetComponent<LayoutElement>()
                : null;

            RectTransform buttonRect = buttonPrefab != null
                ? buttonPrefab.transform as RectTransform
                : null;

            float rectHeight = buttonRect != null && buttonRect.rect.height > 0f
                ? buttonRect.rect.height
                : 0f;

            float layoutHeight = layoutElement != null && layoutElement.preferredHeight > 0f
                ? layoutElement.preferredHeight
                : 0f;

            return Mathf.Max(rectHeight, layoutHeight, 900f);
        }
    }
}
