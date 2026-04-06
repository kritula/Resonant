using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class AuraAbility : AbilityBehaviour
    {
        [Header("Aura settings")]
        [SerializeField] private float _radius = 3f;
        [SerializeField] private float _damagePerTick = 2f;
        [SerializeField] private float _tickRate = 1f;

        [Header("Slow settings")]
        [SerializeField] private float _slowMultiplier = 0.5f; // 0.5 = -50% скорости

        private float _tickTimer;

        private readonly List<Character> _targetsInAura = new List<Character>();

        private readonly Dictionary<Character, float> _originalSpeeds = new Dictionary<Character, float>();

        public override void Initialize(PlayerCharacter owner, AbilityUpgradeData abilityData)
        {
            base.Initialize(owner, abilityData);

            _tickTimer = 0f;
        }

        public override void OnUpdate()
        {
            if (_owner == null)
                return;

            UpdateTargets();

            HandleDamage();

            HandleSlow();
        }

        private void UpdateTargets()
        {
            _targetsInAura.Clear();

            Collider[] hits = Physics.OverlapSphere(_owner.transform.position, _radius);

            foreach (var hit in hits)
            {
                Character character = hit.GetComponent<Character>();

                if (character == null)
                    continue;

                if (character == _owner)
                    continue;

                if (character.CharacterType == _owner.CharacterType)
                    continue;

                if (!character.LiveComponent.IsAlive)
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

            foreach (var target in _targetsInAura)
            {
                target.LiveComponent.GetDamage(_damagePerTick);
            }

            _tickTimer = _tickRate;
        }

        private void HandleSlow()
        {
            // ѕримен€ем замедление
            foreach (var target in _targetsInAura)
            {
                if (target.MovableComponent == null)
                    continue;

                if (!_originalSpeeds.ContainsKey(target))
                {
                    _originalSpeeds[target] = target.MovableComponent.Speed;
                }

                target.MovableComponent.Speed = _originalSpeeds[target] * _slowMultiplier;
            }

            // ¬осстанавливаем скорость тем, кто вышел из ауры
            List<Character> toRestore = new List<Character>();

            foreach (var pair in _originalSpeeds)
            {
                if (!_targetsInAura.Contains(pair.Key))
                {
                    if (pair.Key != null && pair.Key.MovableComponent != null)
                    {
                        pair.Key.MovableComponent.Speed = pair.Value;
                    }

                    toRestore.Add(pair.Key);
                }
            }

            foreach (var character in toRestore)
            {
                _originalSpeeds.Remove(character);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}