using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class MineObject : MonoBehaviour
    {
        [SerializeField] private SphereCollider _triggerCollider;

        private PlayerCharacter _owner;
        private float _damage;
        private float _explosionRadius;
        private float _activationDelay;
        private float _lifeTime;

        private bool _chainReactionEnabled;
        private float _chainReactionDelay;
        private float _chainReactionRadius;

        private bool _isArmed;
        private bool _isExploded;

        public void Initialize(
            PlayerCharacter owner,
            float damage,
            float explosionRadius,
            float activationDelay,
            float lifeTime,
            bool chainReactionEnabled,
            float chainReactionDelay,
            float chainReactionRadius)
        {
            _owner = owner;
            _damage = damage;
            _explosionRadius = explosionRadius;
            _activationDelay = activationDelay;
            _lifeTime = lifeTime;

            _chainReactionEnabled = chainReactionEnabled;
            _chainReactionDelay = chainReactionDelay;
            _chainReactionRadius = chainReactionRadius;

            if (_triggerCollider == null)
            {
                _triggerCollider = GetComponent<SphereCollider>();
            }

            if (_triggerCollider != null)
            {
                _triggerCollider.isTrigger = true;
                _triggerCollider.radius = 0.35f;
            }

            StartCoroutine(ArmRoutine());
            Destroy(gameObject, _lifeTime);
        }

        private IEnumerator ArmRoutine()
        {
            yield return new WaitForSeconds(_activationDelay);
            _isArmed = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isArmed || _isExploded)
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

            if (target.LiveComponent == null || !target.LiveComponent.IsAlive)
                return;

            Explode();
        }

        public void Explode()
        {
            if (_isExploded)
                return;

            _isExploded = true;

            DealExplosionDamage();

            if (_chainReactionEnabled)
            {
                TriggerNearbyMines();
            }

            Destroy(gameObject);
        }

        private void DealExplosionDamage()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius);
            HashSet<Character> damagedTargets = new HashSet<Character>();

            for (int i = 0; i < hits.Length; i++)
            {
                Character target = hits[i].GetComponent<Character>();

                if (target == null)
                {
                    target = hits[i].GetComponentInParent<Character>();
                }

                if (target == null)
                    continue;

                if (_owner == null)
                    continue;

                if (target == _owner)
                    continue;

                if (target.CharacterType == _owner.CharacterType)
                    continue;

                if (target.LiveComponent == null || !target.LiveComponent.IsAlive)
                    continue;

                if (damagedTargets.Contains(target))
                    continue;

                damagedTargets.Add(target);
                target.LiveComponent.GetDamage(_damage);
            }
        }

        private void TriggerNearbyMines()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _chainReactionRadius);
            HashSet<MineObject> minesToTrigger = new HashSet<MineObject>();

            for (int i = 0; i < hits.Length; i++)
            {
                MineObject mine = hits[i].GetComponent<MineObject>();

                if (mine == null)
                {
                    mine = hits[i].GetComponentInParent<MineObject>();
                }

                if (mine == null)
                    continue;

                if (mine == this)
                    continue;

                if (mine._isExploded)
                    continue;

                minesToTrigger.Add(mine);
            }

            foreach (MineObject mine in minesToTrigger)
            {
                mine.StartChainReaction(_chainReactionDelay);
            }
        }

        public void StartChainReaction(float delay)
        {
            if (_isExploded)
                return;

            StartCoroutine(ChainReactionRoutine(delay));
        }

        private IEnumerator ChainReactionRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_isExploded)
                yield break;

            Explode();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius > 0f ? _explosionRadius : 2f);

            if (_chainReactionEnabled)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, _chainReactionRadius);
            }
        }
    }
}