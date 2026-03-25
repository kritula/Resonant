using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class AbilitySelectionService
    {
        private AbilityDatabase _database;

        public AbilitySelectionService(AbilityDatabase database)
        {
            _database = database;
        }

        public List<AbilityData> GetRandomAbilities(int count)
        {
            List<AbilityData> result = new List<AbilityData>();
            List<AbilityData> pool = new List<AbilityData>(_database.Abilities);

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
