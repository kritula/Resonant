using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class PhaseShiftAbility : AbilityBehaviour
    {
        [Header("Data")]
        [SerializeField] private PhaseShiftAbilityData _data;

        [Header("Visual Effects")]
        [SerializeField] private GameObject _phaseVisual;
        [SerializeField] private GameObject _phaseExitEffect;
        [SerializeField] private float _phaseExitEffectLifetime = 1f;

        private float _phaseDuration;
        private float _cooldown;
        private float _speedMultiplier;
        private float _exitDamage;
        private float _exitDamageRadius;

        private bool _hasSpeedBonus;
        private bool _hasExitDamage;

        private bool _isPhaseActive;
        private float _phaseTimer;
        private float _cooldownTimer;

        private float _cachedOriginalSpeed;
        private bool _speedApplied;

        public override void Initialize(PlayerCharacter owner, AbilityUpgradeData abilityData)
        {
            base.Initialize(owner, abilityData);

            _isPhaseActive = false;
            _phaseTimer = 0f;
            _cooldownTimer = _cooldown;

            SetPhaseVisual(false);
        }

        protected override void ApplyLevel(int level)
        {
            if (_data == null)
            {
                Debug.LogError($"{nameof(PhaseShiftAbility)}: Data is missing.", this);
                return;
            }

            _phaseDuration = _data.BasePhaseDuration;
            _cooldown = _data.BaseCooldown;

            _hasSpeedBonus = false;
            _speedMultiplier = 1f;

            _hasExitDamage = false;
            _exitDamage = 0f;
            _exitDamageRadius = 0f;

            if (level >= 2)
            {
                _phaseDuration = _data.Level2PhaseDuration;
            }

            if (level >= 3)
            {
                _hasSpeedBonus = true;
                _speedMultiplier = _data.Level3SpeedMultiplier;
            }

            if (level >= 4)
            {
                _cooldown *= _data.Level4CooldownMultiplier;
            }

            if (level >= 5)
            {
                _hasExitDamage = true;
                _exitDamage = _data.Level5ExitDamage;
                _exitDamageRadius = _data.Level5ExitDamageRadius;
            }
        }

        public override void OnUpdate()
        {
            if (_owner == null || _data == null)
                return;

            if (_isPhaseActive)
            {
                UpdateActivePhase();
                return;
            }

            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
                return;
            }

            StartPhase();
        }

        private void StartPhase()
        {
            _isPhaseActive = true;
            _phaseTimer = _phaseDuration;

            if (_owner.StatusEffectController != null)
            {
                _owner.StatusEffectController.AddEffect(new InvulnerabilityEffect(_phaseDuration));
            }

            ApplySpeedBonus();
            SetPhaseVisual(true);
        }

        private void UpdateActivePhase()
        {
            _phaseTimer -= Time.deltaTime;

            if (_phaseTimer > 0f)
                return;

            EndPhase();
        }

        private void EndPhase()
        {
            _isPhaseActive = false;
            _phaseTimer = 0f;
            _cooldownTimer = _cooldown;

            RemoveSpeedBonus();
            SetPhaseVisual(false);
            SpawnExitEffect();

            if (_hasExitDamage)
            {
                DealExitDamage();
            }
        }

        private void ApplySpeedBonus()
        {
            if (!_hasSpeedBonus)
                return;

            if (_owner.MovableComponent == null)
                return;

            if (_speedApplied)
                return;

            _cachedOriginalSpeed = _owner.MovableComponent.Speed;
            _owner.MovableComponent.Speed = _cachedOriginalSpeed * _speedMultiplier;
            _speedApplied = true;
        }

        private void RemoveSpeedBonus()
        {
            if (!_speedApplied)
                return;

            if (_owner != null && _owner.MovableComponent != null)
            {
                _owner.MovableComponent.Speed = _cachedOriginalSpeed;
            }

            _speedApplied = false;
        }

        private void SetPhaseVisual(bool isActive)
        {
            if (_phaseVisual == null)
                return;

            _phaseVisual.SetActive(isActive);
        }

        private void SpawnExitEffect()
        {
            if (_phaseExitEffect == null || _owner == null)
                return;

            GameObject effect = Object.Instantiate(
                _phaseExitEffect,
                _owner.transform.position,
                Quaternion.identity);

            if (_phaseExitEffectLifetime > 0f)
            {
                Object.Destroy(effect, _phaseExitEffectLifetime);
            }
        }

        private void DealExitDamage()
        {
            Collider[] hits = Physics.OverlapSphere(_owner.transform.position, _exitDamageRadius);
            HashSet<Character> damagedTargets = new HashSet<Character>();

            for (int i = 0; i < hits.Length; i++)
            {
                Character target = hits[i].GetComponent<Character>();

                if (target == null)
                {
                    target = hits[i].GetComponentInParent<Character>();
                }

                if (target == null)
                    continue;

                if (target == _owner)
                    continue;

                if (target.CharacterType == _owner.CharacterType)
                    continue;

                if (target.LiveComponent == null || !target.LiveComponent.IsAlive)
                    continue;

                if (damagedTargets.Contains(target))
                    continue;

                damagedTargets.Add(target);
                target.LiveComponent.GetDamage(_exitDamage);
            }
        }

        private void OnDestroy()
        {
            RemoveSpeedBonus();
            SetPhaseVisual(false);
        }

        private void OnDrawGizmosSelected()
        {
            if (_hasExitDamage && _exitDamageRadius > 0f)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(transform.position, _exitDamageRadius);
            }
        }
    }
}