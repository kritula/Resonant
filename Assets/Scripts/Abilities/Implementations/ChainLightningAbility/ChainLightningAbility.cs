using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class ChainLightningAbility : AbilityBehaviour
    {
        [SerializeField] private ChainLightningAbilityData _data;

        private float _cooldown;
        private float _damage;
        private int _jumpCount;
        private float _castRange;
        private float _jumpRange;
        private float _travelDuration;
        private float _jumpDamageMultiplier;

        private float _doubleDischargeChance;

        private bool _applySlow;
        private float _slowMultiplier;
        private float _slowDuration;

        private float _cooldownTimer;

        public override void Initialize(PlayerCharacter owner, AbilityUpgradeData abilityData)
        {
            base.Initialize(owner, abilityData);
            _cooldownTimer = 0f;
        }

        protected override void ApplyLevel(int level)
        {
            if (_data == null)
            {
                Debug.LogError($"{nameof(ChainLightningAbility)}: Data is missing.", this);
                return;
            }

            _cooldown = _data.BaseCooldown;
            _damage = _data.BaseDamage;
            _jumpCount = _data.BaseTargetCount;
            _castRange = _data.CastRange;
            _jumpRange = _data.JumpRange;
            _travelDuration = _data.TravelDuration;
            _jumpDamageMultiplier = _data.JumpDamageMultiplier;

            _doubleDischargeChance = 0f;

            _applySlow = false;
            _slowMultiplier = 1f;
            _slowDuration = 0f;

            if (level >= 2)
            {
                _jumpCount = _data.Level2TargetCount;
            }

            if (level >= 3)
            {
                _jumpDamageMultiplier = 1f;
            }

            if (level >= 4)
            {
                _doubleDischargeChance = _data.DoubleDischargeChance;
            }

            if (level >= 5)
            {
                _jumpCount = _data.Level5TargetCount;
                _applySlow = true;
                _slowMultiplier = _data.SlowMultiplier;
                _slowDuration = _data.SlowDuration;
            }
        }

        public override void OnUpdate()
        {
            if (_owner == null || _data == null)
                return;

            if (_data.BoltPrefab == null)
                return;

            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
                return;
            }

            Character firstTarget = FindNearestEnemy(_owner.transform.position, _castRange, null);

            if (firstTarget == null)
                return;

            CastChain(firstTarget);

            if (_doubleDischargeChance > 0f && Random.value <= _doubleDischargeChance)
            {
                HashSet<Character> exclude = new HashSet<Character> { firstTarget };
                Character secondTarget = FindNearestEnemy(_owner.transform.position, _castRange, exclude);

                if (secondTarget != null)
                {
                    CastChain(secondTarget);
                }
                else
                {
                    CastChain(firstTarget);
                }
            }

            _cooldownTimer = _cooldown;
        }

        private void CastChain(Character startTarget)
        {
            if (startTarget == null)
                return;

            HashSet<Character> visitedTargets = new HashSet<Character>();

            ChainLightningBolt bolt = Object.Instantiate(
                _data.BoltPrefab,
                _owner.transform.position,
                Quaternion.identity);

            bolt.Initialize(
                _owner,
                startTarget,
                _damage,
                _travelDuration,
                _jumpCount,
                _jumpRange,
                _jumpDamageMultiplier,
                _applySlow,
                _slowMultiplier,
                _slowDuration,
                visitedTargets);
        }

        private Character FindNearestEnemy(Vector3 origin, float maxDistance, HashSet<Character> excludedTargets)
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