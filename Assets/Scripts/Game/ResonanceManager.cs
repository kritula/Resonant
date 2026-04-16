using System;
using UnityEngine;

namespace OmniumLessons
{
    public class ResonanceManager
    {
        private const float ComboResetTime = 3f;

        private int _currentResonance;
        private int _killCombo;
        private float _comboTimer;

        public int CurrentResonance => _currentResonance;

        public event Action<int> OnResonanceChanged;

        public void ResetSession()
        {
            _currentResonance = 0;
            _killCombo = 0;
            _comboTimer = 0f;

            OnResonanceChanged?.Invoke(_currentResonance);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_killCombo <= 0)
                return;

            _comboTimer -= deltaTime;

            if (_comboTimer <= 0f)
            {
                ResetCombo();
            }
        }

        public void AddResonance(int amount)
        {
            int finalAmount = Mathf.Max(0, amount);

            if (finalAmount <= 0)
                return;

            _currentResonance += finalAmount;
            OnResonanceChanged?.Invoke(_currentResonance);
        }

        public bool CanSpendResonance(int amount)
        {
            return _currentResonance >= Mathf.Max(0, amount);
        }

        public bool TrySpendResonance(int amount)
        {
            int finalAmount = Mathf.Max(0, amount);

            if (_currentResonance < finalAmount)
                return false;

            _currentResonance -= finalAmount;
            OnResonanceChanged?.Invoke(_currentResonance);

            return true;
        }

        public void RegisterEnemyKilled(Character enemy)
        {
            if (enemy == null)
                return;

            _killCombo++;
            _comboTimer = ComboResetTime;

            int reward = 1;

            if (_killCombo >= 5)
                reward += 1;

            if (_killCombo >= 10)
                reward += 2;

            AddResonance(reward);
        }

        public void RegisterPlayerDamaged()
        {
            ResetCombo();
        }

        private void ResetCombo()
        {
            _killCombo = 0;
            _comboTimer = 0f;
        }
    }
}