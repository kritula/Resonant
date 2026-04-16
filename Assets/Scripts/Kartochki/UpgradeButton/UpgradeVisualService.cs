using UnityEngine;

namespace OmniumLessons
{
    public class UpgradeVisualService
    {
        private readonly Color _commonColor;
        private readonly Color _uncommonColor;
        private readonly Color _rareColor;
        private readonly Color _resonantColor;
        private readonly Color _unavailableColor;
        private readonly bool _dimUnavailable;

        public UpgradeVisualService(
            Color common,
            Color uncommon,
            Color rare,
            Color resonant,
            Color unavailable,
            bool dimUnavailable)
        {
            _commonColor = common;
            _uncommonColor = uncommon;
            _rareColor = rare;
            _resonantColor = resonant;
            _unavailableColor = unavailable;
            _dimUnavailable = dimUnavailable;
        }

        public Color GetBackgroundColor(UpgradeOfferData offer)
        {
            if (offer == null)
                return _commonColor;

            Color baseColor = GetRarityColor(offer.Rarity);

            if (!offer.IsAvailable && _dimUnavailable)
                return _unavailableColor;

            return baseColor;
        }

        public string GetRarityText(UpgradeRarity rarity)
        {
            switch (rarity)
            {
                case UpgradeRarity.Common: return "Common";
                case UpgradeRarity.Uncommon: return "Uncommon";
                case UpgradeRarity.Rare: return "Rare";
                case UpgradeRarity.Resonant: return "Resonant";
            }

            return string.Empty;
        }

        private Color GetRarityColor(UpgradeRarity rarity)
        {
            switch (rarity)
            {
                case UpgradeRarity.Common: return _commonColor;
                case UpgradeRarity.Uncommon: return _uncommonColor;
                case UpgradeRarity.Rare: return _rareColor;
                case UpgradeRarity.Resonant: return _resonantColor;
            }

            return _commonColor;
        }
    }
}