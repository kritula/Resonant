using UnityEngine;

namespace OmniumLessons
{
    public class CollapseTileBehavior : IActiveHexZoneBehavior
    {
        private ActiveHexZoneContext _context;
        private CollapseHexZoneData _data;
        private PlayerCharacter _player;

        private bool _collapseStarted;
        private bool _isCollapsedHole;
        private float _lifeTimer;
        private float _collapseTimer;
        private float _holeTimer;
        private float _currentHoleLifetime;

        public void Activate(
            ActiveHexZoneContext context,
            ActiveHexZoneData data)
        {
            _context = context;
            _data = data as CollapseHexZoneData;
            _player = null;
            _collapseStarted = false;
            _isCollapsedHole = false;
            _lifeTimer = 0f;
            _collapseTimer = 0f;
            _holeTimer = 0f;
            _currentHoleLifetime = 0f;

            if (_data == null)
            {
                Debug.LogWarning(
                    "CollapseTileBehavior requires CollapseHexZoneData.");
                _context?.CompleteZone();
                return;
            }

            _context?.ShowCollapseTile(_data.ReadyMaterial);
        }

        public void Deactivate()
        {
            _context?.ResetVisuals();
            _context = null;
            _data = null;
            _player = null;
            _collapseStarted = false;
            _isCollapsedHole = false;
            _lifeTimer = 0f;
            _collapseTimer = 0f;
            _holeTimer = 0f;
            _currentHoleLifetime = 0f;
        }

        public void Tick(float deltaTime)
        {
            if (_context == null || _data == null)
                return;

            if (_isCollapsedHole)
            {
                UpdateHole(deltaTime);
                return;
            }

            _lifeTimer += deltaTime;

            if (_lifeTimer >= _data.Lifetime)
            {
                _context.CompleteZone();
                return;
            }

            if (!_collapseStarted)
                return;

            _collapseTimer += deltaTime;

            if (_collapseTimer >= _data.DelayAfterStep)
                CollapseTile();
        }

        public void OnPlayerEnter(PlayerCharacter player)
        {
            if (player == null)
                return;

            _player = player;
            StartCollapseTimer();
        }

        public void OnPlayerExit(PlayerCharacter player)
        {
            if (player == null || player != _player)
                return;

            _player = null;
        }

        private void UpdateHole(float deltaTime)
        {
            _holeTimer += deltaTime;

            if (_holeTimer >= _currentHoleLifetime)
                _context.CompleteZone();
        }

        private void StartCollapseTimer()
        {
            if (_collapseStarted || _isCollapsedHole)
                return;

            _collapseStarted = true;
            _collapseTimer = 0f;
            _context?.ShowCrackedTile(_data.CrackMaterial);
        }

        private void CollapseTile()
        {
            _isCollapsedHole = true;
            _collapseStarted = false;
            _holeTimer = 0f;
            _currentHoleLifetime = Random.Range(
                _data.MinHoleLifetime,
                _data.MaxHoleLifetime);

            _context?.ShowCollapsedHole();
        }
    }
}
