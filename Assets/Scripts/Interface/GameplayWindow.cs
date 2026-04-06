using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class GameplayWindow : Window
    {
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private Slider healthSlider;

        [Space][SerializeField] private Slider experienceSlider;

        [Space][SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Button pauseButton;
        [SerializeField] private TMP_Text levelText;
        protected override void OpenStart()
        {
            base.OpenStart();
            var player = GameManager.Instance.CharacterFactory.Player;

            UpdateHealthVisual(player);
            player.LiveComponent.OnCharacterHealthChange += UpdateHealthVisual;

            ScoreChangeHandler(GameManager.Instance.ScoreManager.GameScore);
            GameManager.Instance.ScoreManager.OnScoreChanged += ScoreChangeHandler;
            GameManager.Instance.ExperienceManager.OnExperienceChanged += UpdateExperience;

            GameManager.Instance.ExperienceManager.OnLevelChanged += UpdateLevel;
            UpdateLevel(GameManager.Instance.ExperienceManager.CurrentLevel);

            pauseButton.onClick.AddListener(OnPauseClicked);

            UpdateTimer();
        }

        protected override void CloseStart()
        {
            base.CloseStart();

            GameManager.Instance.ScoreManager.OnScoreChanged -= ScoreChangeHandler;
            GameManager.Instance.ExperienceManager.OnExperienceChanged -= UpdateExperience;
            GameManager.Instance.ExperienceManager.OnLevelChanged -= UpdateLevel;

            var player = GameManager.Instance.CharacterFactory.Player;
            if (player == null)
                return;

            player.LiveComponent.OnCharacterHealthChange -= UpdateHealthVisual;

            pauseButton.onClick.RemoveListener(OnPauseClicked);
        }

        private void UpdateHealthVisual(Character character)
        {
            int health = (int)character.LiveComponent.Health;
            float healthMax = character.LiveComponent.MaxHealth;

            healthText.text = health + "/" + healthMax;
            healthSlider.maxValue = healthMax;
            healthSlider.value = health;
        }

        private void ScoreChangeHandler(int score)
        {
            scoreText.text = score.ToString();
        }

        private void UpdateTimer()
        {
            var min = (int)(GameManager.Instance.GameTime / 60);
            var sec = (int)(GameManager.Instance.GameTime % 60);
            timerText.text = GetTime(min) + ":" + GetTime(sec);


            string GetTime(int value)
            {
                return (value < 10) ? "0" + value : value.ToString();
            }
        }

        private void UpdateExperience(int currentXP, int xpToNextLevel)
        {
            experienceSlider.maxValue = xpToNextLevel;
            experienceSlider.value = currentXP;
        }

        private void UpdateLevel(int level)
        {
            levelText.text = $"Level {level}";
        }

        private void OnPauseClicked()
        {
            GameManager.Instance.IsGamePaused = true;
            Time.timeScale = 0f;
            GameManager.Instance.WindowsService.ShowWindow<PauseMenuWindow>(false);
        }

        private void Update()
        {
            UpdateTimer();
        }
    }
}