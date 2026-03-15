using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "OmniumLessons/Weapon")]
    public class WeaponData : ScriptableObject
    {
        [Header("Basic info")]
        [SerializeField] private string _weaponName = "Default Weapon";

        [Header("Combat parameters")]
        [SerializeField] private int _damage = 5;
        [SerializeField] private float _attackDistance = 3f;
        [SerializeField] private float _attackCooldown = 1f;

        public string WeaponName => _weaponName;
        public int Damage => _damage;
        public float AttackDistance => _attackDistance;
        public float AttackCooldown => _attackCooldown;
    }
}
