using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "BossPulseAbilityData", menuName = "ZombieIO/Boss/Boss Pulse Ability Data")]
    public class BossPulseAbilityData : ScriptableObject
    {
        [Header("Spawn")]
        [SerializeField] private BossPulseRing _ringPrefab;

        [Header("Timing")]
        [SerializeField] private float _cooldown = 15f;
        [SerializeField] private float _expandDuration = 1.75f;
        [SerializeField] private float _contractDuration = 1.75f;

        [Header("Shape")]
        [SerializeField] private float _minRadius = 1.25f;
        [SerializeField] private float _maxRadius = 8f;
        [SerializeField] private float _ringThickness = 1.1f;
        [SerializeField][Range(10f, 120f)] private float _gapAngle = 45f;
        [SerializeField] private float _visualHeightOffset = 0.15f;

        [Header("Damage")]
        [SerializeField] private int _damage = 10;
        [SerializeField] private bool _damageOnExpand = true;
        [SerializeField] private bool _damageOnContract = true;

        [Header("Visual")]
        [SerializeField] private int _segments = 100;
        [SerializeField] private Material _lineMaterial;
        [SerializeField] private Color _pulseColor = new Color(0.5f, 0.9f, 1f, 1f);

        public BossPulseRing RingPrefab => _ringPrefab;

        public float Cooldown => Mathf.Max(0.1f, _cooldown);
        public float ExpandDuration => Mathf.Max(0.05f, _expandDuration);
        public float ContractDuration => Mathf.Max(0.05f, _contractDuration);

        public float MinRadius => Mathf.Max(0.1f, _minRadius);
        public float MaxRadius => Mathf.Max(MinRadius, _maxRadius);
        public float RingThickness => Mathf.Max(0.05f, _ringThickness);
        public float GapAngle => _gapAngle;
        public float VisualHeightOffset => _visualHeightOffset;

        public int Damage => Mathf.Max(0, _damage);
        public bool DamageOnExpand => _damageOnExpand;
        public bool DamageOnContract => _damageOnContract;

        public int Segments => Mathf.Max(12, _segments);
        public Material LineMaterial => _lineMaterial;
        public Color PulseColor => _pulseColor;
    }
}