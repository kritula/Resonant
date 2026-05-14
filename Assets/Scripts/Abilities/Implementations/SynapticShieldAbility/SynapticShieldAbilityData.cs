using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "SynapticShieldAbilityData", menuName = "Abilities/Synaptic Shield Data")]
    public class SynapticShieldAbilityData : ScriptableObject
    {
        [Header("Base Settings")]
        [SerializeField] private float _baseRadius = 1.5f;
        [SerializeField] private float _baseCooldown = 2f;
        [SerializeField] private float _baseKnockbackForce = 2.5f;

        [Header("Level Bonuses")]
        [SerializeField] private float _level2RadiusMultiplier = 1.25f;
        [SerializeField] private float _level3Damage = 8f;
        [SerializeField] private float _level4CooldownMultiplier = 0.65f;
        [SerializeField] private float _level5KnockbackMultiplier = 1.5f;
        [SerializeField] private float _level5DamageMultiplier = 1.5f;
        [SerializeField] private float _level5StunDuration = 0.5f;

        public float BaseRadius => _baseRadius;
        public float BaseCooldown => _baseCooldown;
        public float BaseKnockbackForce => _baseKnockbackForce;

        public float Level2RadiusMultiplier => _level2RadiusMultiplier;
        public float Level3Damage => _level3Damage;
        public float Level4CooldownMultiplier => _level4CooldownMultiplier;
        public float Level5KnockbackMultiplier => _level5KnockbackMultiplier;
        public float Level5DamageMultiplier => _level5DamageMultiplier;
        public float Level5StunDuration => _level5StunDuration;
    }
}