using System;

namespace OmniumLessons
{
    public class ExperienceManager
    {
        public event Action<int, int> OnExperienceChanged;
        public event Action<int> OnLevelChanged;

        private readonly int _baseExperience;
        private readonly int _growthRate;

        private int _currentLevelExperience;
        private int _totalExperience;
        private int _currentLevel = 1;

        public int CurrentLevel => _currentLevel;
        public int CurrentExperience => _currentLevelExperience;
        public int TotalExperience => _totalExperience;

        public int ExperienceToNextLevel =>
            GetExperienceRequiredForNextLevel(_currentLevel);

        public ExperienceManager(
            int baseExperience = 20,
            int growthRate = 10)
        {
            _baseExperience = Math.Max(1, baseExperience);
            _growthRate = Math.Max(0, growthRate);
        }

        public void StartGame()
        {
            _currentLevelExperience = 0;
            _totalExperience = 0;
            _currentLevel = 1;

            OnExperienceChanged?.Invoke(
                _currentLevelExperience,
                ExperienceToNextLevel);
        }


        public void AddExperience(int amount)
        {
            if (amount <= 0)
                return;

            _currentLevelExperience += amount;
            _totalExperience += amount;

            while (_currentLevelExperience >= ExperienceToNextLevel)
            {
                _currentLevelExperience -= ExperienceToNextLevel;
                IncreaseLevel();
            }

            OnExperienceChanged?.Invoke(
                _currentLevelExperience,
                ExperienceToNextLevel);
        }

        private void IncreaseLevel()
        {
            _currentLevel++;
            OnLevelChanged?.Invoke(_currentLevel);
        }

        private int GetExperienceRequiredForNextLevel(
            int currentLevel)
        {
            currentLevel = Math.Max(1, currentLevel);

            int earlyLevels = Math.Min(currentLevel - 1, 19);
            int midLevels = Math.Min(Math.Max(currentLevel - 20, 0), 20);
            int lateLevels = Math.Max(currentLevel - 40, 0);

            int requiredExperience =
                _baseExperience +
                earlyLevels * _growthRate +
                midLevels * _growthRate * 2 +
                lateLevels * _growthRate * 3;

            return Math.Max(1, requiredExperience);
        }
    }
}
