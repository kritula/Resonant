using UnityEngine;

namespace OmniumLessons
{
    public abstract class UpgradeData : ScriptableObject
    {
        [SerializeField] private string _upgradeName;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;

        public string UpgradeName => _upgradeName;
        public string Description => _description;
        public Sprite Icon => _icon;

        public abstract UpgradeType UpgradeType { get; }
    }
}