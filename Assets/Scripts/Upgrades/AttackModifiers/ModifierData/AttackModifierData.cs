using UnityEngine;

namespace OmniumLessons
{
    public abstract class AttackModifierData : ScriptableObject
    {
        [SerializeField] private AttackModifierType _modifierType;

        public AttackModifierType ModifierType => _modifierType;
    }
}