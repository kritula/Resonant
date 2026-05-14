using System.Collections.Generic;

namespace OmniumLessons
{
    public class UpgradeAvailabilityService
    {
        public List<UpgradeData> GetAvailableUpgrades(List<UpgradeData> allUpgrades, PlayerCharacter player)
        {
            List<UpgradeData> result = new List<UpgradeData>();

            if (allUpgrades == null || player == null)
                return result;

            for (int i = 0; i < allUpgrades.Count; i++)
            {
                UpgradeData upgrade = allUpgrades[i];

                if (upgrade == null)
                    continue;

                if (IsUpgradeAvailableForPlayer(upgrade, player))
                    result.Add(upgrade);
            }

            return result;
        }

        public bool IsUpgradeAvailableForPlayer(UpgradeData upgradeData, PlayerCharacter player)
        {
            if (upgradeData == null || player == null)
                return false;

            switch (upgradeData.UpgradeType)
            {
                case UpgradeType.Ability:
                    {
                        AbilityUpgradeData abilityUpgradeData = upgradeData as AbilityUpgradeData;

                        if (abilityUpgradeData == null || player.AbilityManager == null)
                            return false;

                        int currentLevel = player.AbilityManager.GetAbilityLevel(abilityUpgradeData);
                        return currentLevel < abilityUpgradeData.MaxLevel;
                    }

                case UpgradeType.AttackModifier:
                    {
                        AttackModifierUpgradeData modifierUpgradeData = upgradeData as AttackModifierUpgradeData;

                        if (modifierUpgradeData == null || player.AttackModifierController == null)
                            return false;

                        int currentLevel = player.AttackModifierController.GetModifierLevel(modifierUpgradeData.ModifierType);
                        return currentLevel < modifierUpgradeData.MaxLevel;
                    }
            }

            return false;
        }
    }
}