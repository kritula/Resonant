using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class SkillsWindow : Window
    {
        [SerializeField] private Transform container;
        [SerializeField] private UpgradeButton buttonPrefab;

        private readonly List<UpgradeButton> _buttons = new List<UpgradeButton>();

        public void ShowUpgrades(List<UpgradeOfferData> upgradeOffers)
        {
            ClearButtons();

            if (upgradeOffers == null || upgradeOffers.Count == 0)
                return;

            for (int i = 0; i < upgradeOffers.Count; i++)
            {
                UpgradeOfferData offer = upgradeOffers[i];

                if (offer == null || offer.UpgradeData == null)
                    continue;

                UpgradeButton button = Instantiate(buttonPrefab, container);
                button.Setup(offer);
                _buttons.Add(button);
            }
        }

        private void ClearButtons()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i] != null)
                {
                    Destroy(_buttons[i].gameObject);
                }
            }

            _buttons.Clear();
        }

        public override void Hide(bool isImmediately = false)
        {
            base.Hide(isImmediately);
            ClearButtons();
        }
    }
}