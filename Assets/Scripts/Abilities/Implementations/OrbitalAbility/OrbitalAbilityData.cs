using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "OrbitalAbilityData", menuName = "Abilities/Orbital Data")]
    public class OrbitalAbilityData : ScriptableObject
    {
        [Header("Prefabs")]
        [SerializeField] private OrbitalSphere _spherePrefab;

        [Header("Base (Lv1)")]
        [SerializeField] private int _baseSphereCount = 2;
        [SerializeField] private float _baseOrbitRadius = 1.8f;
        [SerializeField] private float _baseOrbitRotationSpeed = 90f;
        [SerializeField] private float _baseDashDistance = 3.5f;
        [SerializeField] private float _baseDashSpeed = 14f;
        [SerializeField] private float _baseDamage = 10f;
        [SerializeField] private float _baseHoldDuration = 0.25f;
        [SerializeField] private float _baseCooldown = 1.8f;
        [SerializeField] private float _sphereHeight = 0.5f;

        [Header("Wave / Desync")]
        [SerializeField] private float _dashStartDelayStep = 0.15f;

        [Header("Level 2")]
        [SerializeField] private int _level2SphereCount = 4;

        [Header("Level 3")]
        [SerializeField] private float _level3DashDistanceMultiplier = 1.4f;

        [Header("Level 4")]
        [SerializeField] private float _level4DamageMultiplier = 1.4f;
        [SerializeField] private float _level4DashSpeedMultiplier = 1.35f;

        [Header("Level 5")]
        [SerializeField] private int _level5SphereCount = 8;
        [SerializeField] private bool _level5PierceEnabled = true;
        [SerializeField] private int _level5PierceCount = 999;

        public OrbitalSphere SpherePrefab => _spherePrefab;

        public int BaseSphereCount => _baseSphereCount;
        public float BaseOrbitRadius => _baseOrbitRadius;
        public float BaseOrbitRotationSpeed => _baseOrbitRotationSpeed;
        public float BaseDashDistance => _baseDashDistance;
        public float BaseDashSpeed => _baseDashSpeed;
        public float BaseDamage => _baseDamage;
        public float BaseHoldDuration => _baseHoldDuration;
        public float BaseCooldown => _baseCooldown;
        public float SphereHeight => _sphereHeight;

        public float DashStartDelayStep => _dashStartDelayStep;

        public int Level2SphereCount => _level2SphereCount;

        public float Level3DashDistanceMultiplier => _level3DashDistanceMultiplier;

        public float Level4DamageMultiplier => _level4DamageMultiplier;
        public float Level4DashSpeedMultiplier => _level4DashSpeedMultiplier;

        public int Level5SphereCount => _level5SphereCount;
        public bool Level5PierceEnabled => _level5PierceEnabled;
        public int Level5PierceCount => _level5PierceCount;
    }
}