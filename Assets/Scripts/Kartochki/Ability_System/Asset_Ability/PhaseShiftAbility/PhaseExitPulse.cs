using UnityEngine;

namespace OmniumLessons
{
    public class PhaseExitPulse : MonoBehaviour
    {
        [SerializeField] private float _growSpeed = 6f;
        [SerializeField] private float _lifeTime = 0.8f;

        private float _timer;

        private void Update()
        {
            _timer += Time.deltaTime;

            transform.localScale += Vector3.one * _growSpeed * Time.deltaTime;

            if (_timer >= _lifeTime)
            {
                Destroy(gameObject);
            }
        }
    }
}