using System;
using UnityEngine;

namespace OmniumLessons
{
    public class ActiveHexZoneContext
    {
        private readonly HexCellView _cell;
        private readonly Renderer _resonanceRenderer;
        private readonly Collider _resonanceTrigger;
        private readonly CollapseHoleTrigger _collapseHoleTrigger;
        private readonly Action<float?> _completeZone;

        public ActiveHexZoneContext(
            HexCellView cell,
            Renderer resonanceRenderer,
            Collider resonanceTrigger,
            CollapseHoleTrigger collapseHoleTrigger,
            Action<float?> completeZone)
        {
            _cell = cell;
            _resonanceRenderer = resonanceRenderer;
            _resonanceTrigger = resonanceTrigger;
            _collapseHoleTrigger = collapseHoleTrigger;
            _completeZone = completeZone;
        }

        public void CompleteZone(float? respawnDelayOverride = null)
        {
            _completeZone?.Invoke(respawnDelayOverride);
        }

        public Vector3 WorldPosition =>
            _cell != null
                ? _cell.transform.position
                : Vector3.zero;

        public float TileWorldRadius =>
            _cell != null
                ? _cell.WorldRadius
                : 0f;

        public void ShowResonanceVisual(Material material)
        {
            _cell?.HideDefaultHex();

            if (_resonanceRenderer != null)
            {
                _resonanceRenderer.enabled = true;

                if (material != null)
                    _resonanceRenderer.material = material;
            }

            SetZoneTriggerActive(true);
            SetCollapseHoleActive(false);
        }

        public void SetResonanceMaterial(Material material)
        {
            if (_resonanceRenderer == null || material == null)
                return;

            _resonanceRenderer.material = material;
        }

        public void ShowCollapseTile(Material material)
        {
            HideResonanceVisual();
            _cell?.RestoreDefaultHex();

            if (material != null)
                _cell?.SetHexMaterial(material);

            SetZoneTriggerActive(true);
            SetCollapseHoleActive(false);
        }

        public void ShowTileVisual(
            Material material,
            bool enableZoneTrigger = false)
        {
            HideResonanceVisual();
            _cell?.RestoreDefaultHex();

            if (material != null)
                _cell?.SetHexMaterial(material);

            SetZoneTriggerActive(enableZoneTrigger);
            SetCollapseHoleActive(false);
        }

        public void ShowCrackedTile(Material material)
        {
            if (material == null)
                return;

            _cell?.SetHexMaterial(material);
        }

        public void ShowCollapsedHole()
        {
            HideResonanceVisual();
            _cell?.HideDefaultHex();
            SetCollapseHoleActive(true);
        }

        public void ResetVisuals()
        {
            HideResonanceVisual();
            SetCollapseHoleActive(false);
            _cell?.RestoreDefaultHex();
        }

        private void HideResonanceVisual()
        {
            if (_resonanceRenderer != null)
                _resonanceRenderer.enabled = false;

            SetZoneTriggerActive(false);
        }

        private void SetZoneTriggerActive(bool value)
        {
            if (_resonanceTrigger != null)
                _resonanceTrigger.enabled = value;
        }

        private void SetCollapseHoleActive(bool value)
        {
            if (_collapseHoleTrigger != null)
                _collapseHoleTrigger.SetActiveState(value);
        }
    }
}
