using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "DoubleShotModifierData", menuName = "Attack Modifiers/Double Shot Data")]
    public class DoubleShotModifierData : AttackModifierData
    {
        [Header("Level 1")]
        [SerializeField] private int _level1ProjectileCount = 2;
        [SerializeField] private float _level1SpreadAngle = 10f;
        [SerializeField] private float _level1AttackCooldownMultiplier = 1f;

        [Header("Level 2")]
        [SerializeField] private int _level2ProjectileCount = 2;
        [SerializeField] private float _level2SpreadAngle = 20f;
        [SerializeField] private float _level2AttackCooldownMultiplier = 1f;

        [Header("Level 3")]
        [SerializeField] private int _level3ProjectileCount = 3;
        [SerializeField] private float _level3SpreadAngle = 20f;
        [SerializeField] private float _level3AttackCooldownMultiplier = 1f;

        [Header("Level 4")]
        [SerializeField] private int _level4ProjectileCount = 3;
        [SerializeField] private float _level4SpreadAngle = 20f;
        [SerializeField] private float _level4AttackCooldownMultiplier = 0.8f;

        [Header("Level 5")]
        [SerializeField] private int _level5ProjectileCount = 4;
        [SerializeField] private float _level5SpreadAngle = 14f;
        [SerializeField] private float _level5AttackCooldownMultiplier = 0.8f;

        public int GetProjectileCount(int level)
        {
            switch (level)
            {
                case 1: return _level1ProjectileCount;
                case 2: return _level2ProjectileCount;
                case 3: return _level3ProjectileCount;
                case 4: return _level4ProjectileCount;
                case 5: return _level5ProjectileCount;
                default: return 1;
            }
        }

        public float GetSpreadAngle(int level)
        {
            switch (level)
            {
                case 1: return _level1SpreadAngle;
                case 2: return _level2SpreadAngle;
                case 3: return _level3SpreadAngle;
                case 4: return _level4SpreadAngle;
                case 5: return _level5SpreadAngle;
                default: return 0f;
            }
        }

        public float GetAttackCooldownMultiplier(int level)
        {
            switch (level)
            {
                case 1: return _level1AttackCooldownMultiplier;
                case 2: return _level2AttackCooldownMultiplier;
                case 3: return _level3AttackCooldownMultiplier;
                case 4: return _level4AttackCooldownMultiplier;
                case 5: return _level5AttackCooldownMultiplier;
                default: return 1f;
            }
        }
    }
}