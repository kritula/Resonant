using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class NeuralDroneAbility : AbilityBehaviour
    {
        [SerializeField] private NeuralDroneAbilityData _data;

        private readonly List<NeuralDroneUnit> _activeDrones = new List<NeuralDroneUnit>();

        private int _droneCount;
        private float _damage;
        private float _attackCooldown;
        private float _attackRange;
        private bool _usePiercing;
        private int _piercingCount;

        private float _orbitAngle;

        public override void Initialize(PlayerCharacter owner, AbilityUpgradeData abilityData)
        {
            base.Initialize(owner, abilityData);
            _orbitAngle = 0f;
            RebuildDrones();
            UpdateDronesState();
        }

        protected override void ApplyLevel(int level)
        {
            if (_data == null)
            {
                Debug.LogError($"{nameof(NeuralDroneAbility)}: Data is missing.", this);
                return;
            }

            _droneCount = _data.BaseDroneCount;
            _damage = _data.BaseDamage;
            _attackCooldown = _data.BaseAttackCooldown;
            _attackRange = _data.BaseAttackRange;
            _usePiercing = false;
            _piercingCount = 0;

            if (level >= 2)
            {
                _damage *= _data.Level2DamageMultiplier;
            }

            if (level >= 3)
            {
                _droneCount = _data.Level3DroneCount;
            }

            if (level >= 4)
            {
                _attackCooldown *= _data.Level4AttackCooldownMultiplier;
            }

            if (level >= 5)
            {
                _usePiercing = _data.Level5Piercing;
                _piercingCount = _data.Level5PiercingCount;
            }

            RebuildDrones();
            UpdateDronesState();
        }

        public override void OnUpdate()
        {
            if (_owner == null || _data == null)
                return;

            _orbitAngle += _data.OrbitRotationSpeed * Time.deltaTime;
            UpdateDroneSlots();
        }

        private void RebuildDrones()
        {
            RemoveExtraDrones();
            CreateMissingDrones();
        }

        private void RemoveExtraDrones()
        {
            for (int i = _activeDrones.Count - 1; i >= _droneCount; i--)
            {
                if (_activeDrones[i] != null)
                {
                    Object.Destroy(_activeDrones[i].gameObject);
                }

                _activeDrones.RemoveAt(i);
            }
        }

        private void CreateMissingDrones()
        {
            if (_data == null || _data.DronePrefab == null)
                return;

            while (_activeDrones.Count < _droneCount)
            {
                NeuralDroneUnit drone = Object.Instantiate(_data.DronePrefab, transform);
                _activeDrones.Add(drone);
            }
        }

        private void UpdateDronesState()
        {
            for (int i = 0; i < _activeDrones.Count; i++)
            {
                if (_activeDrones[i] == null)
                    continue;

                _activeDrones[i].Initialize(
                    _owner,
                    _data,
                    _damage,
                    _attackCooldown,
                    _attackRange,
                    _usePiercing,
                    _piercingCount);
            }

            UpdateDroneSlots();
        }

        private void UpdateDroneSlots()
        {
            if (_activeDrones.Count == 0)
                return;

            float angleStep = 360f / _activeDrones.Count;

            for (int i = 0; i < _activeDrones.Count; i++)
            {
                NeuralDroneUnit drone = _activeDrones[i];

                if (drone == null)
                    continue;

                float angle = _orbitAngle + angleStep * i;
                drone.SetOrbitSlot(angle, i);
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _activeDrones.Count; i++)
            {
                if (_activeDrones[i] != null)
                {
                    Object.Destroy(_activeDrones[i].gameObject);
                }
            }

            _activeDrones.Clear();
        }
    }
}