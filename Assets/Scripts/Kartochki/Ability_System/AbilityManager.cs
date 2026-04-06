using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class AbilityManager
    {
        private PlayerCharacter _owner;
        private readonly List<AbilityBehaviour> _activeAbilities = new List<AbilityBehaviour>();

        public AbilityManager(PlayerCharacter owner)
        {
            _owner = owner;
        }

        public void AddAbility(AbilityUpgradeData abilityData)
        {
            if (abilityData == null)
                return;

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
        }
    }
}