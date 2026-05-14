namespace OmniumLessons
{
    public class BossAbilityController
    {
        private readonly BossPulseAbility _pulse;
        private readonly BossSpawnAbility _spawn;
        private readonly BossFieldAbility _field;

        public BossAbilityController(BossCharacter boss, BossNullCoreData data)
        {
            _pulse = new BossPulseAbility(boss, data.PulseData);
            _spawn = new BossSpawnAbility(boss, data.SpawnData);
            _field = new BossFieldAbility(boss, data.FieldData);
        }

        public void OnUpdate(float deltaTime)
        {
            _pulse.OnUpdate(deltaTime);
            _spawn.OnUpdate(deltaTime);
            _field.OnUpdate(deltaTime);
        }
    }
}