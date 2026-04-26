using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class BrainBackgroundController : MonoBehaviour
    {
        [Header("Links")]
        [SerializeField] private RectTransform _brain;
        [SerializeField] private Image _brainImage;
        [SerializeField] private RectTransform _impulses;
        [SerializeField] private Image _impulsesImage;

        [Header("Pulse")]
        [SerializeField] private float _pulseSpeed = 1.2f;
        [SerializeField] private float _pulseScale = 0.02f;

        [Header("Impulse Glow")]
        [SerializeField] private float _minImpulseAlpha = 0.45f;
        [SerializeField] private float _maxImpulseAlpha = 1f;

        [Header("Impulse Movement")]
        [SerializeField] private float _impulseMovePower = 8f;
        [SerializeField] private float _impulseMoveSpeed = 3f;

        private Vector3 _brainStartScale;
        private Vector2 _impulsesStartPosition;

        private void Awake()
        {
            _brainStartScale = _brain.localScale;
            _impulsesStartPosition = _impulses.anchoredPosition;
        }

        private void Update()
        {
            float pulse = (Mathf.Sin(Time.time * _pulseSpeed) + 1f) * 0.5f;

            float scale = 1f + pulse * _pulseScale;
            _brain.localScale = _brainStartScale * scale;

            Color impulseColor = _impulsesImage.color;
            impulseColor.a = Mathf.Lerp(_minImpulseAlpha, _maxImpulseAlpha, pulse);
            _impulsesImage.color = impulseColor;

            float moveY = Mathf.Sin(Time.time * _impulseMoveSpeed) * _impulseMovePower;
            _impulses.anchoredPosition = _impulsesStartPosition + new Vector2(0f, moveY);
        }
    }
}