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
        public float MaxHealth => _characterOwner.CharacterData.MaxHealth;

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

        public void GetDamage(float damage)
        {
            if (!IsAlive)
                return;

            if (damage <= 0f)
                return;

            // 🔹 НЕУЯЗВИМОСТЬ (Phase Shift)
            if (_characterOwner != null &&
                _characterOwner.StatusEffectController != null &&
                _characterOwner.StatusEffectController.HasEffect<InvulnerabilityEffect>())
            {
                // можно добавить визуал/лог при желании
                // Debug.Log($"{_characterOwner.name} is INVULNERABLE");

                return;
            }

            Health -= damage;

            OnCharacterHealthChange?.Invoke(_characterOwner);

            Debug.Log($"{_characterOwner.name} get damage by {damage}. Health: {Health}/{MaxHealth}");

            // 💥 если это игрок — сбрасываем серию "без урона"
            if (_characterOwner.CharacterType == CharacterType.DefaultPlayer)
            {
                GameManager.Instance?.ResonanceManager?.RegisterPlayerDamaged();
            }
        }

        private void SetDeath()
        {
            // 💥 если это враг — начисляем resonance
            if (_characterOwner.CharacterType != CharacterType.DefaultPlayer)
            {
                GameManager.Instance?.ResonanceManager?.RegisterEnemyKilled(_characterOwner);
            }

            OnCharacterDeath?.Invoke(_characterOwner);
        }
    }
}