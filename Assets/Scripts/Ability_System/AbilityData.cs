using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(menuName = "Abilities/Ability")]
    public class AbilityData : ScriptableObject
    {
        [SerializeField] private string _abilityName;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;

        [SerializeField] private float _cooldown;
        [SerializeField] private GameObject _abilityPrefab;

        public string AbilityName => _abilityName;
        public string Description => _description;
        public Sprite Icon => _icon;

        public float Cooldown => _cooldown;
        public GameObject AbilityPrefab => _abilityPrefab;
    }
}
