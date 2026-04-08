using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(menuName = "Upgrades/Ability Upgrade")]
    public class AbilityUpgradeData : UpgradeData
    {
        [SerializeField] private int _maxLevel = 1;
        [SerializeField] private GameObject _abilityPrefab;

        public int MaxLevel => Mathf.Max(1, _maxLevel);
        public GameObject AbilityPrefab => _abilityPrefab;

        public override UpgradeType UpgradeType => UpgradeType.Ability;
    }
}