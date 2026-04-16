using UnityEngine;
using Random = UnityEngine.Random;

namespace OmniumLessons
{
    public class UpgradeRarityService
    {
        public UpgradeRarity RollRarityByResonance(int resonance)
        {
            int roll = Random.Range(0, 100);

            if (resonance <= 0)
            {
                if (roll < 90)
                    return UpgradeRarity.Common;

                return UpgradeRarity.Uncommon;
            }

            if (resonance <= 19)
            {
                if (roll < 75)
                    return UpgradeRarity.Common;

                if (roll < 95)
                    return UpgradeRarity.Uncommon;

                return UpgradeRarity.Rare;
            }

            if (resonance <= 39)
            {
                if (roll < 60)
                    return UpgradeRarity.Common;

                if (roll < 80)
                    return UpgradeRarity.Uncommon;

                if (roll < 95)
                    return UpgradeRarity.Rare;

                return UpgradeRarity.Resonant;
            }

            if (resonance <= 69)
            {
                if (roll < 35)
                    return UpgradeRarity.Common;

                if (roll < 70)
                    return UpgradeRarity.Uncommon;

                if (roll < 90)
                    return UpgradeRarity.Rare;

                return UpgradeRarity.Resonant;
            }

            if (resonance <= 99)
            {
                if (roll < 20)
                    return UpgradeRarity.Common;

                if (roll < 40)
                    return UpgradeRarity.Uncommon;

                if (roll < 80)
                    return UpgradeRarity.Rare;

                return UpgradeRarity.Resonant;
            }

            if (roll < 10)
                return UpgradeRarity.Common;

            if (roll < 25)
                return UpgradeRarity.Uncommon;

            if (roll < 65)
                return UpgradeRarity.Rare;

            return UpgradeRarity.Resonant;
        }
    }
}