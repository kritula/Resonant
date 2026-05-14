using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(
        fileName = "PullNodeHexZoneData",
        menuName = "OmniumLessons/Hex Tiles/Pull Node Hex Zone Data")]
    public class PullNodeHexZoneData : ActiveHexZoneData
    {
        [Header("Visual")]
        public Material ReadyMaterial;
        public Material ActiveMaterial;

        [Header("Activation")]
        public float ActivationDuration = 6f;
        public float ActivatedRespawnDelay = 15f;

        [Header("Pull")]
        [Min(0)]
        public int PullExtraTileRings = 1;

        public float PullRadius = 9.6f;
        public float PullSpeed = 4.5f;

        [Range(0f, 0.9f)]
        public float EnemyMoveResistance = 0.35f;

        public float CenterStopRadius = 0.35f;

        private void OnValidate()
        {
            ZoneType = HexActiveZoneType.PullNodeTile;
        }
    }
}
