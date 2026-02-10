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
            restartButton.onClick.AddListener(OnRestartButtonClicked);
            returnToMainMenuButton.onClick.AddListener(OnReturnToMainMenuButtonClicked);
        }

        private void OnReturnToMainMenuButtonClicked()
        {
            Hide(true);

            GameManager.Instance.WindowsService.HideWindow<GameplayWindow>(true);
            GameManager.Instance.WindowsService.ShowWindow<MainMenuWindow>(false);
        }

        private void OnRestartButtonClicked()
        {
            GameManager.Instance.StartNewSession();

            // UI-логика здесь:
            Hide(true); 
            GameManager.Instance.WindowsService.ShowWindow<GameplayWindow>(true);
        }
    }
}