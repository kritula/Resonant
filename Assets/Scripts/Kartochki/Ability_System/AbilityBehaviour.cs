using UnityEngine;

namespace OmniumLessons
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        protected PlayerCharacter _owner;
        protected AbilityUpgradeData _abilityData;

        public virtual void Initialize(PlayerCharacter owner, AbilityUpgradeData abilityData)
        {
            _owner = owner;
            _abilityData = abilityData;
        }

        public virtual void OnUpdate()
        {
        }
    }
}