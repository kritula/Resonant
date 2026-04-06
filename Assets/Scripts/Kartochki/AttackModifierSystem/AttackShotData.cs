namespace OmniumLessons
{
    public class AttackShotData
    {
        public float FinalDamage { get; private set; }
        public int PiercingCount { get; private set; }
        public int RicochetCount { get; private set; }
        public int AdditionalProjectilesCount { get; private set; }
        public float RicochetRadius { get; private set; }
        public float AttackCooldownMultiplier { get; private set; }

        public AttackShotData(
            float finalDamage,
            int piercingCount,
            int ricochetCount,
            int additionalProjectilesCount,
            float ricochetRadius,
            float attackCooldownMultiplier)
        {
            FinalDamage = finalDamage;
            PiercingCount = piercingCount;
            RicochetCount = ricochetCount;
            AdditionalProjectilesCount = additionalProjectilesCount;
            RicochetRadius = ricochetRadius;
            AttackCooldownMultiplier = attackCooldownMultiplier;
        }
    }
}