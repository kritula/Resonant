using System;

namespace OmniumLessons
{
    public class ExperienceManager
    {
        public event Action<int, int> OnExperienceChanged;
        public event Action<int> OnLevelChanged;

        private int _currentExperience;
        private int _currentLevel = 1;

        public int CurrentLevel => _currentLevel;
        public int CurrentExperience => _currentExperience;

        public int ExperienceToNextLevel => GetExperienceForLevel(_currentLevel + 1);

        public void StartGame()
        {
            _currentExperience = 0;
            _currentLevel = 1;

            //OnLevelChanged?.Invoke(_currentLevel);
            OnExperienceChanged?.Invoke(_currentExperience, ExperienceToNextLevel);
        }


        public void AddExperience(int amount)
        {
            _currentExperience += amount;

            // проверяем сколько уровней нужно апнуть
            while (_currentExperience >= ExperienceToNextLevel)
            {
                LevelUp();
            }

            OnExperienceChanged?.Invoke(_currentExperience, ExperienceToNextLevel);
        }

        private void LevelUp()
        {
            _currentLevel++;
            OnLevelChanged?.Invoke(_currentLevel);
        }

        // формула роста опыта
        private int GetExperienceForLevel(int level)
        {
            if (level == 1)
                return 0;

            return 120 + (level - 2) * 180;
        }
    }
}