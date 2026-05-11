using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(
        fileName = "ActiveHexZoneData",
        menuName = "OmniumLessons/Hex Tiles/Active Hex Zone Data")]
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

        [Header("Resonance Zone Materials")]
        public Material ResonanceReadyMaterial;
        public Material ResonanceActiveMaterial;

        [Header("Resonance Zone")]
        public float ResonanceActivationDuration = 8f;

        [Range(0.1f, 1f)] public float MinDamageBonus = 0.10f;
        [Range(0.1f, 1f)] public float MaxDamageBonus = 0.30f;

        [Range(0.1f, 1f)] public float MinAttackSpeedBonus = 0.10f;
        [Range(0.1f, 1f)] public float MaxAttackSpeedBonus = 0.25f;

        [Header("Collapse Tile")]
        public Material CollapseCrackMaterial;
        public float CollapseDelayAfterStep = 2f;
        public float MinHoleLifetime = 5f;
        public float MaxHoleLifetime = 8f;
    }
}