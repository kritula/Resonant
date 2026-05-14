using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(menuName = "Weapons/Player Weapon Data")]
    public class PlayerWeaponData : WeaponData
    {
        [Header("Projectile settings")]
        [SerializeField] private BaseAttackProjectile _projectilePrefab;
        [SerializeField] private float _projectileSpeed = 10f;
        [SerializeField] private float _projectileLifeTime = 3f;
        [SerializeField] private float _projectileSpawnHeight = 1f;

        [Header("Ricochet settings")]
        [SerializeField] private float _ricochetRadius = 5f;

        [Header("Multi shot settings")]
        [SerializeField] private float _projectileSpawnOffset = 0.35f;

        public BaseAttackProjectile ProjectilePrefab => _projectilePrefab;
        public float ProjectileSpeed => _projectileSpeed;
        public float ProjectileLifeTime => _projectileLifeTime;
        public float ProjectileSpawnHeight => _projectileSpawnHeight;
        public float RicochetRadius => _ricochetRadius;
        public float ProjectileSpawnOffset => _projectileSpawnOffset;
    }
}