using UnityEngine;

namespace OmniumLessons
{
    public class UpgradeTextBuilder
    {
        public string BuildTitle(UpgradeData upgradeData, PlayerCharacter player)
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
                        AbilityUpgradeData ability = upgradeData as AbilityUpgradeData;

                        if (ability == null || player.AbilityManager == null)
                            return baseName;

                        int current = player.AbilityManager.GetAbilityLevel(ability);
                        int next = Mathf.Min(current + 1, ability.MaxLevel);

                        if (current <= 0)
                            return $"{baseName} Lv1";

                        if (current >= ability.MaxLevel)
                            return $"{baseName} Lv{ability.MaxLevel}";

                        return $"{baseName} Lv{current} → Lv{next}";
                    }

                case UpgradeType.AttackModifier:
                    {
                        AttackModifierUpgradeData mod = upgradeData as AttackModifierUpgradeData;

                        if (mod == null || player.AttackModifierController == null)
                            return baseName;

                        int current = player.AttackModifierController.GetModifierLevel(mod.ModifierType);
                        int next = Mathf.Min(current + 1, mod.MaxLevel);

                        if (current <= 0)
                            return $"{baseName} Lv1";

                        if (current >= mod.MaxLevel)
                            return $"{baseName} Lv{mod.MaxLevel}";

                        return $"{baseName} Lv{current} → Lv{next}";
                    }
            }

            return baseName;
        }

        public string BuildDescription(UpgradeData upgradeData, PlayerCharacter player)
        {
            if (upgradeData == null)
                return string.Empty;

            if (player == null)
                return upgradeData.Description;

            switch (upgradeData.UpgradeType)
            {
                case UpgradeType.Ability:
                    {
                        AbilityUpgradeData ability = upgradeData as AbilityUpgradeData;

                        if (ability == null || player.AbilityManager == null)
                            return upgradeData.Description;

                        int current = player.AbilityManager.GetAbilityLevel(ability);
                        return ability.GetNextLevelDescription(current, ability.MaxLevel);
                    }

                case UpgradeType.AttackModifier:
                    {
                        AttackModifierUpgradeData mod = upgradeData as AttackModifierUpgradeData;

                        if (mod == null || player.AttackModifierController == null)
                            return upgradeData.Description;

                        int current = player.AttackModifierController.GetModifierLevel(mod.ModifierType);
                        return mod.GetNextLevelDescription(current, mod.MaxLevel);
                    }
            }

            return upgradeData.Description;
        }
    }
}