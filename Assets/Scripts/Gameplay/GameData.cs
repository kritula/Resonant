using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "GameData", menuName = "ZombieIO/GameData")]
    public class GameData : ScriptableObject
    {
        [SerializeField] private float gameTimeMinutesMax = 15f;

        [Space(10)]
        [Header("Experience progress")]
        [SerializeField] private int baseExperience = 20;
        [SerializeField] private int grownRate = 10;

        [Space(10)]
        [Header("Spawn position")]
        [SerializeField] private float minEnemySpawnOffset = 6f;
        [SerializeField] private float maxEnemySpawnOffset = 12f;

        [Space(10)]
        [Header("Spawn safety")]
        [SerializeField] private int maxSpawnAttemptsPerTick = 50;

        [Space(10)]
        [Header("Spawn waves")]
        [SerializeField] private List<SpawnWaveData> _spawnWaves = new List<SpawnWaveData>();

        public float GameTimeMinutesMax =>
            gameTimeMinutesMax;

        public int BaseExperience =>
            baseExperience;

        public int GrownRate =>
            grownRate;

        public float GameTimeSecondsMax =>
            gameTimeMinutesMax * 60f;

        public float MinEnemySpawnOffset =>
            minEnemySpawnOffset;

        public float MaxEnemySpawnOffset =>
            maxEnemySpawnOffset;

        public int MaxSpawnAttemptsPerTick =>
            Mathf.Max(1, maxSpawnAttemptsPerTick);

        public List<SpawnWaveData> SpawnWaves =>
            _spawnWaves;
    }
}