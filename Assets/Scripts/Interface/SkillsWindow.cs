using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class SkillsWindow : Window
    {
        [SerializeField] private Transform container;
        [SerializeField] private UpgradeButton buttonPrefab;

        private readonly List<UpgradeButton> _buttons = new List<UpgradeButton>();

        public void ShowUpgrades(List<UpgradeData> upgrades)
        {
            ClearButtons();

            foreach (var upgrade in upgrades)
            {
                UpgradeButton button = Instantiate(buttonPrefab, container);
                button.Setup(upgrade);
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
    }
}