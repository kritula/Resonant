using UnityEngine;

namespace OmniumLessons
{
    public class SlowEffect : IStatusEffect
    {
        private Character _owner;
        private float _timer;
        private readonly float _slowMultiplier;
        private float _originalSpeed;
        private bool _speedApplied;

        public bool IsFinished => _timer <= 0f;

        public SlowEffect(float slowMultiplier, float duration)
        {
            _slowMultiplier = Mathf.Clamp(slowMultiplier, 0.01f, 1f);
            _timer = Mathf.Max(0.1f, duration);
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

            if (_timer <= 0f)
            {
                ForceFinish();
            }
        }

        public void ForceFinish()
        {
            if (_speedApplied && _owner != null && _owner.MovableComponent != null)
            {
                _owner.MovableComponent.Speed = _originalSpeed;
            }

            _speedApplied = false;
            _timer = 0f;
        }
    }
}