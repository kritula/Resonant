using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class WhipHitbox : MonoBehaviour
    {
        [SerializeField] private BoxCollider _boxCollider;

        private PlayerCharacter _owner;
        private float _damage;
        private float _lifeTime;

        private readonly HashSet<Character> _damagedTargets = new HashSet<Character>();
        private bool _isInitialized;

        public void Initialize(
            PlayerCharacter owner,
            float damage,
            float attackDistance,
            float attackWidth,
            float lifeTime)
        {
            _owner = owner;
            _damage = damage;
            _lifeTime = lifeTime;
            _isInitialized = true;

            if (_boxCollider == null)
            {
                _boxCollider = GetComponent<BoxCollider>();
            }

            if (_boxCollider != null)
            {
                _boxCollider.isTrigger = true;
                _boxCollider.size = new Vector3(attackDistance, 1f, attackWidth);
                _boxCollider.center = Vector3.zero;
            }

            Vector3 localScale = transform.localScale;
            localScale.x = attackDistance;
            localScale.z = attackWidth;
            transform.localScale = localScale;

            Destroy(gameObject, _lifeTime);
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

            if (_owner == null)
                return;

            if (target == _owner)
                return;

            if (target.CharacterType == _owner.CharacterType)
                return;

            if (!target.LiveComponent.IsAlive)
                return;

            if (_damagedTargets.Contains(target))
                return;

            _damagedTargets.Add(target);
            target.LiveComponent.GetDamage(_damage);
        }
    }
}