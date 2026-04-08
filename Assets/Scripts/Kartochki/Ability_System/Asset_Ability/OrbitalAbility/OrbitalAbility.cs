using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class OrbitalAbility : AbilityBehaviour
    {
        [SerializeField] private OrbitalAbilityData _data;

        private readonly List<OrbitalBall> _activeBalls = new List<OrbitalBall>();

        private int _ballCount;
        private float _orbitRadius;
        private float _rotationSpeed;
        private float _damage;
        private float _hitCooldownPerTarget;
        private float _spawnHeight;
        private float _criticalChance;
        private float _criticalDamageMultiplier;

        private float _currentAngle;

        public override void Initialize(PlayerCharacter owner, AbilityUpgradeData abilityData)
        {
            base.Initialize(owner, abilityData);

            _currentAngle = 0f;
            RebuildBalls();
            UpdateBallsState();
        }

        protected override void ApplyLevel(int level)
        {
            if (_data == null)
            {
                Debug.LogError($"{nameof(OrbitalAbility)}: Data is missing.", this);
                return;
            }

            _ballCount = _data.BaseBallCount;
            _orbitRadius = _data.BaseOrbitRadius;
            _rotationSpeed = _data.BaseRotationSpeed;
            _damage = _data.BaseDamage;
            _hitCooldownPerTarget = _data.HitCooldownPerTarget;
            _spawnHeight = _data.SpawnHeight;
            _criticalChance = 0f;
            _criticalDamageMultiplier = 1f;

            if (level >= 2)
            {
                _ballCount = 2;
            }

            if (level >= 3)
            {
                _orbitRadius *= _data.Level3RadiusMultiplier;
            }

            if (level >= 4)
            {
                _rotationSpeed *= _data.Level4RotationSpeedMultiplier;
            }

            if (level >= 5)
            {
                _ballCount = _data.Level5BallCount;
                _criticalChance = _data.CriticalChance;
                _criticalDamageMultiplier = _data.CriticalDamageMultiplier;
            }

            RebuildBalls();
            UpdateBallsState();
        }

        public override void OnUpdate()
        {
            if (_owner == null || _data == null)
                return;

            if (_activeBalls.Count == 0)
                return;

            _currentAngle += _rotationSpeed * Time.deltaTime;
            UpdateBallsPositions();
        }

        private void RebuildBalls()
        {
            RemoveExtraBalls();
            CreateMissingBalls();
        }

        private void RemoveExtraBalls()
        {
            for (int i = _activeBalls.Count - 1; i >= _ballCount; i--)
            {
                if (_activeBalls[i] != null)
                {
                    Destroy(_activeBalls[i].gameObject);
                }

                _activeBalls.RemoveAt(i);
            }
        }

        private void CreateMissingBalls()
        {
            if (_data == null || _data.OrbitalBallPrefab == null)
                return;

            while (_activeBalls.Count < _ballCount)
            {
                OrbitalBall newBall = Instantiate(_data.OrbitalBallPrefab, transform);
                _activeBalls.Add(newBall);
            }
        }

        private void UpdateBallsState()
        {
            for (int i = 0; i < _activeBalls.Count; i++)
            {
                if (_activeBalls[i] == null)
                    continue;

                _activeBalls[i].Initialize(
                    _owner,
                    _damage,
                    _hitCooldownPerTarget,
                    _criticalChance,
                    _criticalDamageMultiplier);
            }

            UpdateBallsPositions();
        }

        private void UpdateBallsPositions()
        {
            if (_activeBalls.Count == 0)
                return;

            float angleStep = 360f / _activeBalls.Count;

            for (int i = 0; i < _activeBalls.Count; i++)
            {
                OrbitalBall ball = _activeBalls[i];

                if (ball == null)
                    continue;

                float angle = _currentAngle + angleStep * i;
                float angleRad = angle * Mathf.Deg2Rad;

                Vector3 localPosition = new Vector3(
                    Mathf.Cos(angleRad) * _orbitRadius,
                    _spawnHeight,
                    Mathf.Sin(angleRad) * _orbitRadius);

                ball.transform.localPosition = localPosition;
                ball.transform.localRotation = Quaternion.identity;
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _activeBalls.Count; i++)
            {
                if (_activeBalls[i] != null)
                {
                    Destroy(_activeBalls[i].gameObject);
                }
            }

            _activeBalls.Clear();
        }
    }
}