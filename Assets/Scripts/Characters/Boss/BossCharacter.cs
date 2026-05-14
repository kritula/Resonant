using UnityEngine;

namespace OmniumLessons
{
    public class BossCharacter : EnemyCharacter
    {
        private BossAbilityController _abilityController;
        private BossNullCoreData _bossData;

        public override void Initialize()
        {
            base.Initialize();

            BossCharacterDataAdapter adapter = _characterData as BossCharacterDataAdapter;

            if (adapter == null)
            {
                Debug.LogError("BossCharacter: CharacterData is not BossCharacterDataAdapter!", this);
                return;
            }

            _bossData = adapter.BossData;

            _abilityController = new BossAbilityController(this, _bossData);
        }

        public override void Update()
        {
            base.Update();

            if (LiveComponent == null || !LiveComponent.IsAlive)
                return;

            _abilityController?.OnUpdate(Time.deltaTime);
        }
    }
}