using System.Collections.Generic;

namespace OmniumLessons
{
    public class StatusEffectController
    {
        private readonly Character _owner;
        private readonly List<IStatusEffect> _effects = new List<IStatusEffect>();

        public StatusEffectController(Character owner)
        {
            _owner = owner;
        }

        public void AddEffect(IStatusEffect effect)
        {
            if (effect == null)
                return;

            effect.Initialize(_owner);
            _effects.Add(effect);
        }

        public void OnUpdate()
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                _effects[i].OnUpdate();

                if (_effects[i].IsFinished)
                {
                    _effects.RemoveAt(i);
                }
            }
        }

        public bool HasEffect<T>() where T : class, IStatusEffect
        {
            for (int i = 0; i < _effects.Count; i++)
            {
                if (_effects[i] is T)
                    return true;
            }

            return false;
        }
    }
}