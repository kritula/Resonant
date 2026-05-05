using UnityEngine;

namespace OmniumLessons
{
    public class PlayerCharacter : Character
    {
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
                    if (activeCharacter.CharacterType == CharacterType.DefaultPlayer)
                        continue;

                    if (!activeCharacter.LiveComponent.IsAlive)
                        continue;

                    float distance = Vector3.Distance(activeCharacter.transform.position, transform.position);
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
            if (!LiveComponent.IsAlive)
                return;

            StatusEffectController?.OnUpdate();

            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movementVector = new Vector3(moveHorizontal, 0f, moveVertical);

            if (movementVector.sqrMagnitude > 1f)
                movementVector.Normalize();

            UpdateSpriteDirection(movementVector);
            MovableComponent.Move(movementVector);

            float speed = movementVector.magnitude;

            Animator.SetFloat(AnimatorHashes.Speed, speed);

            if (CharacterTarget != null)
            {
                Vector3 attackDirection = CharacterTarget.transform.position - transform.position;
                attackDirection.y = 0f;

                float xDistance = attackDirection.x;
                float zDistance = attackDirection.z / 1.35f;

                float correctedDistance = Mathf.Sqrt(xDistance * xDistance + zDistance * zDistance);

                if (correctedDistance <= _characterData.WeaponData.AttackDistance)
                {
                    UpdateSpriteDirection(attackDirection); // 👈 ВСТАВЛЯЕМ СЮДА

                    if (AttackComponent.MakeDamage(CharacterTarget))
                    {
                        PlayAttackAnimation();
                    }
                }
            }

            AttackComponent.OnUpdate();
            AbilityManager?.OnUpdate();
        }

        private void PlayAttackAnimation()
        {
            if (Animator != null)
            {
                Animator.SetTrigger(AnimatorHashes.Attack);
            }
        }
        public void AddAttackModifier(AttackModifierUpgradeData upgradeData)
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
    }
}