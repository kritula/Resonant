using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "PiercingProjectileData", menuName = "Ability System/Piercing Projectile Data")]
    public class PiercingProjectileData : AbilityData
    {
        [Header("Projectile Settings")]
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private int _damage = 5;
        [SerializeField] private float _projectileSpeed = 10f;
        [SerializeField] private float _lifeTime = 3f;
        [SerializeField] private int _maxPierceCount = 3;
        [SerializeField] private float _searchRadius = 15f;

        public GameObject ProjectilePrefab => _projectilePrefab;
        public int Damage => _damage;
        public float ProjectileSpeed => _projectileSpeed;
        public float LifeTime => _lifeTime;
        public int MaxPierceCount => _maxPierceCount;
        public float SearchRadius => _searchRadius;
    }
}