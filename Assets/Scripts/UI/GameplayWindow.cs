using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class GameplayWindow : Window
    {
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private Slider healthSlider;

        [Space]
        [SerializeField] private Slider experienceSlider;
        [SerializeField] private Slider _timerSlider;

        [Space]
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text resonanceText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text combatFeedbackText;
        [SerializeField] private Button pauseButton;

        private Coroutine _feedbackCoroutine;

        public override void Initialize()
        {
            base.Initialize();

            if (pauseButton == null)
            {
                Debug.LogWarning($"{nameof(GameplayWindow)}: PauseButton is not assigned.", this);
                return;
            }

            pauseButton.onClick.RemoveListener(OnPauseClicked);
            pauseButton.onClick.AddListener(OnPauseClicked);
            pauseButton.transform.SetAsLastSibling();
        }

        protected override void OpenStart()
        {
            base.OpenStart();

            Character player = GameManager.Instance.CharacterFactory.Player;
            if (player != null)
            {
                UpdateHealthVisual(player);
                player.LiveComponent.OnCharacterHealthChange += UpdateHealthVisual;
            }

            if (GameManager.Instance?.ResonanceManager != null)
            {
                ResonanceChangeHandler(GameManager.Instance.ResonanceManager.CurrentResonance);
                GameManager.Instance.ResonanceManager.OnResonanceChanged += ResonanceChangeHandler;
            }

            GameManager.Instance.ExperienceManager.OnExperienceChanged += UpdateExperience;
            GameManager.Instance.ExperienceManager.OnLevelChanged += UpdateLevel;

            UpdateLevel(GameManager.Instance.ExperienceManager.CurrentLevel);
            UpdateExperience(
                GameManager.Instance.ExperienceManager.CurrentExperience,
                GameManager.Instance.ExperienceManager.ExperienceToNextLevel);
            UpdateTimer();

            if (combatFeedbackText != null)
            {
                combatFeedbackText.gameObject.SetActive(false);
            }
        }

        protected override void CloseStart()
        {
            base.CloseStart();

            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.ResonanceManager != null)
                {
                    GameManager.Instance.ResonanceManager.OnResonanceChanged -= ResonanceChangeHandler;
                }

                if (GameManager.Instance.ExperienceManager != null)
                {
                    GameManager.Instance.ExperienceManager.OnExperienceChanged -= UpdateExperience;
                    GameManager.Instance.ExperienceManager.OnLevelChanged -= UpdateLevel;
                }

                Character player = GameManager.Instance.CharacterFactory.Player;
                if (player != null)
                {
                    player.LiveComponent.OnCharacterHealthChange -= UpdateHealthVisual;
                }
            }
        }

        private void UpdateHealthVisual(Character character)
        {
            int health = (int)character.LiveComponent.Health;
            float healthMax = character.LiveComponent.MaxHealth;

            healthText.text = health + "/" + healthMax;
            healthSlider.maxValue = healthMax;
            healthSlider.value = health;
        }

        private void ResonanceChangeHandler(int resonance)
        {
            resonanceText.text = resonance.ToString();
        }

        public void ShowCombatFeedback(string message)
        {
            if (combatFeedbackText == null)
                return;

            if (_feedbackCoroutine != null)
            {
                StopCoroutine(_feedbackCoroutine);
            }

            _feedbackCoroutine = StartCoroutine(ShowCombatFeedbackRoutine(message));
        }

        private IEnumerator ShowCombatFeedbackRoutine(string message)
        {
            combatFeedbackText.gameObject.SetActive(true);
            combatFeedbackText.text = message;

            yield return new WaitForSecondsRealtime(0.8f);

            combatFeedbackText.gameObject.SetActive(false);
            _feedbackCoroutine = null;
        }

        private void UpdateTimer()
        {
            
            float currentTime = GameManager.Instance.GameTime;
            float maxTime = GameManager.Instance.GameData.GameTimeMinutesMax * 60f;

            float progress = currentTime / maxTime;
            progress = Mathf.Clamp01(progress);

            _timerSlider.value = progress;
            int min = (int)(GameManager.Instance.GameTime / 60);
            int sec = (int)(GameManager.Instance.GameTime % 60);
            timerText.text = GetTime(min) + ":" + GetTime(sec);

            string GetTime(int value)
            {
                return value < 10 ? "0" + value : value.ToString();
            }
        }

        private void UpdateExperience(int currentXP, int xpToNextLevel)
        {
            experienceSlider.maxValue = xpToNextLevel;
            experienceSlider.value = currentXP;
        }

        private void UpdateLevel(int level)
        {
            levelText.text = $"LvL {level}";
        }

        private void OnPauseClicked()
        {
            GameManager.Instance?.ShowPauseMenu();
        }

        private void Update()
        {
            UpdateTimer();
        }
    }
}
