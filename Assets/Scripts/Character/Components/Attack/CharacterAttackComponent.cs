using UnityEngine;

namespace OmniumLessons
{
    public class CharacterAttackComponent : IAttackComponent
    {
        private float _lockDamageTimeMax;

        private float _lockDamageTime = 0;

        private CharacterData _characterData;

        public int Damage => _characterData.WeaponData.Damage;


        public void Initialize(CharacterData characterData)
        {
            _characterData = characterData;
            _lockDamageTimeMax = _characterData.WeaponData.AttackCooldown;
        }
        
        public void MakeDamage(Character damageTarget) 
        {
            if (_lockDamageTime > 0)
                return;
            
            if (damageTarget == null || damageTarget.LiveComponent == null)
                return;

            if (!damageTarget.LiveComponent.IsAlive)
                return;
            
            damageTarget.LiveComponent.GetDamage(Damage);

            _lockDamageTime = _lockDamageTimeMax;
        }

        public void OnUpdate()
        {
            if (_lockDamageTime > 0)
                _lockDamageTime -= Time.deltaTime;
        }
    }
}