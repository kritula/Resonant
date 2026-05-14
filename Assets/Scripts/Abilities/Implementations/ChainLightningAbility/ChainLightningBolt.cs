using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class ChainLightningBolt : MonoBehaviour
    {
        private PlayerCharacter _owner;
        private Character _target;

        private float _damage;
        private float _travelDuration;
        private int _remainingTargets;
        private float _jumpRange;
        private float _jumpDamageMultiplier;

        private bool _applySlow;
        private float _slowMultiplier;
        private float _slowDuration;

        private HashSet<Character> _visitedTargets;

        private Vector3 _startPosition;
        private float _moveTimer;
        private bool _isInitialized;

        public void Initialize(
            PlayerCharacter owner,
            Character target,
            float damage,
            float travelDuration,
            int remainingTargets,
            float jumpRange,
            float jumpDamageMultiplier,
            bool applySlow,
            float slowMultiplier,
            float slowDuration,
            HashSet<Character> visitedTargets)
        {
            _owner = owner;
            _target = target;
            _damage = damage;
            _travelDuration = Mathf.Max(0.01f, travelDuration);
            _remainingTargets = remainingTargets;
            _jumpRange = jumpRange;
            _jumpDamageMultiplier = jumpDamageMultiplier;
            _applySlow = applySlow;
            _slowMultiplier = slowMultiplier;
            _slowDuration = slowDuration;
            _visitedTargets = visitedTargets ?? new HashSet<Character>();

            _startPosition = transform.position;
            _moveTimer = 0f;
            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            if (_target == null || _target.LiveComponent == null || !_target.LiveComponent.IsAlive)
            {
                Destroy(gameObject);
                return;
            }

            _moveTimer += Time.deltaTime;

            float t = Mathf.Clamp01(_moveTimer / _travelDuration);
            transform.position = Vector3.Lerp(_startPosition, _target.transform.position, t);

            if (t >= 1f)
            {
                HitTarget();
            }
        }

        private void HitTarget()
        {
            if (_target == null)
            {
                Destroy(gameObject);
                return;
            }

            if (_target.LiveComponent != null && _target.LiveComponent.IsAlive)
            {
                _target.LiveComponent.GetDamage(_damage);

                if (_applySlow && _target.StatusEffectController != null)
                {
                    if (!_target.StatusEffectController.HasEffect<SlowEffect>())
                    {
                        _target.StatusEffectController.AddEffect(new SlowEffect(_slowMultiplier, _slowDuration));
                    }
                }
            }

            _visitedTargets.Add(_target);
            _remainingTargets--;

            if (_remainingTargets > 0)
            {
                Character nextTarget = FindNearestNextTarget(_target.transform.position, _jumpRange, _visitedTargets);

                if (nextTarget != null)
                {
                    ChainLightningBolt nextBolt = Object.Instantiate(
                        this,
                        _target.transform.position,
                        Quaternion.identity);

                    nextBolt.Initialize(
                        _owner,
                        nextTarget,
                        _damage * _jumpDamageMultiplier,
                        _travelDuration,
                        _remainingTargets,
                        _jumpRange,
                        _jumpDamageMultiplier,
                        _applySlow,
                        _slowMultiplier,
                        _slowDuration,
                        _visitedTargets);
                }
            }

            Destroy(gameObject);
        }

        private Character FindNearestNextTarget(Vector3 origin, float maxDistance, HashSet<Character> excludedTargets)
        {
            if (GameManager.Instance == null || GameManager.Instance.CharacterFactory == null)
                return null;

            List<Character> activeCharacters = GameManager.Instance.CharacterFactory.ActiveCharacters;

            Character nearestTarget = null;
            float nearestDistanceSqr = maxDistance * maxDistance;

            for (int i = 0; i < activeCharacters.Count; i++)
            {
                Character candidate = activeCharacters[i];

                if (candidate == null)
                    continue;

                if (candidate == _owner)
                    continue;

                if (candidate.CharacterType == _owner.CharacterType)
                    continue;

                if (candidate.LiveComponent == null || !candidate.LiveComponent.IsAlive)
                    continue;

                if (excludedTargets != null && excludedTargets.Contains(candidate))
                    continue;

                float distanceSqr = (candidate.transform.position - origin).sqrMagnitude;

                if (distanceSqr > nearestDistanceSqr)
                    continue;

                nearestDistanceSqr = distanceSqr;
                nearestTarget = candidate;
            }

            return nearestTarget;
        }
    }
}