using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class PlayerAttackStatsDecoratorController : MonoBehaviour
    {
        private readonly List<IAttackStatsDecorator> _decorators = new();

        public void AddDecorator(IAttackStatsDecorator decorator)
        {
            if (decorator == null)
                return;

            if (_decorators.Contains(decorator))
                return;

            _decorators.Add(decorator);
        }

        public void RemoveDecorator(IAttackStatsDecorator decorator)
        {
            if (decorator == null)
                return;

            _decorators.Remove(decorator);
        }

        public AttackStats BuildStats(WeaponData weaponData)
        {
            AttackStats stats = new AttackStats(
                weaponData.Damage,
                weaponData.AttackCooldown);

            for (int i = 0; i < _decorators.Count; i++)
            {
                stats = _decorators[i].Decorate(stats);
            }

            return stats;
        }

        public void ClearDecorators()
        {
            _decorators.Clear();
        }
    }
}