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

    }
}