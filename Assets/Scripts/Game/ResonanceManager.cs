using System;
using UnityEngine;

namespace OmniumLessons
{
    public class ResonanceManager
    {
        public event Action<int> OnResonanceChanged;
        public int CurrentResonance { get; private set; }
        public int CurrentCombo { get; private set; }
        public int NoDamageKillStreak { get; private set; }

        private float _lastKillTime = -999f;
        private float _lastDamageTime = -999f;
        private float _lastMultiKillTime = -999f;

        private int _multiKillCount = 0;

        private const float ComboWindow = 2.5f;
        private const float MultiKillWindow = 0.35f;
        private const float NoDamageBonusDelay = 10f;

        public void ResetProgress()
        {
            CurrentResonance = 0;
            CurrentCombo = 0;
            NoDamageKillStreak = 0;

            _lastKillTime = -999f;
            _lastDamageTime = -999f;
            _lastMultiKillTime = -999f;
            _multiKillCount = 0;

            OnResonanceChanged?.Invoke(CurrentResonance);
        }

        public void RegisterEnemyKilled(Character enemy, bool isCreativeKill = false)
        {
            if (enemy == null || enemy.CharacterData == null)
                return;

            int baseResonance = enemy.CharacterData.ResonanceCost;
            if (baseResonance <= 0)
                baseResonance = 1;

            float currentTime = Time.time;

            UpdateCombo(currentTime);
            UpdateMultiKill(currentTime);

            int reward = baseResonance;

            reward += GetComboBonus(baseResonance);
            reward += GetMultiKillBonus(baseResonance);

            if (isCreativeKill)
            {
                reward += Mathf.CeilToInt(baseResonance * 1.0f);
            }

            if (currentTime - _lastDamageTime >= NoDamageBonusDelay)
            {
                reward += Mathf.CeilToInt(baseResonance * 0.5f);
                NoDamageKillStreak++;
            }
            else
            {
                NoDamageKillStreak = 0;
            }

            AddResonance(reward); 
            ShowCombatMessage(isCreativeKill);
            _lastKillTime = currentTime;

            Debug.Log(
                $"[RESONANCE] Enemy: {enemy.CharacterType}, " +
                $"Base: {baseResonance}, " +
                $"Combo: {CurrentCombo}, " +
                $"MultiKill: {_multiKillCount}, " +
                $"NoDamageKills: {NoDamageKillStreak}, " +
                $"Creative: {isCreativeKill}, " +
                $"Reward: {reward}, " +
                $"Total: {CurrentResonance}");
        }

        public void RegisterPlayerDamaged()
        {
            _lastDamageTime = Time.time;
            NoDamageKillStreak = 0;
            CurrentCombo = 0;
            _multiKillCount = 0;
        }

        public bool CanSpend(int cost)
        {
            return CurrentResonance >= cost;
        }

        public bool TrySpend(int cost)
        {
            if (cost <= 0)
                return true;

            if (!CanSpend(cost))
                return false;

            CurrentResonance -= cost;
            OnResonanceChanged?.Invoke(CurrentResonance);
            return true;
        }

        private void AddResonance(int amount)
        {
            if (amount <= 0)
                return;

            CurrentResonance += amount;
            OnResonanceChanged?.Invoke(CurrentResonance);
        }

        private void UpdateCombo(float currentTime)
        {
            if (currentTime - _lastKillTime <= ComboWindow)
            {
                CurrentCombo++;
            }
            else
            {
                CurrentCombo = 1;
            }
        }

        private void UpdateMultiKill(float currentTime)
        {
            if (currentTime - _lastMultiKillTime <= MultiKillWindow)
            {
                _multiKillCount++;
            }
            else
            {
                _multiKillCount = 1;
            }

            _lastMultiKillTime = currentTime;
        }

        private int GetComboBonus(int baseResonance)
        {
            if (CurrentCombo <= 1)
                return 0;

            float bonusPercent = Mathf.Min((CurrentCombo - 1) * 0.25f, 1.5f);
            return Mathf.CeilToInt(baseResonance * bonusPercent);
        }

        private int GetMultiKillBonus(int baseResonance)
        {
            if (_multiKillCount <= 1)
                return 0;

            float bonusPercent = Mathf.Min((_multiKillCount - 1) * 0.5f, 2.0f);
            return Mathf.CeilToInt(baseResonance * bonusPercent);
        }

        private void ShowCombatMessage(bool isCreativeKill)
        {
            if (GameManager.Instance == null || GameManager.Instance.WindowsService == null)
                return;

            GameplayWindow gameplayWindow = GameManager.Instance.WindowsService.GetWindow<GameplayWindow>();
            if (gameplayWindow == null)
                return;

            if (isCreativeKill)
            {
                gameplayWindow.ShowCombatFeedback("CREATIVE KILL");
                return;
            }

            if (_multiKillCount >= 2)
            {
                gameplayWindow.ShowCombatFeedback("MULTI KILL");
                return;
            }

            if (CurrentCombo >= 3)
            {
                gameplayWindow.ShowCombatFeedback($"COMBO x{CurrentCombo}");
                return;
            }

            if (NoDamageKillStreak >= 3)
            {
                gameplayWindow.ShowCombatFeedback("PERFECT");
            }
        }
    }
}