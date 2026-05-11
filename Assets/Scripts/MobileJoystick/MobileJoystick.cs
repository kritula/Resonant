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
            Vector2 position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background,
                eventData.position,
                eventData.pressEventCamera,
                out position);

            Vector2 offset = Vector2.ClampMagnitude(position, maxRadius);

            _inputVector = offset / maxRadius;

            background.anchoredPosition = offset * backgroundMoveMultiplier;

            handle.anchoredPosition = offset * handleMoveMultiplier;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _inputVector = Vector2.zero;

            background.anchoredPosition = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
        }
    }
}