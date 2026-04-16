using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OmniumLessons
{
    public class UpgradeFallbackService
    {
        private readonly UpgradeCostService _costService;

        public UpgradeFallbackService(UpgradeCostService costService)
        {
            _costService = costService;
        }

        public void EnsureAtLeastOneAffordableOffer(List<UpgradeOfferData> offers, List<UpgradeData> availablePool, int currentResonance)
        {
            if (offers == null || offers.Count == 0)
                return;

            if (HasAffordableOffer(offers))
                return;

            int replaceIndex = Random.Range(0, offers.Count);
            UpgradeData replacement = GetAffordableReplacement(availablePool, offers, currentResonance);

            if (replacement == null)
            {
                if (offers[replaceIndex] != null)
                {
                    offers[replaceIndex].ResonanceCost = 0;
                    offers[replaceIndex].IsAvailable = true;
                }

                return;
            }

            offers[replaceIndex] = new UpgradeOfferData()
            {
                UpgradeData = replacement,
                Rarity = replacement.Rarity,
                ResonanceCost = 0,
                IsAvailable = true
            };
        }

        private bool HasAffordableOffer(List<UpgradeOfferData> offers)
        {
            for (int i = 0; i < offers.Count; i++)
            {
                if (offers[i] != null && offers[i].IsAvailable)
                    return true;
            }

            return false;
        }

        private UpgradeData GetAffordableReplacement(List<UpgradeData> availablePool, List<UpgradeOfferData> currentOffers, int currentResonance)
        {
            List<UpgradeData> affordable = new List<UpgradeData>();

            for (int i = 0; i < availablePool.Count; i++)
            {
                UpgradeData upgrade = availablePool[i];

                if (upgrade == null)
                    continue;

                if (ContainsUpgrade(currentOffers, upgrade))
                    continue;

                int minCost = _costService.GetMinimumResonanceCost(upgrade.Rarity);

                if (minCost > currentResonance)
                    continue;

                affordable.Add(upgrade);
            }

            if (affordable.Count == 0)
                return null;

            return affordable[Random.Range(0, affordable.Count)];
        }

        private bool ContainsUpgrade(List<UpgradeOfferData> offers, UpgradeData upgrade)
        {
            if (offers == null || upgrade == null)
                return false;

            for (int i = 0; i < offers.Count; i++)
            {
                if (offers[i] == null || offers[i].UpgradeData == null)
                    continue;

                if (offers[i].UpgradeData == upgrade)
                    return true;
            }

            return false;
        }
    }
}