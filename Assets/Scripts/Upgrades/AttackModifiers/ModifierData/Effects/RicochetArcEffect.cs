using UnityEngine;

namespace OmniumLessons
{
    [RequireComponent(typeof(LineRenderer))]
    public class RicochetArcEffect : MonoBehaviour
    {
        [SerializeField] private float _lifeTime = 0.12f;
        [SerializeField] private float _arcHeight = 0.4f;

        private LineRenderer _lineRenderer;
        private Vector3 _startPoint;
        private Vector3 _endPoint;

        public void Initialize(Vector3 startPoint, Vector3 endPoint)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;

            _lineRenderer = GetComponent<LineRenderer>();

            if (_lineRenderer != null)
            {
                _lineRenderer.positionCount = 3;

                Vector3 middlePoint = (_startPoint + _endPoint) * 0.5f;
                middlePoint.y += _arcHeight;

                _lineRenderer.SetPosition(0, _startPoint);
                _lineRenderer.SetPosition(1, middlePoint);
                _lineRenderer.SetPosition(2, _endPoint);
            }

            Destroy(gameObject, _lifeTime);
        }
    }
}