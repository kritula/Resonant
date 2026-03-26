using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class PiercingProjectile : MonoBehaviour
    {
        private Character _owner;
        private PiercingProjectileData _data;
        private Vector3 _direction;
        private float _lifeTimer;
        private int _currentPierceCount;

        private HashSet<Character> _damagedCharacters = new HashSet<Character>();

        public void Initialize(Character owner, Vector3 direction, PiercingProjectileData data)
        {
            _owner = owner;
            _direction = direction.normalized;
            _data = data;
            _lifeTimer = _data.LifeTime;
            _currentPierceCount = 0;

            transform.forward = _direction;
        }

        private void Update()
        {
            if (_data == null)
            {
                return;
            }

            transform.position += _direction * _data.ProjectileSpeed * Time.deltaTime;

            _lifeTimer -= Time.deltaTime;

            if (_lifeTimer <= 0f)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Character target = other.GetComponentInParent<Character>();

            if (target == null)
            {
                return;
            }

            if (target == _owner)
            {
                return;
            }

            if (target.CharacterType == CharacterType.DefaultPlayer)
            {
                return;
            }

            if (target.LiveComponent == null || target.LiveComponent.IsAlive == false)
            {
                return;
            }

            if (_damagedCharacters.Contains(target))
            {
                return;
            }

            target.LiveComponent.GetDamage(_data.Damage);
            _damagedCharacters.Add(target);
            Debug.LogError("Дотронулся");

            _currentPierceCount++;

            if (_currentPierceCount >= _data.MaxPierceCount)
            {
                Destroy(gameObject);
            }
        }
    }
}