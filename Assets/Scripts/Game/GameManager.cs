using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private CharacterFactory _characterFactory;
        [SerializeField] private GameData _gameData;
        [SerializeField] private WindowsService _windowsService;
        [SerializeField] private UpgradeDatabase _abilityDatabase;

        private bool _isGameActive = false;
        private float _gameTimeSec = 0f;

        private CharacterSpawnController _spawnController;
        private UpgradeSelectionService _upgradeSelectionService;

        public static GameManager Instance { get; private set; }

        public ResonanceManager ResonanceManager { get; private set; }
        public ExperienceManager ExperienceManager { get; private set; }

        public WindowsService WindowsService => _windowsService;
        public CharacterFactory CharacterFactory => _characterFactory;
        public GameData GameData => _gameData;
        public float GameTime => _gameTimeSec;

        public bool IsGamePaused { get; set; } = true;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            ResonanceManager = new ResonanceManager();
            ExperienceManager = new ExperienceManager();
            _upgradeSelectionService = new UpgradeSelectionService(_abilityDatabase);

            _windowsService.Initialize();
            ExperienceManager.OnLevelChanged += OnLevelUp;
        }

        public void StartGame()
        {
            if (_isGameActive)
            {
                Debug.Log("Game is already active");
                return;
            }

            ResonanceManager.ResetProgress();
            ExperienceManager.StartGame();

            Character player = CharacterFactory.CreateCharacter(CharacterType.DefaultPlayer);
            player.transform.position = Vector3.zero;
            player.gameObject.SetActive(true);
            RegisterCharacter(player);

            CameraFollow cameraFollow = Camera.main != null ? Camera.main.GetComponent<CameraFollow>() : null;
            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(player.transform);
            }

            _gameTimeSec = 0f;

            _spawnController = new CharacterSpawnController();
            _spawnController.StartSpawn();

            _isGameActive = true;
            IsGamePaused = false;
            Time.timeScale = 1f;
        }

        private void Update()
        {
            if (!_isGameActive || IsGamePaused)
                return;

            _gameTimeSec += Time.deltaTime;

            if (_spawnController != null)
            {
                _spawnController.OnUpdate(Time.deltaTime);
            }

            if (_gameTimeSec >= _gameData.GameTimeSecondsMax)
            {
                GameVictory();
            }
        }

        public void RegisterCharacter(Character character)
        {
            if (character == null || character.LiveComponent == null)
                return;

            character.LiveComponent.OnCharacterDeath -= OnCharacterDeathHandler;
            character.LiveComponent.OnCharacterDeath += OnCharacterDeathHandler;
        }

        private void OnCharacterDeathHandler(Character deathCharacter)
        {
            Debug.Log("character " + deathCharacter.gameObject.name + " is dead");

            switch (deathCharacter.CharacterType)
            {
                case CharacterType.DefaultPlayer:
                    GameOver();
                    break;

                case CharacterType.DefaultEnemy:
                case CharacterType.FastEnemy:
                case CharacterType.TankEnemy:
                    ExperienceManager.AddExperience(deathCharacter.CharacterData.ExperienceReward);
                    Debug.Log("Exp = " + ExperienceManager.CurrentExperience);
                    break;
            }

            if (deathCharacter.LiveComponent != null)
            {
                deathCharacter.LiveComponent.OnCharacterDeath -= OnCharacterDeathHandler;
            }

            CharacterFactory.ReturnCharacterToPool(deathCharacter);
            deathCharacter.gameObject.SetActive(false);
        }

        public void ClearSession()
        {
            Time.timeScale = 1f;

            _isGameActive = false;
            IsGamePaused = true;

            if (_spawnController != null)
            {
                _spawnController.StopSpawn();
                _spawnController = null;
            }

            _gameTimeSec = 0f;

            ResonanceManager.ResetProgress();
            ExperienceManager.StartGame();

            CharacterFactory.ClearAll();
        }

        public void StartNewSession()
        {
            ClearSession();
            StartGame();
        }

        private void GameOver()
        {
            Debug.Log("GameOver!");
            Debug.Log("Resonance = " + ResonanceManager.CurrentResonance);

            _isGameActive = false;
            IsGamePaused = true;

            if (_spawnController != null)
                _spawnController.StopSpawn();

            WindowsService.ShowWindow<DefeatWindow>(false);
        }

        private void GameVictory()
        {
            Debug.Log("Victory! Time's up!");
            Debug.Log("Resonance = " + ResonanceManager.CurrentResonance);

            _isGameActive = false;
            IsGamePaused = true;

            if (_spawnController != null)
                _spawnController.StopSpawn();

            WindowsService.ShowWindow<VictoryWindow>(false);
        }

        private void OnLevelUp(int level)
        {
            IsGamePaused = true;
            Time.timeScale = 0f;

            List<UpgradeData> upgrades = _upgradeSelectionService.GetRandomUpgrades(3);

            SkillsWindow skillsWindow = WindowsService.GetWindow<SkillsWindow>();
            skillsWindow.ShowUpgrades(upgrades);

            WindowsService.ShowWindow<SkillsWindow>(true);
        }
    }
}