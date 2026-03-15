using System;
using UnityEngine;

namespace OmniumLessons
{
    public class CharacterLiveComponent : ILiveComponent
    {
        public event Action<Character> OnCharacterDeath;
        public event Action<Character> OnCharacterHealthChange;

        private Character _characterOwner;
        private float _health;

        public bool IsAlive => Health > 0;
        public int MaxHealth => _characterOwner.CharacterData.MaxHealth;

        public float Health
        {
            get => _health;
            private set
            {
                _health = value;

                if (_health > MaxHealth)
                    _health = MaxHealth;

                if (_health <= 0)
                {
                    _health = 0;
                    SetDeath();
                }
            }
        }

        public CharacterLiveComponent(Character characterOwner)
        {
            _characterOwner = characterOwner;
            _health = MaxHealth;
        }

        public void GetDamage(int damage)
        {
            Health -= damage;
            OnCharacterHealthChange?.Invoke(_characterOwner);

            Debug.Log($"{_characterOwner.name} get damage by {damage}. Health: {Health}/{MaxHealth}");
        }

        private void SetDeath()
        {
            OnCharacterDeath?.Invoke(_characterOwner);
        }
    }
}