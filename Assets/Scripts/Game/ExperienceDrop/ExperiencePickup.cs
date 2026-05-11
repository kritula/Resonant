using UnityEngine;

namespace OmniumLessons
{
    public class ExperiencePickup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Collider _triggerCollider;

        [Header("Lifetime")]
        [SerializeField] private float _lifeTime = 12f;

        private int _experienceAmount;
        private ExperiencePickupSpawner _spawner;

        private bool _isCollected;
        private float _lifeTimer;

        private void Reset()
        {
            _triggerCollider = GetComponent<Collider>();

            if (_triggerCollider != null)
                _triggerCollider.isTrigger = true;
        }

        private void Update()
        {
            if (!gameObject.activeSelf)
                return;

            if (_isCollected)
                return;

            _lifeTimer += Time.deltaTime;

            if (_lifeTimer >= _lifeTime)
            {
                Despawn();
            }
        }

        public void Initialize(
            int experienceAmount,
            ExperiencePickupSpawner spawner)
        {
            _experienceAmount = experienceAmount;
            _spawner = spawner;

            _isCollected = false;
            _lifeTimer = 0f;

            if (_triggerCollider != null)
                _triggerCollider.enabled = true;

            gameObject.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isCollected)
                return;

            PlayerCharacter player =
                other.GetComponent<PlayerCharacter>();

            if (player == null)
            {
                player =
                    other.GetComponentInParent<PlayerCharacter>();
            }

            if (player == null)
                return;

            Collect();
        }

        private void Collect()
        {
            if (_isCollected)
                return;

            _isCollected = true;

            if (GameManager.Instance != null &&
                GameManager.Instance.ExperienceManager != null)
            {
                GameManager.Instance.ExperienceManager
                    .AddExperience(_experienceAmount);
            }

            Despawn();
        }

        private void Despawn()
        {
            if (_triggerCollider != null)
                _triggerCollider.enabled = false;

            if (_spawner != null)
            {
                _spawner.ReturnToPool(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}