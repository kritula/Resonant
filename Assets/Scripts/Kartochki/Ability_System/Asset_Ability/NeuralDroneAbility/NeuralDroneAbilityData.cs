using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "NeuralDroneAbilityData", menuName = "Abilities/Neural Drone Data")]
    public class NeuralDroneAbilityData : ScriptableObject
    {
        [Header("Prefabs")]
        [SerializeField] private NeuralDroneUnit _dronePrefab;
        [SerializeField] private NeuralDroneProjectile _projectilePrefab;

        [Header("Base (Lv1)")]
        [SerializeField] private int _baseDroneCount = 1;
        [SerializeField] private float _baseDamage = 10f;
        [SerializeField] private float _baseAttackCooldown = 1.2f;
        [SerializeField] private float _baseAttackRange = 8f;

        [Header("Drone Movement")]
        [SerializeField] private float _followRadius = 1.75f;
        [SerializeField] private float _hoverHeight = 1.5f;
        [SerializeField] private float _followLerpSpeed = 8f;
        [SerializeField] private float _orbitRotationSpeed = 90f;

        [Header("Projectile")]
        [SerializeField] private float _projectileSpeed = 20f;
        [SerializeField] private float _projectileLifetime = 1.5f;

        [Header("Level 2")]
        [SerializeField] private float _level2DamageMultiplier = 1.25f;

        [Header("Level 3")]
        [SerializeField] private int _level3DroneCount = 2;

        [Header("Level 4")]
        [SerializeField] private float _level4AttackCooldownMultiplier = 0.65f;

        [Header("Level 5")]
        [SerializeField] private bool _level5Piercing = true;
        [SerializeField] private int _level5PiercingCount = 99;

        public NeuralDroneUnit DronePrefab => _dronePrefab;
        public NeuralDroneProjectile ProjectilePrefab => _projectilePrefab;

        public int BaseDroneCount => _baseDroneCount;
        public float BaseDamage => _baseDamage;
        public float BaseAttackCooldown => _baseAttackCooldown;
        public float BaseAttackRange => _baseAttackRange;

        public float FollowRadius => _followRadius;
        public float HoverHeight => _hoverHeight;
        public float FollowLerpSpeed => _followLerpSpeed;
        public float OrbitRotationSpeed => _orbitRotationSpeed;

        public float ProjectileSpeed => _projectileSpeed;
        public float ProjectileLifetime => _projectileLifetime;

        public float Level2DamageMultiplier => _level2DamageMultiplier;
        public int Level3DroneCount => _level3DroneCount;
        public float Level4AttackCooldownMultiplier => _level4AttackCooldownMultiplier;

        public bool Level5Piercing => _level5Piercing;
        public int Level5PiercingCount => _level5PiercingCount;
    }
}