using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(menuName = "Upgrades/Attack Modifier Upgrade")]
    public class AttackModifierUpgradeData : UpgradeData
    {
        [SerializeField] private AttackModifierType _modifierType;
        [SerializeField] private int _maxLevel = 5;
        [SerializeField] private AttackModifierData _modifierData;

        public AttackModifierType ModifierType => _modifierType;
        public int MaxLevel => Mathf.Max(1, _maxLevel);
        public AttackModifierData ModifierData => _modifierData;

        public override UpgradeType UpgradeType => UpgradeType.AttackModifier;
    }
}