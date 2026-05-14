using UnityEngine;

namespace OmniumLessons
{
    public class BossCharacterDataAdapter : CharacterData
    {
        [SerializeField] private BossNullCoreData _bossData;

        public BossNullCoreData BossData => _bossData;

        private void Awake()
        {
            if (_bossData == null)
            {
                Debug.LogError("BossCharacterDataAdapter: BossNullCoreData is not assigned!", this);
                return;
            }

            // Заполняем базовые поля CharacterData
            SetDataFromBoss();
        }

        private void SetDataFromBoss()
        {
            SetMaxHealth(_bossData.MaxHealth);
            SetDefaultSpeed(_bossData.MoveSpeed);
            SetExperienceReward(_bossData.ExperienceReward);
        }
    }
}