using UnityEngine;

namespace OmniumLessons
{
    public abstract class WeaponData : ScriptableObject
    {
        [SerializeField] private string _weaponName;
        [SerializeField] private float _damage = 5f;
        [SerializeField] private float _attackDistance = 2f;
        [SerializeField] private float _attackCooldown = 1f;

        public string WeaponName => _weaponName;
        public float Damage => _damage;
        public float AttackDistance => _attackDistance;
        public float AttackCooldown => _attackCooldown;
    }
}