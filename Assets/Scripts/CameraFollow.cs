using UnityEngine;

namespace OmniumLessons
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;   // игрок
        [SerializeField] private Vector3 offset;     // смещение камеры
        [SerializeField] private float smoothSpeed = 10f;


        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
        private void LateUpdate()
        {
            if (target == null)
                return;

            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime
            );
        }
    }
}

