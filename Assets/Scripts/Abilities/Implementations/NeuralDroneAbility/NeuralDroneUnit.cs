using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class NeuralDroneUnit : MonoBehaviour
    {
        [SerializeField] private Transform _visualRoot;
        [SerializeField] private Transform _firePoint;

        [Header("Alive Movement")]
        [SerializeField] private float _bobAmplitude = 0.15f;
        [SerializeField] private float _bobFrequency = 2.5f;
        [SerializeField] private float _noiseAmplitude = 0.18f;
        [SerializeField] private float _noiseSpeed = 0.8f;
        [SerializeField] private float _springSmoothTime = 0.12f;

        /*_bobAmplitude Ч сила микро-покачивани€
        _bobFrequency Ч скорость покачивани€
        _noiseAmplitude Ч сила хаотичного шума
        _noiseSpeed Ч скорость изменени€ шума
        _springSmoothTime Ч насколько движение УпружинистоеФ
        меньше значение = быстрее и резче
        больше значение = м€гче и в€зче*/
        private PlayerCharacter _owner;
        private NeuralDroneAbilityData _data;

        private float _damage;
        private float _attackCooldown;
        private float _attackRange;
        private bool _usePiercing;
        private int _piercingCount;

        private float _attackTimer;
        private float _slotAngle;
        private int _slotIndex;

        private bool _isInitialized;

        private Vector3 _moveVelocity;
        private float _bobOffsetX;
        private float _bobOffsetZ;
        private float _noiseOffsetX;
        private float _noiseOffsetZ;

        public void Initialize(
            PlayerCharacter owner,
            NeuralDroneAbilityData data,
            float damage,
            float attackCooldown,
            float attackRange,
            bool usePiercing,
            int piercingCount)
        {
            _owner = owner;
            _data = data;
            _damage = damage;
            _attackCooldown = attackCooldown;
            _attackRange = attackRange;
            _usePiercing = usePiercing;
            _piercingCount = piercingCount;

            _bobOffsetX = Random.Range(0f, 100f);
            _bobOffsetZ = Random.Range(0f, 100f);
            _noiseOffsetX = Random.Range(0f, 100f);
            _noiseOffsetZ = Random.Range(0f, 100f);

            _isInitialized = true;
        }

        public void SetOrbitSlot(float slotAngle, int slotIndex)
        {
            _slotAngle = slotAngle;
            _slotIndex = slotIndex;
        }

        private void Update()
        {
            if (!_isInitialized || _owner == null || _data == null)
                return;

            UpdateFollowPosition();
            UpdateAttack();
        }

        private void UpdateFollowPosition()
        {
            float angleRad = _slotAngle * Mathf.Deg2Rad;

            Vector3 orbitPosition = _owner.transform.position + new Vector3(
                Mathf.Cos(angleRad) * _data.FollowRadius,
                _data.HoverHeight,
                Mathf.Sin(angleRad) * _data.FollowRadius);

            float bobX = Mathf.Sin(Time.time * _bobFrequency + _bobOffsetX) * _bobAmplitude;
            float bobZ = Mathf.Cos(Time.time * _bobFrequency + _bobOffsetZ) * _bobAmplitude;

            float noiseX = (Mathf.PerlinNoise(Time.time * _noiseSpeed, _noiseOffsetX) - 0.5f) * 2f * _noiseAmplitude;
            float noiseZ = (Mathf.PerlinNoise(_noiseOffsetZ, Time.time * _noiseSpeed) - 0.5f) * 2f * _noiseAmplitude;

            Vector3 finalTarget = orbitPosition + new Vector3(
                bobX + noiseX,
                0f,
                bobZ + noiseZ);

            transform.position = Vector3.SmoothDamp(
                transform.position,
                finalTarget,
                ref _moveVelocity,
                _springSmoothTime);
        }

        private void UpdateAttack()
        {
            if (_attackTimer > 0f)
            {
                _attackTimer -= Time.deltaTime;
                return;
            }

            Character target = FindNearestEnemy();

            if (target == null)
                return;

            Shoot(target);
            _attackTimer = _attackCooldown;
        }

        private Character FindNearestEnemy()
        {
            if (GameManager.Instance == null || GameManager.Instance.CharacterFactory == null)
                return null;

            List<Character> activeCharacters = GameManager.Instance.CharacterFactory.ActiveCharacters;

            Character nearestTarget = null;
            float nearestDistanceSqr = _attackRange * _attackRange;

            for (int i = 0; i < activeCharacters.Count; i++)
            {
                Character candidate = activeCharacters[i];

                if (candidate == null)
                    continue;

                if (candidate == _owner)
                    continue;

                if (candidate.CharacterType == _owner.CharacterType)
                    continue;

                if (candidate.LiveComponent == null || !candidate.LiveComponent.IsAlive)
                    continue;

                float distanceSqr = (candidate.transform.position - transform.position).sqrMagnitude;

                if (distanceSqr > nearestDistanceSqr)
                    continue;

                nearestDistanceSqr = distanceSqr;
                nearestTarget = candidate;
            }

            return nearestTarget;
        }

        private void Shoot(Character target)
        {
            if (_data.ProjectilePrefab == null || target == null)
                return;

            Vector3 spawnPosition = _firePoint != null ? _firePoint.position : transform.position;

            NeuralDroneProjectile projectile = Object.Instantiate(
                _data.ProjectilePrefab,
                spawnPosition,
                Quaternion.identity);

            projectile.Initialize(
                _owner,
                target,
                _damage,
                _data.ProjectileSpeed,
                _data.ProjectileLifetime,
                _usePiercing,
                _piercingCount);

            Vector3 lookDirection = target.transform.position - transform.position;
            lookDirection.y = 0f;

            if (_visualRoot != null && lookDirection.sqrMagnitude > 0.001f)
            {
                _visualRoot.rotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
            }
        }
    }
}