using UnityEngine;

namespace OmniumLessons
{
    public class EnemyCharacter : Character
    {
        [SerializeField] private EnemyHealthBarView _healthBarView;
        private MeleeAttackComponent _meleeAttackComponent;

        public override Character CharacterTarget => GameManager.Instance.CharacterFactory.Player;

        private void Reset()
        {
            CacheHealthBarView();
        }

        public override void Initialize()
        {
            if (_characterData == null)
            {
                _characterData = GetComponent<CharacterData>();
            }

            base.Initialize();

            LiveComponent = new CharacterLiveComponent(this);

            CacheHealthBarView();
            _healthBarView?.Initialize(this);

            _isDead = false;

            LiveComponent.OnCharacterDeath += OnDeath;

            _meleeAttackComponent = new MeleeAttackComponent();
            AttackComponent = _meleeAttackComponent;
            AttackComponent.Initialize(_characterData);

        }

        private void CacheHealthBarView()
        {
            if (_healthBarView != null)
                return;

            _healthBarView = GetComponentInChildren<EnemyHealthBarView>(true);
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

                if (UsesAnimationEventDamage())
                {
                    if (_meleeAttackComponent.StartDelayedDamage(CharacterTarget))
                    {
                        PlayAttackAnimation();
                    }
                }
                else if (AttackComponent.MakeDamage(CharacterTarget))
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

        public void OnAttackHit()
        {
            if (LiveComponent == null || !LiveComponent.IsAlive)
            {
                _meleeAttackComponent?.ClearPendingDamage();
                return;
            }

            if (StatusEffectController != null && StatusEffectController.HasEffect<StunEffect>())
            {
                _meleeAttackComponent?.ClearPendingDamage();
                return;
            }

            if (CharacterTarget == null || _characterData == null || _characterData.WeaponData == null)
            {
                _meleeAttackComponent?.ClearPendingDamage();
                return;
            }

            Vector3 attackDirection = CharacterTarget.transform.position - transform.position;
            attackDirection.y = 0f;

            if (attackDirection.magnitude > _characterData.WeaponData.AttackDistance)
            {
                _meleeAttackComponent?.ClearPendingDamage();
                return;
            }

            _meleeAttackComponent?.ApplyPendingDamage();
        }

        private bool UsesAnimationEventDamage()
        {
            return CharacterType == CharacterType.DefaultEnemy;
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
            float speed = MovableComponent.Speed;
            float pullNodeSpeedMultiplier =
                PullNodeTileBehavior.GetEnemyMoveSpeedMultiplier(
                    transform.position);

            MovableComponent.Speed = speed * pullNodeSpeedMultiplier;
            MovableComponent.Move(direction);
            MovableComponent.Speed = speed;

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
