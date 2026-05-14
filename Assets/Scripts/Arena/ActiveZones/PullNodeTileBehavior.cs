using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class PullNodeTileBehavior : IActiveHexZoneBehavior
    {
        private static readonly List<PullNodeTileBehavior> ActiveNodes = new();

        private ActiveHexZoneContext _context;
        private PullNodeHexZoneData _data;
        private float _lifeTimer;
        private float _activationTimer;
        private bool _isActivated;

        public static float GetEnemyMoveSpeedMultiplier(Vector3 enemyPosition)
        {
            float resistance = 0f;

            for (int i = ActiveNodes.Count - 1; i >= 0; i--)
            {
                PullNodeTileBehavior node = ActiveNodes[i];

                if (node == null || node._context == null || node._data == null)
                {
                    ActiveNodes.RemoveAt(i);
                    continue;
                }

                resistance = Mathf.Max(
                    resistance,
                    node.GetResistanceAt(enemyPosition));
            }

            return Mathf.Clamp01(1f - resistance);
        }

        public void Activate(
            ActiveHexZoneContext context,
            ActiveHexZoneData data)
        {
            _context = context;
            _data = data as PullNodeHexZoneData;
            _lifeTimer = 0f;
            _activationTimer = 0f;
            _isActivated = false;

            if (_data == null)
            {
                Debug.LogWarning(
                    "PullNodeTileBehavior requires PullNodeHexZoneData.");
                _context?.CompleteZone();
                return;
            }

            _context?.ShowTileVisual(
                _data.ReadyMaterial,
                enableZoneTrigger: true);
        }

        public void Deactivate()
        {
            UnregisterActiveNode();
            _context?.ResetVisuals();
            _context = null;
            _data = null;
            _lifeTimer = 0f;
            _activationTimer = 0f;
            _isActivated = false;
        }

        public void Tick(float deltaTime)
        {
            if (_context == null || _data == null)
                return;

            if (!_isActivated)
            {
                _lifeTimer += deltaTime;

                if (_lifeTimer >= _data.Lifetime)
                    _context.CompleteZone(0f);

                return;
            }

            _activationTimer += deltaTime;

            if (_activationTimer >= _data.ActivationDuration)
            {
                _context.CompleteZone(_data.ActivatedRespawnDelay);
                return;
            }

            PullEnemies(deltaTime);
        }

        public void OnPlayerEnter(PlayerCharacter player)
        {
            if (player == null || _isActivated || _data == null)
                return;

            _isActivated = true;
            _lifeTimer = 0f;
            _activationTimer = 0f;

            Material activeMaterial =
                _data.ActiveMaterial != null
                    ? _data.ActiveMaterial
                    : _data.ReadyMaterial;

            _context?.ShowTileVisual(activeMaterial);
            RegisterActiveNode();
        }

        public void OnPlayerExit(PlayerCharacter player) { }

        private void PullEnemies(float deltaTime)
        {
            if (GameManager.Instance == null ||
                GameManager.Instance.CharacterFactory == null)
            {
                return;
            }

            List<Character> characters =
                GameManager.Instance.CharacterFactory.ActiveCharacters;

            for (int i = 0; i < characters.Count; i++)
            {
                Character character = characters[i];

                if (!CanPullCharacter(character))
                    continue;

                ApplyPull(character, deltaTime);
            }
        }

        private bool CanPullCharacter(Character character)
        {
            if (character == null ||
                character.CharacterType == CharacterType.DefaultPlayer)
            {
                return false;
            }

            if (character.LiveComponent == null ||
                !character.LiveComponent.IsAlive)
            {
                return false;
            }

            if (character.CharacterData == null ||
                character.CharacterData.CharacterController == null ||
                !character.CharacterData.CharacterController.enabled)
            {
                return false;
            }

            return true;
        }

        private void ApplyPull(Character character, float deltaTime)
        {
            Vector3 toCenter =
                _context.WorldPosition - character.transform.position;

            toCenter.y = 0f;

            float distance = toCenter.magnitude;
            float radius = GetPullRadius();

            if (distance > radius ||
                distance <= Mathf.Max(0.01f, _data.CenterStopRadius))
            {
                return;
            }

            Vector3 direction = toCenter / distance;
            float strength = 1f - Mathf.Clamp01(distance / radius);
            float pullDistance = _data.PullSpeed * strength * deltaTime;

            character.CharacterData.CharacterController.Move(
                direction * pullDistance);
        }

        private float GetResistanceAt(Vector3 enemyPosition)
        {
            Vector3 offset = enemyPosition - _context.WorldPosition;
            offset.y = 0f;

            float radius = GetPullRadius();
            float distance = offset.magnitude;

            if (distance > radius)
                return 0f;

            float strength = 1f - Mathf.Clamp01(distance / radius);
            return _data.EnemyMoveResistance * strength;
        }

        private float GetPullRadius()
        {
            float tileRadius =
                _context != null
                    ? _context.TileWorldRadius
                    : 0f;

            if (tileRadius <= 0f)
                return Mathf.Max(0.1f, _data.PullRadius);

            int extraRings = Mathf.Max(0, _data.PullExtraTileRings);
            float radius = tileRadius * (1f + Mathf.Sqrt(3f) * extraRings);

            return Mathf.Max(0.1f, radius);
        }

        private void RegisterActiveNode()
        {
            if (!ActiveNodes.Contains(this))
                ActiveNodes.Add(this);
        }

        private void UnregisterActiveNode()
        {
            ActiveNodes.Remove(this);
        }
    }
}
