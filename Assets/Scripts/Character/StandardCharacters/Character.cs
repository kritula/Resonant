using UnityEngine;

namespace OmniumLessons
{
    public abstract class Character : MonoBehaviour
    {
        [SerializeField] protected CharacterData _characterData;
        [SerializeField] protected CharacterType _characterType;
        [SerializeField] private float _deathAnimationDuration = 0.6f;

        
        protected bool _isDead;
        protected SpriteRenderer _spriteRenderer;

        public CharacterData CharacterData => _characterData;
        public CharacterType CharacterType => _characterType;
        public float DeathAnimationDuration => _deathAnimationDuration;

        public virtual Character CharacterTarget { get; }

        public IMovable MovableComponent { get; protected set; }
        public ILiveComponent LiveComponent { get; protected set; }
        public IAttackComponent AttackComponent { get; protected set; }
        public StatusEffectController StatusEffectController { get; protected set; }
        public Animator Animator { get; protected set; }

        public virtual void Initialize()
        {
            MovableComponent = new CharacterMovementComponent();
            MovableComponent.Initialize(_characterData);

            StatusEffectController = new StatusEffectController(this);

            Animator = GetComponentInChildren<Animator>();
        
            if (Animator != null)
            {
                Animator.Rebind();
                Animator.Update(0f);
            }
            _isDead = false;
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        protected virtual void OnDeath(Character deathCharacter)
        {
            if (_isDead)
                return;

            _isDead = true;

            if (Animator != null)
            {
                Animator.SetFloat(AnimatorHashes.Speed, 0f);
                Animator.ResetTrigger(AnimatorHashes.Attack);
                Animator.SetTrigger(AnimatorHashes.Death);
            }
        }
        public virtual void OnDeathAnimationFinished()
        {
            if (GameManager.Instance == null)
                return;

            GameManager.Instance.FinishCharacterDeath(this);
        }
        protected void UpdateSpriteDirection(Vector3 direction)
        {
            if (_spriteRenderer == null)
                return;

            if (direction.x > 0.01f)
                _spriteRenderer.flipX = false;
            else if (direction.x < -0.01f)
                _spriteRenderer.flipX = true;
        }

        public abstract void Update();
    }
}