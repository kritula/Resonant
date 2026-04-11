using UnityEngine;

namespace OmniumLessons
{
    public class ProjectileAttackComponent : IAttackComponent
    {
        private CharacterData _characterData;
        private float _attackTimer;

        public void Initialize(CharacterData characterData)
        {
            _characterData = characterData;
        }

        public void MakeDamage(Character target)
        {
            if (_characterData == null)
                return;

            if (_characterData.WeaponData == null)
                return;

            if (target == null || target.LiveComponent == null || !target.LiveComponent.IsAlive)
                return;

            PlayerCharacter owner = _characterData.GetComponent<PlayerCharacter>();

            if (owner == null)
                return;

            AttackShotData shotData = owner.AttackModifierController.BuildShotData();

            float finalCooldown = _characterData.WeaponData.AttackCooldown * shotData.AttackCooldownMultiplier;

            if (_attackTimer > 0f)
                return;

            SpawnProjectiles(owner, target, shotData);
            _attackTimer = finalCooldown;
        }

        public void OnUpdate()
        {
            if (_attackTimer > 0f)
            {
                _attackTimer -= Time.deltaTime;
            }
        }

        private void SpawnProjectiles(PlayerCharacter owner, Character target, AttackShotData shotData)
        {
            PlayerWeaponData weaponData = _characterData.WeaponData as PlayerWeaponData;

            if (weaponData == null || weaponData.ProjectilePrefab == null)
                return;

            Vector3 baseDirection = (target.transform.position - owner.transform.position).normalized;
            baseDirection.y = 0f;

            if (baseDirection.sqrMagnitude <= 0.001f)
                return;

            int projectileCount = Mathf.Max(1, shotData.ProjectileCount);

            for (int i = 0; i < projectileCount; i++)
            {
                float angleOffset = CalculateSpreadOffset(i, projectileCount, shotData.SpreadAngle);
                Vector3 shotDirection = Quaternion.Euler(0f, angleOffset, 0f) * baseDirection;

                Vector3 spawnPosition = owner.transform.position + Vector3.up * weaponData.ProjectileSpawnHeight;

                BaseAttackProjectile projectile = Object.Instantiate(
                    weaponData.ProjectilePrefab,
                    spawnPosition,
                    Quaternion.LookRotation(shotDirection, Vector3.up));

                projectile.Initialize(owner, shotDirection, shotData);
            }
        }

        private float CalculateSpreadOffset(int index, int totalCount, float spreadAngle)
        {
            if (totalCount <= 1)
                return 0f;

            float halfSpread = spreadAngle * 0.5f;
            float step = spreadAngle / (totalCount - 1);

            return -halfSpread + step * index;
        }
    }
}