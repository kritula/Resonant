using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OmniumLessons
{
    [Serializable]
    public class EnemySpawnSettings
    {
        public CharacterType CharacterType;
        public float SpawnStartTime;
    }

    public class CharacterSpawnController
    {
        private CharacterFactory CharacterFactory => GameManager.Instance.CharacterFactory;
        private GameData GameData => GameManager.Instance.GameData;

        private float _spawnTimerEnemy;

        private int _baseMaxEnemies = 2;
        private int _enemiesAddedPerStep = 1;
        private float _enemyGrowthIntervalSeconds = 10f;

        private bool _isActiveSpawn;

        private readonly List<EnemySpawnSettings> _enemySpawnSettings = new List<EnemySpawnSettings>
        {
            new EnemySpawnSettings { CharacterType = CharacterType.DefaultEnemy, SpawnStartTime = 0f },
            new EnemySpawnSettings { CharacterType = CharacterType.FastEnemy, SpawnStartTime = 15f },
            new EnemySpawnSettings { CharacterType = CharacterType.TankEnemy, SpawnStartTime = 30f }
        };

        public void StartSpawn()
        {
            _isActiveSpawn = true;
            _spawnTimerEnemy = 0f;
        }

        public void StopSpawn()
        {
            _isActiveSpawn = false;
        }

        public void OnUpdate(float deltaTime)
        {
            if (!_isActiveSpawn)
                return;

            _spawnTimerEnemy += deltaTime;

            float gameTime = GameManager.Instance.GameTime;
            int growthSteps = (int)(gameTime / _enemyGrowthIntervalSeconds);
            int maxEnemiesNow = _baseMaxEnemies + growthSteps * _enemiesAddedPerStep;

            int currentEnemies = CountActiveEnemies();
            if (currentEnemies >= maxEnemiesNow)
                return;

            if (_spawnTimerEnemy >= GameData.TimeBetweenEnemySpawn)
            {
                SpawnEnemy();
                _spawnTimerEnemy = 0f;
            }
        }

        private int CountActiveEnemies()
        {
            int count = 0;
            List<Character> activeCharacters = CharacterFactory.ActiveCharacters;

            for (int i = 0; i < activeCharacters.Count; i++)
            {
                if (activeCharacters[i].CharacterType != CharacterType.DefaultPlayer &&
                    activeCharacters[i].LiveComponent.IsAlive)
                {
                    count++;
                }
            }

            return count;
        }

        private void SpawnEnemy()
        {
            if (CharacterFactory.Player == null)
                return;

            CharacterType enemyType = GetEnemyTypeForSpawn();
            Character enemy = CharacterFactory.CreateCharacter(enemyType);

            Vector3 spawnPoint = GetSpawnPointOutsideScreen();
            enemy.transform.position = spawnPoint;

            GameManager.Instance.RegisterCharacter(enemy);
            enemy.gameObject.SetActive(true);
        }

        private CharacterType GetEnemyTypeForSpawn()
        {
            float gameTime = GameManager.Instance.GameTime;
            List<CharacterType> availableEnemies = new List<CharacterType>();

            for (int i = 0; i < _enemySpawnSettings.Count; i++)
            {
                if (gameTime >= _enemySpawnSettings[i].SpawnStartTime)
                {
                    availableEnemies.Add(_enemySpawnSettings[i].CharacterType);
                }
            }

            int randomIndex = Random.Range(0, availableEnemies.Count);
            return availableEnemies[randomIndex];
        }

        private Vector3 GetSpawnPointOutsideScreen()
        {
            Camera mainCamera = Camera.main;

            if (mainCamera == null || CharacterFactory.Player == null)
                return Vector3.zero;

            Transform playerTransform = CharacterFactory.Player.transform;

            float halfHeight = mainCamera.orthographicSize;
            float halfWidth = halfHeight * mainCamera.aspect;

            // ВАЖНО: берём центр не от игрока, а от камеры
            Vector3 cameraPosition = mainCamera.transform.position;

            float minX = cameraPosition.x - halfWidth;
            float maxX = cameraPosition.x + halfWidth;
            float minZ = cameraPosition.z - halfHeight;
            float maxZ = cameraPosition.z + halfHeight;

            float extraOffset = Mathf.Max(GameData.MaxEnemySpawnOffset, 5f);
            float minDistanceFromPlayer = halfHeight + 5f;

            for (int attempt = 0; attempt < 20; attempt++)
            {
                int side = Random.Range(0, 4);
                Vector3 spawnPoint;

                switch (side)
                {
                    case 0: // left
                        spawnPoint = new Vector3(
                            minX - extraOffset,
                            playerTransform.position.y,
                            Random.Range(minZ, maxZ));
                        break;

                    case 1: // right
                        spawnPoint = new Vector3(
                            maxX + extraOffset,
                            playerTransform.position.y,
                            Random.Range(minZ, maxZ));
                        break;

                    case 2: // bottom
                        spawnPoint = new Vector3(
                            Random.Range(minX, maxX),
                            playerTransform.position.y,
                            minZ - extraOffset);
                        break;

                    default: // top
                        spawnPoint = new Vector3(
                            Random.Range(minX, maxX),
                            playerTransform.position.y,
                            maxZ + extraOffset);
                        break;
                }

                // Проверка 1: не слишком близко к игроку
                float distanceToPlayer = Vector3.Distance(
                    new Vector3(spawnPoint.x, 0f, spawnPoint.z),
                    new Vector3(playerTransform.position.x, 0f, playerTransform.position.z));

                if (distanceToPlayer < minDistanceFromPlayer)
                    continue;

                // Проверка 2: действительно ли точка вне экрана
                Vector3 viewportPoint = mainCamera.WorldToViewportPoint(spawnPoint);

                bool isOutsideScreen =
                    viewportPoint.x < 0f || viewportPoint.x > 1f ||
                    viewportPoint.y < 0f || viewportPoint.y > 1f ||
                    viewportPoint.z < 0f;

                if (!isOutsideScreen)
                    continue;

                return spawnPoint;
            }

            // запасной вариант — далеко по кругу от игрока
            Vector2 fallbackDirection = Random.insideUnitCircle.normalized;
            if (fallbackDirection == Vector2.zero)
                fallbackDirection = Vector2.right;

            float fallbackDistance = halfHeight + extraOffset + 5f;

            return new Vector3(
                playerTransform.position.x + fallbackDirection.x * fallbackDistance,
                playerTransform.position.y,
                playerTransform.position.z + fallbackDirection.y * fallbackDistance);
        }
    }
}