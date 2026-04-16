using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "BossNullCoreData", menuName = "ZombieIO/Boss/Null Core Data")]
    public class BossNullCoreData : ScriptableObject
    {
        [Header("Base")]
        [SerializeField] private float _maxHealth = 500f;
        [SerializeField] private float _moveSpeed = 2.5f;

        [Header("Reward")]
        [SerializeField] private int _experienceReward = 100;

        [Header("Abilities")]
        [SerializeField] private BossPulseAbilityData _pulseData;
        [SerializeField] private BossSpawnAbilityData _spawnData;
        [SerializeField] private BossFieldAbilityData _fieldData;

        public float MaxHealth => _maxHealth;
        public float MoveSpeed => _moveSpeed;
        public int ExperienceReward => _experienceReward;

        public BossPulseAbilityData PulseData => _pulseData;
        public BossSpawnAbilityData SpawnData => _spawnData;
        public BossFieldAbilityData FieldData => _fieldData;
    }
}