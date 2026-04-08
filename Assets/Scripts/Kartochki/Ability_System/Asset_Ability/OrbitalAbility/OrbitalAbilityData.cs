using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "OrbitalAbilityData", menuName = "Abilities/Orbital Data")]
    public class OrbitalAbilityData : ScriptableObject
    {
        [Header("Base (Lv1)")]
        [SerializeField] private OrbitalBall _orbitalBallPrefab;
        [SerializeField] private int _baseBallCount = 1;
        [SerializeField] private float _baseOrbitRadius = 1.5f;
        [SerializeField] private float _baseRotationSpeed = 120f;
        [SerializeField] private float _baseDamage = 10f;
        [SerializeField] private float _hitCooldownPerTarget = 0.5f;
        [SerializeField] private float _spawnHeight = 0.5f;

        [Header("Level 3")]
        [SerializeField] private float _level3RadiusMultiplier = 1.35f;

        [Header("Level 4")]
        [SerializeField] private float _level4RotationSpeedMultiplier = 1.4f;

        [Header("Level 5")]
        [SerializeField] private int _level5BallCount = 4;
        [SerializeField] private float _criticalChance = 0.25f;
        [SerializeField] private float _criticalDamageMultiplier = 2f;

        public OrbitalBall OrbitalBallPrefab => _orbitalBallPrefab;
        public int BaseBallCount => _baseBallCount;
        public float BaseOrbitRadius => _baseOrbitRadius;
        public float BaseRotationSpeed => _baseRotationSpeed;
        public float BaseDamage => _baseDamage;
        public float HitCooldownPerTarget => _hitCooldownPerTarget;
        public float SpawnHeight => _spawnHeight;

        public float Level3RadiusMultiplier => _level3RadiusMultiplier;
        public float Level4RotationSpeedMultiplier => _level4RotationSpeedMultiplier;

        public int Level5BallCount => _level5BallCount;
        public float CriticalChance => _criticalChance;
        public float CriticalDamageMultiplier => _criticalDamageMultiplier;
    }
}