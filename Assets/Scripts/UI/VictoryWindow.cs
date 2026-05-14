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

            Time.timeScale = 1f;
            SceneLoader.LoadMainMenu();
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