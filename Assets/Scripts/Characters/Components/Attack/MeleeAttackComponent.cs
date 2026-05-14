using UnityEngine;

namespace OmniumLessons
{
    public class MeleeAttackComponent : IAttackComponent
    {
        public float Damage { get; private set; }

        private CharacterData _characterData;
        private Character _pendingDamageTarget;
        private float _lockDamageTime;
        private float _lockDamageTimeMax;

        public void Initialize(CharacterData characterData)
        {
            _characterData = characterData;

            if (_characterData != null && _characterData.WeaponData != null)
            {
                Damage = _characterData.WeaponData.Damage;
                _lockDamageTimeMax = _characterData.WeaponData.AttackCooldown;
            }
            else
            {
                Damage = 5f;
                _lockDamageTimeMax = 1f;
            }

            _lockDamageTime = 0f;
        }

        public bool MakeDamage(Character damageTarget)
        {
            if (!CanStartDamage(damageTarget))
                return false;

            ApplyDamage(damageTarget);
            _lockDamageTime = _lockDamageTimeMax;

            return true;
        }

        public bool StartDelayedDamage(Character damageTarget)
        {
            if (!CanStartDamage(damageTarget))
                return false;

            _pendingDamageTarget = damageTarget;
            _lockDamageTime = _lockDamageTimeMax;

            return true;
        }

        public bool ApplyPendingDamage()
        {
            if (_pendingDamageTarget == null)
                return false;

            Character damageTarget = _pendingDamageTarget;
            _pendingDamageTarget = null;

            return ApplyDamage(damageTarget);
        }

        public void ClearPendingDamage()
        {
            _pendingDamageTarget = null;
        }

        public void OnUpdate()
        {
            if (_lockDamageTime > 0f)
            {
                _lockDamageTime -= Time.deltaTime;
            }
        }

        private bool CanStartDamage(Character damageTarget)
        {
            if (_characterData == null)
                return false;

            if (_lockDamageTime > 0f)
                return false;

            return CanDamage(damageTarget);
        }

        private bool CanDamage(Character damageTarget)
        {
            if (damageTarget == null)
                return false;

            if (damageTarget.LiveComponent == null)
                return false;

            if (!damageTarget.LiveComponent.IsAlive)
                return false;

            return true;
        }

        private bool ApplyDamage(Character damageTarget)
        {
            if (!CanDamage(damageTarget))
                return false;

            damageTarget.LiveComponent.GetDamage(Damage);
            return true;
        }
    }
}
