using UnityEngine;

namespace OmniumLessons
{
    public class EnemyCharacter : Character
    {
        [SerializeField] private EnemyHealthBarView _healthBarView;
        public override Character CharacterTarget => GameManager.Instance.CharacterFactory.Player;

        public override void Initialize()
        {
            if (_characterData == null)
            {
                _characterData = GetComponent<CharacterData>();
            }

            base.Initialize();

            LiveComponent = new CharacterLiveComponent(this);

            _healthBarView?.Initialize(this);

            _isDead = false;

            LiveComponent.OnCharacterDeath += OnDeath;

            AttackComponent = new MeleeAttackComponent();
            AttackComponent.Initialize(_characterData);

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

            Vector3 attackDirection = CharacterTarget.transform.position - transform.position;
            attackDirection.y = 0f;

            float distance = attackDirection.magnitude;

            if (distance <= _characterData.WeaponData.AttackDistance)
            {
                UpdateMovementAnimation(0f);

                if (AttackComponent.MakeDamage(CharacterTarget))
                {
                    PlayAttackAnimation();
                }
            }
            else
            {
                MoveToTarget();
            }

            AttackComponent.OnUpdate();
        }

        private void MoveToTarget()
        {
            if (CharacterTarget == null)
            {
                UpdateMovementAnimation(0f);
                return;
            }

            Vector3 direction = CharacterTarget.transform.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
            {
                UpdateMovementAnimation(0f);
                return;
            }

            direction.Normalize();

            UpdateSpriteDirection(direction);
            MovableComponent.Move(direction);

            UpdateMovementAnimation(1f);
        }

        private void UpdateMovementAnimation(float speed)
        {
            if (Animator == null)
                return;

            Animator.SetFloat(AnimatorHashes.Speed, speed);
        }

        private void PlayAttackAnimation()
        {
            Animator.SetTrigger(AnimatorHashes.Attack);
        }
    }
}