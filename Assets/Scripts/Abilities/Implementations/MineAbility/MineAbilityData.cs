using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "MineAbilityData", menuName = "Abilities/Mine Data")]
    public class MineAbilityData : ScriptableObject
    {
        [Header("Base (Lv1)")]
        [SerializeField] private MineObject _minePrefab;
        [SerializeField] private float _baseSpawnCooldown = 4f;
        [SerializeField] private float _baseSpawnDistance = 2f;
        [SerializeField] private float _baseDamage = 12f;
        [SerializeField] private float _baseExplosionRadius = 2f;
        [SerializeField] private float _activationDelay = 0.5f;
        [SerializeField] private float _mineLifeTime = 12f;

        [Header("Level 2")]
        [SerializeField] private float _level2CooldownMultiplier = 0.65f;

        [Header("Level 3")]
        [SerializeField] private float _level3RadiusMultiplier = 1.35f;

        [Header("Level 4")]
        [SerializeField] private float _level4DamageMultiplier = 1.6f;

        [Header("Level 5")]
        [SerializeField] private bool _enableChainReaction = true;
        [SerializeField] private float _chainReactionDelay = 0.15f;
        [SerializeField] private float _chainReactionRadius = 3f;

        public MineObject MinePrefab => _minePrefab;
        public float BaseSpawnCooldown => _baseSpawnCooldown;
        public float BaseSpawnDistance => _baseSpawnDistance;
        public float BaseDamage => _baseDamage;
        public float BaseExplosionRadius => _baseExplosionRadius;
        public float ActivationDelay => _activationDelay;
        public float MineLifeTime => _mineLifeTime;

        public float Level2CooldownMultiplier => _level2CooldownMultiplier;
        public float Level3RadiusMultiplier => _level3RadiusMultiplier;
        public float Level4DamageMultiplier => _level4DamageMultiplier;

        public bool EnableChainReaction => _enableChainReaction;
        public float ChainReactionDelay => _chainReactionDelay;
        public float ChainReactionRadius => _chainReactionRadius;
    }
}