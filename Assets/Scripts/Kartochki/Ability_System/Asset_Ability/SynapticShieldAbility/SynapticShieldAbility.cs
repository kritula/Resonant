using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class SynapticShieldAbility : AbilityBehaviour
    {
        [SerializeField] private SynapticShieldAbilityData _data;

        private float _radius;
        private float _cooldown;
        private float _knockbackForce;
        private float _damage;
        private float _stunDuration;

        private float _cooldownTimer;

        public override void Initialize(PlayerCharacter owner, AbilityUpgradeData abilityData)
        {
            base.Initialize(owner, abilityData);
            _cooldownTimer = 0f;
        }

        protected override void ApplyLevel(int level)
        {
            if (_data == null)
            {
                Debug.LogError($"{nameof(SynapticShieldAbility)}: Data is missing.", this);
                return;
            }

            _radius = _data.BaseRadius;
            _cooldown = _data.BaseCooldown;
            _knockbackForce = _data.BaseKnockbackForce;
            _damage = 0f;
            _stunDuration = 0f;

            if (level >= 2)
            {
                _radius *= _data.Level2RadiusMultiplier;
            }

            if (level >= 3)
            {
                _damage = _data.Level3Damage;
            }

            if (level >= 4)
            {
                _cooldown *= _data.Level4CooldownMultiplier;
            }

            if (level >= 5)
            {
                _knockbackForce *= _data.Level5KnockbackMultiplier;
                _damage *= _data.Level5DamageMultiplier;
                _stunDuration = _data.Level5StunDuration;
            }
        }

        public override void OnUpdate()
        {
            if (_owner == null || _data == null)
                return;

            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
                return;
            }

            EmitImpulse();
            _cooldownTimer = _cooldown;
        }

        private void EmitImpulse()
        {
            Collider[] hits = Physics.OverlapSphere(_owner.transform.position, _radius);
            HashSet<Character> affectedTargets = new HashSet<Character>();

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

                if (affectedTargets.Contains(target))
                    continue;

                affectedTargets.Add(target);
                ApplyImpulseToTarget(target);
            }
        }

        private void ApplyImpulseToTarget(Character target)
        {
            Vector3 direction = target.transform.position - _owner.transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.0001f)
            {
                direction = target.transform.forward;
                direction.y = 0f;
            }

            direction.Normalize();

            if (target.CharacterData != null && target.CharacterData.CharacterController != null)
            {
                target.CharacterData.CharacterController.Move(direction * _knockbackForce);
            }
            else
            {
                target.transform.position += direction * _knockbackForce;
            }

            if (_damage > 0f)
            {
                target.LiveComponent.GetDamage(_damage);
            }

            if (_stunDuration > 0f && target.StatusEffectController != null)
            {
                target.StatusEffectController.AddEffect(new StunEffect(_stunDuration));
            }
        }

        private void OnDrawGizmosSelected()
        {
            float gizmoRadius = _data != null ? _data.BaseRadius : 1.5f;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, gizmoRadius);
        }
    }
}