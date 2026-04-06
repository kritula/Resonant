using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class UpgradeSelectionService
    {
        private UpgradeDatabase _database;

        public UpgradeSelectionService(UpgradeDatabase database)
        {
            _database = database;
        }

        public List<UpgradeData> GetRandomUpgrades(int count)
        {
            List<UpgradeData> result = new List<UpgradeData>();
            List<UpgradeData> pool = new List<UpgradeData>(_database.Upgrades);

            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                int index = Random.Range(0, pool.Count);
                result.Add(pool[index]);
                pool.RemoveAt(index);
            }

            return result;
        }
    }
}