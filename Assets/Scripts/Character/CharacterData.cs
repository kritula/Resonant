using UnityEngine;

namespace OmniumLessons
{
    public class CharacterData : MonoBehaviour
    {
        [SerializeField] private float _defaultSpeed = 5f;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Transform _characterTransform;
        [SerializeField] private WeaponData _weaponData;
        [SerializeField] private float _maxHealth = 50f;
        [SerializeField] private int _resonanceCost = 1;
        [SerializeField] private int _experienceReward = 1;

        private Character _character;

        public float DefaultSpeed => _defaultSpeed;
        public CharacterController CharacterController => _characterController;
        public Transform CharacterTransform => _characterTransform;
        public WeaponData WeaponData => _weaponData;
        public float MaxHealth => _maxHealth;
        public int ResonanceCost => _resonanceCost;
        public int ExperienceReward => _experienceReward;
        public Character Character => _character;

        private void Awake()
        {
            _character = GetComponent<Character>();
        }
    }
}