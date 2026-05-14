using UnityEngine;

namespace OmniumLessons
{
    public class ActiveHexZoneData : ScriptableObject
    {
        [Header("Zone")]
        public HexActiveZoneType ZoneType = HexActiveZoneType.None;

        [Header("Spawn Limits")]
        public int MaxActiveInstances = 1;

        [Header("Lifetime")]
        public float Lifetime = 30f;

        [Header("Respawn After Complete")]
        public float MinRespawnDelay = 1f;
        public float MaxRespawnDelay = 1f;
    }
}
