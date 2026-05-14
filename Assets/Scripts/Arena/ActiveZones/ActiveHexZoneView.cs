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
        private ActiveHexZoneContext _context;
        private IActiveHexZoneBehavior _behavior;

        private bool _isActiveZone;
        private float? _respawnDelayOverride;

        public bool IsActiveZone => _isActiveZone;

        public HexActiveZoneType ZoneType =>
            _data != null
                ? _data.ZoneType
                : HexActiveZoneType.None;

        private void Awake()
        {
            CacheReferences();
        }

        private void Reset()
        {
            CacheReferences();
        }

        private void CacheReferences()
        {
            if (_resonanceRenderer == null)
                _resonanceRenderer = GetComponentInChildren<Renderer>(true);

            if (_resonanceTrigger == null)
                _resonanceTrigger = GetComponentInChildren<Collider>(true);

            if (_resonanceTrigger != null)
                _resonanceTrigger.isTrigger = true;

            if (_collapseHoleTrigger == null)
                _collapseHoleTrigger = GetComponentInChildren<CollapseHoleTrigger>(true);

            if (_collapseHoleTrigger == null && transform.parent != null)
            {
                _collapseHoleTrigger =
                    transform.parent.GetComponentInChildren<CollapseHoleTrigger>(true);
            }
        }

        public void Initialize(HexCellView cell)
        {
            CacheReferences();
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

            _behavior?.Tick(Time.deltaTime);
        }

        public void Activate(ActiveHexZoneData data)
        {
            Deactivate();

            if (data == null)
            {
                Debug.LogWarning($"{name}: ActiveHexZoneData is NULL");
                return;
            }

            _data = data;
            _behavior =
                ActiveHexZoneBehaviorFactory.Create(data.ZoneType);

            if (_behavior == null)
            {
                Debug.LogWarning($"{name}: Unsupported active zone type {data.ZoneType}");
                _data = null;
                return;
            }

            _context = new ActiveHexZoneContext(
                _cell,
                _resonanceRenderer,
                _resonanceTrigger,
                _collapseHoleTrigger,
                CompleteZone);

            _isActiveZone = true;
            _behavior.Activate(_context, _data);
        }

        public void Deactivate()
        {
            if (_behavior != null)
                _behavior.Deactivate();
            else
                ResetInactiveVisuals();

            _data = null;
            _context = null;
            _behavior = null;
            _isActiveZone = false;
            _respawnDelayOverride = null;
        }

        public float? ConsumeRespawnDelayOverride()
        {
            float? value = _respawnDelayOverride;
            _respawnDelayOverride = null;

            return value;
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

            _behavior?.OnPlayerEnter(player);
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

            _behavior?.OnPlayerExit(player);
        }

        private PlayerCharacter GetPlayerFromCollider(Collider other)
        {
            PlayerCharacter player = other.GetComponent<PlayerCharacter>();

            if (player == null)
                player = other.GetComponentInParent<PlayerCharacter>();

            return player;
        }

        private void ResetInactiveVisuals()
        {
            ActiveHexZoneContext inactiveContext =
                new ActiveHexZoneContext(
                    _cell,
                    _resonanceRenderer,
                    _resonanceTrigger,
                    _collapseHoleTrigger,
                    null);

            inactiveContext.ResetVisuals();
        }

        private void CompleteZone(float? respawnDelayOverride)
        {
            if (!_isActiveZone)
                return;

            _respawnDelayOverride = respawnDelayOverride;
            _isActiveZone = false;
            _manager?.OnZoneCompleted(this);
        }
    }
}
