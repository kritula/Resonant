using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "WhipAbilityData", menuName = "Abilities/Whip Data")]
    public class WhipAbilityData : ScriptableObject
    {
        [Header("Base (Lv1)")]
        [SerializeField] private WhipHitbox _hitboxPrefab;
        [SerializeField] private float _baseCooldown = 1.2f;
        [SerializeField] private float _baseAttackDistance = 3f;
        [SerializeField] private float _baseDamage = 10f;
        [SerializeField] private float _hitboxLifeTime = 0.2f;
        [SerializeField] private float _spawnHeight = 0.5f;

        [Header("Hitbox Size")]
        [SerializeField] private float _baseHitboxLength = 3f;
        [SerializeField] private float _baseHitboxWidth = 1.2f;
        [SerializeField] private float _baseHitboxHeight = 1f;

        [Header("Level 2")]
        [SerializeField] private float _level2WidthMultiplier = 1.5f;

        [Header("Level 4")]
        [SerializeField] private float _level4DamageMultiplier = 1.5f;

        [Header("Level 5")]
        [SerializeField] private float _level5DistanceMultiplier = 1.35f;
        [SerializeField] private float _level5LengthMultiplier = 1.25f;

        public WhipHitbox HitboxPrefab => _hitboxPrefab;
        public float BaseCooldown => _baseCooldown;
        public float BaseAttackDistance => _baseAttackDistance;
        public float BaseDamage => _baseDamage;
        public float HitboxLifeTime => _hitboxLifeTime;
        public float SpawnHeight => _spawnHeight;

        public float BaseHitboxLength => _baseHitboxLength;
        public float BaseHitboxWidth => _baseHitboxWidth;
        public float BaseHitboxHeight => _baseHitboxHeight;

        public float Level2WidthMultiplier => _level2WidthMultiplier;
        public float Level4DamageMultiplier => _level4DamageMultiplier;
        public float Level5DistanceMultiplier => _level5DistanceMultiplier;
        public float Level5LengthMultiplier => _level5LengthMultiplier;
    }
}