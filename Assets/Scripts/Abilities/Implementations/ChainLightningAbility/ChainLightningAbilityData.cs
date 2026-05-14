using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "ChainLightningAbilityData", menuName = "Abilities/Chain Lightning Data")]
    public class ChainLightningAbilityData : ScriptableObject
    {
        [Header("Base (Lv1)")]
        [SerializeField] private ChainLightningBolt _boltPrefab;
        [SerializeField] private float _baseCooldown = 2.5f;
        [SerializeField] private float _baseDamage = 14f;
        [SerializeField] private int _baseTargetCount = 2;
        [SerializeField] private float _castRange = 8f;
        [SerializeField] private float _jumpRange = 6f;
        [SerializeField] private float _travelDuration = 0.3f;
        [SerializeField] private float _jumpDamageMultiplier = 0.9f;

        [Header("Level 2")]
        [SerializeField] private int _level2TargetCount = 3;

        [Header("Level 4")]
        [SerializeField] private float _doubleDischargeChance = 0.25f;

        [Header("Level 5")]
        [SerializeField] private int _level5TargetCount = 5;
        [SerializeField] private float _slowMultiplier = 0.8f;
        [SerializeField] private float _slowDuration = 1.5f;

        public ChainLightningBolt BoltPrefab => _boltPrefab;
        public float BaseCooldown => _baseCooldown;
        public float BaseDamage => _baseDamage;
        public int BaseTargetCount => _baseTargetCount;
        public float CastRange => _castRange;
        public float JumpRange => _jumpRange;
        public float TravelDuration => _travelDuration;
        public float JumpDamageMultiplier => _jumpDamageMultiplier;

        public int Level2TargetCount => _level2TargetCount;
        public float DoubleDischargeChance => _doubleDischargeChance;

        public int Level5TargetCount => _level5TargetCount;
        public float SlowMultiplier => _slowMultiplier;
        public float SlowDuration => _slowDuration;
    }
}