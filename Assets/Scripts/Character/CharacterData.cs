using UnityEngine;

namespace OmniumLessons
{
    public class CharacterData : MonoBehaviour
    {
        [SerializeField] private int _scoreCost;
        [SerializeField] private float _defaultSpeed;
        [SerializeField] private Transform _characterTransform;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private WeaponData _weaponData;
        [SerializeField] private int _maxHealth;
        [SerializeField] private int _experienceReward;

        public int ScoreCost => _scoreCost;
        public float DefaultSpeed => _defaultSpeed;
        public Transform CharacterTransform => _characterTransform;
        public CharacterController CharacterController => _characterController;
        public WeaponData WeaponData => _weaponData;
        public int MaxHealth => _maxHealth;
        public int ExperienceReward => _experienceReward;
    }
}