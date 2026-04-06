using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class BaseAttackProjectile : MonoBehaviour
    {
        private Character _owner;
        private float _damage;
        private float _speed;
        private float _lifeTime;
        private Vector3 _direction;
        private bool _isInitialized;

        private int _remainingPiercingCount;
        private int _remainingRicochetCount;
        private float _ricochetRadius;

        private readonly List<Character> _damagedCharacters = new List<Character>();

        public void Initialize(
            Character owner,
            float damage,
            float speed,
            float lifeTime,
            Vector3 direction,
            int piercingCount,
            int ricochetCount,
            float ricochetRadius)
        {
            _owner = owner;
            _damage = damage;
            _speed = speed;
            _lifeTime = lifeTime;
            _direction = direction.normalized;
            _remainingPiercingCount = piercingCount;
            _remainingRicochetCount = ricochetCount;
            _ricochetRadius = ricochetRadius;
            _isInitialized = true;

            Destroy(gameObject, _lifeTime);
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            transform.position += _direction * _speed * Time.deltaTime;

            if (_direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(_direction);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isInitialized)
                return;

            Character target = other.GetComponent<Character>();

            if (target == null)
            {
                target = other.GetComponentInParent<Character>();
            }

            if (target == null)
                return;

            if (target == _owner)
                return;

            if (_owner != null && target.CharacterType == _owner.CharacterType)
                return;

            if (!target.LiveComponent.IsAlive)
                return;

            if (_damagedCharacters.Contains(target))
                return;

            _damagedCharacters.Add(target);
            target.LiveComponent.GetDamage(_damage);

            if (_remainingPiercingCount > 0)
            {
                _remainingPiercingCount--;
                return;
            }

            if (_remainingRicochetCount > 0)
            {
                Character ricochetTarget = FindRicochetTarget(target);

                if (ricochetTarget != null)
                {
                    Vector3 newDirection = ricochetTarget.transform.position - transform.position;
                    newDirection.y = 0f;

                    if (newDirection != Vector3.zero)
                    {
                        _direction = newDirection.normalized;
                        _remainingRicochetCount--;
                        return;
                    }
                }
            }

            Destroy(gameObject);
        }

        private Character FindRicochetTarget(Character currentTarget)
        {
            if (_owner == null)
                return null;

            Character result = null;
            float nearestDistance = float.MaxValue;

            List<Character> activeCharacters = GameManager.Instance.CharacterFactory.ActiveCharacters;

            for (int i = 0; i < activeCharacters.Count; i++)
            {
                Character candidate = activeCharacters[i];

                if (candidate == null)
                    continue;

                if (candidate == currentTarget)
                    continue;

                if (candidate == _owner)
                    continue;

                if (candidate.CharacterType == _owner.CharacterType)
                    continue;

                if (!candidate.LiveComponent.IsAlive)
                    continue;

                if (_damagedCharacters.Contains(candidate))
                    continue;

                float distance = Vector3.Distance(currentTarget.transform.position, candidate.transform.position);

                if (distance > _ricochetRadius)
                    continue;

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    result = candidate;
                }
            }

            return result;
        }
    }
}