using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class AuraAbility : AbilityBehaviour
    {
        [SerializeField] private AuraAbilityData _data;

        private float _radius;
        private float _damagePerTick;
        private float _tickRate;
        private float _slowMultiplier;

        private bool _hasSlow;
        private bool _hasPulse;

        private float _tickTimer;
        private float _pulseTimer;
        private float _currentRadius;

        private readonly List<Character> _targetsInAura = new List<Character>();
        private readonly Dictionary<Character, float> _originalSpeeds = new Dictionary<Character, float>();

        public override void Initialize(PlayerCharacter owner, AbilityUpgradeData abilityData)
        {
            base.Initialize(owner, abilityData);

            _tickTimer = 0f;
            _pulseTimer = 0f;
        }

        protected override void ApplyLevel(int level)
        {
            if (_data == null)
            {
                Debug.LogError($"{nameof(AuraAbility)}: Data is missing.", this);
                return;
            }

            _radius = _data.BaseRadius;
            _damagePerTick = _data.BaseDamagePerTick;
            _tickRate = _data.BaseTickRate;

            _hasSlow = false;
            _hasPulse = false;
            _slowMultiplier = 1f;

            if (level >= 2)
            {
                _radius = _data.Level2Radius;
            }

            if (level >= 3)
            {
                _hasSlow = true;
                _slowMultiplier = _data.Level3SlowMultiplier;
            }

            if (level >= 4)
            {
                _damagePerTick *= _data.Level4DamageMultiplier;
            }

            if (level >= 5)
            {
                _slowMultiplier = _data.Level5SlowMultiplier;
                _damagePerTick *= _data.Level5DamageMultiplier;
                _hasPulse = true;
            }

            _currentRadius = _radius;
        }

        public override void OnUpdate()
        {
            if (_owner == null || _data == null)
                return;

            UpdatePulse();
            UpdateTargets();
            HandleDamage();
            HandleSlow();
            CleanupDeadTargets();
        }

        private void UpdatePulse()
        {
            if (!_hasPulse)
            {
                _currentRadius = _radius;
                return;
            }

            _pulseTimer += Time.deltaTime;

            float t = Mathf.Sin((_pulseTimer / _data.PulseInterval) * Mathf.PI * 2f) * 0.5f + 0.5f;
            float pulseRadius = _radius * _data.PulseRadiusMultiplier;

            _currentRadius = Mathf.Lerp(_radius, pulseRadius, t);
        }

        private void UpdateTargets()
        {
            _targetsInAura.Clear();

            Collider[] hits = Physics.OverlapSphere(_owner.transform.position, _currentRadius);

            for (int i = 0; i < hits.Length; i++)
            {
                Character character = hits[i].GetComponent<Character>();

                if (character == null)
                {
                    character = hits[i].GetComponentInParent<Character>();
                }

                if (character == null)
                    continue;

                if (character == _owner)
                    continue;

                if (character.CharacterType == _owner.CharacterType)
                    continue;

                if (character.LiveComponent == null || !character.LiveComponent.IsAlive)
                    continue;

                if (_targetsInAura.Contains(character))
                    continue;

                _targetsInAura.Add(character);
            }
        }

        private void HandleDamage()
        {
            if (_tickTimer > 0f)
            {
                _tickTimer -= Time.deltaTime;
                return;
            }

            for (int i = 0; i < _targetsInAura.Count; i++)
            {
                Character target = _targetsInAura[i];

                if (target == null || target.LiveComponent == null || !target.LiveComponent.IsAlive)
                    continue;

                target.LiveComponent.GetDamage(_damagePerTick);
            }

            _tickTimer = _tickRate;
        }

        private void HandleSlow()
        {
            if (!_hasSlow)
            {
                RestoreAllSpeeds();
                return;
            }

            for (int i = 0; i < _targetsInAura.Count; i++)
            {
                Character target = _targetsInAura[i];

                if (target == null || target.MovableComponent == null)
                    continue;

                if (!_originalSpeeds.ContainsKey(target))
                {
                    _originalSpeeds[target] = target.MovableComponent.Speed;
                }

                target.MovableComponent.Speed = _originalSpeeds[target] * _slowMultiplier;
            }

            List<Character> toRestore = new List<Character>();

            foreach (var pair in _originalSpeeds)
            {
                Character target = pair.Key;

                bool shouldRestore =
                    target == null ||
                    target.MovableComponent == null ||
                    !_targetsInAura.Contains(target);

                if (!shouldRestore)
                    continue;

                if (target != null && target.MovableComponent != null)
                {
                    target.MovableComponent.Speed = pair.Value;
                }

                toRestore.Add(target);
            }

            for (int i = 0; i < toRestore.Count; i++)
            {
                _originalSpeeds.Remove(toRestore[i]);
            }
        }

        private void CleanupDeadTargets()
        {
            List<Character> toRemove = new List<Character>();

            foreach (var pair in _originalSpeeds)
            {
                Character target = pair.Key;

                if (target == null)
                {
                    toRemove.Add(target);
                    continue;
                }

                if (target.LiveComponent != null && !target.LiveComponent.IsAlive)
                {
                    if (target.MovableComponent != null)
                    {
                        target.MovableComponent.Speed = pair.Value;
                    }

                    toRemove.Add(target);
                }
            }

            for (int i = 0; i < toRemove.Count; i++)
            {
                _originalSpeeds.Remove(toRemove[i]);
            }
        }

        private void RestoreAllSpeeds()
        {
            foreach (var pair in _originalSpeeds)
            {
                if (pair.Key != null && pair.Key.MovableComponent != null)
                {
                    pair.Key.MovableComponent.Speed = pair.Value;
                }
            }

            _originalSpeeds.Clear();
        }

        private void OnDestroy()
        {
            RestoreAllSpeeds();
        }

        private void OnDrawGizmosSelected()
        {
            float gizmoRadius = _currentRadius > 0f ? _currentRadius : (_data != null ? _data.BaseRadius : 2f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, gizmoRadius);
        }
    }
}