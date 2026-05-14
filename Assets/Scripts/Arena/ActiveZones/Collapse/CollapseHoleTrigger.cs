using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace OmniumLessons
{
    public class CollapseHoleTrigger : MonoBehaviour
    {
        [Header("Damage")]
        [FormerlySerializedAs("_damagePercentFromMaxHealth")]
        [SerializeField] private float _damagePercentFromCurrentHealth = 0.5f;

        [Header("Push")]
        [SerializeField] private float _pushDistance = 4f;

        [Header("Damage Cooldown")]
        [SerializeField] private float _damageCooldown = 1.5f;

        [Header("Invulnerability")]
        [SerializeField] private float _invulnerabilityTime = 1.5f;

        private Collider _collider;
        private bool _isActive;

        private readonly Dictionary<PlayerCharacter, float>
            _damageCooldowns = new();

        private void Awake()
        {
            CacheCollider();
            SetActiveState(false);
        }

        private void Update()
        {
            UpdateCooldowns();
        }

        public void SetActiveState(bool value)
        {
            CacheCollider();

            _isActive = value;

            if (_collider != null)
                _collider.enabled = value;

            if (!value)
                _damageCooldowns.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            TryDamagePlayer(other);
            TryKillEnemy(other);
        }

        private void OnTriggerStay(Collider other)
        {
            TryDamagePlayer(other);
            TryKillEnemy(other);
        }

        private void TryDamagePlayer(Collider other)
        {
            PlayerCharacter player =
                other.GetComponent<PlayerCharacter>();

            if (!_isActive)
                return;

            if (player == null)
            {
                player =
                    other.GetComponentInParent<PlayerCharacter>();
            }

            if (player == null)
                return;

            if (player.LiveComponent == null)
                return;

            if (!player.LiveComponent.IsAlive)
                return;

            if (IsPlayerOnCooldown(player))
                return;

            DamageAndPushPlayer(player);
        }

        private void DamageAndPushPlayer(PlayerCharacter player)
        {
            bool effectApplied =
                CollapseHolePlayerEffect.TryApply(
                    player,
                    transform,
                    _damagePercentFromCurrentHealth,
                    _invulnerabilityTime,
                    _pushDistance);

            if (effectApplied)
                _damageCooldowns[player] = _damageCooldown;
        }

        private bool IsPlayerOnCooldown(
            PlayerCharacter player)
        {
            if (player == null)
                return true;

            if (_damageCooldowns.TryGetValue(
                    player,
                    out float cooldown))
            {
                return cooldown > 0f;
            }

            return false;
        }

        private void UpdateCooldowns()
        {
            if (_damageCooldowns.Count == 0)
                return;

            List<PlayerCharacter> keys =
                new List<PlayerCharacter>(_damageCooldowns.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                PlayerCharacter player = keys[i];

                if (player == null)
                {
                    _damageCooldowns.Remove(player);
                    continue;
                }

                _damageCooldowns[player] -= Time.deltaTime;

                if (_damageCooldowns[player] <= 0f)
                {
                    _damageCooldowns.Remove(player);
                }
            }
        }

        private void CacheCollider()
        {
            if (_collider == null)
                _collider = GetComponent<Collider>();

            if (_collider != null)
                _collider.isTrigger = true;
        }

        private void TryKillEnemy(Collider other)
        {
            if (!_isActive)
                return;

            Character character = other.GetComponent<Character>();

            if (character == null)
                character = other.GetComponentInParent<Character>();

            if (character == null)
                return;

            if (character.CharacterType == CharacterType.DefaultPlayer)
                return;

            if (character.LiveComponent == null ||
                !character.LiveComponent.IsAlive)
            {
                return;
            }

            if (character.CharacterData == null)
                return;

            character.LiveComponent.GetDamage(character.CharacterData.MaxHealth);
        }
    }
}
