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
                    activeCharacters[i].LiveComponent.IsAlive)
                {
                    count++;
                }
            }

            return count;
        }

        private void SpawnEnemy()
        {
            CharacterType enemyType = GetEnemyTypeForSpawn();
            Character enemy = CharacterFactory.CreateCharacter(enemyType);

            float posX = CharacterFactory.Player.transform.position.x + GetOffset();
            float posZ = CharacterFactory.Player.transform.position.z + GetOffset();

            Vector3 spawnPoint = new Vector3(posX, 0, posZ);
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

        private float GetOffset()
        {
            bool isPlus = Random.value > 0.5f;
            float randomOffset = Random.Range(GameData.MinEnemySpawnOffset, GameData.MaxEnemySpawnOffset);

            return isPlus ? randomOffset : -randomOffset;
        }
    }
}