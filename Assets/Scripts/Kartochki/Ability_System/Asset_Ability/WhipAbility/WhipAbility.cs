using UnityEngine;

namespace OmniumLessons
{
    public class WhipAbility : AbilityBehaviour
    {
        [SerializeField] private WhipAbilityData _data;

        private float _cooldown;
        private float _attackDistance;
        private float _damage;
        private float _hitboxLifeTime;
        private float _spawnHeight;

        private float _hitboxLength;
        private float _hitboxWidth;
        private float _hitboxHeight;

        private bool _attackForward;
        private bool _attackBackward;
        private bool _fullCircleAttack;

        private float _cooldownTimer;
        private int _lastHorizontalDirection = 1;

        public override void Initialize(PlayerCharacter owner, AbilityUpgradeData abilityData)
        {
            base.Initialize(owner, abilityData);

            _cooldownTimer = 0f;
            _lastHorizontalDirection = 1;
        }

        protected override void ApplyLevel(int level)
        {
            if (_data == null)
            {
                Debug.LogError($"{nameof(WhipAbility)}: Data is missing.", this);
                return;
            }

            _cooldown = _data.BaseCooldown;
            _attackDistance = _data.BaseAttackDistance;
            _damage = _data.BaseDamage;
            _hitboxLifeTime = _data.HitboxLifeTime;
            _spawnHeight = _data.SpawnHeight;

            _hitboxLength = _data.BaseHitboxLength;
            _hitboxWidth = _data.BaseHitboxWidth;
            _hitboxHeight = _data.BaseHitboxHeight;

            _attackForward = true;
            _attackBackward = false;
            _fullCircleAttack = false;

            if (level >= 2)
            {
                _hitboxWidth *= _data.Level2WidthMultiplier;
            }

            if (level >= 3)
            {
                _attackBackward = true;
            }

            if (level >= 4)
            {
                _damage *= _data.Level4DamageMultiplier;
            }

            if (level >= 5)
            {
                _fullCircleAttack = true;
                _attackBackward = false;
                _attackDistance *= _data.Level5DistanceMultiplier;
                _hitboxLength *= _data.Level5LengthMultiplier;
            }
        }

        public override void OnUpdate()
        {
            if (_owner == null || _data == null)
                return;

            if (_data.HitboxPrefab == null)
                return;

            UpdateLastHorizontalDirection();

            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
                return;
            }

            PerformAttack();
            _cooldownTimer = _cooldown;
        }

        private void UpdateLastHorizontalDirection()
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");

            if (horizontalInput > 0.01f)
            {
                _lastHorizontalDirection = 1;
            }
            else if (horizontalInput < -0.01f)
            {
                _lastHorizontalDirection = -1;
            }
        }

        private void PerformAttack()
        {
            if (_fullCircleAttack)
            {
                SpawnHitbox(Vector3.right);
                SpawnHitbox(Vector3.left);
                SpawnHitbox(Vector3.forward);
                SpawnHitbox(Vector3.back);
                return;
            }

            if (_attackForward)
            {
                SpawnHitbox(Vector3.right * _lastHorizontalDirection);
            }

            if (_attackBackward)
            {
                SpawnHitbox(Vector3.right * -_lastHorizontalDirection);
            }
        }

        private void SpawnHitbox(Vector3 direction)
        {
            if (direction == Vector3.zero)
                return;

            direction.Normalize();

            Vector3 localPosition = direction * (_attackDistance * 0.5f);
            localPosition += Vector3.up * _spawnHeight;

            Quaternion localRotation = Quaternion.LookRotation(direction, Vector3.up);

            WhipHitbox whipHitbox = Object.Instantiate(_data.HitboxPrefab, _owner.transform);
            whipHitbox.transform.localPosition = localPosition;
            whipHitbox.transform.localRotation = localRotation;

            whipHitbox.Initialize(
                _owner,
                _damage,
                _hitboxLength,
                _hitboxWidth,
                _hitboxHeight,
                _hitboxLifeTime);
        }
    }
}