using System;
using UnityEngine;

namespace OmniumLessons
{
    public class GameSessionController
    {
        private readonly CharacterFactory _characterFactory;
        private readonly ExperiencePickupSpawner _experiencePickupSpawner;
        private readonly Action<Character> _registerCharacter;
        private readonly Func<HexGridManager> _getHexGridManager;
        private readonly Action<HexGridManager> _setHexGridManager;

        public GameSessionController(
            CharacterFactory characterFactory,
            ExperiencePickupSpawner experiencePickupSpawner,
            Action<Character> registerCharacter,
            Func<HexGridManager> getHexGridManager,
            Action<HexGridManager> setHexGridManager)
        {
            _characterFactory = characterFactory;
            _experiencePickupSpawner = experiencePickupSpawner;
            _registerCharacter = registerCharacter;
            _getHexGridManager = getHexGridManager;
            _setHexGridManager = setHexGridManager;
        }

        public void StartSession(
            ResonanceManager resonanceManager,
            ExperienceManager experienceManager)
        {
            resonanceManager.ResetSession();
            experienceManager.StartGame();

            Character player =
                _characterFactory.CreateCharacter(CharacterType.DefaultPlayer);

            player.transform.position =
                CharacterSpawnHeights.Apply(Vector3.zero, CharacterType.DefaultPlayer);

            CharacterController controller =
                player.CharacterData != null
                    ? player.CharacterData.CharacterController
                    : null;

            if (controller != null)
                controller.enabled = true;

            player.gameObject.SetActive(true);
            _registerCharacter?.Invoke(player);

            AttachCameraToPlayer(player);
            InitializeHexGrid(player);
        }

        public void ClearSession(
            ResonanceManager resonanceManager,
            ExperienceManager experienceManager)
        {
            resonanceManager.ResetSession();
            experienceManager.StartGame();

            HexGridManager hexGridManager = ResolveHexGridManager();

            if (hexGridManager != null)
                hexGridManager.ResetGrid();

            _experiencePickupSpawner?.ClearAll();
            _characterFactory.ClearAll();
        }

        private void AttachCameraToPlayer(Character player)
        {
            CameraFollow cameraFollow =
                Camera.main != null
                    ? Camera.main.GetComponent<CameraFollow>()
                    : null;

            if (cameraFollow != null)
                cameraFollow.SetTarget(player.transform);
        }

        private void InitializeHexGrid(Character player)
        {
            HexGridManager hexGridManager = ResolveHexGridManager();

            if (hexGridManager != null)
                hexGridManager.Initialize(player.transform);
        }

        private HexGridManager ResolveHexGridManager()
        {
            HexGridManager hexGridManager = _getHexGridManager?.Invoke();

            if (hexGridManager == null)
            {
                hexGridManager = UnityEngine.Object
                    .FindFirstObjectByType<HexGridManager>();

                _setHexGridManager?.Invoke(hexGridManager);
            }

            return hexGridManager;
        }
    }
}
