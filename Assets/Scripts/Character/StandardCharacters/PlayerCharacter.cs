using UnityEngine;

namespace OmniumLessons
{
    public class PlayerCharacter : Character
    {
        private bool _isInvulnerable;
        private float _invulnerabilityTimer;

        public bool IsInvulnerable => _isInvulnerable;
        public AbilityManager AbilityManager { get; private set; }
        public AttackModifierController AttackModifierController { get; private set; }

        public override Character CharacterTarget
        {
            get
            {
                Character target = null;
                float nearest = float.MaxValue;

                var activePool = GameManager.Instance.CharacterFactory.ActiveCharacters;

                foreach (var activeCharacter in activePool)
                {
                    if (activeCharacter == null)
                        continue;

                    if (activeCharacter.CharacterType == CharacterType.DefaultPlayer)
                        continue;

                    if (activeCharacter.LiveComponent == null)
                        continue;

                    if (!activeCharacter.LiveComponent.IsAlive)
                        continue;

                    float distance = Vector3.Distance(
                        activeCharacter.transform.position,
                        transform.position);

                    if (distance < nearest)
                    {
                        nearest = distance;
                        target = activeCharacter;
                    }
                }

                return target;
            }
        }

        public override void Initialize()
        {
            if (_characterData == null)
            {
                _characterData = GetComponent<CharacterData>();
            }

            base.Initialize();

            LiveComponent = new CharacterLiveComponent(this);

            AttackComponent = new ProjectileAttackComponent();
            AttackComponent.Initialize(_characterData);

            AbilityManager = new AbilityManager(this);
            AttackModifierController = new AttackModifierController();

            LiveComponent.OnCharacterDeath += OnDeath;
        }

        public override void Update()
        {
            UpdateInvulnerability();
            if (LiveComponent == null)
                return;

            if (!LiveComponent.IsAlive)
                return;

            StatusEffectController?.OnUpdate();

            HandleMovement();
            HandleAttack();

            AttackComponent?.OnUpdate();
            AbilityManager?.OnUpdate();
        }

        private void HandleMovement()
        {
            float moveHorizontal = Input.GetAxisRaw("Horizontal");
            float moveVertical = Input.GetAxisRaw("Vertical");

            if (MobileJoystick.Instance != null)
            {
                moveHorizontal += MobileJoystick.Instance.InputVector.x;
                moveVertical += MobileJoystick.Instance.InputVector.y;
            }

            Vector3 movementVector = new Vector3(
                moveHorizontal,
                0f,
                moveVertical);

            movementVector = Vector3.ClampMagnitude(movementVector, 1f);

            UpdateSpriteDirection(movementVector);

            MovableComponent?.Move(movementVector);

            if (Animator != null)
            {
                Animator.SetFloat(
                    AnimatorHashes.Speed,
                    movementVector.magnitude);
            }
        }

        private void HandleAttack()
        {
            if (CharacterTarget == null)
                return;

            if (_characterData == null)
                return;

            if (_characterData.WeaponData == null)
                return;

            Vector3 attackDirection =
                CharacterTarget.transform.position - transform.position;

            attackDirection.y = 0f;

            float xDistance = attackDirection.x;
            float zDistance = attackDirection.z / 1.35f;

            float correctedDistance =
                Mathf.Sqrt(xDistance * xDistance + zDistance * zDistance);

            if (correctedDistance >
                _characterData.WeaponData.AttackDistance)
                return;

            UpdateSpriteDirection(attackDirection);

            if (AttackComponent.MakeDamage(CharacterTarget))
            {
                PlayAttackAnimation();
            }
        }

        private void PlayAttackAnimation()
        {
            if (Animator != null)
            {
                Animator.SetTrigger(AnimatorHashes.Attack);
            }
        }

        public void AddAttackModifier(
            AttackModifierUpgradeData upgradeData)
        {
            if (upgradeData == null)
                return;

            if (AttackModifierController == null)
                return;

            AttackModifierController.AddModifier(upgradeData);
        }

        public void ClearAbilities()
        {
            AbilityManager?.ClearAbilities();
            AttackModifierController?.Clear();
        }

        private void UpdateInvulnerability()
        {
            if (!_isInvulnerable)
                return;

            _invulnerabilityTimer -= Time.deltaTime;

            if (_invulnerabilityTimer <= 0f)
            {
                _isInvulnerable = false;
            }
        }

        public void EnableTemporaryInvulnerability(float duration)
        {
            _isInvulnerable = true;
            _invulnerabilityTimer = duration;
        }

        
    }
}