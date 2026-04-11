namespace OmniumLessons
{
    public struct AttackShotData
    {
        public int ProjectileCount;
        public float SpreadAngle;
        public float AttackCooldownMultiplier;

        public int PierceCount;
        public bool InfinitePierce;
        public bool PierceDamageFalloff;
        public float PierceDamageFalloffPerTarget;
        public bool PierceBonusAfterFirstPierce;
        public float PierceBonusMultiplierAfterFirstPierce;

        public int RicochetCount;
        public float RicochetDamageMultiplier;
        public bool RicochetNoDamageFalloff;
        public float RicochetSearchRadius;
        public bool RicochetHoming;
    }
}