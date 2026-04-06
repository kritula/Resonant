using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(menuName = "Upgrades/Ability Upgrade")]
    public class AbilityUpgradeData : UpgradeData
    {
        [SerializeField] private float _cooldown;
        [SerializeField] private GameObject _abilityPrefab;

        public float Cooldown => _cooldown;
        public GameObject AbilityPrefab => _abilityPrefab;

        public override UpgradeType UpgradeType => UpgradeType.Ability;
    }
}