using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class UpgradeButton : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private Image iconImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Button selectButton;

        [Header("Colors")]
        [SerializeField] private Color commonColor = new Color(0.55f, 0.55f, 0.55f);
        [SerializeField] private Color uncommonColor = new Color(0.35f, 0.75f, 0.35f);
        [SerializeField] private Color rareColor = new Color(0.35f, 0.75f, 1f);
        [SerializeField] private Color resonantColor = new Color(1f, 0.82f, 0.2f);
        [SerializeField] private Color unavailableColor = new Color(0.35f, 0.35f, 0.35f, 1f);
        [SerializeField] private bool dimUnavailable = true;

        private UpgradeOfferData _offer;

        private UpgradeVisualService _visualService;
        private UpgradeTextBuilder _textBuilder;

        public void Setup(UpgradeOfferData offer)
        {
            _offer = offer;

            if (_offer == null || _offer.UpgradeData == null)
                return;

            PlayerCharacter player = GameManager.Instance?.CharacterFactory?.Player as PlayerCharacter;

            _visualService = new UpgradeVisualService(
                commonColor,
                uncommonColor,
                rareColor,
                resonantColor,
                unavailableColor,
                dimUnavailable
            );

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
                if (_offer.ResonanceCost > 0)
                    costText.text = $"{_offer.ResonanceCost} R";
                else
                    costText.text = "FREE";
            }
        }

        private void SetupVisuals()
        {
            if (backgroundImage != null)
                backgroundImage.color = _visualService.GetBackgroundColor(_offer);

            if (iconImage != null)
                iconImage.sprite = _offer.UpgradeData.Icon;
        }

        private void SetupButton()
        {
            if (selectButton == null)
                return;

            selectButton.interactable = _offer.IsAvailable;
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelect);
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