using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class ExperiencePickupSpawner : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private ExperiencePickup _experiencePickupPrefab;

        [Header("Spawn Settings")]
        [SerializeField] private float _spawnHeight = 0.25f;

        private readonly Queue<ExperiencePickup> _pool = new Queue<ExperiencePickup>();
        private readonly List<ExperiencePickup> _activePickups = new List<ExperiencePickup>();

        public void SpawnExperiencePickup(Vector3 position, int experienceAmount)
        {
            if (experienceAmount <= 0)
                return;

            if (_experiencePickupPrefab == null)
            {
                Debug.LogWarning("ExperiencePickupSpawner: ExperiencePickup prefab is not assigned.");
                return;
            }

            ExperiencePickup pickup = GetFromPool();

            Vector3 spawnPosition = position;
            spawnPosition.y = _spawnHeight;

            pickup.transform.position = spawnPosition;
            pickup.Initialize(experienceAmount, this);

            if (!_activePickups.Contains(pickup))
                _activePickups.Add(pickup);
        }

        public void ReturnToPool(ExperiencePickup pickup)
        {
            if (pickup == null)
                return;

            _activePickups.Remove(pickup);

            pickup.gameObject.SetActive(false);
            _pool.Enqueue(pickup);
        }

        public void ClearAll()
        {
            for (int i = _activePickups.Count - 1; i >= 0; i--)
            {
                ExperiencePickup pickup = _activePickups[i];

                if (pickup == null)
                    continue;

                pickup.gameObject.SetActive(false);
                _pool.Enqueue(pickup);
            }

            _activePickups.Clear();
        }

        private ExperiencePickup GetFromPool()
        {
            while (_pool.Count > 0)
            {
                ExperiencePickup pickup = _pool.Dequeue();

                if (pickup != null)
                    return pickup;
            }

            return Instantiate(_experiencePickupPrefab, transform);
        }
    }
}