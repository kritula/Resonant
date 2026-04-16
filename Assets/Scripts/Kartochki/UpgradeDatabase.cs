using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "UpgradeDatabase", menuName = "ZombieIO/Upgrade Database")]
    public class UpgradeDatabase : ScriptableObject
    {
        [SerializeField] private List<UpgradeData> _upgrades = new List<UpgradeData>();

        public List<UpgradeData> Upgrades => _upgrades;
    }
}