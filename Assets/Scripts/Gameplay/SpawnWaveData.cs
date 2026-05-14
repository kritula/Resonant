using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "SpawnWaveData", menuName = "ZombieIO/Spawn/Spawn Wave Data")]
    public class SpawnWaveData : ScriptableObject
    {
        [Header("Wave Time")]
        [SerializeField] private string _waveName = "Wave";
        [SerializeField] private float _startTimeSeconds = 0f;
        [SerializeField] private float _endTimeSeconds = 60f;

        [Header("Alive Enemies Limits")]
        [SerializeField] private int _minimumAliveEnemies = 15;
        [SerializeField] private int _maximumAliveEnemies = 35;

        [Header("Regular Spawn")]
        [SerializeField] private float _spawnInterval = 1f;
        [SerializeField] private int _spawnPerTick = 1;

        [Header("Emergency Fill")]
        [SerializeField] private bool _useEmergencyFill = true;
        [SerializeField] private int _emergencyThreshold = 8;
        [SerializeField] private int _emergencySpawnCount = 4;

        [Header("Burst Spawn")]
        [SerializeField] private bool _useBurstSpawn = false;
        [SerializeField] private float _burstInterval = 10f;
        [SerializeField] private int _burstSpawnCount = 8;

        [Header("Enemies")]
        [SerializeField] private List<WaveEnemyEntry> _enemies = new List<WaveEnemyEntry>();

        public string WaveName => _waveName;
        public float StartTimeSeconds => _startTimeSeconds;
        public float EndTimeSeconds => _endTimeSeconds;

        public int MinimumAliveEnemies => Mathf.Max(0, _minimumAliveEnemies);
        public int MaximumAliveEnemies => Mathf.Max(_minimumAliveEnemies, _maximumAliveEnemies);

        public float SpawnInterval => Mathf.Max(0.05f, _spawnInterval);
        public int SpawnPerTick => Mathf.Max(1, _spawnPerTick);

        public bool UseEmergencyFill => _useEmergencyFill;
        public int EmergencyThreshold => Mathf.Max(0, _emergencyThreshold);
        public int EmergencySpawnCount => Mathf.Max(1, _emergencySpawnCount);

        public bool UseBurstSpawn => _useBurstSpawn;
        public float BurstInterval => Mathf.Max(0.25f, _burstInterval);
        public int BurstSpawnCount => Mathf.Max(1, _burstSpawnCount);

        public List<WaveEnemyEntry> Enemies => _enemies;

        public bool IsInWave(float gameTime)
        {
            return gameTime >= _startTimeSeconds && gameTime < _endTimeSeconds;
        }
    }
}