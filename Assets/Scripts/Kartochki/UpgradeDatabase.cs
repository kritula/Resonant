using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(menuName = "Upgrades/Upgrade Database")]
    public class UpgradeDatabase : ScriptableObject
    {
        [SerializeField] private List<UpgradeData> _upgrades;

        public List<UpgradeData> Upgrades => _upgrades;
    }
}