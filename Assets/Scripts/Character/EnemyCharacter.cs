using UnityEngine;

namespace OmniumLessons
{
    public class EnemyCharacter : Character
    {
        [SerializeField] private AiState _aiState;

        public override Character CharacterTarget => GameManager.Instance.CharacterFactory.Player;

        public override void Initialize()
        {
            if (_characterData == null)
            {
                _characterData = GetComponent<CharacterData>();
            }

            base.Initialize();

            LiveComponent = new CharacterLiveComponent(this);

            AttackComponent = new MeleeAttackComponent();
            AttackComponent.Initialize(_characterData);

            _aiState = AiState.MoveToTarget;
        }

        public override void Update()
        {
            StatusEffectController?.OnUpdate();

            if (!LiveComponent.IsAlive)
                return;

            if (StatusEffectController != null && StatusEffectController.HasEffect<StunEffect>())
            {
                AttackComponent.OnUpdate();
                return;
            }

            if (CharacterTarget == null)
                return;

            float distance = Vector3.Distance(transform.position, CharacterTarget.transform.position);

            if (distance <= _characterData.WeaponData.AttackDistance)
            {
                _aiState = AiState.Attack;
            }
            else
            {
                _aiState = AiState.MoveToTarget;
            }

            switch (_aiState)
            {
                case AiState.MoveToTarget:
                    MoveToTarget();
                    break;

                case AiState.Attack:
                    AttackTarget();
                    break;
            }

            AttackComponent.OnUpdate();
        }

        private void MoveToTarget()
        {
            Vector3 direction = CharacterTarget.transform.position - transform.position;
            direction.y = 0f;
            direction.Normalize();

            MovableComponent.Move(direction);

            if (direction != Vector3.zero)
            {
                MovableComponent.Rotation(direction);
            }
        }

        private void AttackTarget()
        {
            Vector3 direction = CharacterTarget.transform.position - transform.position;
            direction.y = 0f;
            direction.Normalize();

            if (direction != Vector3.zero)
            {
                MovableComponent.Rotation(direction);
            }

            AttackComponent.MakeDamage(CharacterTarget);
        }
    }
}