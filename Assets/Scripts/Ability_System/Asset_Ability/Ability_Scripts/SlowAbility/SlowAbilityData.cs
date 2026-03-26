using UnityEngine;

namespace OmniumLessons
{
    [CreateAssetMenu(fileName = "SlowAbilityData", menuName = "Ability System/Slow Ability Data")]
    public class SlowAbilityData : AbilityData
    {
        [Header("Slow Settings")]
        [SerializeField] private float _radius = 5f;
        [SerializeField][Range(0f, 1f)] private float _slowPercent = 0.3f;

        public float Radius => _radius;
        public float SlowPercent => _slowPercent;
    }
}