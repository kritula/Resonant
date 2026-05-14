using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(
        fileName = "CollapseHexZoneData",
        menuName = "OmniumLessons/Hex Tiles/Collapse Hex Zone Data")]
    public class CollapseHexZoneData : ActiveHexZoneData
    {
        [Header("Materials")]
        public Material ReadyMaterial;
        public Material CrackMaterial;

        [Header("Collapse")]
        public float DelayAfterStep = 2f;
        public float MinHoleLifetime = 5f;
        public float MaxHoleLifetime = 8f;

        private void OnValidate()
        {
            ZoneType = HexActiveZoneType.CollapseTile;
        }
    }
}
