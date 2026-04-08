using UnityEngine;

namespace OmniumLessons
{
    public class SlowEffect : IStatusEffect
    {
        private Character _owner;
        private float _timer;
        private float _slowMultiplier;
        private float _originalSpeed;
        private bool _speedApplied;

        public bool IsFinished => _timer <= 0f;

        public SlowEffect(float slowMultiplier, float duration)
        {
            _slowMultiplier = slowMultiplier;
            _timer = duration;
        }

        public void Initialize(Character owner)
        {
            _owner = owner;

            if (_owner == null || _owner.MovableComponent == null)
                return;

            _originalSpeed = _owner.MovableComponent.Speed;
            _owner.MovableComponent.Speed = _originalSpeed * _slowMultiplier;
            _speedApplied = true;
        }

        public void OnUpdate()
        {
            if (_timer > 0f)
            {
                _timer -= Time.deltaTime;
            }

            if (_timer <= 0f && _speedApplied)
            {
                if (_owner != null && _owner.MovableComponent != null)
                {
                    _owner.MovableComponent.Speed = _originalSpeed;
                }

                _speedApplied = false;
            }
        }
    }
}