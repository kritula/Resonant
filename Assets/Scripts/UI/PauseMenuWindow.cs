using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class PauseMenuWindow : Window
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private Button optionsMenuButton;
        //[SerializeField] private Button skillsButton;
        [SerializeField] private Button exitMainMenuButton;



        public override void Initialize()
        {
            continueButton.onClick.AddListener(OnContinueClicked);
            optionsMenuButton.onClick.AddListener(OnOptionsClicked);
            //skillsButton.onClick.AddListener(OnSkillsClicked);
            exitMainMenuButton.onClick.AddListener(OnExitToMainMenuClicked);
        }

        private void OnContinueClicked()
        {
            GameManager.Instance.IsGamePaused = false;
            Time.timeScale = 1f;
            Hide(false);
        }

        private void OnOptionsClicked()
        {
            Hide(true);
            GameManager.Instance.WindowsService.ShowWindow<SettingsWindow>(false);
        }

        private void OnExitToMainMenuClicked()
        {
            GameManager.Instance.ClearSession();

            Time.timeScale = 1f;
            SceneLoader.LoadMainMenu();
        }
    }
}
