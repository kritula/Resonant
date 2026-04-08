using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class NeuralDroneProjectile : MonoBehaviour
    {
        private PlayerCharacter _owner;
        private Character _target;
        private float _damage;
        private float _speed;
        private float _lifeTime;
        private bool _usePiercing;
        private int _remainingPierces;

        private readonly HashSet<Character> _damagedTargets = new HashSet<Character>();
        private bool _isInitialized;

        public void Initialize(
            PlayerCharacter owner,
            Character target,
            float damage,
            float speed,
            float lifeTime,
            bool usePiercing,
            int piercingCount)
        {
            _owner = owner;
            _target = target;
            _damage = damage;
            _speed = speed;
            _lifeTime = lifeTime;
            _usePiercing = usePiercing;
            _remainingPierces = piercingCount;
            _isInitialized = true;

            Destroy(gameObject, _lifeTime);
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            Vector3 direction;

            if (_target != null && _target.LiveComponent != null && _target.LiveComponent.IsAlive)
            {
                direction = (_target.transform.position - transform.position).normalized;
            }
            else
            {
                direction = transform.forward;
            }

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.forward = direction;
                transform.position += direction * _speed * Time.deltaTime;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Character target = other.GetComponent<Character>();

            if (target == null)
            {
                target = other.GetComponentInParent<Character>();
            }

            if (target == null)
                return;

            if (_owner == null)
                return;

            if (target == _owner)
                return;

            if (target.CharacterType == _owner.CharacterType)
                return;

            if (target.LiveComponent == null || !target.LiveComponent.IsAlive)
                return;

            if (_damagedTargets.Contains(target))
                return;

            _damagedTargets.Add(target);
            target.LiveComponent.GetDamage(_damage);

            if (!_usePiercing)
            {
                Destroy(gameObject);
                return;
            }

            _remainingPierces--;

            if (_remainingPierces < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}