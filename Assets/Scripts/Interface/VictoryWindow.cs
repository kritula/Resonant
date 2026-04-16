using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class VictoryWindow : Window
    {
        [Space]
        [SerializeField] private Button continueButton;
        [SerializeField] private TMP_Text currentResonance;

        public override void Initialize()
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(ContinueButtonClickHandler);
        }

        private void ContinueButtonClickHandler()
        {
            if (GameManager.Instance == null)
                return;

            GameManager.Instance.ClearSession();

            Hide(true);

            if (GameManager.Instance.WindowsService != null)
            {
                GameManager.Instance.WindowsService.HideWindow<GameplayWindow>(true);
                GameManager.Instance.WindowsService.HideWindow<VictoryWindow>(true);
                GameManager.Instance.WindowsService.HideWindow<DefeatWindow>(true);
                GameManager.Instance.WindowsService.HideWindow<PauseMenuWindow>(true);
                GameManager.Instance.WindowsService.HideWindow<SkillsWindow>(true);

                GameManager.Instance.WindowsService.ShowWindow<MainMenuWindow>(true);
            }
        }

        protected override void OpenStart()
        {
            base.OpenStart();

            if (GameManager.Instance == null || GameManager.Instance.ResonanceManager == null)
                return;

            int resonance = GameManager.Instance.ResonanceManager.CurrentResonance;
            currentResonance.text = resonance.ToString();
        }
    }
}