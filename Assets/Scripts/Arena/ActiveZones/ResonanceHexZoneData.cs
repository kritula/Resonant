using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(
        fileName = "ResonanceHexZoneData",
        menuName = "OmniumLessons/Hex Tiles/Resonance Hex Zone Data")]
    public class ResonanceHexZoneData : ActiveHexZoneData
    {
        [Header("Materials")]
        public Material ReadyMaterial;
        public Material ActiveMaterial;

        [Header("Activation")]
        public float ActivationDuration = 8f;

        [Header("Damage Bonus")]
        [Range(0.1f, 1f)] public float MinDamageBonus = 0.10f;
        [Range(0.1f, 1f)] public float MaxDamageBonus = 0.30f;

        [Header("Attack Speed Bonus")]
        [Range(0.1f, 1f)] public float MinAttackSpeedBonus = 0.10f;
        [Range(0.1f, 1f)] public float MaxAttackSpeedBonus = 0.25f;

        private void OnValidate()
        {
            ZoneType = HexActiveZoneType.ResonanceZone;
        }
    }
}
