using UnityEngine;

namespace OmniumLessons
{
    public class CharacterMovementComponent : IMovable
    {
        private CharacterData characterData;
        private float _speed;
        private float _rotationVelocity;

        public float Speed
        {
            get => _speed;
            set
            {
                if (value < 0)
                    value = 0;

                _speed = value;
            }
        }

        public void Initialize(CharacterData characterData)
        {
            this.characterData = characterData;
            _speed = characterData.DefaultSpeed;
        }

        public void Move(Vector3 direction)
        {
            if (direction == Vector3.zero)
                return;

            direction.y = 0f;
            direction.Normalize();

            characterData.CharacterController.Move(direction * Speed * Time.deltaTime);
        }

        public void Rotation(Vector3 direction)
        {
            if (direction == Vector3.zero)
                return;

            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
                return;

            direction.Normalize();

            float rotationSmoothTime = 0.1f;
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            float angle = Mathf.SmoothDampAngle(
                characterData.CharacterTransform.eulerAngles.y,
                targetAngle,
                ref _rotationVelocity,
                rotationSmoothTime);

            characterData.CharacterTransform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }
}