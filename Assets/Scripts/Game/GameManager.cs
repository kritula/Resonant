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

        [Header("Boss")]
        [SerializeField] private CharacterType _bossCharacterType = CharacterType.Boss_Null_Core;
        [SerializeField] private float _bossSpawnDistanceFromPlayer = 10f;
        [SerializeField] private bool _stopRegularSpawnOnBossAppear = true;

        private bool _isGameActive = false;
        private bool _bossSpawned = false;
        private float _gameTimeSec = 0f;

        private CharacterSpawnController _spawnController;
        private UpgradeSelectionService _upgradeSelectionService;
        private CharacterDeathService _characterDeathService;

        public static GameManager Instance { get; private set; }

        public ResonanceManager ResonanceManager { get; private set; }
        public ExperienceManager ExperienceManager { get; private set; }
        public HexGridManager HexGridManager { get; private set; }

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
            _upgradeSelectionService = new UpgradeSelectionService();

            ResonanceManager.ResetSession();

            _windowsService.Initialize();

            ExperienceManager.OnLevelChanged += OnLevelUp;

            HexGridManager = FindFirstObjectByType<HexGridManager>();

            _characterDeathService = new CharacterDeathService(
                _characterFactory,
                ExperienceManager,
                _windowsService,
                GameOver,
                GameVictory);
        }

        public void StartGame()
        {
            if (_isGameActive)
            {
                Debug.Log("Game is already active");
                return;
            }

            ResonanceManager.ResetSession();
            ExperienceManager.StartGame();

            Character player = CharacterFactory.CreateCharacter(CharacterType.DefaultPlayer);
            player.transform.position = Vector3.zero;

            CharacterController controller = player.CharacterData != null
                ? player.CharacterData.CharacterController
                : null;

            if (controller != null)
                controller.enabled = true;

            player.gameObject.SetActive(true);
            RegisterCharacter(player);

            CameraFollow cameraFollow = Camera.main != null ? Camera.main.GetComponent<CameraFollow>() : null;
            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(player.transform);
            }

            if (HexGridManager == null)
            {
                HexGridManager = FindFirstObjectByType<HexGridManager>();
            }

            if (HexGridManager != null)
            {
                HexGridManager.Initialize(player.transform);
            }

            _gameTimeSec = 0f;
            _bossSpawned = false;

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

            ResonanceManager?.OnUpdate(Time.deltaTime);

            if (!_bossSpawned && _spawnController != null)
            {
                _spawnController.OnUpdate(Time.deltaTime);
            }

            if (!_bossSpawned && _gameTimeSec >= _gameData.GameTimeSecondsMax)
            {
                SpawnBossPhase();
            }
        }

        private void SpawnBossPhase()
        {
            _bossSpawned = true;

            if (_stopRegularSpawnOnBossAppear && _spawnController != null)
            {
                _spawnController.StopSpawn();
            }

            SpawnBoss();
        }

        private void SpawnBoss()
        {
            Character player = CharacterFactory.Player;

            if (player == null)
            {
                Debug.LogWarning("GameManager: Player not found. Boss spawn cancelled.");
                return;
            }

            Character boss = CharacterFactory.CreateCharacter(_bossCharacterType);

            if (boss == null)
            {
                Debug.LogWarning("GameManager: Boss was not created. Check CharacterFactory boss prefab.");
                return;
            }

            Vector3 spawnDirection = Random.insideUnitSphere;
            spawnDirection.y = 0f;

            if (spawnDirection.sqrMagnitude < 0.01f)
                spawnDirection = Vector3.forward;

            spawnDirection.Normalize();

            Vector3 spawnPosition = player.transform.position + spawnDirection * _bossSpawnDistanceFromPlayer;

            CharacterController bossController = boss.CharacterData != null
                ? boss.CharacterData.CharacterController
                : null;

            if (bossController != null)
                bossController.enabled = false;

            boss.transform.position = spawnPosition;

            if (boss.CharacterData != null && boss.CharacterData.CharacterTransform != null)
            {
                boss.CharacterData.CharacterTransform.position = spawnPosition;
            }

            if (bossController != null)
                bossController.enabled = true;

            boss.gameObject.SetActive(true);
            _characterDeathService.RegisterCharacter(boss); ;

            Debug.Log("Boss spawned: Null Core");
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
            _bossSpawned = false;

            ResonanceManager.ResetSession();
            ExperienceManager.StartGame();

            if (HexGridManager == null)
            {
                HexGridManager = FindFirstObjectByType<HexGridManager>();
            }

            if (HexGridManager != null)
            {
                HexGridManager.ResetGrid();
            }

            CharacterFactory.ClearAll();
        }

        public void StartNewSession()
        {
            ClearSession();
            StartGame();
        }

        private void GameOver()
        {
            if (!_isGameActive)
                return;

            _isGameActive = false;

            Debug.Log("GameOver!");
            Debug.Log("Resonance = " + ResonanceManager.CurrentResonance);

            IsGamePaused = true;
            Time.timeScale = 0f;

            if (_spawnController != null)
                _spawnController.StopSpawn();

            if (WindowsService != null)
            {
                WindowsService.HideWindow<SkillsWindow>(true);
                WindowsService.ShowWindow<DefeatWindow>(false);
            }
        }

        private void GameVictory()
        {
            if (!_isGameActive)
                return;

            _isGameActive = false;

            Debug.Log("Victory! Boss defeated!");
            Debug.Log("Resonance = " + ResonanceManager.CurrentResonance);

            IsGamePaused = true;
            Time.timeScale = 0f;

            if (_spawnController != null)
                _spawnController.StopSpawn();

            if (WindowsService != null)
            {
                WindowsService.HideWindow<SkillsWindow>(true);
                WindowsService.ShowWindow<VictoryWindow>(false);
            }
        }

        private void OnLevelUp(int level)
        {
            if (!_isGameActive)
                return;

            if (IsGamePaused)
                return;

            if (_upgradeSelectionService == null)
                return;

            if (_abilityDatabase == null)
                return;

            List<UpgradeOfferData> upgradeOffers = _upgradeSelectionService.GetUpgradeOffers(_abilityDatabase.Upgrades);

            if (upgradeOffers == null || upgradeOffers.Count == 0)
                return;

            SkillsWindow skillsWindow = WindowsService.GetWindow<SkillsWindow>();
            if (skillsWindow == null)
                return;

            IsGamePaused = true;
            Time.timeScale = 0f;

            WindowsService.ShowWindow<SkillsWindow>(true);
            skillsWindow.ShowUpgrades(upgradeOffers);
        }
        public void FinishCharacterDeath(Character deathCharacter)
        {
            _characterDeathService?.FinishCharacterDeath(deathCharacter);
        }
        public void RegisterCharacter(Character character)
        {
            _characterDeathService?.RegisterCharacter(character);
        }
    }
}