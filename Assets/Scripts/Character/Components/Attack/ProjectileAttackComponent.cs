using UnityEngine;

namespace OmniumLessons
{
    public class ProjectileAttackComponent : IAttackComponent
    {
        public float Damage { get; private set; }

        private CharacterData _characterData;
        private float _lockDamageTime;
        private float _lockDamageTimeMax;

        public void Initialize(CharacterData characterData)
        {
            _characterData = characterData;

            if (_characterData != null && _characterData.WeaponData != null)
            {
                Damage = _characterData.WeaponData.Damage;
                _lockDamageTimeMax = _characterData.WeaponData.AttackCooldown;
            }
            else
            {
                Damage = 5f;
                _lockDamageTimeMax = 1f;
            }

            _lockDamageTime = 0f;
        }

        public void MakeDamage(Character damageTarget)
        {
            if (_characterData == null)
                return;

            if (_characterData.WeaponData == null)
                return;

            if (_lockDamageTime > 0f)
                return;

            if (damageTarget == null)
                return;

            if (!damageTarget.LiveComponent.IsAlive)
                return;

            PlayerWeaponData playerWeaponData = _characterData.WeaponData as PlayerWeaponData;
            if (playerWeaponData == null)
                return;

            BaseAttackProjectile projectilePrefab = playerWeaponData.ProjectilePrefab;
            if (projectilePrefab == null)
                return;

            Character ownerCharacter = _characterData.Character;
            if (ownerCharacter == null)
                return;

            PlayerCharacter playerCharacter = ownerCharacter as PlayerCharacter;
            if (playerCharacter == null)
                return;

            AttackShotData shotData = playerCharacter.AttackModifierController.BuildShotData(Damage, playerWeaponData);
            if (shotData == null)
                return;

            int projectileCount = 1 + shotData.AdditionalProjectilesCount;

            for (int i = 0; i < projectileCount; i++)
            {
                Vector3 spawnPosition = GetSpawnPosition(
                    i,
                    projectileCount,
                    playerWeaponData.ProjectileSpawnOffset,
                    playerWeaponData.ProjectileSpawnHeight);

                Vector3 direction = damageTarget.transform.position - spawnPosition;
                direction.y = 0f;

                if (direction == Vector3.zero)
                    continue;

                BaseAttackProjectile projectile = Object.Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

                projectile.Initialize(
                    ownerCharacter,
                    shotData.FinalDamage,
                    playerWeaponData.ProjectileSpeed,
                    playerWeaponData.ProjectileLifeTime,
                    direction.normalized,
                    shotData.PiercingCount,
                    shotData.RicochetCount,
                    shotData.RicochetRadius);
            }

            _lockDamageTime = _lockDamageTimeMax * shotData.AttackCooldownMultiplier;
        }

        public void OnUpdate()
        {
            if (_lockDamageTime > 0f)
            {
                _lockDamageTime -= Time.deltaTime;
            }
        }

        private Vector3 GetSpawnPosition(int projectileIndex, int projectileCount, float spawnOffset, float spawnHeight)
        {
            Vector3 basePosition = _characterData.CharacterTransform.position + Vector3.up * spawnHeight;

            if (projectileCount <= 1)
                return basePosition;

            Transform ownerTransform = _characterData.CharacterTransform;
            Vector3 right = ownerTransform.right;

            float centerOffset = (projectileCount - 1) * 0.5f;
            float horizontalOffset = (projectileIndex - centerOffset) * spawnOffset;

            return basePosition + right * horizontalOffset;
        }
    }
}