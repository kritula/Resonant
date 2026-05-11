using UnityEngine;

namespace OmniumLessons
{
    public class ActiveHexZoneView : MonoBehaviour
    {
        [Header("Resonance Visual")]
        [SerializeField] private Renderer _resonanceRenderer;
        [SerializeField] private Collider _resonanceTrigger;

        [Header("Collapse Hole")]
        [SerializeField] private CollapseHoleTrigger _collapseHoleTrigger;

        private HexCellView _cell;
        private ActiveHexZoneManager _manager;
        private ActiveHexZoneData _data;

        private PlayerCharacter _player;
        private PlayerAttackStatsDecoratorController _decoratorController;
        private ResonanceAttackStatsDecorator _decorator;

        private bool _isActiveZone;
        private bool _isPlayerInside;

        private bool _collapseStarted;
        private bool _isCollapsedHole;

        private float _lifeTimer;
        private float _resonanceTimer;
        private float _collapseTimer;
        private float _holeTimer;
        private float _currentHoleLifetime;

        public bool IsActiveZone => _isActiveZone;

        public HexActiveZoneType ZoneType =>
            _data != null
                ? _data.ZoneType
                : HexActiveZoneType.None;

        private void Reset()
        {
            if (_resonanceRenderer == null)
                _resonanceRenderer = GetComponentInChildren<Renderer>(true);

            if (_resonanceTrigger == null)
                _resonanceTrigger = GetComponentInChildren<Collider>(true);

            if (_resonanceTrigger != null)
                _resonanceTrigger.isTrigger = true;

            if (_collapseHoleTrigger == null)
                _collapseHoleTrigger = GetComponentInChildren<CollapseHoleTrigger>(true);
        }

        public void Initialize(HexCellView cell)
        {
            _cell = cell;
            Deactivate();
        }

        public void SetManager(ActiveHexZoneManager manager)
        {
            _manager = manager;
        }

        private void Update()
        {
            if (!_isActiveZone)
                return;

            if (_data == null)
                return;

            if (!_isCollapsedHole)
            {
                _lifeTimer += Time.deltaTime;

                if (_lifeTimer >= _data.Lifetime)
                {
                    CompleteZone();
                    return;
                }
            }

            switch (_data.ZoneType)
            {
                case HexActiveZoneType.ResonanceZone:
                    UpdateResonanceZone();
                    break;

                case HexActiveZoneType.CollapseTile:
                    UpdateCollapseTile();
                    break;
            }
        }

        public void Activate(ActiveHexZoneData data)
        {
            if (data == null)
            {
                Debug.LogWarning($"{name}: ActiveHexZoneData is NULL");
                return;
            }

            _data = data;

            _isActiveZone = true;
            _isPlayerInside = false;

            _collapseStarted = false;
            _isCollapsedHole = false;

            _lifeTimer = 0f;
            _resonanceTimer = 0f;
            _collapseTimer = 0f;
            _holeTimer = 0f;
            _currentHoleLifetime = 0f;

            RemoveResonanceBuff();

            switch (_data.ZoneType)
            {
                case HexActiveZoneType.ResonanceZone:
                    ActivateResonanceZone();
                    break;

                case HexActiveZoneType.CollapseTile:
                    ActivateCollapseTile();
                    break;
            }
        }

        public void Deactivate()
        {
            RemoveResonanceBuff();

            _data = null;
            _player = null;

            _isActiveZone = false;
            _isPlayerInside = false;

            _collapseStarted = false;
            _isCollapsedHole = false;

            _lifeTimer = 0f;
            _resonanceTimer = 0f;
            _collapseTimer = 0f;
            _holeTimer = 0f;
            _currentHoleLifetime = 0f;

            HideResonanceVisual();

            if (_collapseHoleTrigger != null)
                _collapseHoleTrigger.SetActiveState(false);

            if (_cell != null)
                _cell.RestoreDefaultHex();
        }

        private void ActivateResonanceZone()
        {
            if (_cell != null)
                _cell.HideDefaultHex();

            if (_resonanceRenderer != null)
            {
                _resonanceRenderer.enabled = true;

                if (_data.ResonanceReadyMaterial != null)
                    _resonanceRenderer.material = _data.ResonanceReadyMaterial;
            }

            if (_resonanceTrigger != null)
                _resonanceTrigger.enabled = true;

            if (_collapseHoleTrigger != null)
                _collapseHoleTrigger.SetActiveState(false);
        }

