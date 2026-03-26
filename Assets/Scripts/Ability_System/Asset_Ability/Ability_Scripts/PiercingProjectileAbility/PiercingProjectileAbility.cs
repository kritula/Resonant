using UnityEngine;

namespace OmniumLessons
{
    public class PiercingProjectileAbility : AbilityBehaviour
    {
        private PiercingProjectileData _piercingProjectileData;
        private float _cooldownTimer;

        public override void Initialize(Character owner, AbilityData data)
        {
            base.Initialize(owner, data);

            _piercingProjectileData = data as PiercingProjectileData;

            if (_piercingProjectileData == null)
            {
                Debug.LogError("PiercingProjectileAbility: data is not PiercingProjectileData");
                return;
            }

            _cooldownTimer = 0f;
        }

        public override void OnUpdate()
        {
            if (_owner == null || _piercingProjectileData == null)
            {
                return;
            }

            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
                return;
            }

            Character target = FindNearestEnemy();

            if (target == null)
            {
                return;
            }

            SpawnProjectile(target);
            _cooldownTimer = _piercingProjectileData.Cooldown;
        }

        private Character FindNearestEnemy()
        {
            if (GameManager.Instance == null || GameManager.Instance.CharacterFactory == null)
            {
                return null;
            }

            Character nearestEnemy = null;
            float minDistance = float.MaxValue;

            foreach (Character character in GameManager.Instance.CharacterFactory.ActiveCharacters)
            {
                if (character == null)
                {
                    continue;
                }

                if (character == _owner)
                {
                    continue;
                }

                if (character.CharacterType == CharacterType.DefaultPlayer)
                {
                    continue;
                }

                if (character.LiveComponent == null || character.LiveComponent.IsAlive == false)
                {
                    continue;
                }

                float distance = Vector3.Distance(_owner.transform.position, character.transform.position);

                if (distance > _piercingProjectileData.SearchRadius)
                {
                    continue;
                }

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = character;
                }
            }

            return nearestEnemy;
        }

        private void SpawnProjectile(Character target)
        {
            if (_piercingProjectileData.ProjectilePrefab == null)
            {
                Debug.LogError("PiercingProjectileAbility: ProjectilePrefab is null");
                return;
            }

            Vector3 spawnPosition = _owner.transform.position + Vector3.up * 1f;
            Vector3 direction = (target.transform.position - spawnPosition).normalized;

            GameObject projectileObject = Instantiate(
                _piercingProjectileData.ProjectilePrefab,
                spawnPosition,
                Quaternion.LookRotation(direction)
            );

            PiercingProjectile projectile = projectileObject.GetComponent<PiercingProjectile>();

            if (projectile == null)
            {
                Debug.LogError("PiercingProjectileAbility: prefab does not contain PiercingProjectile");
                Destroy(projectileObject);
                return;
            }

            projectile.Initialize(_owner, direction, _piercingProjectileData);
        }
    }
}