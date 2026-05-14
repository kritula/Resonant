using UnityEngine;
using UnityEngine.EventSystems;

namespace OmniumLessons
{
    public class MobileJoystick : MonoBehaviour,
        IPointerDownHandler,
        IDragHandler,
        IPointerUpHandler
    {
        public static MobileJoystick Instance { get; private set; }

        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;

        [SerializeField] private float maxRadius = 120f;

        [SerializeField] private float backgroundMoveMultiplier = 0.25f;
        [SerializeField] private float handleMoveMultiplier = 1f;

        private Vector2 _inputVector;

        public Vector2 InputVector => _inputVector;

        private void Awake()
        {
            Instance = this;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (background == null || handle == null)
                return;

            Vector2 position;
            float radius = GetEffectiveMaxRadius();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background,
                eventData.position,
                eventData.pressEventCamera,
                out position);

            Vector2 offset = Vector2.ClampMagnitude(position, radius);

            _inputVector = offset / radius;

            background.anchoredPosition = offset * backgroundMoveMultiplier;

            handle.anchoredPosition = offset * handleMoveMultiplier;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _inputVector = Vector2.zero;

            if (background == null || handle == null)
                return;

            background.anchoredPosition = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
        }

        private float GetEffectiveMaxRadius()
        {
            float configuredRadius = Mathf.Max(1f, maxRadius);

            if (background == null)
                return configuredRadius;

            float visualRadius =
                Mathf.Min(background.rect.width, background.rect.height) * 0.5f;

            if (visualRadius <= 0f)
                return configuredRadius;

            return Mathf.Min(configuredRadius, visualRadius);
        }
    }
}
