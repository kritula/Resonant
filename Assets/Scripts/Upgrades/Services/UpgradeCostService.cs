using UnityEngine;
using Random = UnityEngine.Random;

namespace OmniumLessons
{
    public class UpgradeCostService
    {
        public int RollResonanceCost(UpgradeRarity rarity)
        {
            switch (rarity)
            {
                case UpgradeRarity.Common:
                    return 0;

                case UpgradeRarity.Uncommon:
                    return Random.Range(10, 16);

                case UpgradeRarity.Rare:
                    return Random.Range(15, 36);

                case UpgradeRarity.Resonant:
                    return Random.Range(40, 71);
            }

            return 0;
        }

        public int GetMinimumResonanceCost(UpgradeRarity rarity)
        {
            switch (rarity)
            {
                case UpgradeRarity.Common:
                    return 0;

                case UpgradeRarity.Uncommon:
                    return 10;

                case UpgradeRarity.Rare:
                    return 15;

                case UpgradeRarity.Resonant:
                    return 40;
            }

            return 0;
        }
    }
}