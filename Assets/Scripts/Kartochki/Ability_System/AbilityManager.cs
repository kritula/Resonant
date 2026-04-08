using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class AbilityManager
    {
        private readonly PlayerCharacter _owner;
        private readonly List<AbilityBehaviour> _activeAbilities = new List<AbilityBehaviour>();
        private readonly Dictionary<AbilityUpgradeData, AbilityBehaviour> _abilityMap = new Dictionary<AbilityUpgradeData, AbilityBehaviour>();

        public AbilityManager(PlayerCharacter owner)
        {
            _owner = owner;
        }

        public void AddAbility(AbilityUpgradeData abilityData)
        {
            if (abilityData == null)
                return;

            if (_abilityMap.TryGetValue(abilityData, out AbilityBehaviour existingAbility))
            {
                if (existingAbility != null)
                {
                    existingAbility.TryUpgrade();
                    return;
                }

                _abilityMap.Remove(abilityData);
            }

            if (abilityData.AbilityPrefab == null)
                return;

            GameObject abilityObject = Object.Instantiate(abilityData.AbilityPrefab, _owner.transform);
            AbilityBehaviour abilityBehaviour = abilityObject.GetComponent<AbilityBehaviour>();

            if (abilityBehaviour == null)
            {
                Object.Destroy(abilityObject);
                return;
            }

            abilityBehaviour.Initialize(_owner, abilityData);

            _activeAbilities.Add(abilityBehaviour);
            _abilityMap[abilityData] = abilityBehaviour;
        }

        public bool HasAbility(AbilityUpgradeData abilityData)
        {
            if (abilityData == null)
                return false;

            return _abilityMap.ContainsKey(abilityData) && _abilityMap[abilityData] != null;
        }

        public int GetAbilityLevel(AbilityUpgradeData abilityData)
        {
            if (abilityData == null)
                return 0;

            if (_abilityMap.TryGetValue(abilityData, out AbilityBehaviour ability) && ability != null)
                return ability.CurrentLevel;

            return 0;
        }

        public bool IsAbilityMaxLevel(AbilityUpgradeData abilityData)
        {
            if (abilityData == null)
                return false;

            if (_abilityMap.TryGetValue(abilityData, out AbilityBehaviour ability) && ability != null)
                return !ability.CanUpgrade;

            return false;
        }

        public void OnUpdate()
        {
            for (int i = 0; i < _activeAbilities.Count; i++)
            {
                if (_activeAbilities[i] == null)
                    continue;

                _activeAbilities[i].OnUpdate();
            }
        }

        public void ClearAbilities()
        {
            for (int i = 0; i < _activeAbilities.Count; i++)
            {
                if (_activeAbilities[i] != null)
                {
                    Object.Destroy(_activeAbilities[i].gameObject);
                }
            }

            _activeAbilities.Clear();
            _abilityMap.Clear();
        }
    }
}