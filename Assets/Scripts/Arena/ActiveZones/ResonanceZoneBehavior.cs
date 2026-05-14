using UnityEngine;

namespace OmniumLessons
{
    public class ResonanceZoneBehavior : IActiveHexZoneBehavior
    {
        private ActiveHexZoneContext _context;
        private ResonanceHexZoneData _data;
        private PlayerCharacter _player;
        private PlayerAttackStatsDecoratorController _decoratorController;
        private ResonanceAttackStatsDecorator _decorator;

        private float _lifeTimer;
        private float _resonanceTimer;
        private bool _isPlayerInside;

        public void Activate(
            ActiveHexZoneContext context,
            ActiveHexZoneData data)
        {
            _context = context;
            _data = data as ResonanceHexZoneData;
            _player = null;
            _lifeTimer = 0f;
            _resonanceTimer = 0f;
            _isPlayerInside = false;

            if (_data == null)
            {
                Debug.LogWarning(
                    "ResonanceZoneBehavior requires ResonanceHexZoneData.");
                _context?.CompleteZone();
                return;
            }

            _context?.ShowResonanceVisual(_data.ReadyMaterial);
        }

        public void Deactivate()
        {
            RemoveBuff();

            _context?.ResetVisuals();
            _context = null;
            _data = null;
            _player = null;
            _lifeTimer = 0f;
            _resonanceTimer = 0f;
            _isPlayerInside = false;
        }

        public void Tick(float deltaTime)
        {
            if (_context == null || _data == null)
                return;

            _lifeTimer += deltaTime;

            if (_lifeTimer >= _data.Lifetime)
            {
                _context.CompleteZone();
                return;
            }

            if (!_isPlayerInside)
                return;

            _resonanceTimer += deltaTime;

            if (_resonanceTimer >= _data.ActivationDuration)
                _context.CompleteZone();
        }

        public void OnPlayerEnter(PlayerCharacter player)
        {
            if (player == null || _data == null)
                return;

            _player = player;
            _isPlayerInside = true;

            ApplyBuff();
            _context?.SetResonanceMaterial(_data.ActiveMaterial);
        }

        public void OnPlayerExit(PlayerCharacter player)
        {
            if (player == null || player != _player)
                return;

            RemoveBuff();
            _context?.CompleteZone();
        }

        private void ApplyBuff()
        {
            if (_data == null || _player == null || _decorator != null)
                return;

            _decoratorController =
                _player.GetComponent<PlayerAttackStatsDecoratorController>();

            if (_decoratorController == null)
            {
                _decoratorController =
                    _player.gameObject.AddComponent<PlayerAttackStatsDecoratorController>();
            }

            float damageBonus = Random.Range(
                _data.MinDamageBonus,
                _data.MaxDamageBonus);

            float attackSpeedBonus = Random.Range(
                _data.MinAttackSpeedBonus,
                _data.MaxAttackSpeedBonus);

            _decorator = new ResonanceAttackStatsDecorator(
                1f + damageBonus,
                1f + attackSpeedBonus);

            _decoratorController.AddDecorator(_decorator);
        }

        private void RemoveBuff()
        {
            if (_decoratorController != null && _decorator != null)
                _decoratorController.RemoveDecorator(_decorator);

            _decorator = null;
            _decoratorController = null;
        }
    }
}
