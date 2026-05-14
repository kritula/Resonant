using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "RicochetModifierData", menuName = "Attack Modifiers/Ricochet Data")]
    public class RicochetModifierData : AttackModifierData
    {
        [Header("Level 1")]
        [SerializeField] private int _level1RicochetCount = 1;
        [SerializeField] private float _level1SearchRadius = 5f;
        [SerializeField] private float _level1DamageMultiplier = 0.85f;
        [SerializeField] private bool _level1Homing = false;

        [Header("Level 2")]
        [SerializeField] private int _level2RicochetCount = 2;
        [SerializeField] private float _level2SearchRadius = 5.5f;
        [SerializeField] private float _level2DamageMultiplier = 0.85f;
        [SerializeField] private bool _level2Homing = false;

        [Header("Level 3")]
        [SerializeField] private int _level3RicochetCount = 2;
        [SerializeField] private float _level3SearchRadius = 5.5f;
        [SerializeField] private float _level3DamageMultiplier = 1f;
        [SerializeField] private bool _level3Homing = false;

        [Header("Level 4")]
        [SerializeField] private int _level4RicochetCount = 3;
        [SerializeField] private float _level4SearchRadius = 6f;
        [SerializeField] private float _level4DamageMultiplier = 1f;
        [SerializeField] private bool _level4Homing = false;

        [Header("Level 5")]
        [SerializeField] private int _level5RicochetCount = 3;
        [SerializeField] private float _level5SearchRadius = 7f;
        [SerializeField] private float _level5DamageMultiplier = 1f;
        [SerializeField] private bool _level5Homing = true;

        public int GetRicochetCount(int level)
        {
            switch (level)
            {
                case 1: return _level1RicochetCount;
                case 2: return _level2RicochetCount;
                case 3: return _level3RicochetCount;
                case 4: return _level4RicochetCount;
                case 5: return _level5RicochetCount;
                default: return 0;
            }
        }

        public float GetSearchRadius(int level)
        {
            switch (level)
            {
                case 1: return _level1SearchRadius;
                case 2: return _level2SearchRadius;
                case 3: return _level3SearchRadius;
                case 4: return _level4SearchRadius;
                case 5: return _level5SearchRadius;
                default: return 0f;
            }
        }

        public float GetDamageMultiplier(int level)
        {
            switch (level)
            {
                case 1: return _level1DamageMultiplier;
                case 2: return _level2DamageMultiplier;
                case 3: return _level3DamageMultiplier;
                case 4: return _level4DamageMultiplier;
                case 5: return _level5DamageMultiplier;
                default: return 1f;
            }
        }

        public bool HasHoming(int level)
        {
            switch (level)
            {
                case 1: return _level1Homing;
                case 2: return _level2Homing;
                case 3: return _level3Homing;
                case 4: return _level4Homing;
                case 5: return _level5Homing;
                default: return false;
            }
        }

        public bool HasNoDamageFalloff(int level)
        {
            return level >= 3;
        }
    }
}