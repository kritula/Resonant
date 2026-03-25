using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(menuName = "Abilities/AbilityDatabase")]
    public class AbilityDatabase : ScriptableObject
    {
        [SerializeField] private List<AbilityData> _abilities;

        public List<AbilityData> Abilities => _abilities;
    }
}
