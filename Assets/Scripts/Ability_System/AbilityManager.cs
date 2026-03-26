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

            GameObject abilityObject = GameObject.Instantiate(
                abilityData.AbilityPrefab,
                _owner.transform.position,
                Quaternion.identity,
                _owner.transform
            );

            AbilityBehaviour ability = abilityObject.GetComponent<AbilityBehaviour>();

            if (ability == null)
            {
                Debug.LogError($"AbilityBehaviour not found on prefab {abilityData.AbilityPrefab.name}");
                GameObject.Destroy(abilityObject);
                return;
            }

            ability.Initialize(_owner, abilityData);
            _abilities.Add(ability);
        }

        public void OnUpdate()
        {
            for (int i = 0; i < _abilities.Count; i++)
            {
                if (_abilities[i] != null)
                {
                    _abilities[i].OnUpdate();
                }
            }
        }

        public void ClearAbilities()
        {
            for (int i = 0; i < _abilities.Count; i++)
            {
                if (_abilities[i] != null)
                {
                    GameObject.Destroy(_abilities[i].gameObject);
                }
            }

            _abilities.Clear();
        }
    }
}