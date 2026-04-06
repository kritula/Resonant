using UnityEngine;

namespace OmniumLessons
{
    public class WhipAbility : AbilityBehaviour
    {
        [Header("Whip settings")]
        [SerializeField] private WhipHitbox _whipHitboxPrefab;
        [SerializeField] private float _attackDistance = 3f;
        [SerializeField] private float _attackWidth = 1.2f;
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _hitboxLifeTime = 0.2f;
        [SerializeField] private float _spawnHeight = 0.5f;

        private float _cooldownTimer;
        private int _lastHorizontalDirection = 1;

        public override void Initialize(PlayerCharacter owner, AbilityUpgradeData abilityData)
        {
            base.Initialize(owner, abilityData);

            _cooldownTimer = 0f;
            _lastHorizontalDirection = 1;
        }

        public override void OnUpdate()
        {
            if (_owner == null)
                return;

            if (_whipHitboxPrefab == null)
                return;

            UpdateLastHorizontalDirection();

            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
                return;
            }

            PerformAttack();

            if (_abilityData != null && _abilityData.Cooldown > 0f)
            {
                _cooldownTimer = _abilityData.Cooldown;
            }
            else
            {
                _cooldownTimer = 1f;
            }
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
            Vector3 localPosition = Vector3.zero;
            localPosition += Vector3.right * _lastHorizontalDirection * (_attackDistance * 0.5f);
            localPosition += Vector3.up * _spawnHeight;

            Quaternion localRotation = _lastHorizontalDirection > 0
                ? Quaternion.identity
                : Quaternion.Euler(0f, 180f, 0f);

            WhipHitbox whipHitbox = Object.Instantiate(_whipHitboxPrefab, _owner.transform);
            whipHitbox.transform.localPosition = localPosition;
            whipHitbox.transform.localRotation = localRotation;

            whipHitbox.Initialize(
                _owner,
                _damage,
                _attackDistance,
                _attackWidth,
                _hitboxLifeTime);
        }
    }
}