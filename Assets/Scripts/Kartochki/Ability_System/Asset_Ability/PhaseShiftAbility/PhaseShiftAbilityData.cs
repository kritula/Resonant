using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "PhaseShiftAbilityData", menuName = "Abilities/Phase Shift Data")]
    public class PhaseShiftAbilityData : ScriptableObject
    {
        [Header("Base (Lv1)")]
        [SerializeField] private float _basePhaseDuration = 0.5f;
        [SerializeField] private float _baseCooldown = 8f;

        [Header("Level 2")]
        [SerializeField] private float _level2PhaseDuration = 0.9f;

        [Header("Level 3")]
        [SerializeField] private float _level3SpeedMultiplier = 1.35f;

        [Header("Level 4")]
        [SerializeField] private float _level4CooldownMultiplier = 0.7f;

        [Header("Level 5")]
        [SerializeField] private float _level5ExitDamage = 20f;
        [SerializeField] private float _level5ExitDamageRadius = 2.5f;

        public float BasePhaseDuration => _basePhaseDuration;
        public float BaseCooldown => _baseCooldown;

        public float Level2PhaseDuration => _level2PhaseDuration;
        public float Level3SpeedMultiplier => _level3SpeedMultiplier;
        public float Level4CooldownMultiplier => _level4CooldownMultiplier;

        public float Level5ExitDamage => _level5ExitDamage;
        public float Level5ExitDamageRadius => _level5ExitDamageRadius;
    }
}