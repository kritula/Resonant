using System;
using UnityEngine;

namespace OmniumLessons
{
    public class BossPhaseController
    {
        private readonly CharacterFactory _characterFactory;
        private readonly CharacterType _bossCharacterType;
        private readonly float _spawnDistanceFromPlayer;
        private readonly bool _stopRegularSpawnOnBossAppear;
        private readonly Action<Character> _registerCharacter;

        public BossPhaseController(
            CharacterFactory characterFactory,
            CharacterType bossCharacterType,
            float spawnDistanceFromPlayer,
            bool stopRegularSpawnOnBossAppear,
            Action<Character> registerCharacter)
        {
            _characterFactory = characterFactory;
            _bossCharacterType = bossCharacterType;
            _spawnDistanceFromPlayer = spawnDistanceFromPlayer;
            _stopRegularSpawnOnBossAppear = stopRegularSpawnOnBossAppear;
            _registerCharacter = registerCharacter;
        }

        public void StartBossPhase(CharacterSpawnController spawnController)
        {
            if (_stopRegularSpawnOnBossAppear)
                spawnController?.StopSpawn();

            SpawnBoss();
        }

        private void SpawnBoss()
        {
            Character player = _characterFactory.Player;

            if (player == null)
            {
                Debug.LogWarning(
                    "BossPhaseController: Player not found. Boss spawn cancelled.");
                return;
            }

            Character boss =
                _characterFactory.CreateCharacter(_bossCharacterType);

            if (boss == null)
            {
                Debug.LogWarning(
                    "BossPhaseController: Boss was not created. Check boss prefab.");
                return;
            }

            Vector3 spawnPosition =
                player.transform.position +
                GetSpawnDirection() * _spawnDistanceFromPlayer;
            spawnPosition = CharacterSpawnHeights.Apply(
                spawnPosition,
                _bossCharacterType);

            PlaceBoss(boss, spawnPosition);
            _registerCharacter?.Invoke(boss);

            Debug.Log("Boss spawned: Null Core");
        }

        private Vector3 GetSpawnDirection()
        {
            Vector3 spawnDirection = UnityEngine.Random.insideUnitSphere;
            spawnDirection.y = 0f;

            if (spawnDirection.sqrMagnitude < 0.01f)
                spawnDirection = Vector3.forward;

            return spawnDirection.normalized;
        }

        private void PlaceBoss(Character boss, Vector3 spawnPosition)
        {
            CharacterController controller =
                boss.CharacterData != null
                    ? boss.CharacterData.CharacterController
                    : null;

            if (controller != null)
                controller.enabled = false;

            boss.transform.position = spawnPosition;

            if (boss.CharacterData != null &&
                boss.CharacterData.CharacterTransform != null)
            {
                boss.CharacterData.CharacterTransform.position =
                    spawnPosition;
            }

            if (controller != null)
                controller.enabled = true;

            boss.gameObject.SetActive(true);
        }
    }
}
