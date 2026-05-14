using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class DefeatWindow : Window
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button returnToMainMenuButton;

        public override void Initialize()
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnRestartButtonClicked);

            returnToMainMenuButton.onClick.RemoveAllListeners();
            returnToMainMenuButton.onClick.AddListener(OnReturnToMainMenuButtonClicked);
        }

        private void OnReturnToMainMenuButtonClicked()
        {
            if (GameManager.Instance == null)
                return;

            GameManager.Instance.ClearSession();

            Time.timeScale = 1f;
            SceneLoader.LoadMainMenu();
        }

        private void OnRestartButtonClicked()
        {
            if (GameManager.Instance == null)
                return;

            if (GameManager.Instance.WindowsService != null)
            {
                GameManager.Instance.WindowsService.HideWindow<DefeatWindow>(true);
                GameManager.Instance.WindowsService.HideWindow<VictoryWindow>(true);
                GameManager.Instance.WindowsService.HideWindow<PauseMenuWindow>(true);
                GameManager.Instance.WindowsService.HideWindow<SkillsWindow>(true);
            }

            GameManager.Instance.StartNewSession();

            if (GameManager.Instance.WindowsService != null)
            {
                GameManager.Instance.WindowsService.ShowWindow<GameplayWindow>(true);
            }
        }
    }
}