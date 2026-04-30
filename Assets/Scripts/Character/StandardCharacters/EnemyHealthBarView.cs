using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class EnemyHealthBarView : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private float _visibleTime = 1.5f;
        [SerializeField] private bool _lookAtCamera = true;

        private ILiveComponent _liveComponent;
        private float _hideTimer;

        public void Initialize(Character character)
        {
            if (_liveComponent != null)
                _liveComponent.OnCharacterHealthChange -= OnHealthChanged;

            _liveComponent = character.LiveComponent;

            _liveComponent.OnCharacterHealthChange += OnHealthChanged;

            Hide();
            UpdateSlider();
        }

        private void Update()
        {
            if (_canvas.gameObject.activeSelf)
            {
                _hideTimer -= Time.deltaTime;

                if (_hideTimer <= 0f)
                    Hide();
            }

            if (_lookAtCamera && _canvas.gameObject.activeSelf && Camera.main != null)
            {
                _canvas.transform.forward = Camera.main.transform.forward;
            }
        }

        private void OnHealthChanged(Character character)
        {
            UpdateSlider();

            if (_liveComponent.Health <= 0f)
            {
                Hide();
                return;
            }

            Show();
        }

        private void UpdateSlider()
        {
            if (_liveComponent == null || _healthSlider == null)
                return;

            _healthSlider.value = _liveComponent.Health / _liveComponent.MaxHealth;
        }

        private void Show()
        {
            _hideTimer = _visibleTime;
            _canvas.gameObject.SetActive(true);
        }

        private void Hide()
        {
            _hideTimer = 0f;

            if (_canvas != null)
                _canvas.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (_liveComponent != null)
                _liveComponent.OnCharacterHealthChange -= OnHealthChanged;

            _liveComponent = null;
            Hide();
        }
    }
}