using UnityEngine;

namespace OmniumLessons
{
    public class BossPulseCaster : MonoBehaviour
    {
        [SerializeField] private BossPulseAbilityData _pulseData;

        private Character _ownerCharacter;
        private float _cooldownTimer;
        private BossPulseRing _activeRing;

        private void Awake()
        {
            _ownerCharacter = GetComponent<Character>();
        }

        private void OnEnable()
        {
            _cooldownTimer = 0f;
        }

        private void Update()
        {
            if (_pulseData == null)
                return;

            if (GameManager.Instance == null)
                return;

            if (GameManager.Instance.IsGamePaused)
                return;

            if (_ownerCharacter == null || _ownerCharacter.LiveComponent == null)
                return;

            if (!_ownerCharacter.LiveComponent.IsAlive)
                return;

            if (_activeRing != null)
                return;

            _cooldownTimer -= Time.deltaTime;

            if (_cooldownTimer > 0f)
                return;

            CastPulse();
            _cooldownTimer = _pulseData.Cooldown;
        }

        private void CastPulse()
        {
            if (_pulseData.RingPrefab == null)
            {
                Debug.LogWarning("BossPulseCaster: Ring Prefab is not assigned.");
                return;
            }

            _activeRing = Instantiate(_pulseData.RingPrefab, transform);
            _activeRing.transform.localPosition = Vector3.zero;
            _activeRing.transform.localRotation = Quaternion.identity;

            _activeRing.Initialize(_pulseData, transform, OnRingFinished);
        }

        private void OnRingFinished(BossPulseRing ring)
        {
            if (_activeRing == ring)
            {
                _activeRing = null;
            }
        }
    }
}