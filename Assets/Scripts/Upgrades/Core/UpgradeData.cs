using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public abstract class UpgradeData : ScriptableObject
    {
        [SerializeField] private string _upgradeName;
        [SerializeField][TextArea(2, 4)] private string _description;

        [Header("Visuals")]
        [SerializeField] private Sprite _cardSprite;

        [Header("Rarity")]
        [SerializeField] private UpgradeRarity _rarity = UpgradeRarity.Common;

        [Header("Per Level Descriptions")]
        [SerializeField][TextArea(2, 4)] private List<string> _levelDescriptions = new List<string>();

        public string UpgradeName => _upgradeName;
        public string Description => _description;

        public Sprite CardSprite => _cardSprite;

        public UpgradeRarity Rarity => _rarity;
        public List<string> LevelDescriptions => _levelDescriptions;

        public abstract UpgradeType UpgradeType { get; }

        public virtual string GetDescriptionForLevel(int level)
        {
            if (_levelDescriptions == null || _levelDescriptions.Count == 0)
                return _description;

            int index = Mathf.Clamp(level - 1, 0, _levelDescriptions.Count - 1);

            if (string.IsNullOrWhiteSpace(_levelDescriptions[index]))
                return _description;

            return _levelDescriptions[index];
        }

        public virtual string GetNextLevelDescription(int currentLevel, int maxLevel)
        {
            int nextLevel = Mathf.Clamp(currentLevel + 1, 1, Mathf.Max(1, maxLevel));
            return GetDescriptionForLevel(nextLevel);
        }
    }
}