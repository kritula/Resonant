namespace OmniumLessons
{
    public struct AttackStats
    {
        public float Damage;
        public float AttackCooldown;

        public AttackStats(float damage, float attackCooldown)
        {
            Damage = damage;
            AttackCooldown = attackCooldown;
        }
    }
}