using UnityEngine;

namespace OmniumLessons
{
    public class StunEffect : IStatusEffect
    {
        private Character _owner;
        private float _timer;

        public bool IsFinished => _timer <= 0f;

        public StunEffect(float duration)
        {
            _timer = duration;
        }

        public void Initialize(Character owner)
        {
            _owner = owner;
        }

        public void OnUpdate()
        {
            if (_timer > 0f)
            {
                _timer -= Time.deltaTime;
            }
        }
    }
}