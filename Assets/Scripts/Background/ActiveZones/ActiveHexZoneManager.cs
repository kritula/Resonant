using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class ActiveHexZoneManager : MonoBehaviour
    {
        private class DelayedZoneSpawn
        {
            public ActiveHexZoneData Data;
            public float Timer;
        }

        [Header("References")]
        [SerializeField] private HexGridManager _hexGridManager;

        [Header("Zones")]
        [SerializeField] private List<ActiveHexZoneData> _zoneDatas = new();

        [Header("Spawn Settings")]
        [SerializeField] private int _maxActiveZones = 5;
        [SerializeField] private int _minDistanceFromPlayerInCells = 2;

        private readonly List<ActiveHexZoneView> _activeZones = new();
        private readonly List<DelayedZoneSpawn> _delayedSpawns = new();

        private bool _isInitialized;

        public void Initialize(HexGridManager hexGridManager)
        {
            _hexGridManager = hexGridManager;

            _activeZones.Clear();
            _delayedSpawns.Clear();

            _isInitialized = true;

            SpawnInitialZones();
        }

        public void ResetZones()
        {
            for (int i = 0; i < _activeZones.Count; i++)
            {
                if (_activeZones[i] != null)
                    _activeZones[i].Deactivate();
            }

            _activeZones.Clear();
            _delayedSpawns.Clear();

            _isInitialized = false;
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            if (_hexGridManager == null)
                return;

            UpdateDelayedSpawns();
            SpawnMissingZones();
        }

        public void OnZoneCompleted(ActiveHexZoneView zoneView)
        {
            if (zoneView == null)
                return;

            ActiveHexZoneData completedData =
                GetZoneDataByType(zoneView.ZoneType);

            _activeZones.Remove(zoneView);
            zoneView.Deactivate();

            if (completedData != null)
                ScheduleRespawn(completedData);
        }

        public void OnCellReused(HexCellView cell)
        {
            if (cell == null)
                return;

            ActiveHexZoneView zoneView = cell.ActiveZoneView;

            if (zoneView == null)
                return;

            if (!zoneView.IsActiveZone)
                return;

            ActiveHexZoneData completedData =
                GetZoneDataByType(zoneView.ZoneType);

            _activeZones.Remove(zoneView);
            zoneView.Deactivate();

            if (completedData != null)
                ScheduleRespawn(completedData);
        }

        private void SpawnInitialZones()
        {
            for (int i = 0; i < _zoneDatas.Count; i++)
            {
                ActiveHexZoneData data = _zoneDatas[i];

                if (data == null)
                    continue;

                TrySpawnZone(data);
            }
        }

        private void SpawnMissingZones()
        {
            if (_activeZones.Count >= _maxActiveZones)
                return;

            for (int i = 0; i < _zoneDatas.Count; i++)
            {
                if (_activeZones.Count >= _maxActiveZones)
                    return;

                ActiveHexZoneData data = _zoneDatas[i];

                if (data == null)
                    continue;

                if (IsRespawnScheduled(data))
                    continue;

                if (CountActiveZonesOfType(data.ZoneType) >=
                    data.MaxActiveInstances)
                {
                    continue;
                }

                TrySpawnZone(data);
            }
        }

        private void UpdateDelayedSpawns()
        {
            for (int i = _delayedSpawns.Count - 1; i >= 0; i--)
            {
                DelayedZoneSpawn delayedSpawn = _delayedSpawns[i];

                if (delayedSpawn == null || delayedSpawn.Data == null)
                {
                    _delayedSpawns.RemoveAt(i);
                    continue;
                }

                delayedSpawn.Timer -= Time.deltaTime;

                if (delayedSpawn.Timer > 0f)
                    continue;

                if (_activeZones.Count >= _maxActiveZones)
                    continue;

                if (CountActiveZonesOfType(delayedSpawn.Data.ZoneType) >=
                    delayedSpawn.Data.MaxActiveInstances)
                {
                    _delayedSpawns.RemoveAt(i);
                    continue;
                }

                bool spawned = TrySpawnZone(delayedSpawn.Data);

                if (spawned)
                    _delayedSpawns.RemoveAt(i);
            }
        }

        private bool TrySpawnZone(ActiveHexZoneData data)
        {
            if (data == null)
                return false;

            if (_activeZones.Count >= _maxActiveZones)
                return false;

            if (CountActiveZonesOfType(data.ZoneType) >=
                data.MaxActiveInstances)
            {
                return false;
            }

            List<HexCellView> cells = _hexGridManager.SpawnedCells;

            if (cells == null || cells.Count == 0)
                return false;

            List<HexCellView> availableCells =
                new List<HexCellView>();

            for (int i = 0; i < cells.Count; i++)
            {
                HexCellView cell = cells[i];

                if (cell == null)
                    continue;

                if (cell.ActiveZoneView == null)
                    continue;

                if (cell.ActiveZoneView.IsActiveZone)
                    continue;

                if (IsTooCloseToPlayer(cell))
                    continue;

                availableCells.Add(cell);
            }

            if (availableCells.Count == 0)
                return false;

            HexCellView selectedCell =
                availableCells[Random.Range(0, availableCells.Count)];

            selectedCell.ActiveZoneView.SetManager(this);
            selectedCell.ActiveZoneView.Activate(data);

            _activeZones.Add(selectedCell.ActiveZoneView);

            return true;
        }

        private void ScheduleRespawn(ActiveHexZoneData data)
        {
            if (data == null)
                return;

            if (IsRespawnScheduled(data))
                return;

            float delay = Random.Range(
                data.MinRespawnDelay,
                data.MaxRespawnDelay);

            _delayedSpawns.Add(new DelayedZoneSpawn
            {
                Data = data,
                Timer = delay
            });
        }

        private bool IsRespawnScheduled(ActiveHexZoneData data)
        {
            for (int i = 0; i < _delayedSpawns.Count; i++)
            {
                if (_delayedSpawns[i] == null)
                    continue;

                if (_delayedSpawns[i].Data == data)
                    return true;
            }

            return false;
        }

        private int CountActiveZonesOfType(HexActiveZoneType zoneType)
        {
            int count = 0;

            for (int i = 0; i < _activeZones.Count; i++)
            {
                ActiveHexZoneView zone = _activeZones[i];

                if (zone == null)
                    continue;

                if (!zone.IsActiveZone)
                    continue;

                if (zone.ZoneType == zoneType)
                    count++;
            }

            return count;
        }

        private ActiveHexZoneData GetZoneDataByType(
            HexActiveZoneType zoneType)
        {
            for (int i = 0; i < _zoneDatas.Count; i++)
            {
                ActiveHexZoneData data = _zoneDatas[i];

                if (data == null)
                    continue;

                if (data.ZoneType == zoneType)
                    return data;
            }

            return null;
        }

        private bool IsTooCloseToPlayer(HexCellView cell)
        {
            if (GameManager.Instance == null)
                return false;

            if (GameManager.Instance.CharacterFactory == null)
                return false;

            Character player =
                GameManager.Instance.CharacterFactory.Player;

            if (player == null)
                return false;

            HexCoord playerCoord =
                _hexGridManager.GetApproximateCoordFromWorld(
                    player.transform.position);

            int columnDistance =
                Mathf.Abs(cell.Coord.Column - playerCoord.Column);

            int rowDistance =
                Mathf.Abs(cell.Coord.Row - playerCoord.Row);

            return columnDistance <= _minDistanceFromPlayerInCells &&
                   rowDistance <= _minDistanceFromPlayerInCells;
        }
    }
}