using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OmniumLessons
{
    public class UpgradeSelectionService
    {
        private const int UpgradeChoicesCount = 3;

        private readonly UpgradeAvailabilityService _availabilityService;
        private readonly UpgradeRarityService _rarityService;
        private readonly UpgradeCostService _costService;
        private readonly UpgradeOfferFactory _offerFactory;
        private readonly UpgradeFallbackService _fallbackService;

        public UpgradeSelectionService()
        {
            _availabilityService = new UpgradeAvailabilityService();
            _rarityService = new UpgradeRarityService();
            _costService = new UpgradeCostService();
            _offerFactory = new UpgradeOfferFactory(_costService);
            _fallbackService = new UpgradeFallbackService(_costService);
        }

        public List<UpgradeOfferData> GetUpgradeOffers(List<UpgradeData> allUpgrades)
        {
            List<UpgradeOfferData> result = new List<UpgradeOfferData>();

            if (allUpgrades == null || allUpgrades.Count == 0)
                return result;

            PlayerCharacter player = GameManager.Instance.CharacterFactory.Player as PlayerCharacter;
            if (player == null)
                return result;

            int currentResonance = 0;

            if (GameManager.Instance.ResonanceManager != null)
                currentResonance = GameManager.Instance.ResonanceManager.CurrentResonance;

            List<UpgradeData> availablePool = _availabilityService.GetAvailableUpgrades(allUpgrades, player);

            if (availablePool.Count == 0)
                return result;

            List<UpgradeData> usedUpgrades = new List<UpgradeData>();

            for (int i = 0; i < UpgradeChoicesCount; i++)
            {
                UpgradeData selectedUpgrade = RollUpgradeForSlot(availablePool, usedUpgrades, currentResonance);

                if (selectedUpgrade == null)
                    continue;

                UpgradeOfferData offer = _offerFactory.CreateOffer(selectedUpgrade, currentResonance);

                if (offer == null)
                    continue;

                result.Add(offer);
                usedUpgrades.Add(selectedUpgrade);
            }

            _fallbackService.EnsureAtLeastOneAffordableOffer(result, availablePool, currentResonance);

            return result;
        }

        private UpgradeData RollUpgradeForSlot(List<UpgradeData> availablePool, List<UpgradeData> usedUpgrades, int currentResonance)
        {
            UpgradeRarity targetRarity = _rarityService.RollRarityByResonance(currentResonance);
            UpgradeData selectedUpgrade = GetRandomUpgradeByRarity(availablePool, usedUpgrades, targetRarity);

            if (selectedUpgrade != null)
                return selectedUpgrade;

            return GetRandomUpgradeAnyRarity(availablePool, usedUpgrades);
        }

        private UpgradeData GetRandomUpgradeByRarity(List<UpgradeData> pool, List<UpgradeData> usedUpgrades, UpgradeRarity rarity)
        {
            List<UpgradeData> filtered = new List<UpgradeData>();

            for (int i = 0; i < pool.Count; i++)
            {
                UpgradeData upgrade = pool[i];

                if (upgrade == null)
                    continue;

                if (usedUpgrades.Contains(upgrade))
                    continue;

                if (upgrade.Rarity != rarity)
                    continue;

                filtered.Add(upgrade);
            }

            if (filtered.Count == 0)
                return null;

            return filtered[Random.Range(0, filtered.Count)];
        }

        private UpgradeData GetRandomUpgradeAnyRarity(List<UpgradeData> pool, List<UpgradeData> usedUpgrades)
        {
            List<UpgradeData> filtered = new List<UpgradeData>();

            for (int i = 0; i < pool.Count; i++)
            {
                UpgradeData upgrade = pool[i];

                if (upgrade == null)
                    continue;

                if (usedUpgrades.Contains(upgrade))
                    continue;

                filtered.Add(upgrade);
            }

            if (filtered.Count == 0)
                return null;

            return filtered[Random.Range(0, filtered.Count)];
        }
    }
}