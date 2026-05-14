using UnityEngine;

namespace OmniumLessons
{
    public class GameResultController
    {
        private readonly WindowsService _windowsService;
        private readonly ResonanceManager _resonanceManager;

        public GameResultController(
            WindowsService windowsService,
            ResonanceManager resonanceManager)
        {
            _windowsService = windowsService;
            _resonanceManager = resonanceManager;
        }

        public void ShowGameOver(CharacterSpawnController spawnController)
        {
            Debug.Log("GameOver!");
            Debug.Log("Resonance = " + _resonanceManager.CurrentResonance);

            StopGameplay(spawnController);

            if (_windowsService == null)
                return;

            _windowsService.HideWindow<SkillsWindow>(true);
            _windowsService.ShowWindow<DefeatWindow>(false);
        }

        public void ShowVictory(CharacterSpawnController spawnController)
        {
            Debug.Log("Victory! Boss defeated!");
            Debug.Log("Resonance = " + _resonanceManager.CurrentResonance);

            StopGameplay(spawnController);

            if (_windowsService == null)
                return;

            _windowsService.HideWindow<SkillsWindow>(true);
            _windowsService.ShowWindow<VictoryWindow>(false);
        }

        private void StopGameplay(CharacterSpawnController spawnController)
        {
            Time.timeScale = 0f;
            spawnController?.StopSpawn();
        }
    }
}
