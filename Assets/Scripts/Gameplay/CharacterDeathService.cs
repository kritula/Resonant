using UnityEngine;

namespace OmniumLessons
{
    public class CharacterDeathService
    {
        private readonly CharacterFactory _characterFactory;
        private readonly ExperiencePickupSpawner _experiencePickupSpawner;
        private readonly WindowsService _windowsService;

        private readonly System.Action _onGameOver;
        private readonly System.Action _onGameVictory;

        public CharacterDeathService(
            CharacterFactory characterFactory,
            ExperiencePickupSpawner experiencePickupSpawner,
            WindowsService windowsService,
            System.Action onGameOver,
            System.Action onGameVictory)
        {
            _characterFactory = characterFactory;
            _experiencePickupSpawner = experiencePickupSpawner;
            _windowsService = windowsService;
            _onGameOver = onGameOver;
            _onGameVictory = onGameVictory;
        }

        public void RegisterCharacter(Character character)
        {
            if (character == null || character.LiveComponent == null)
                return;

            character.LiveComponent.OnCharacterDeath -= OnCharacterDeathHandler;
            character.LiveComponent.OnCharacterDeath += OnCharacterDeathHandler;
        }

        public void FinishCharacterDeath(Character deathCharacter)
        {
            if (deathCharacter == null)
                return;

            if (deathCharacter.CharacterType == CharacterType.DefaultPlayer)
            {
                _onGameOver?.Invoke();
                return;
            }

            _characterFactory.ReturnCharacterToPool(deathCharacter);
            deathCharacter.gameObject.SetActive(false);
        }

        private void OnCharacterDeathHandler(Character deathCharacter)
        {
            if (deathCharacter == null)
                return;

            Debug.Log("character " + deathCharacter.gameObject.name + " is dead");

            switch (deathCharacter.CharacterType)
            {
                case CharacterType.DefaultPlayer:
                    break;

                case CharacterType.DefaultEnemy:
                case CharacterType.FastEnemy:
                case CharacterType.TankEnemy:
                    SpawnExperienceFromEnemy(deathCharacter);
                    break;

                case CharacterType.Boss_Null_Core:
                    _onGameVictory?.Invoke();
                    break;
            }

            if (deathCharacter.LiveComponent != null)
            {
                deathCharacter.LiveComponent.OnCharacterDeath -= OnCharacterDeathHandler;
            }

            CharacterController controller = deathCharacter.CharacterData != null
                ? deathCharacter.CharacterData.CharacterController
                : null;

            if (controller != null)
                controller.enabled = false;
        }

        private void SpawnExperienceFromEnemy(Character enemy)
        {
            if (enemy == null)
                return;

            if (enemy.CharacterData == null)
                return;

            if (_experiencePickupSpawner == null)
            {
                Debug.LogWarning("CharacterDeathService: ExperiencePickupSpawner is not assigned.");
                return;
            }

            int experienceReward = enemy.CharacterData.ExperienceReward;

            _experiencePickupSpawner.SpawnExperiencePickup(
                enemy.transform.position,
                experienceReward);
        }
    }
}