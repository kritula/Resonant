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

        public void OnUpdate()
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                IStatusEffect effect = _effects[i];

                if (effect == null)
                {
                    _effects.RemoveAt(i);
                    continue;
                }

                effect.OnUpdate();

                if (effect.IsFinished)
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

        public void AddEffect(IStatusEffect effect)
        {
            if (effect == null || _owner == null)
                return;

            effect.Initialize(_owner);
            _effects.Add(effect);
        }

        public void RemoveEffect<T>() where T : class, IStatusEffect
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                if (_effects[i] is T)
                {
                    _effects[i].ForceFinish();
                    _effects.RemoveAt(i);
                }
            }
        }

        public void ReplaceEffect<T>(IStatusEffect newEffect) where T : class, IStatusEffect
        {
            if (newEffect == null)
                return;

            RemoveEffect<T>();
            AddEffect(newEffect);
        }

        public void ClearAll()
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                if (_effects[i] != null)
                {
                    _effects[i].ForceFinish();
                }
            }

            _effects.Clear();
        }
    }
}