namespace OmniumLessons
{
    public class UpgradeOfferFactory
    {
        private readonly UpgradeCostService _costService;

        public UpgradeOfferFactory(UpgradeCostService costService)
        {
            _costService = costService;
        }

        public UpgradeOfferData CreateOffer(UpgradeData upgradeData, int currentResonance)
        {
            if (upgradeData == null)
                return null;

            UpgradeOfferData offer = new UpgradeOfferData();
            offer.UpgradeData = upgradeData;
            offer.Rarity = upgradeData.Rarity;
            offer.ResonanceCost = _costService.RollResonanceCost(upgradeData.Rarity);
            offer.IsAvailable = currentResonance >= offer.ResonanceCost;

            return offer;
        }
    }
}