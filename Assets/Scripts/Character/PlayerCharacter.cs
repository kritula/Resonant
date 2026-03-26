using UnityEngine;

namespace OmniumLessons
{
    public class PlayerCharacter : Character
    {
        public AbilityManager AbilityManager { get; private set; }

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

            AttackComponent = new CharacterAttackComponent();
            AttackComponent.Initialize(_characterData);

            AbilityManager = new AbilityManager(this);
        }

        public void ClearAbilities()
        {
            AbilityManager?.ClearAbilities();
        }

        public override void Update()
        {
            if (!LiveComponent.IsAlive)
                return;

            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movementVector = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

            MovableComponent.Move(movementVector);
            MovableComponent.Rotation(movementVector);

            if (CharacterTarget == null)
            {
                MovableComponent.Rotation(movementVector);
            }
            else
            {
                Vector3 directionToTarget = CharacterTarget.transform.position - transform.position;
                directionToTarget.Normalize();
                MovableComponent.Rotation(directionToTarget);

                float distance = Vector3.Distance(transform.position, CharacterTarget.transform.position);

                if (distance <= _characterData.WeaponData.AttackDistance)
                {
                    AttackComponent.MakeDamage(CharacterTarget);
                }
            }

            AttackComponent.OnUpdate();
            AbilityManager?.OnUpdate();
        }
    }
}