using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class BaseAttackProjectile : MonoBehaviour
    {
        private PlayerCharacter _owner;
        private Vector3 _direction;
        private AttackShotData _shotData;

        private float _baseDamage;
        private float _speed;
        private float _lifeTime;

        private int _piercedTargetsCount;
        private int _remainingRicochets;

        private bool _isRicochetBranch;
        [SerializeField] private RicochetArcEffect _ricochetArcEffectPrefab;

        private Character _homingTarget;
        private bool _isHoming;

        private readonly HashSet<Character> _damagedTargets = new HashSet<Character>();
        private bool _isInitialized;

        public void Initialize(PlayerCharacter owner, Vector3 direction, AttackShotData shotData)
        {
            _owner = owner;
            _direction = direction.normalized;
            _shotData = shotData;

            PlayerWeaponData weaponData = owner.CharacterData.WeaponData as PlayerWeaponData;

            if (weaponData == null)
                return;

            _baseDamage = weaponData.Damage;
            _speed = weaponData.ProjectileSpeed;
            _lifeTime = weaponData.ProjectileLifeTime;

            _remainingRicochets = shotData.RicochetCount;
            _piercedTargetsCount = 0;

            _isRicochetBranch = false;

            _isHoming = false;
            _homingTarget = null;

            _damagedTargets.Clear();
            _isInitialized = true;

            Destroy(gameObject, _lifeTime);
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            UpdateDirection();

            transform.position += _direction * _speed * Time.deltaTime;

            if (_direction.sqrMagnitude > 0.001f)
            {
                transform.forward = _direction;
            }
        }

        private void UpdateDirection()
        {
            if (!_isHoming)
                return;

            if (_homingTarget == null || !_homingTarget.LiveComponent.IsAlive)
                return;

            Vector3 dir = _homingTarget.transform.position - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
            {
                _direction = dir.normalized;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Character target = other.GetComponent<Character>();

            if (target == null)
                target = other.GetComponentInParent<Character>();

            if (target == null)
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

            float damage = CalculateDamage();
            target.LiveComponent.GetDamage(damage);

            TrySpawnRicochetProjectile(target);

            if (_isRicochetBranch)
            {
                Destroy(gameObject);
                return;
            }

            if (ShouldDestroyAfterHit())
            {
                Destroy(gameObject);
                return;
            }

            _piercedTargetsCount++;
        }

        private float CalculateDamage()
        {
            float damage = _baseDamage;

            if (_shotData.PierceBonusAfterFirstPierce && _piercedTargetsCount >= 1)
                damage *= _shotData.PierceBonusMultiplierAfterFirstPierce;

            if (_shotData.PierceDamageFalloff && _piercedTargetsCount > 0)
            {
                float multiplier = 1f - (_shotData.PierceDamageFalloffPerTarget * _piercedTargetsCount);
                multiplier = Mathf.Max(0.1f, multiplier);
                damage *= multiplier;
            }

            return damage;
        }

        private void TrySpawnRicochetProjectile(Character hitTarget)
        {
            if (_remainingRicochets <= 0)
                return;

            Character nextTarget = FindRicochetTarget(hitTarget);

            if (nextTarget == null)
                return;

            Vector3 startPosition = hitTarget.transform.position;
            Vector3 directionToTarget = nextTarget.transform.position - startPosition;
            directionToTarget.y = 0f;

            if (directionToTarget.sqrMagnitude <= 0.001f)
                return;

            directionToTarget.Normalize();

            if (_ricochetArcEffectPrefab != null)
            {
                RicochetArcEffect arcEffect = Instantiate(
                    _ricochetArcEffectPrefab,
                    startPosition,
                    Quaternion.identity);

                arcEffect.Initialize(startPosition, nextTarget.transform.position);
            }

            float damageMultiplier = _shotData.RicochetNoDamageFalloff ? 1f : _shotData.RicochetDamageMultiplier;

            BaseAttackProjectile ricochetProjectile = Instantiate(
                this,
                startPosition,
                Quaternion.LookRotation(directionToTarget, Vector3.up));

            ricochetProjectile.InitializeRicochet(
                _owner,
                directionToTarget,
                _shotData,
                _baseDamage,
                _remainingRicochets - 1,
                damageMultiplier,
                _shotData.RicochetHoming,
                nextTarget);
        }

        private void InitializeRicochet(
            PlayerCharacter owner,
            Vector3 direction,
            AttackShotData shotData,
            float baseDamage,
            int remainingRicochets,
            float damageMultiplier,
            bool homing,
            Character target)
        {
            _owner = owner;
            _direction = direction;
            _shotData = shotData;

            PlayerWeaponData weaponData = owner.CharacterData.WeaponData as PlayerWeaponData;

            _baseDamage = baseDamage * damageMultiplier;
            _speed = weaponData.ProjectileSpeed;
            _lifeTime = weaponData.ProjectileLifeTime;

            _remainingRicochets = remainingRicochets;
            _isRicochetBranch = true;

            _isHoming = homing;
            _homingTarget = homing ? target : null;

            _damagedTargets.Clear();
            _isInitialized = true;

            Destroy(gameObject, _lifeTime);
        }

        private Character FindRicochetTarget(Character hitTarget)
        {
            var list = GameManager.Instance.CharacterFactory.ActiveCharacters;

            Character best = null;
            float bestDist = _shotData.RicochetSearchRadius * _shotData.RicochetSearchRadius;

            for (int i = 0; i < list.Count; i++)
            {
                var c = list[i];

                if (c == null || c == hitTarget || c == _owner)
                    continue;

                if (c.CharacterType == _owner.CharacterType)
                    continue;

                if (!c.LiveComponent.IsAlive)
                    continue;

                float dist = (c.transform.position - hitTarget.transform.position).sqrMagnitude;

                if (dist > bestDist)
                    continue;

                bestDist = dist;
                best = c;
            }

            return best;
        }

        private bool ShouldDestroyAfterHit()
        {
            if (_shotData.InfinitePierce)
                return false;

            if (_shotData.PierceCount <= 0)
                return true;

            return _piercedTargetsCount >= _shotData.PierceCount;
        }
    }
}