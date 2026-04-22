using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OmniumLessons
{
    public class CharacterSpawnController
    {
        private CharacterFactory CharacterFactory => GameManager.Instance.CharacterFactory;
        private GameData GameData => GameManager.Instance.GameData;

        private bool _isActiveSpawn;

        private SpawnWaveData _currentWave;

        private float _regularSpawnTimer;
        private float _burstSpawnTimer;

        public void StartSpawn()
        {
            _isActiveSpawn = true;
            _currentWave = null;
            _regularSpawnTimer = 0f;
            _burstSpawnTimer = 0f;
        }

        public void StopSpawn()
        {
            _isActiveSpawn = false;
            _currentWave = null;
            _regularSpawnTimer = 0f;
            _burstSpawnTimer = 0f;
        }

        public void OnUpdate(float deltaTime)
        {
            if (!_isActiveSpawn)
                return;

            if (CharacterFactory.Player == null)
                return;

            SpawnWaveData wave = GetCurrentWave();
            if (wave == null)
                return;

            if (_currentWave != wave)
            {
                _currentWave = wave;
                _regularSpawnTimer = 0f;
                _burstSpawnTimer = 0f;
            }

            int currentEnemies = CountActiveEnemies();

            if (HandleEmergencyFill(wave, currentEnemies))
                return;

            _regularSpawnTimer += deltaTime;

            if (wave.UseBurstSpawn)
                _burstSpawnTimer += deltaTime;

            if (_regularSpawnTimer >= wave.SpawnInterval)
            {
                _regularSpawnTimer = 0f;
                SpawnBatch(wave, wave.SpawnPerTick);
            }

            if (wave.UseBurstSpawn && _burstSpawnTimer >= wave.BurstInterval)
            {
                _burstSpawnTimer = 0f;
                SpawnBatch(wave, wave.BurstSpawnCount);
            }
        }

        private bool HandleEmergencyFill(SpawnWaveData wave, int currentEnemies)
        {
            if (!wave.UseEmergencyFill)
                return false;

            if (currentEnemies > wave.EmergencyThreshold)
                return false;

            int missingToMinimum = wave.MinimumAliveEnemies - currentEnemies;
            if (missingToMinimum <= 0)
                return false;

            int spawnCount = Mathf.Min(missingToMinimum, wave.EmergencySpawnCount);
            SpawnBatch(wave, spawnCount);

            return true;
        }

        private void SpawnBatch(SpawnWaveData wave, int requestedCount)
        {
            if (wave == null)
                return;

            if (wave.Enemies == null || wave.Enemies.Count == 0)
                return;

            int currentEnemies = CountActiveEnemies();
            if (currentEnemies >= wave.MaximumAliveEnemies)
                return;

            int freeSlots = wave.MaximumAliveEnemies - currentEnemies;
            int finalSpawnCount = Mathf.Min(requestedCount, freeSlots);
            int maxAttempts = Mathf.Min(finalSpawnCount, GameData.MaxSpawnAttemptsPerTick);

            for (int i = 0; i < maxAttempts; i++)
            {
                CharacterType enemyType = GetWeightedRandomEnemyType(wave);
                SpawnEnemy(enemyType);
            }
        }

        private SpawnWaveData GetCurrentWave()
        {
            float gameTime = GameManager.Instance.GameTime;
            List<SpawnWaveData> waves = GameData.SpawnWaves;

            if (waves == null || waves.Count == 0)
                return null;

            SpawnWaveData fallbackWave = null;

            for (int i = 0; i < waves.Count; i++)
            {
                SpawnWaveData wave = waves[i];

                if (wave == null)
                    continue;

                if (wave.IsInWave(gameTime))
                    return wave;

                if (gameTime >= wave.StartTimeSeconds)
                    fallbackWave = wave;
            }

            return fallbackWave;
        }

        private CharacterType GetWeightedRandomEnemyType(SpawnWaveData wave)
        {
            if (wave == null || wave.Enemies == null || wave.Enemies.Count == 0)
                return CharacterType.DefaultEnemy;

            int totalWeight = 0;

            for (int i = 0; i < wave.Enemies.Count; i++)
            {
                totalWeight += wave.Enemies[i].Weight;
            }

            if (totalWeight <= 0)
                return wave.Enemies[0].CharacterType;

            int randomValue = Random.Range(0, totalWeight);
            int accumulatedWeight = 0;

            for (int i = 0; i < wave.Enemies.Count; i++)
            {
                accumulatedWeight += wave.Enemies[i].Weight;

                if (randomValue < accumulatedWeight)
                    return wave.Enemies[i].CharacterType;
            }

            return wave.Enemies[wave.Enemies.Count - 1].CharacterType;
        }

        private int CountActiveEnemies()
        {
            int count = 0;
            List<Character> activeCharacters = CharacterFactory.ActiveCharacters;

            for (int i = 0; i < activeCharacters.Count; i++)
            {
                Character character = activeCharacters[i];

                if (character == null)
                    continue;

                if (character.CharacterType == CharacterType.DefaultPlayer)
                    continue;

                if (character.LiveComponent == null)
                    continue;

                if (!character.LiveComponent.IsAlive)
                    continue;

                count++;
            }

            return count;
        }

        private void SpawnEnemy(CharacterType enemyType)
        {
            if (CharacterFactory.Player == null)
                return;

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

        private Vector3 GetSpawnPointOutsideCamera()
        {
            Camera camera = Camera.main;

            if (camera == null)
            {
                Debug.LogWarning("CharacterSpawnController: Camera.main not found.");
                return Vector3.zero;
            }

            if (!TryGetCameraGroundBounds(camera, out float minX, out float maxX, out float minZ, out float maxZ))
            {
                Debug.LogWarning("CharacterSpawnController: Failed to calculate camera ground bounds.");
                return CharacterFactory.Player.transform.position;
            }

            float offsetMin = GameData.MinEnemySpawnOffset;
            float offsetMax = GameData.MaxEnemySpawnOffset;

            float spawnY = 1f;
            int side = Random.Range(0, 4);

            switch (side)
            {
                case 0:
                    return new Vector3(
                        minX - Random.Range(offsetMin, offsetMax),
                        spawnY,
                        Random.Range(minZ, maxZ));

                case 1:
                    return new Vector3(
                        maxX + Random.Range(offsetMin, offsetMax),
                        spawnY,
                        Random.Range(minZ, maxZ));

                case 2:
                    return new Vector3(
                        Random.Range(minX, maxX),
                        spawnY,
                        minZ - Random.Range(offsetMin, offsetMax));

                default:
                    return new Vector3(
                        Random.Range(minX, maxX),
                        spawnY,
                        maxZ + Random.Range(offsetMin, offsetMax));
            }
        }

        private bool TryGetCameraGroundBounds(Camera camera, out float minX, out float maxX, out float minZ, out float maxZ)
        {
            minX = maxX = minZ = maxZ = 0f;

            float groundY = CharacterFactory.Player != null
                ? CharacterFactory.Player.transform.position.y
                : 0f;

            if (!TryGetGroundPointFromViewport(camera, new Vector3(0f, 0f, 0f), groundY, out Vector3 leftBottom))
                return false;

            if (!TryGetGroundPointFromViewport(camera, new Vector3(0f, 1f, 0f), groundY, out Vector3 leftTop))
                return false;

            if (!TryGetGroundPointFromViewport(camera, new Vector3(1f, 0f, 0f), groundY, out Vector3 rightBottom))
                return false;

            if (!TryGetGroundPointFromViewport(camera, new Vector3(1f, 1f, 0f), groundY, out Vector3 rightTop))
                return false;

            minX = Mathf.Min(leftBottom.x, leftTop.x, rightBottom.x, rightTop.x);
            maxX = Mathf.Max(leftBottom.x, leftTop.x, rightBottom.x, rightTop.x);

            minZ = Mathf.Min(leftBottom.z, leftTop.z, rightBottom.z, rightTop.z);
            maxZ = Mathf.Max(leftBottom.z, leftTop.z, rightBottom.z, rightTop.z);

            return true;
        }

        private bool TryGetGroundPointFromViewport(Camera camera, Vector3 viewportPoint, float groundY, out Vector3 groundPoint)
        {
            Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, groundY, 0f));
            Ray ray = camera.ViewportPointToRay(viewportPoint);

            if (groundPlane.Raycast(ray, out float enter))
            {
                groundPoint = ray.GetPoint(enter);
                return true;
            }

            groundPoint = Vector3.zero;
            return false;
        }
    }
}