using UnityEngine;

namespace OmniumLessons
{
    public class MeleeAttackComponent : IAttackComponent
    {
        public float Damage { get; private set; }

        private CharacterData _characterData;
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

        public void MakeDamage(Character damageTarget)
        {
            if (_characterData == null)
                return;

            if (_lockDamageTime > 0f)
                return;

            if (damageTarget == null)
                return;

            if (!damageTarget.LiveComponent.IsAlive)
                return;

            damageTarget.LiveComponent.GetDamage(Damage);
            _lockDamageTime = _lockDamageTimeMax;
        }

        public void OnUpdate()
        {
            if (_lockDamageTime > 0f)
            {
                _lockDamageTime -= Time.deltaTime;
            }
        }
    }
}