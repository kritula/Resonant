using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "PiercingModifierData", menuName = "Attack Modifiers/Piercing Data")]
    public class PiercingModifierData : AttackModifierData
    {
        [Header("Level 1")]
        [SerializeField] private int _level1PierceCount = 1;
        [SerializeField] private bool _level1InfinitePierce = false;
        [SerializeField] private bool _level1DamageFalloff = false;
        [SerializeField] private float _level1DamageFalloffPerTarget = 0f;
        [SerializeField] private bool _level1BonusAfterFirstPierce = false;
        [SerializeField] private float _level1BonusMultiplierAfterFirstPierce = 1f;

        [Header("Level 2")]
        [SerializeField] private int _level2PierceCount = 2;
        [SerializeField] private bool _level2InfinitePierce = false;
        [SerializeField] private bool _level2DamageFalloff = false;
        [SerializeField] private float _level2DamageFalloffPerTarget = 0f;
        [SerializeField] private bool _level2BonusAfterFirstPierce = false;
        [SerializeField] private float _level2BonusMultiplierAfterFirstPierce = 1f;

        [Header("Level 3")]
        [SerializeField] private int _level3PierceCount = 3;
        [SerializeField] private bool _level3InfinitePierce = false;
        [SerializeField] private bool _level3DamageFalloff = false;
        [SerializeField] private float _level3DamageFalloffPerTarget = 0f;
        [SerializeField] private bool _level3BonusAfterFirstPierce = false;
        [SerializeField] private float _level3BonusMultiplierAfterFirstPierce = 1f;

        [Header("Level 4")]
        [SerializeField] private int _level4PierceCount = 3;
        [SerializeField] private bool _level4InfinitePierce = false;
        [SerializeField] private bool _level4DamageFalloff = false;
        [SerializeField] private float _level4DamageFalloffPerTarget = 0f;
        [SerializeField] private bool _level4BonusAfterFirstPierce = true;
        [SerializeField] private float _level4BonusMultiplierAfterFirstPierce = 1.3f;

        [Header("Level 5")]
        [SerializeField] private int _level5PierceCount = 0;
        [SerializeField] private bool _level5InfinitePierce = true;
        [SerializeField] private bool _level5DamageFalloff = true;
        [SerializeField] private float _level5DamageFalloffPerTarget = 0.1f;
        [SerializeField] private bool _level5BonusAfterFirstPierce = false;
        [SerializeField] private float _level5BonusMultiplierAfterFirstPierce = 1f;

        public int GetPierceCount(int level)
        {
            switch (level)
            {
                case 1: return _level1PierceCount;
                case 2: return _level2PierceCount;
                case 3: return _level3PierceCount;
                case 4: return _level4PierceCount;
                case 5: return _level5PierceCount;
                default: return 0;
            }
        }

        public bool HasInfinitePierce(int level)
        {
            switch (level)
            {
                case 1: return _level1InfinitePierce;
                case 2: return _level2InfinitePierce;
                case 3: return _level3InfinitePierce;
                case 4: return _level4InfinitePierce;
                case 5: return _level5InfinitePierce;
                default: return false;
            }
        }

        public bool HasDamageFalloff(int level)
        {
            switch (level)
            {
                case 1: return _level1DamageFalloff;
                case 2: return _level2DamageFalloff;
                case 3: return _level3DamageFalloff;
                case 4: return _level4DamageFalloff;
                case 5: return _level5DamageFalloff;
                default: return false;
            }
        }

        public float GetDamageFalloffPerTarget(int level)
        {
            switch (level)
            {
                case 1: return _level1DamageFalloffPerTarget;
                case 2: return _level2DamageFalloffPerTarget;
                case 3: return _level3DamageFalloffPerTarget;
                case 4: return _level4DamageFalloffPerTarget;
                case 5: return _level5DamageFalloffPerTarget;
                default: return 0f;
            }
        }

        public bool HasBonusAfterFirstPierce(int level)
        {
            switch (level)
            {
                case 1: return _level1BonusAfterFirstPierce;
                case 2: return _level2BonusAfterFirstPierce;
                case 3: return _level3BonusAfterFirstPierce;
                case 4: return _level4BonusAfterFirstPierce;
                case 5: return _level5BonusAfterFirstPierce;
                default: return false;
            }
        }

        public float GetBonusMultiplierAfterFirstPierce(int level)
        {
            switch (level)
            {
                case 1: return _level1BonusMultiplierAfterFirstPierce;
                case 2: return _level2BonusMultiplierAfterFirstPierce;
                case 3: return _level3BonusMultiplierAfterFirstPierce;
                case 4: return _level4BonusMultiplierAfterFirstPierce;
                case 5: return _level5BonusMultiplierAfterFirstPierce;
                default: return 1f;
            }
        }
    }
}