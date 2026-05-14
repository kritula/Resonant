using System;
using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class LevelUpUpgradeController
    {
        private readonly WindowsService _windowsService;
        private readonly UpgradeDatabase _upgradeDatabase;
        private readonly UpgradeSelectionService _upgradeSelectionService;
        private readonly Action _pauseGame;

        public LevelUpUpgradeController(
            WindowsService windowsService,
            UpgradeDatabase upgradeDatabase,
            Action pauseGame)
        {
            _windowsService = windowsService;
            _upgradeDatabase = upgradeDatabase;
            _pauseGame = pauseGame;
            _upgradeSelectionService = new UpgradeSelectionService();
        }

        public void TryShowLevelUpChoices(bool isGameActive, bool isGamePaused)
        {
            if (!isGameActive || isGamePaused)
                return;

            if (_upgradeDatabase == null || _windowsService == null)
                return;

            List<UpgradeOfferData> upgradeOffers =
                _upgradeSelectionService.GetUpgradeOffers(
                    _upgradeDatabase.Upgrades);

            if (upgradeOffers == null || upgradeOffers.Count == 0)
                return;

            SkillsWindow skillsWindow =
                _windowsService.GetWindow<SkillsWindow>();

            if (skillsWindow == null)
                return;

            _pauseGame?.Invoke();
            _windowsService.ShowWindow<SkillsWindow>(true);
            skillsWindow.ShowUpgrades(upgradeOffers);
        }
    }
}
