using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class HexGridManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HexCellView _hexCellPrefab;
        [SerializeField] private Transform _target;

        [Header("Grid Size")]
        [SerializeField] private int _columns = 25;
        [SerializeField] private int _rows = 25;

        [Header("Hex Size")]
        [SerializeField] private float _hexWidth = 7f;
        [SerializeField] private float _hexHeight = 6.070313f;

        [Header("World Settings")]
        [SerializeField] private float _groundY = 0f;

        [Header("Recenter Settings")]
        [SerializeField] private int _recenterThresholdColumns = 7;
        [SerializeField] private int _recenterThresholdRows = 7;

        private readonly List<HexCellView> _spawnedCells = new();
        private readonly Dictionary<HexCoord, HexCellView> _activeCellsByCoord = new();

        private HexCoord _currentCenterCoord;
        private bool _isInitialized;

        public void Initialize(Transform target)
        {
            _target = target;

            if (_hexCellPrefab == null)
            {
                Debug.LogError("HexGridManager: Hex cell prefab is not assigned.");
                return;
            }

            _currentCenterCoord = GetApproximateCoordFromWorld(_target.position);

            CreateInitialCells();
            _isInitialized = true;
        }

        public void ResetGrid()
        {
            _isInitialized = false;
            _target = null;
            _currentCenterCoord = new HexCoord(0, 0);

            ClearGrid();
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            if (_target == null)
                return;

            HexCoord targetCoord = GetApproximateCoordFromWorld(_target.position);

            int columnDistance = Mathf.Abs(targetCoord.Column - _currentCenterCoord.Column);
            int rowDistance = Mathf.Abs(targetCoord.Row - _currentCenterCoord.Row);

            if (columnDistance < _recenterThresholdColumns &&
                rowDistance < _recenterThresholdRows)
                return;

            ShiftGridToNewCenter(targetCoord);
        }

        private void CreateInitialCells()
        {
            ClearGrid();

            List<HexCoord> requiredCoords = BuildRequiredCoords(_currentCenterCoord);

            for (int i = 0; i < requiredCoords.Count; i++)
            {
                HexCoord coord = requiredCoords[i];
                Vector3 worldPosition = GetWorldPosition(coord);

                HexCellView cell = Instantiate(
                    _hexCellPrefab,
                    worldPosition,
                    _hexCellPrefab.transform.rotation,
                    transform);

                cell.Initialize(coord, worldPosition);

                _spawnedCells.Add(cell);
                _activeCellsByCoord[coord] = cell;
            }
        }

        private void ShiftGridToNewCenter(HexCoord newCenterCoord)
        {
            List<HexCoord> requiredCoords = BuildRequiredCoords(newCenterCoord);
            HashSet<HexCoord> requiredSet = new HashSet<HexCoord>(requiredCoords);

            List<HexCoord> missingCoords = new List<HexCoord>();
            for (int i = 0; i < requiredCoords.Count; i++)
            {
                HexCoord coord = requiredCoords[i];

                if (!_activeCellsByCoord.ContainsKey(coord))
                    missingCoords.Add(coord);
            }

            List<HexCellView> reusableCells = new List<HexCellView>();
            foreach (HexCellView cell in _spawnedCells)
            {
                if (!requiredSet.Contains(cell.Coord))
                    reusableCells.Add(cell);
            }

            int reassignCount = Mathf.Min(missingCoords.Count, reusableCells.Count);

            for (int i = 0; i < reassignCount; i++)
            {
                HexCellView cell = reusableCells[i];
                HexCoord oldCoord = cell.Coord;
                HexCoord newCoord = missingCoords[i];

                _activeCellsByCoord.Remove(oldCoord);

                Vector3 worldPosition = GetWorldPosition(newCoord);
                cell.UpdateCell(newCoord, worldPosition);

                _activeCellsByCoord[newCoord] = cell;
            }

            _currentCenterCoord = newCenterCoord;
        }

        private List<HexCoord> BuildRequiredCoords(HexCoord centerCoord)
        {
            List<HexCoord> coords = new List<HexCoord>(_columns * _rows);

            int halfColumns = _columns / 2;
            int halfRows = _rows / 2;

            for (int column = -halfColumns; column <= halfColumns; column++)
            {
                for (int row = -halfRows; row <= halfRows; row++)
                {
                    HexCoord coord = new HexCoord(
                        centerCoord.Column + column,
                        centerCoord.Row + row);

                    coords.Add(coord);
                }
            }

            return coords;
        }

        private void ClearGrid()
        {
            for (int i = 0; i < _spawnedCells.Count; i++)
            {
                if (_spawnedCells[i] != null)
                    Destroy(_spawnedCells[i].gameObject);
            }

            _spawnedCells.Clear();
            _activeCellsByCoord.Clear();
        }

        public Vector3 GetWorldPosition(HexCoord coord)
        {
            float horizontalStep = _hexWidth * 0.75f;
            float verticalStep = _hexHeight;

            float xPosition = coord.Column * horizontalStep;
            float zPosition = coord.Row * verticalStep;

            if (coord.Column % 2 != 0)
                zPosition += verticalStep * 0.5f;

            return new Vector3(xPosition, _groundY, zPosition);
        }

        public HexCoord GetApproximateCoordFromWorld(Vector3 worldPosition)
        {
            float horizontalStep = _hexWidth * 0.75f;
            float verticalStep = _hexHeight;

            int approxColumn = Mathf.RoundToInt(worldPosition.x / horizontalStep);

            float rowOffset = 0f;
            if (approxColumn % 2 != 0)
                rowOffset = verticalStep * 0.5f;

            int approxRow = Mathf.RoundToInt((worldPosition.z - rowOffset) / verticalStep);

            return new HexCoord(approxColumn, approxRow);
        }
    }
}