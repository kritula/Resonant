using UnityEngine;

namespace OmniumLessons
{
    public class HexCellView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Renderer _hexRenderer;
        [SerializeField] private ActiveHexZoneView _activeZoneView;

        private Material _defaultHexMaterial;

        public HexCoord Coord { get; private set; }
        public ActiveHexZoneView ActiveZoneView => _activeZoneView;

        private void Reset()
        {
            if (_hexRenderer == null)
            {
                Transform hexVisual = transform.Find("HexVisual");

                if (hexVisual != null)
                    _hexRenderer = hexVisual.GetComponent<Renderer>();
            }

            if (_activeZoneView == null)
                _activeZoneView = GetComponentInChildren<ActiveHexZoneView>(true);
        }

        private void Awake()
        {
            CacheDefaultMaterial();
        }

        public void Initialize(HexCoord coord, Vector3 worldPosition)
        {
            Coord = coord;
            transform.position = worldPosition;

            CacheDefaultMaterial();
            RestoreDefaultHex();

            if (_activeZoneView != null)
                _activeZoneView.Initialize(this);
        }

        public void UpdateCell(HexCoord coord, Vector3 worldPosition)
        {
            Coord = coord;
            transform.position = worldPosition;

            if (_activeZoneView != null)
                _activeZoneView.Deactivate();

            RestoreDefaultHex();
        }

        public void ShowDefaultHex()
        {
            if (_hexRenderer != null)
                _hexRenderer.enabled = true;
        }

        public void HideDefaultHex()
        {
            if (_hexRenderer != null)
                _hexRenderer.enabled = false;
        }

        public void SetHexMaterial(Material material)
        {
            if (_hexRenderer == null)
                return;

            if (material == null)
                return;

            _hexRenderer.material = material;
        }

        public void RestoreDefaultHex()
        {
            ShowDefaultHex();

            if (_hexRenderer != null && _defaultHexMaterial != null)
                _hexRenderer.material = _defaultHexMaterial;
        }

        public void SetVisible(bool value)
        {
            gameObject.SetActive(value);
        }

        private void CacheDefaultMaterial()
        {
            if (_hexRenderer == null)
                return;

            if (_defaultHexMaterial != null)
                return;

            _defaultHexMaterial = _hexRenderer.sharedMaterial;
        }
    }
}