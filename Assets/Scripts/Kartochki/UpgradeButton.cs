using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Image iconImage;
        [SerializeField] private Button selectButton;

        private UpgradeData _upgradeData;

        public void Setup(UpgradeData upgradeData)
        {
            _upgradeData = upgradeData;

            nameText.text = upgradeData.UpgradeName;
            descriptionText.text = upgradeData.Description;
            iconImage.sprite = upgradeData.Icon;

            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelect);
        }

        private void OnSelect()
        {
            PlayerCharacter player = GameManager.Instance.CharacterFactory.Player as PlayerCharacter;

            if (player == null)
                return;

            switch (_upgradeData.UpgradeType)
            {
                case UpgradeType.Ability:
                    {
                        AbilityUpgradeData abilityUpgrade = _upgradeData as AbilityUpgradeData;
                        if (abilityUpgrade != null)
                        {
                            player.AbilityManager.AddAbility(abilityUpgrade);
                        }
                        break;
                    }

                case UpgradeType.AttackModifier:
                    {
                        AttackModifierUpgradeData modifierUpgrade = _upgradeData as AttackModifierUpgradeData;
                        if (modifierUpgrade != null)
                        {
                            player.AddAttackModifier(modifierUpgrade);
                        }
                        break;
                    }
            }

            GameManager.Instance.IsGamePaused = false;
            Time.timeScale = 1f;
            GameManager.Instance.WindowsService.HideWindow<SkillsWindow>(true);
        }
    }
}