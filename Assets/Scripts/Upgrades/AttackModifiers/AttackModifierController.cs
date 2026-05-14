using System.Collections.Generic;

namespace OmniumLessons
{
    public class AttackModifierController
    {
        private readonly Dictionary<AttackModifierType, int> _modifierLevels = new Dictionary<AttackModifierType, int>();
        private readonly Dictionary<AttackModifierType, AttackModifierData> _modifierDataMap = new Dictionary<AttackModifierType, AttackModifierData>();

        public int GetModifierLevel(AttackModifierType modifierType)
        {
            if (_modifierLevels.TryGetValue(modifierType, out int level))
                return level;

            return 0;
        }

        public void AddModifier(AttackModifierUpgradeData upgradeData)
        {
            if (upgradeData == null)
                return;

            AttackModifierType modifierType = upgradeData.ModifierType;

            if (modifierType == AttackModifierType.None)
                return;

            if (upgradeData.ModifierData != null)
            {
                _modifierDataMap[modifierType] = upgradeData.ModifierData;
            }

            int currentLevel = GetModifierLevel(modifierType);

            if (currentLevel >= upgradeData.MaxLevel)
                return;

            _modifierLevels[modifierType] = currentLevel + 1;
        }

        public bool HasModifier(AttackModifierType modifierType)
        {
            return GetModifierLevel(modifierType) > 0;
        }

        public bool IsModifierMaxLevel(AttackModifierType modifierType)
        {
            return GetModifierLevel(modifierType) >= 5;
        }

        public void Clear()
        {
            _modifierLevels.Clear();
            _modifierDataMap.Clear();
        }

        public AttackShotData BuildShotData()
        {
            AttackShotData shotData = new AttackShotData
            {
                ProjectileCount = 1,
                SpreadAngle = 0f,
                AttackCooldownMultiplier = 1f,

                PierceCount = 0,
                InfinitePierce = false,
                PierceDamageFalloff = false,
                PierceDamageFalloffPerTarget = 0f,
                PierceBonusAfterFirstPierce = false,
                PierceBonusMultiplierAfterFirstPierce = 1f,

                RicochetCount = 0,
                RicochetSearchRadius = 5f,
                RicochetDamageMultiplier = 0.85f,
                RicochetNoDamageFalloff = false,
                RicochetHoming = false
            };

            ApplyPiercing(ref shotData);
            ApplyRicochet(ref shotData);
            ApplyDoubleShot(ref shotData);

            return shotData;
        }

        private void ApplyPiercing(ref AttackShotData shotData)
        {
            int level = GetModifierLevel(AttackModifierType.Piercing);

            if (level <= 0)
                return;

            if (!_modifierDataMap.TryGetValue(AttackModifierType.Piercing, out AttackModifierData baseData))
                return;

            PiercingModifierData data = baseData as PiercingModifierData;

            if (data == null)
                return;

            shotData.PierceCount = data.GetPierceCount(level);
            shotData.InfinitePierce = data.HasInfinitePierce(level);
            shotData.PierceDamageFalloff = data.HasDamageFalloff(level);
            shotData.PierceDamageFalloffPerTarget = data.GetDamageFalloffPerTarget(level);
            shotData.PierceBonusAfterFirstPierce = data.HasBonusAfterFirstPierce(level);
            shotData.PierceBonusMultiplierAfterFirstPierce = data.GetBonusMultiplierAfterFirstPierce(level);
        }

        private void ApplyRicochet(ref AttackShotData shotData)
        {
            int level = GetModifierLevel(AttackModifierType.Ricochet);

            if (level <= 0)
                return;

            if (!_modifierDataMap.TryGetValue(AttackModifierType.Ricochet, out AttackModifierData baseData))
                return;

            RicochetModifierData data = baseData as RicochetModifierData;

            if (data == null)
                return;

            shotData.RicochetCount = data.GetRicochetCount(level);
            shotData.RicochetSearchRadius = data.GetSearchRadius(level);
            shotData.RicochetDamageMultiplier = data.GetDamageMultiplier(level);
            shotData.RicochetNoDamageFalloff = data.HasNoDamageFalloff(level);
            shotData.RicochetHoming = data.HasHoming(level);
        }

        private void ApplyDoubleShot(ref AttackShotData shotData)
        {
            int level = GetModifierLevel(AttackModifierType.DoubleShot);

            if (level <= 0)
                return;

            if (!_modifierDataMap.TryGetValue(AttackModifierType.DoubleShot, out AttackModifierData baseData))
                return;

            DoubleShotModifierData data = baseData as DoubleShotModifierData;

            if (data == null)
                return;

            shotData.ProjectileCount = data.GetProjectileCount(level);
            shotData.SpreadAngle = data.GetSpreadAngle(level);
            shotData.AttackCooldownMultiplier = data.GetAttackCooldownMultiplier(level);
        }
    }
}