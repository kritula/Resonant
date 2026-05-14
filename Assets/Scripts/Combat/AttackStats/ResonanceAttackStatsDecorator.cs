using UnityEngine;

namespace OmniumLessons
{
    public class ResonanceAttackStatsDecorator : IAttackStatsDecorator
    {
        private readonly float _damageMultiplier;
        private readonly float _attackSpeedMultiplier;

        public ResonanceAttackStatsDecorator(
            float damageMultiplier,
            float attackSpeedMultiplier)
        {
            _damageMultiplier = damageMultiplier;
            _attackSpeedMultiplier = attackSpeedMultiplier;
        }

        public AttackStats Decorate(AttackStats stats)
        {
            stats.Damage *= _damageMultiplier;
            stats.AttackCooldown /= _attackSpeedMultiplier;

            return stats;
        }
    }
}