using UnityEngine;

namespace OmniumLessons
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        protected Character _owner;
        protected AbilityData _data;

        public virtual void Initialize(Character owner, AbilityData data)
        {
            _owner = owner;
            _data = data;
        }
        public virtual void OnUpdate()
        {
        }
    }
}
