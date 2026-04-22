using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OmniumLessons
{
    public class BossSpawnAbility
    {
        private readonly BossCharacter _boss;
        private readonly BossSpawnAbilityData _data;

        private float _timer;

        public BossSpawnAbility(BossCharacter boss, BossSpawnAbilityData data)
        {
            _boss = boss;
            _data = data;
            _timer = _data != null ? _data.Cooldown : 0f;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_boss == null || _data == null)
                return;

            if (_boss.LiveComponent == null || !_boss.LiveComponent.IsAlive)
                return;

            if (_data.Enemies == null || _data.Enemies.Count == 0)
                return;

            _timer -= deltaTime;

            if (_timer > 0f)
                return;

            Spawn();
            _timer = _data.Cooldown;
        }

        private void Spawn()
        {
            CharacterFactory characterFactory = GameManager.Instance?.CharacterFactory;

            if (characterFactory == null)
                return;

            for (int i = 0; i < _data.SpawnCount; i++)
            {
                CharacterType spawnType = GetRandomCharacterTypeByWeight();

                if (spawnType == CharacterType.None ||
                    spawnType == CharacterType.DefaultPlayer ||
                    spawnType == CharacterType.Boss_Null_Core)
                {
                    continue;
                }

                Character spawnedCharacter = characterFactory.CreateCharacter(spawnType);

                if (spawnedCharacter == null)
                    continue;

                Vector3 spawnPosition = GetSpawnPositionAroundBoss();
                PlaceCharacter(spawnedCharacter, spawnPosition);

                GameManager.Instance.RegisterCharacter(spawnedCharacter);
                spawnedCharacter.gameObject.SetActive(true);
            }
        }

        private CharacterType GetRandomCharacterTypeByWeight()
        {
            List<BossSpawnEntry> enemies = _data.Enemies;

            if (enemies == null || enemies.Count == 0)
                return CharacterType.None;

            int totalWeight = 0;

            for (int i = 0; i < enemies.Count; i++)
            {
                BossSpawnEntry entry = enemies[i];

                if (entry == null)
                    continue;

                totalWeight += Mathf.Max(0, entry.Weight);
            }

            if (totalWeight <= 0)
                return CharacterType.None;

            int randomValue = Random.Range(0, totalWeight);
            int currentWeight = 0;

            for (int i = 0; i < enemies.Count; i++)
            {
                BossSpawnEntry entry = enemies[i];

                if (entry == null)
                    continue;

                currentWeight += Mathf.Max(0, entry.Weight);

                if (randomValue < currentWeight)
                    return entry.Type;
            }

            return enemies[enemies.Count - 1] != null
                ? enemies[enemies.Count - 1].Type
                : CharacterType.None;
        }

        private Vector3 GetSpawnPositionAroundBoss()
        {
            Vector3 bossPosition = _boss.transform.position;

            float minDistance = Mathf.Max(0.5f, _data.MinSpawnDistance);
            float maxDistance = Mathf.Max(minDistance, _data.MaxSpawnDistance);

            Vector2 randomCircle = Random.insideUnitCircle.normalized;

            if (randomCircle.sqrMagnitude <= 0.001f)
                randomCircle = Vector2.right;

            float distance = Random.Range(minDistance, maxDistance);

            Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y) * distance;

            Vector3 spawnPosition = bossPosition + offset;
            spawnPosition.y = _data.SpawnY;

            return spawnPosition;
        }

        private void PlaceCharacter(Character character, Vector3 position)
        {
            if (character == null)
                return;

            CharacterController characterController = character.CharacterData != null
                ? character.CharacterData.CharacterController
                : null;

            if (characterController != null)
                characterController.enabled = false;

            character.transform.position = position;

            if (character.CharacterData != null && character.CharacterData.CharacterTransform != null)
            {
                character.CharacterData.CharacterTransform.position = position;
            }

            if (characterController != null)
                characterController.enabled = true;
        }
    }
}