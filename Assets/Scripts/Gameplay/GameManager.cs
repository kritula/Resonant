using UnityEngine;

namespace OmniumLessons
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private CharacterFactory _characterFactory;
        [SerializeField] private GameData _gameData;
        [SerializeField] private WindowsService _windowsService;
        [SerializeField] private UpgradeDatabase _abilityDatabase;
        [SerializeField] private ExperiencePickupSpawner _experiencePickupSpawner;
        [Header("Boss")]
        [SerializeField] private CharacterType _bossCharacterType = CharacterType.Boss_Null_Core;
        [SerializeField] private float _bossSpawnDistanceFromPlayer = 10f;
        [SerializeField] private bool _stopRegularSpawnOnBossAppear = true;

        private bool _isGameActive;
        private bool _bossSpawned;
        private float _gameTimeSec;

        private CharacterSpawnController _spawnController;
        private CharacterDeathService _characterDeathService;
        private GameSessionController _sessionController;
        private BossPhaseController _bossPhaseController;
        private GameResultController _resultController;
        private LevelUpUpgradeController _levelUpUpgradeController;
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
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Initialize();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void Initialize()
        {
            ResonanceManager = new ResonanceManager();
            ExperienceManager = new ExperienceManager(
                _gameData.BaseExperience,
                _gameData.GrownRate);

            ResonanceManager.ResetSession();
            _windowsService.Initialize();
            HexGridManager = FindFirstObjectByType<HexGridManager>();

            _characterDeathService = new CharacterDeathService(
                _characterFactory,
                _experiencePickupSpawner,
                _windowsService,
                GameOver,
                GameVictory);

            _sessionController = new GameSessionController(
                _characterFactory,
                _experiencePickupSpawner,
                RegisterCharacter,
                () => HexGridManager,
                hexGridManager => HexGridManager = hexGridManager);

            _bossPhaseController = new BossPhaseController(
                _characterFactory,
                _bossCharacterType,
                _bossSpawnDistanceFromPlayer,
                _stopRegularSpawnOnBossAppear,
                RegisterCharacter);

            _resultController = new GameResultController(
                _windowsService,
                ResonanceManager);
            _levelUpUpgradeController = new LevelUpUpgradeController(
                _windowsService,
                _abilityDatabase,
                PauseGame);

            ExperienceManager.OnLevelChanged += OnLevelUp;
        }

        public void StartGame()
        {
            if (_isGameActive)
            {
                Debug.Log("Game is already active");
                return;
            }

            _gameTimeSec = 0f;
            _bossSpawned = false;

            _sessionController.StartSession(
                ResonanceManager,
                ExperienceManager);

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
                _spawnController.OnUpdate(Time.deltaTime);

            if (!_bossSpawned && _gameTimeSec >= _gameData.GameTimeSecondsMax)
                SpawnBossPhase();
        }

        public void ClearSession()
        {
            Time.timeScale = 1f;
            _isGameActive = false;
            IsGamePaused = true;
            StopSpawnController();

            _gameTimeSec = 0f;
            _bossSpawned = false;

            _sessionController?.ClearSession(
                ResonanceManager,
                ExperienceManager);
        }

        public void StartNewSession() { ClearSession(); StartGame(); }

        public void FinishCharacterDeath(Character deathCharacter) =>
            _characterDeathService?.FinishCharacterDeath(deathCharacter);

        public void RegisterCharacter(Character character) =>
            _characterDeathService?.RegisterCharacter(character);

        private void SpawnBossPhase()
        {
            _bossSpawned = true;
            _bossPhaseController.StartBossPhase(_spawnController);
        }

        private void GameOver()
        {
            if (!_isGameActive)
                return;

            _isGameActive = false;
            IsGamePaused = true;
            _resultController.ShowGameOver(_spawnController);
        }

        private void GameVictory()
        {
            if (!_isGameActive)
                return;

            _isGameActive = false;
            IsGamePaused = true;
            _resultController.ShowVictory(_spawnController);
        }

        private void OnLevelUp(int level) =>
            _levelUpUpgradeController.TryShowLevelUpChoices(_isGameActive, IsGamePaused);

        public void ShowPauseMenu()
        {
            PauseGame();
            _windowsService?.ShowWindow<PauseMenuWindow>(false);
        }

        private void PauseGame()
        {
            IsGamePaused = true;
            Time.timeScale = 0f;
        }

        private void StopSpawnController()
        {
            if (_spawnController == null)
                return;
            _spawnController.StopSpawn();
            _spawnController = null;
        }
    }
}
