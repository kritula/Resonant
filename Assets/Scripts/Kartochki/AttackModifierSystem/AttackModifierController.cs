namespace OmniumLessons
{
    public class AttackModifierController
    {
        public int PiercingCount { get; private set; }
        public int RicochetCount { get; private set; }
        public int AdditionalProjectilesCount { get; private set; }

        public float DamageMultiplier { get; private set; } = 1f;
        public float AttackCooldownMultiplier { get; private set; } = 1f;

        public void Reset()
        {
            PiercingCount = 0;
            RicochetCount = 0;
            AdditionalProjectilesCount = 0;

            DamageMultiplier = 1f;
            AttackCooldownMultiplier = 1f;
        }

        public void ApplyModifier(AttackModifierUpgradeData upgradeData)
        {
            if (upgradeData == null)
                return;

            switch (upgradeData.AttackModifierType)
            {
                case AttackModifierType.Piercing:
                    PiercingCount += (int)upgradeData.ModifierValue;
                    break;

                case AttackModifierType.Ricochet:
                    RicochetCount += (int)upgradeData.ModifierValue;
                    break;

                case AttackModifierType.DamageBoost:
                    DamageMultiplier += upgradeData.ModifierValue;
                    break;

                case AttackModifierType.AttackSpeed:
                    AttackCooldownMultiplier -= upgradeData.ModifierValue;
                    if (AttackCooldownMultiplier < 0.1f)
                        AttackCooldownMultiplier = 0.1f;
                    break;

                case AttackModifierType.AdditionalProjectiles:
                    AdditionalProjectilesCount += (int)upgradeData.ModifierValue;
                    break;
            }
        }

        public AttackShotData BuildShotData(float baseDamage, PlayerWeaponData weaponData)
        {
            if (weaponData == null)
                return null;

            float finalDamage = baseDamage * DamageMultiplier;

            return new AttackShotData(
                finalDamage,
                PiercingCount,
                RicochetCount,
                AdditionalProjectilesCount,
                weaponData.RicochetRadius,
                AttackCooldownMultiplier);
        }
    }
}