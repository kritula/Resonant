using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "BossFieldAbilityData", menuName = "ZombieIO/Boss/Boss Field Ability Data")]
    public class BossFieldAbilityData : ScriptableObject
    {
        [SerializeField] private float _cooldown = 12f;
        [SerializeField] private float _slowPercent = 0.4f;
        [SerializeField] private float _duration = 3f;

        public float Cooldown => Mathf.Max(0.1f, _cooldown);
        public float SlowPercent => Mathf.Clamp01(_slowPercent);
        public float Duration => Mathf.Max(0.1f, _duration);
    }
}