        private void ActivateCollapseTile()
        {
            HideResonanceVisual();

            if (_cell != null)
                _cell.RestoreDefaultHex();

            if (_resonanceTrigger != null)
                _resonanceTrigger.enabled = true;

            if (_collapseHoleTrigger != null)
                _collapseHoleTrigger.SetActiveState(false);
        }

        private void UpdateResonanceZone()
        {
            if (!_isPlayerInside)
                return;

            _resonanceTimer += Time.deltaTime;

            if (_resonanceTimer >= _data.ResonanceActivationDuration)
                CompleteZone();
        }

        private void UpdateCollapseTile()
        {
            if (_isCollapsedHole)
            {
                _holeTimer += Time.deltaTime;

                if (_holeTimer >= _currentHoleLifetime)
                    CompleteZone();

                return;
            }

            if (!_collapseStarted)
                return;

            _collapseTimer += Time.deltaTime;

            if (_collapseTimer >= _data.CollapseDelayAfterStep)
                CollapseTile();
        }

        private void CollapseTile()
        {
            _isCollapsedHole = true;
            _collapseStarted = false;

            _holeTimer = 0f;
            _currentHoleLifetime = Random.Range(
                _data.MinHoleLifetime,
                _data.MaxHoleLifetime);

            HideResonanceVisual();

            if (_cell != null)
                _cell.HideDefaultHex();

            if (_collapseHoleTrigger != null)
                _collapseHoleTrigger.SetActiveState(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isActiveZone)
                return;

            if (_data == null)
                return;

            PlayerCharacter player = GetPlayerFromCollider(other);

            if (player == null)
                return;

            _player = player;
            _isPlayerInside = true;

            switch (_data.ZoneType)
            {
                case HexActiveZoneType.ResonanceZone:
                    ApplyResonanceBuff();

                    if (_resonanceRenderer != null &&
                        _data.ResonanceActiveMaterial != null)
                    {
                        _resonanceRenderer.material = _data.ResonanceActiveMaterial;
                    }

                    break;

                case HexActiveZoneType.CollapseTile:
                    StartCollapseTimer();
                    break;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_isActiveZone)
                return;

            if (_data == null)
                return;

            PlayerCharacter player = GetPlayerFromCollider(other);

            if (player == null)
                return;

            if (_player != player)
                return;

            switch (_data.ZoneType)
            {
                case HexActiveZoneType.ResonanceZone:
                    RemoveResonanceBuff();
                    CompleteZone();
                    break;

                case HexActiveZoneType.CollapseTile:
                    _player = null;
                    _isPlayerInside = false;
                    break;
            }
        }

        private void StartCollapseTimer()
        {
            if (_collapseStarted)
                return;

            if (_isCollapsedHole)
                return;

            _collapseStarted = true;
            _collapseTimer = 0f;

            if (_cell != null && _data.CollapseCrackMaterial != null)
                _cell.SetHexMaterial(_data.CollapseCrackMaterial);
        }

        private PlayerCharacter GetPlayerFromCollider(Collider other)
        {
            PlayerCharacter player = other.GetComponent<PlayerCharacter>();

            if (player == null)
                player = other.GetComponentInParent<PlayerCharacter>();

            return player;
        }

        private void ApplyResonanceBuff()
        {
            if (_data == null)
                return;

            if (_player == null)
                return;

            if (_decorator != null)
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

            float damageMultiplier = 1f + damageBonus;
            float attackSpeedMultiplier = 1f + attackSpeedBonus;

            _decorator = new ResonanceAttackStatsDecorator(
                damageMultiplier,
                attackSpeedMultiplier);

            _decoratorController.AddDecorator(_decorator);
        }

        private void RemoveResonanceBuff()
        {
            if (_decoratorController != null &&
                _decorator != null)
            {
                _decoratorController.RemoveDecorator(_decorator);
            }

            _decorator = null;
            _decoratorController = null;
        }

        private void HideResonanceVisual()
        {
            if (_resonanceRenderer != null)
                _resonanceRenderer.enabled = false;

            if (_resonanceTrigger != null)
                _resonanceTrigger.enabled = false;
        }

        private void CompleteZone()
        {
            RemoveResonanceBuff();

            _isPlayerInside = false;
            _collapseStarted = false;
            _isCollapsedHole = false;

            _lifeTimer = 0f;
            _resonanceTimer = 0f;
            _collapseTimer = 0f;
            _holeTimer = 0f;

            _manager?.OnZoneCompleted(this);
        }
    }
}