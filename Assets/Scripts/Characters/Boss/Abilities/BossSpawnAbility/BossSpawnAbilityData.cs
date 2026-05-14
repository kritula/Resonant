using System;
using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    [Serializable]
    public class BossSpawnEntry
    {
        [SerializeField] private CharacterType _type;
        [SerializeField] private int _weight = 1;

        public CharacterType Type => _type;
        public int Weight => Mathf.Max(0, _weight);
    }

    [CreateAssetMenu(fileName = "BossSpawnAbilityData", menuName = "ZombieIO/Boss/Boss Spawn Ability Data")]
    public class BossSpawnAbilityData : ScriptableObject
    {
        [SerializeField] private float _cooldown = 10f;
        [SerializeField] private int _spawnCount = 3;
        [SerializeField] private float _minSpawnDistance = 2f;
        [SerializeField] private float _maxSpawnDistance = 5f;
        [SerializeField] private float _spawnY = 1f;
        [SerializeField] private List<BossSpawnEntry> _enemies = new List<BossSpawnEntry>();

        public float Cooldown => Mathf.Max(0.1f, _cooldown);
        public int SpawnCount => Mathf.Max(1, _spawnCount);
        public float MinSpawnDistance => Mathf.Max(0.1f, _minSpawnDistance);
        public float MaxSpawnDistance => Mathf.Max(MinSpawnDistance, _maxSpawnDistance);
        public float SpawnY => _spawnY;
        public List<BossSpawnEntry> Enemies => _enemies;
    }
}