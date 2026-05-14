using UnityEngine;

namespace OmniumLessons
{
    public class BossFieldAbility
    {
        private readonly BossCharacter _boss;
        private readonly BossFieldAbilityData _data;

        private float _timer;

        public BossFieldAbility(BossCharacter boss, BossFieldAbilityData data)
        {
            _boss = boss;
            _data = data;
            _timer = _data != null ? _data.Cooldown : 0f;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_boss == null || _data == null)
                return;

            if (_boss.LiveComponent == null || !_boss.LiveComponent.IsAlive)
                return;

            _timer -= deltaTime;

            if (_timer > 0f)
                return;

            Activate();
            _timer = _data.Cooldown;
        }

        private void Activate()
        {
            Character player = GameManager.Instance?.CharacterFactory?.Player;

            if (player == null)
                return;

            if (player.LiveComponent == null || !player.LiveComponent.IsAlive)
                return;

            if (player.StatusEffectController == null)
                return;

            float slowMultiplier = 1f - _data.SlowPercent;
            SlowEffect slowEffect = new SlowEffect(slowMultiplier, _data.Duration);

            player.StatusEffectController.ReplaceEffect<SlowEffect>(slowEffect);
            Debug.Log($"Boss Field Ability: Slow replaced. Percent = {_data.SlowPercent}, Duration = {_data.Duration}");
        }
        
    }
}