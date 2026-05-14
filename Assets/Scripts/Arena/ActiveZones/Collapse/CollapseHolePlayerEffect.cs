using UnityEngine;

namespace OmniumLessons
{
    public static class CollapseHolePlayerEffect
    {
        public static bool TryApply(
            PlayerCharacter player,
            Transform origin,
            float damagePercentFromCurrentHealth,
            float invulnerabilityTime,
            float pushDistance)
        {
            if (player == null || origin == null)
                return false;

            if (player.IsInvulnerable)
                return false;

            if (player.LiveComponent == null)
                return false;

            if (!player.LiveComponent.IsAlive)
                return false;

            float damage =
                player.LiveComponent.Health *
                damagePercentFromCurrentHealth;

            player.LiveComponent.GetDamage(damage);

            if (!player.LiveComponent.IsAlive)
                return false;

            player.EnableTemporaryInvulnerability(invulnerabilityTime);
            PushPlayer(player, origin, pushDistance);

            return true;
        }

        private static void PushPlayer(
            PlayerCharacter player,
            Transform origin,
            float pushDistance)
        {
            if (player.CharacterData == null)
                return;

            CharacterController controller =
                player.CharacterData.CharacterController;

            if (controller == null || !controller.enabled)
                return;

            Vector3 direction =
                player.transform.position - origin.position;

            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
                direction = Vector3.back;

            direction.Normalize();
            controller.Move(direction * pushDistance);
        }
    }
}
