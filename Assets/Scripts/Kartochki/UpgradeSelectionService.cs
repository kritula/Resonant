using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class UpgradeSelectionService
    {
        private UpgradeDatabase _database;

        public UpgradeSelectionService(UpgradeDatabase database)
        {
            _database = database;
        }

        public List<UpgradeData> GetRandomUpgrades(int count, PlayerCharacter player)
        {
            List<UpgradeData> result = new List<UpgradeData>();
            List<UpgradeData> pool = new List<UpgradeData>();

            for (int i = 0; i < _database.Upgrades.Count; i++)
            {
                UpgradeData upgrade = _database.Upgrades[i];

                if (!CanSelectUpgrade(upgrade, player))
                    continue;

                pool.Add(upgrade);
            }

            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                int index = Random.Range(0, pool.Count);
                result.Add(pool[index]);
                pool.RemoveAt(index);
            }

            return result;
        }

        private bool CanSelectUpgrade(UpgradeData upgradeData, PlayerCharacter player)
        {
            if (upgradeData == null)
                return false;

            if (upgradeData.UpgradeType == UpgradeType.Ability)
            {
                AbilityUpgradeData abilityUpgrade = upgradeData as AbilityUpgradeData;

                if (abilityUpgrade == null)
                    return false;

                if (player == null || player.AbilityManager == null)
                    return true;

                if (player.AbilityManager.IsAbilityMaxLevel(abilityUpgrade))
                    return false;
            }

            return true;
        }
    }
}