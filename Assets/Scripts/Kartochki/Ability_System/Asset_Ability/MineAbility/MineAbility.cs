using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class MineAbility : AbilityBehaviour
    {
        [Header("Mine spawn settings")]
        [SerializeField] private MineObject _minePrefab;
        [SerializeField] private float _spawnRadius = 3f;
        [SerializeField] private float _spawnInterval = 5f;
        [SerializeField] private float _spawnHeight = 0.1f;
        [SerializeField] private int _maxActiveMines = 5;

        private float _spawnTimer;
        private readonly List<MineObject> _activeMines = new List<MineObject>();

        public override void Initialize(PlayerCharacter owner, AbilityUpgradeData abilityData)
        {
            base.Initialize(owner, abilityData);
            _spawnTimer = _spawnInterval;
        }

        public override void OnUpdate()
        {
            if (_owner == null)
                return;

            if (_minePrefab == null)
                return;

            CleanupDestroyedMines();

            if (_activeMines.Count >= _maxActiveMines)
                return;

            if (_spawnTimer > 0f)
            {
                _spawnTimer -= Time.deltaTime;
                return;
            }

            SpawnMine();
            _spawnTimer = _spawnInterval;
        }

        private void SpawnMine()
        {
            Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;

            Vector3 spawnPosition = _owner.transform.position;
            spawnPosition += new Vector3(randomCircle.x, _spawnHeight, randomCircle.y);

            MineObject mineObject = Object.Instantiate(_minePrefab, spawnPosition, Quaternion.identity);
            mineObject.Initialize(_owner, this);

            _activeMines.Add(mineObject);
        }

        private void CleanupDestroyedMines()
        {
            for (int i = _activeMines.Count - 1; i >= 0; i--)
            {
                if (_activeMines[i] == null)
                {
                    _activeMines.RemoveAt(i);
                }
            }
        }

        public void RemoveMine(MineObject mineObject)
        {
            if (mineObject == null)
                return;

            _activeMines.Remove(mineObject);
        }
    }
}