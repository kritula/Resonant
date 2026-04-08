using UnityEngine;

namespace OmniumLessons
{
    public class MineAbility : AbilityBehaviour
    {
        [SerializeField] private MineAbilityData _data;

        private float _spawnCooldown;
        private float _spawnDistance;
        private float _damage;
        private float _explosionRadius;
        private float _activationDelay;
        private float _mineLifeTime;

        private bool _chainReactionEnabled;
        private float _chainReactionDelay;
        private float _chainReactionRadius;

        private float _spawnTimer;

        public override void Initialize(PlayerCharacter owner, AbilityUpgradeData abilityData)
        {
            base.Initialize(owner, abilityData);
            _spawnTimer = 0f;
        }

        protected override void ApplyLevel(int level)
        {
            if (_data == null)
            {
                Debug.LogError($"{nameof(MineAbility)}: Data is missing.", this);
                return;
            }

            _spawnCooldown = _data.BaseSpawnCooldown;
            _spawnDistance = _data.BaseSpawnDistance;
            _damage = _data.BaseDamage;
            _explosionRadius = _data.BaseExplosionRadius;
            _activationDelay = _data.ActivationDelay;
            _mineLifeTime = _data.MineLifeTime;

            _chainReactionEnabled = false;
            _chainReactionDelay = 0f;
            _chainReactionRadius = 0f;

            if (level >= 2)
            {
                _spawnCooldown *= _data.Level2CooldownMultiplier;
            }

            if (level >= 3)
            {
                _explosionRadius *= _data.Level3RadiusMultiplier;
            }

            if (level >= 4)
            {
                _damage *= _data.Level4DamageMultiplier;
            }

            if (level >= 5)
            {
                _chainReactionEnabled = _data.EnableChainReaction;
                _chainReactionDelay = _data.ChainReactionDelay;
                _chainReactionRadius = _data.ChainReactionRadius;
            }
        }

        public override void OnUpdate()
        {
            if (_owner == null || _data == null)
                return;

            if (_data.MinePrefab == null)
                return;

            if (_spawnTimer > 0f)
            {
                _spawnTimer -= Time.deltaTime;
                return;
            }

            SpawnMine();
            _spawnTimer = _spawnCooldown;
        }

        private void SpawnMine()
        {
            Vector3 direction = GetSpawnDirection();
            Vector3 spawnPosition = _owner.transform.position + direction * _spawnDistance;
            spawnPosition.y = 0f;

            MineObject mine = Object.Instantiate(_data.MinePrefab, spawnPosition, Quaternion.identity);

            mine.Initialize(
                _owner,
                _damage,
                _explosionRadius,
                _activationDelay,
                _mineLifeTime,
                _chainReactionEnabled,
                _chainReactionDelay,
                _chainReactionRadius);
        }

        private Vector3 GetSpawnDirection()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);

            if (inputDirection.sqrMagnitude > 0.001f)
            {
                return inputDirection.normalized;
            }

            if (_owner.transform.forward.sqrMagnitude > 0.001f)
            {
                Vector3 forward = _owner.transform.forward;
                forward.y = 0f;
                return forward.normalized;
            }

            return Vector3.forward;
        }
    }
}