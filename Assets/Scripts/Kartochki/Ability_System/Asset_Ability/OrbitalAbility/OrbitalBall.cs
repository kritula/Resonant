using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class OrbitalBall : MonoBehaviour
    {
        [SerializeField] private Collider _triggerCollider;

        private PlayerCharacter _owner;
        private float _damage;
        private float _hitCooldownPerTarget;
        private float _criticalChance;
        private float _criticalDamageMultiplier;

        private readonly Dictionary<Character, float> _targetHitTimers = new Dictionary<Character, float>();

        public void Initialize(
            PlayerCharacter owner,
            float damage,
            float hitCooldownPerTarget,
            float criticalChance,
            float criticalDamageMultiplier)
        {
            _owner = owner;
            _damage = damage;
            _hitCooldownPerTarget = hitCooldownPerTarget;
            _criticalChance = criticalChance;
            _criticalDamageMultiplier = criticalDamageMultiplier;

            if (_triggerCollider == null)
            {
                _triggerCollider = GetComponent<Collider>();
            }

            if (_triggerCollider != null)
            {
                _triggerCollider.isTrigger = true;
            }
        }

        private void Update()
        {
            UpdateCooldowns();
        }

        private void UpdateCooldowns()
        {
            if (_targetHitTimers.Count == 0)
                return;

            List<Character> keys = new List<Character>(_targetHitTimers.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                Character target = keys[i];

                if (target == null)
                {
                    _targetHitTimers.Remove(target);
                    continue;
                }

                float newTime = _targetHitTimers[target] - Time.deltaTime;

                if (newTime > 0f)
                {
                    _targetHitTimers[target] = newTime;
                }
                else
                {
                    _targetHitTimers.Remove(target);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            Character target = other.GetComponent<Character>();

            if (target == null)
            {
                target = other.GetComponentInParent<Character>();
            }

            if (target == null)
                return;

            if (_owner == null)
                return;

            if (target == _owner)
                return;

            if (target.CharacterType == _owner.CharacterType)
                return;

            if (target.LiveComponent == null || !target.LiveComponent.IsAlive)
                return;

            if (_targetHitTimers.ContainsKey(target))
                return;

            float finalDamage = _damage;

            if (_criticalChance > 0f && Random.value <= _criticalChance)
            {
                finalDamage *= _criticalDamageMultiplier;
            }

            target.LiveComponent.GetDamage(finalDamage);
            _targetHitTimers[target] = _hitCooldownPerTarget;
        }
    }
}