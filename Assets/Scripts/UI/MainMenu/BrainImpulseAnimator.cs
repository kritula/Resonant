using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class BrainImpulseAnimator : MonoBehaviour
    {
        [SerializeField] private Image _impulseImage;
        [SerializeField] private RectTransform _impulseRect;

        [Header("Flicker")]
        [SerializeField] private float _minAlpha = 0.35f;
        [SerializeField] private float _maxAlpha = 1f;
        [SerializeField] private float _flickerSpeed = 5f;

        [Header("Movement")]
        [SerializeField] private float _movePower = 4f;
        [SerializeField] private float _moveSpeed = 2f;

        [Header("Scale")]
        [SerializeField] private float _scalePower = 0.025f;
        [SerializeField] private float _scaleSpeed = 2f;

        private Vector2 _startPosition;
        private Vector3 _startScale;

        private void Awake()
        {
            if (_impulseImage == null)
                _impulseImage = GetComponent<Image>();

            if (_impulseRect == null)
                _impulseRect = GetComponent<RectTransform>();

            _startPosition = _impulseRect.anchoredPosition;
            _startScale = _impulseRect.localScale;
        }

        private void Update()
        {
            float flicker = (Mathf.Sin(Time.time * _flickerSpeed) + 1f) * 0.5f;
            float softPulse = (Mathf.Sin(Time.time * _scaleSpeed) + 1f) * 0.5f;

            Color color = _impulseImage.color;
            color.a = Mathf.Lerp(_minAlpha, _maxAlpha, flicker);
            _impulseImage.color = color;

            float moveY = Mathf.Sin(Time.time * _moveSpeed) * _movePower;
            _impulseRect.anchoredPosition = _startPosition + new Vector2(0f, moveY);

            float scale = 1f + softPulse * _scalePower;
            _impulseRect.localScale = _startScale * scale;
        }
    }
}