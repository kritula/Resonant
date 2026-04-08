using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "AuraAbilityData", menuName = "Abilities/Aura Data")]
    public class AuraAbilityData : ScriptableObject
    {
        [Header("Base (Lv1)")]
        [SerializeField] private float _baseRadius = 2f;
        [SerializeField] private float _baseDamagePerTick = 1f;
        [SerializeField] private float _baseTickRate = 1f;

        [Header("Level 2")]
        [SerializeField] private float _level2Radius = 3f;

        [Header("Level 3")]
        [SerializeField] private float _level3SlowMultiplier = 0.8f; // 20% slow

        [Header("Level 4")]
        [SerializeField] private float _level4DamageMultiplier = 2f;

        [Header("Level 5")]
        [SerializeField] private float _level5SlowMultiplier = 0.6f; // 40% slow
        [SerializeField] private float _level5DamageMultiplier = 1.5f;
        [SerializeField] private float _pulseRadiusMultiplier = 1.5f;
        [SerializeField] private float _pulseInterval = 2f;

        public float BaseRadius => _baseRadius;
        public float BaseDamagePerTick => _baseDamagePerTick;
        public float BaseTickRate => _baseTickRate;

        public float Level2Radius => _level2Radius;

        public float Level3SlowMultiplier => _level3SlowMultiplier;

        public float Level4DamageMultiplier => _level4DamageMultiplier;

        public float Level5SlowMultiplier => _level5SlowMultiplier;
        public float Level5DamageMultiplier => _level5DamageMultiplier;
        public float PulseRadiusMultiplier => _pulseRadiusMultiplier;
        public float PulseInterval => _pulseInterval;
    }
}