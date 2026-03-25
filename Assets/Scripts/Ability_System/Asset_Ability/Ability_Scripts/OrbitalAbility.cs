using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class OrbitalAbility : AbilityBehaviour
    {
        [SerializeField] private float _radius = 2f;
        [SerializeField] private float _rotationSpeed = 120f;

        [SerializeField] private float _hitRadius = 0.7f;
        [SerializeField] private int _damage = 3;
        [SerializeField] private float _damageCooldown = 0.5f;

        private float _currentAngle;

        // запоминаем, кого недавно били, чтобы не наносить урон каждый кадр
        private Dictionary<Character, float> _damageTimers = new Dictionary<Character, float>();

        public override void Initialize(Character owner, AbilityData data)
        {
            base.Initialize(owner, data);

            if (_owner == null)
            {
                Debug.LogError("OrbitalAbility: owner is null");
                return;
            }

            transform.position = _owner.transform.position + Vector3.right * _radius;
        }

        public override void OnUpdate()
        {
            if (_owner == null)
                return;

            UpdateTimers();

            RotateAroundOwner();
            DealDamageToEnemies();
        }

        private void RotateAroundOwner()
        {
            _currentAngle += _rotationSpeed * Time.deltaTime;

            float angleInRadians = _currentAngle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(angleInRadians) * _radius,
                0f,
                Mathf.Sin(angleInRadians) * _radius
            );

            transform.position = _owner.transform.position + offset;
        }

        private void DealDamageToEnemies()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _hitRadius);

            for (int i = 0; i < hits.Length; i++)
            {
                Character target = hits[i].GetComponent<Character>();

                if (target == null)
                    continue;

                if (target == _owner)
                    continue;

                if (!target.LiveComponent.IsAlive)
                    continue;

                // бьём только не-игроков
                if (target.CharacterType == CharacterType.DefaultPlayer)
                    continue;

                if (_damageTimers.ContainsKey(target) && _damageTimers[target] > 0f)
                    continue;

                target.LiveComponent.GetDamage(_damage);
                _damageTimers[target] = _damageCooldown;
            }
        }

        private void UpdateTimers()
        {
            List<Character> keys = new List<Character>(_damageTimers.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                Character target = keys[i];

                if (target == null)
                {
                    _damageTimers.Remove(target);
                    continue;
                }

                _damageTimers[target] -= Time.deltaTime;

                if (_damageTimers[target] <= 0f)
                    _damageTimers[target] = 0f;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _hitRadius);
        }
    }
}