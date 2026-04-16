using UnityEngine;

namespace OmniumLessons
{
    public class BossPulseAbility
    {
        private readonly BossCharacter _boss;
        private readonly BossPulseAbilityData _data;

        private float _cooldownTimer;

        public BossPulseAbility(BossCharacter boss, BossPulseAbilityData data)
        {
            _boss = boss;
            _data = data;
        }

        public void OnUpdate(float deltaTime)
        {
            _cooldownTimer -= deltaTime;

            if (_cooldownTimer > 0)
                return;

            Activate();
            _cooldownTimer = _data.Cooldown;
        }

        private void Activate()
        {
            // здесь потом спавн визуального кольца
            Debug.Log("Pulse Activated");
        }
    }
}