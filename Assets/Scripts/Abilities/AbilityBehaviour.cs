using UnityEngine;

namespace OmniumLessons
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        protected PlayerCharacter _owner;
        protected AbilityUpgradeData _abilityData;

        public int CurrentLevel { get; private set; }
        public int MaxLevel => _abilityData != null ? _abilityData.MaxLevel : 1;
        public bool CanUpgrade => CurrentLevel < MaxLevel;

        public virtual void Initialize(PlayerCharacter owner, AbilityUpgradeData abilityData)
        {
            _owner = owner;
            _abilityData = abilityData;

            CurrentLevel = 1;
            ApplyLevel(CurrentLevel);
        }

        public bool TryUpgrade()
        {
            if (!CanUpgrade)
                return false;

            CurrentLevel++;
            ApplyLevel(CurrentLevel);
            return true;
        }

        protected virtual void ApplyLevel(int level)
        {
        }

        public virtual void OnUpdate()
        {
        }
    }
}