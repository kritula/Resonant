using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class AbilityManager
    {
        private Character _owner;

        private List<AbilityBehaviour> _abilities = new List<AbilityBehaviour>();

        public AbilityManager(Character owner)
        {
            _owner = owner;
        }

        public void AddAbility(AbilityData abilityData)
        {
            if (abilityData == null)
            {
                Debug.LogError("AbilityData is null");
                return;
            }

            if (abilityData.AbilityPrefab == null)
            {
                Debug.LogError($"AbilityPrefab is not assigned for {abilityData.name}");
                return;
            }
            GameObject abilityObject = GameObject.Instantiate(abilityData.AbilityPrefab);
            AbilityBehaviour ability = abilityObject.GetComponent<AbilityBehaviour>();

            ability.Initialize(_owner, abilityData);

            _abilities.Add(ability);
        }
        public void OnUpdate()
        {
            for (int i = 0; i < _abilities.Count; i++)
            {
                if (_abilities[i] != null)
                    _abilities[i].OnUpdate();
            }
        }
    }
}