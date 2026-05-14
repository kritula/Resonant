using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class OrbitalAbility : AbilityBehaviour
    {
        [SerializeField] private OrbitalAbilityData _data;

        private readonly List<OrbitalSphere> _activeSpheres = new List<OrbitalSphere>();

        private int _sphereCount;
        private float _orbitRadius;
        private float _orbitRotationSpeed;
        private float _dashDistance;
        private float _dashSpeed;
        private float _damage;
        private float _holdDuration;
        private float _cooldown;
        private float _sphereHeight;
        private float _dashStartDelayStep;

        private bool _pierceEnabled;
        private int _pierceCount;

        private float _orbitAngle;

        public override void Initialize(PlayerCharacter owner, AbilityUpgradeData abilityData)
        {
            base.Initialize(owner, abilityData);

            _orbitAngle = 0f;
            RebuildSpheres();
            ApplyConfigToSpheres();
        }

        protected override void ApplyLevel(int level)
        {
            if (_data == null)
            {
                Debug.LogError($"{nameof(OrbitalAbility)}: Data is missing.", this);
                return;
            }

            _sphereCount = _data.BaseSphereCount;
            _orbitRadius = _data.BaseOrbitRadius;
            _orbitRotationSpeed = _data.BaseOrbitRotationSpeed;
            _dashDistance = _data.BaseDashDistance;
            _dashSpeed = _data.BaseDashSpeed;
            _damage = _data.BaseDamage;
            _holdDuration = _data.BaseHoldDuration;
            _cooldown = _data.BaseCooldown;
            _sphereHeight = _data.SphereHeight;
            _dashStartDelayStep = _data.DashStartDelayStep;

            _pierceEnabled = false;
            _pierceCount = 0;

            if (level >= 2)
            {
                _sphereCount = _data.Level2SphereCount;
            }

            if (level >= 3)
            {
                _dashDistance *= _data.Level3DashDistanceMultiplier;
            }

            if (level >= 4)
            {
                _damage *= _data.Level4DamageMultiplier;
                _dashSpeed *= _data.Level4DashSpeedMultiplier;
            }

            if (level >= 5)
            {
                _sphereCount = _data.Level5SphereCount;
                _pierceEnabled = _data.Level5PierceEnabled;
                _pierceCount = _data.Level5PierceCount;
            }

            RebuildSpheres();
            ApplyConfigToSpheres();
        }

        public override void OnUpdate()
        {
            if (_owner == null || _data == null)
                return;

            _orbitAngle += _orbitRotationSpeed * Time.deltaTime;
            UpdateSphereSlots();
        }

        private void RebuildSpheres()
        {
            RemoveExtraSpheres();
            CreateMissingSpheres();
        }

        private void RemoveExtraSpheres()
        {
            for (int i = _activeSpheres.Count - 1; i >= _sphereCount; i--)
            {
                if (_activeSpheres[i] != null)
                {
                    Destroy(_activeSpheres[i].gameObject);
                }

                _activeSpheres.RemoveAt(i);
            }
        }

        private void CreateMissingSpheres()
        {
            if (_data == null || _data.SpherePrefab == null)
                return;

            while (_activeSpheres.Count < _sphereCount)
            {
                OrbitalSphere sphere = Instantiate(_data.SpherePrefab, transform);
                _activeSpheres.Add(sphere);
            }
        }

        private void ApplyConfigToSpheres()
        {
            for (int i = 0; i < _activeSpheres.Count; i++)
            {
                OrbitalSphere sphere = _activeSpheres[i];

                if (sphere == null)
                    continue;

                sphere.Initialize(
                    _owner,
                    _damage,
                    _dashDistance,
                    _dashSpeed,
                    _holdDuration,
                    _cooldown,
                    _sphereHeight,
                    _pierceEnabled,
                    _pierceCount,
                    i * _dashStartDelayStep);
            }

            UpdateSphereSlots();
        }

        private void UpdateSphereSlots()
        {
            if (_activeSpheres.Count == 0)
                return;

            float angleStep = 360f / _activeSpheres.Count;

            for (int i = 0; i < _activeSpheres.Count; i++)
            {
                OrbitalSphere sphere = _activeSpheres[i];

                if (sphere == null)
                    continue;

                float slotAngle = _orbitAngle + angleStep * i;
                sphere.SetOrbitData(slotAngle, _orbitRadius);
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _activeSpheres.Count; i++)
            {
                if (_activeSpheres[i] != null)
                {
                    Destroy(_activeSpheres[i].gameObject);
                }
            }

            _activeSpheres.Clear();
        }
    }
}