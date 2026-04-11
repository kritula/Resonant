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

            if (_upgradeData == null)
                return;

            PlayerCharacter player = GameManager.Instance?.CharacterFactory?.Player as PlayerCharacter;

            if (nameText != null)
            {
                nameText.text = BuildUpgradeTitle(_upgradeData, player);
            }

            if (descriptionText != null)
            {
                descriptionText.text = BuildUpgradeDescription(_upgradeData, player);
            }

            if (iconImage != null)
            {
                iconImage.sprite = _upgradeData.Icon;
            }

            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(OnSelect);
            }
        }

        private string BuildUpgradeTitle(UpgradeData upgradeData, PlayerCharacter player)
        {
            if (upgradeData == null)
                return string.Empty;

            string baseName = upgradeData.UpgradeName;

            if (player == null)
                return baseName;

            switch (upgradeData.UpgradeType)
            {
                case UpgradeType.Ability:
                    {
                        AbilityUpgradeData abilityUpgrade = upgradeData as AbilityUpgradeData;

                        if (abilityUpgrade == null || player.AbilityManager == null)
                            return baseName;

                        int currentLevel = player.AbilityManager.GetAbilityLevel(abilityUpgrade);
                        int nextLevel = Mathf.Min(currentLevel + 1, abilityUpgrade.MaxLevel);

                        if (currentLevel <= 0)
                            return $"{baseName} Lv1";

                        if (currentLevel >= abilityUpgrade.MaxLevel)
                            return $"{baseName} Lv{abilityUpgrade.MaxLevel}";

                        return $"{baseName} Lv{currentLevel} → Lv{nextLevel}";
                    }

                case UpgradeType.AttackModifier:
                    {
                        AttackModifierUpgradeData modifierUpgrade = upgradeData as AttackModifierUpgradeData;

                        if (modifierUpgrade == null || player.AttackModifierController == null)
                            return baseName;

                        int currentLevel = player.AttackModifierController.GetModifierLevel(modifierUpgrade.ModifierType);
                        int nextLevel = Mathf.Min(currentLevel + 1, modifierUpgrade.MaxLevel);

                        if (currentLevel <= 0)
                            return $"{baseName} Lv1";

                        if (currentLevel >= modifierUpgrade.MaxLevel)
                            return $"{baseName} Lv{modifierUpgrade.MaxLevel}";

                        return $"{baseName} Lv{currentLevel} → Lv{nextLevel}";
                    }
            }

            return baseName;
        }

        private string BuildUpgradeDescription(UpgradeData upgradeData, PlayerCharacter player)
        {
            if (upgradeData == null)
                return string.Empty;

            if (player == null)
                return upgradeData.Description;

            switch (upgradeData.UpgradeType)
            {
                case UpgradeType.Ability:
                    {
                        AbilityUpgradeData abilityUpgrade = upgradeData as AbilityUpgradeData;

                        if (abilityUpgrade == null || player.AbilityManager == null)
                            return upgradeData.Description;

                        int currentLevel = player.AbilityManager.GetAbilityLevel(abilityUpgrade);
                        return abilityUpgrade.GetNextLevelDescription(currentLevel, abilityUpgrade.MaxLevel);
                    }

                case UpgradeType.AttackModifier:
                    {
                        AttackModifierUpgradeData modifierUpgrade = upgradeData as AttackModifierUpgradeData;

                        if (modifierUpgrade == null || player.AttackModifierController == null)
                            return upgradeData.Description;

                        int currentLevel = player.AttackModifierController.GetModifierLevel(modifierUpgrade.ModifierType);
                        return modifierUpgrade.GetNextLevelDescription(currentLevel, modifierUpgrade.MaxLevel);
                    }
            }

            return upgradeData.Description;
        }

        private void OnSelect()
        {
            PlayerCharacter player = GameManager.Instance?.CharacterFactory?.Player as PlayerCharacter;

            if (player == null)
                return;

            if (_upgradeData == null)
                return;

            switch (_upgradeData.UpgradeType)
            {
                case UpgradeType.Ability:
                    {
                        AbilityUpgradeData abilityUpgrade = _upgradeData as AbilityUpgradeData;

                        if (abilityUpgrade != null && player.AbilityManager != null)
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

            if (GameManager.Instance != null)
            {
                GameManager.Instance.IsGamePaused = false;

                if (GameManager.Instance.WindowsService != null)
                {
                    GameManager.Instance.WindowsService.HideWindow<SkillsWindow>(true);
                }
            }

            Time.timeScale = 1f;
        }
    }
}