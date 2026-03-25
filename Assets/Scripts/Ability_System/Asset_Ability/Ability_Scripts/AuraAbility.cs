using UnityEngine;

namespace OmniumLessons
{
    public class AuraAbility : AbilityBehaviour
    {
        [SerializeField] private float _radius = 3f;
        [SerializeField] private int _damage = 2;
        [SerializeField] private float _damageInterval = 1f;

        private float _damageTimer;

        public override void Initialize(Character owner, AbilityData data)
        {
            base.Initialize(owner, data);

            if (_owner == null)
            {
                Debug.LogError("AuraAbility: owner is null");
                return;
            }

            _damageTimer = 0f;
            transform.position = _owner.transform.position;
        }

        public override void OnUpdate()
        {
            if (_owner == null)
                return;

            transform.position = _owner.transform.position;

            _damageTimer += Time.deltaTime;

            if (_damageTimer >= _damageInterval)
            {
                DealDamageToEnemies();
                _damageTimer = 0f;
            }
        }

        private void DealDamageToEnemies()
        {
            Collider[] hits = Physics.OverlapSphere(_owner.transform.position, _radius);

            for (int i = 0; i < hits.Length; i++)
            {
                Character target = hits[i].GetComponent<Character>();

                if (target == null)
                    continue;

                if (target == _owner)
                    continue;

                if (!target.LiveComponent.IsAlive)
                    continue;

                if (target.CharacterType == CharacterType.DefaultPlayer)
                    continue;

                target.LiveComponent.GetDamage(_damage);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}