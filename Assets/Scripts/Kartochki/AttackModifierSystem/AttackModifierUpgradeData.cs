using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(menuName = "Upgrades/Attack Modifier Upgrade")]
    public class AttackModifierUpgradeData : UpgradeData
    {
        [SerializeField] private AttackModifierType _attackModifierType;
        [SerializeField] private float _modifierValue = 1f;

        public AttackModifierType AttackModifierType => _attackModifierType;
        public float ModifierValue => _modifierValue;

        public override UpgradeType UpgradeType => UpgradeType.AttackModifier;
    }
}