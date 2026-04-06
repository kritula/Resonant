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

        private List<EnemySpawnSettings> _enemySpawnSettings = new List<EnemySpawnSettings>
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
            var activeCharacters = CharacterFactory.ActiveCharacters;

            for (int i = 0; i < activeCharacters.Count; i++)
            {
                if (activeCharacters[i].CharacterType != CharacterType.DefaultPlayer &&
                    activeCharacters[i].LiveComponent != null &&
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

            Vector3 spawnPoint = GetSpawnPointOutsideCamera();

            CharacterController controller = enemy.CharacterData.CharacterController;
            if (controller != null)
                controller.enabled = false;

            enemy.transform.position = spawnPoint;

            if (enemy.CharacterData != null && enemy.CharacterData.CharacterTransform != null)
                enemy.CharacterData.CharacterTransform.position = spawnPoint;

            if (controller != null)
                controller.enabled = true;

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

        private Vector3 GetSpawnPointOutsideCamera()
        {
            Camera camera = Camera.main;

            if (camera == null)
            {
                Debug.LogWarning("CharacterSpawnController: Camera.main not found.");
                return Vector3.zero;
            }

            GetCameraBounds(camera, out float minX, out float maxX, out float minZ, out float maxZ);

            float offsetMin = GameData.MinEnemySpawnOffset;
            float offsetMax = GameData.MaxEnemySpawnOffset;

            float spawnY = CharacterFactory.Player.transform.position.y;

            int side = Random.Range(0, 4);

            switch (side)
            {
                case 0: // left
                    return new Vector3(
                        minX - Random.Range(offsetMin, offsetMax),
                        spawnY,
                        Random.Range(minZ, maxZ));

                case 1: // right
                    return new Vector3(
                        maxX + Random.Range(offsetMin, offsetMax),
                        spawnY,
                        Random.Range(minZ, maxZ));

                case 2: // bottom
                    return new Vector3(
                        Random.Range(minX, maxX),
                        spawnY,
                        minZ - Random.Range(offsetMin, offsetMax));

                default: // top
                    return new Vector3(
                        Random.Range(minX, maxX),
                        spawnY,
                        maxZ + Random.Range(offsetMin, offsetMax));
            }
        }

        private void GetCameraBounds(Camera camera, out float minX, out float maxX, out float minZ, out float maxZ)
        {
            float height = camera.orthographicSize * 2f;
            float width = height * camera.aspect;

            Vector3 cameraPosition = camera.transform.position;

            minX = cameraPosition.x - width / 2f;
            maxX = cameraPosition.x + width / 2f;

            minZ = cameraPosition.z - height / 2f;
            maxZ = cameraPosition.z + height / 2f;
        }
    }
}