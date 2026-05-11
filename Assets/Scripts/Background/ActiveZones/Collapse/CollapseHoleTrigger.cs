using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class CollapseHoleTrigger : MonoBehaviour
    {
        [Header("Damage")]
        [SerializeField] private float _damagePercentFromMaxHealth = 0.5f;

        [Header("Push")]
        [SerializeField] private float _pushDistance = 4f;

        [Header("Damage Cooldown")]
        [SerializeField] private float _damageCooldown = 1.5f;

        [Header("Invulnerability")]
        [SerializeField] private float _invulnerabilityTime = 1.5f;

        private Collider _collider;

        private readonly Dictionary<PlayerCharacter, float>
            _damageCooldowns = new();

        private void Awake()
        {
            _collider = GetComponent<Collider>();

            if (_collider != null)
                _collider.isTrigger = true;
        }

        private void Update()
        {
            UpdateCooldowns();
        }

        public void SetActiveState(bool value)
        {
            if (_collider == null)
                _collider = GetComponent<Collider>();

            if (_collider != null)
                _collider.enabled = value;

            if (!value)
                _damageCooldowns.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerCharacter player =
                other.GetComponent<PlayerCharacter>();

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
            if (player == null)
                return;

            if (player.IsInvulnerable)
                return;

            if (player.LiveComponent == null)
                return;

            if (!player.LiveComponent.IsAlive)
                return;

            float damage =
                player.LiveComponent.MaxHealth *
                _damagePercentFromMaxHealth;

            player.LiveComponent.GetDamage(damage);

            if (!player.LiveComponent.IsAlive)
                return;

            player.EnableTemporaryInvulnerability(
                _invulnerabilityTime);

            PushPlayer(player);

            _damageCooldowns[player] = _damageCooldown;
        }

        private void PushPlayer(PlayerCharacter player)
        {
            if (player == null)
                return;

            if (player.CharacterData == null)
                return;

            CharacterController controller =
                player.CharacterData.CharacterController;

            if (controller == null)
                return;

            if (!controller.enabled)
                return;

            Vector3 direction =
                player.transform.position - transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
                direction = Vector3.back;

            direction.Normalize();

            controller.Move(direction * _pushDistance);
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
    }
}