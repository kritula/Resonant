using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text costText;

        [SerializeField] private Image _cardImage;
        [SerializeField] private Button selectButton;

        [Header("Unavailable Visual")]
        [SerializeField] private Color unavailableColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        [SerializeField] private bool dimUnavailable = true;

        [Header("Hover Visual")]
        [SerializeField] private Color hoverColor = new Color(1.15f, 1.15f, 1.15f, 1f);
        [SerializeField] private float hoverScale = 1.05f;
        [SerializeField] private float hoverAnimationSpeed = 12f;

        private UpgradeOfferData _offer;
        private UpgradeTextBuilder _textBuilder;

        private Vector3 _defaultScale;
        private Color _defaultCardColor;
        private Color _targetColor;
        private Vector3 _targetScale;

        private bool _isHovered;

        private void Awake()
        {
            _defaultScale = transform.localScale;
            _targetScale = _defaultScale;

            if (_cardImage != null)
                _defaultCardColor = _cardImage.color;

            _targetColor = _defaultCardColor;
        }

        private void Update()
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                _targetScale,
                Time.unscaledDeltaTime * hoverAnimationSpeed
            );

            if (_cardImage != null)
            {
                _cardImage.color = Color.Lerp(
                    _cardImage.color,
                    _targetColor,
                    Time.unscaledDeltaTime * hoverAnimationSpeed
                );
            }
        }

        public void Setup(UpgradeOfferData offer)
        {
            _offer = offer;

            if (_offer == null || _offer.UpgradeData == null)
                return;

            PlayerCharacter player = GameManager.Instance?.CharacterFactory?.Player as PlayerCharacter;

            _textBuilder = new UpgradeTextBuilder();

            SetupTexts(player);
            SetupVisuals();
            SetupButton();
        }

        private void SetupTexts(PlayerCharacter player)
        {
            if (nameText != null)
                nameText.text = _textBuilder.BuildTitle(_offer.UpgradeData, player);

            if (descriptionText != null)
                descriptionText.text = _textBuilder.BuildDescription(_offer.UpgradeData, player);

            if (costText != null)
            {
                costText.text = _offer.ResonanceCost > 0
                    ? $"{_offer.ResonanceCost} R"
                    : "FREE";
            }
        }

        private void SetupVisuals()
        {
            UpgradeData upgradeData = _offer.UpgradeData;

            if (_cardImage != null && upgradeData.CardSprite != null)
            {
                _cardImage.sprite = upgradeData.CardSprite;

                _defaultCardColor = GetAvailabilityColor();
                _targetColor = _defaultCardColor;
                _cardImage.color = _defaultCardColor;
            }

            _defaultScale = transform.localScale;
            _targetScale = _defaultScale;
        }

        private Color GetAvailabilityColor()
        {
            if (_offer.IsAvailable || !dimUnavailable)
                return Color.white;

            return unavailableColor;
        }

        private void SetupButton()
        {
            if (selectButton == null)
                return;

            selectButton.interactable = _offer.IsAvailable;
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelect);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_offer == null || !_offer.IsAvailable)
                return;

            _isHovered = true;
            _targetScale = _defaultScale * hoverScale;
            _targetColor = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            _targetScale = _defaultScale;
            _targetColor = _defaultCardColor;
        }

        private void OnSelect()
        {
            PlayerCharacter player = GameManager.Instance?.CharacterFactory?.Player as PlayerCharacter;

            if (player == null)
                return;

            if (_offer.ResonanceCost > 0)
            {
                if (!GameManager.Instance.ResonanceManager.TrySpendResonance(_offer.ResonanceCost))
                    return;
            }

            ApplyUpgrade(player);

            GameManager.Instance.IsGamePaused = false;
            GameManager.Instance.WindowsService.HideWindow<SkillsWindow>(true);
            Time.timeScale = 1f;
        }

        private void ApplyUpgrade(PlayerCharacter player)
        {
            switch (_offer.UpgradeData.UpgradeType)
            {
                case UpgradeType.Ability:
                    player.AbilityManager.AddAbility((AbilityUpgradeData)_offer.UpgradeData);
                    break;

                case UpgradeType.AttackModifier:
                    player.AddAttackModifier((AttackModifierUpgradeData)_offer.UpgradeData);
                    break;
            }
        }
    }
}