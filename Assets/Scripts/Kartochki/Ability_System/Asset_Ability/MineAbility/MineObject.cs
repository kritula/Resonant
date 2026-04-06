using UnityEngine;

namespace OmniumLessons
{
    public class MineObject : MonoBehaviour
    {
        [Header("Mine settings")]
        [SerializeField] private float _damage = 15f;
        [SerializeField] private float _explosionRadius = 2.5f;
        [SerializeField] private float _lifeTime = 15f;
        [SerializeField] private float _armDelay = 0.2f;

        private PlayerCharacter _owner;
        private MineAbility _mineAbility;
        private float _armTimer;
        private bool _isArmed;
        private bool _isDestroyed;

        public void Initialize(PlayerCharacter owner, MineAbility mineAbility)
        {
            _owner = owner;
            _mineAbility = mineAbility;
            _armTimer = _armDelay;
            _isArmed = false;
            _isDestroyed = false;

            Destroy(gameObject, _lifeTime);
        }

        private void Update()
        {
            if (_isArmed)
                return;

            if (_armTimer > 0f)
            {
                _armTimer -= Time.deltaTime;
                return;
            }

            _isArmed = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isArmed)
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

            Explode();
        }

        private void Explode()
        {
            if (_isDestroyed)
                return;

            _isDestroyed = true;

            Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius);

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

                if (!target.LiveComponent.IsAlive)
                    continue;

                target.LiveComponent.GetDamage(_damage);
            }

            if (_mineAbility != null)
            {
                _mineAbility.RemoveMine(this);
            }

            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_mineAbility != null)
            {
                _mineAbility.RemoveMine(this);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
    }
}