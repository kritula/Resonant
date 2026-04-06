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
        private  float _gameTimeSec = 0;

        private CharacterSpawnController _spawnController;

        private UpgradeSelectionService _upgradeSelectionService;

        public static GameManager Instance { get; private set; }
        
        public ScoreManager ScoreManager { get; private set; }
        public WindowsService WindowsService => _windowsService;
        public CharacterFactory CharacterFactory => _characterFactory;
        public GameData GameData => _gameData;
        public float GameTime => _gameTimeSec;
        public ExperienceManager ExperienceManager { get; private set; }

        


        // Добавим концептуально еще понятие "игра на паузе", в отличие от нашего урока. В целом, ничего не поменяется, но сам контроль над игрой станет более гибким.
        public bool IsGamePaused
        {
            get;
            set;
        } = true;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
                Initialize();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Initialize()
        {
            ScoreManager = new ScoreManager();
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
            
            var player = CharacterFactory.CreateCharacter(CharacterType.DefaultPlayer);
            player.transform.position = Vector3.zero;
            player.gameObject.SetActive(true);
            RegisterCharacter(player);

            // привязываем камеру к игроку
            var cameraFollow = Camera.main.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(player.transform);
            }

            _gameTimeSec = 0f;
            ScoreManager.StartGame();

            // 4) Запускаем спавн-контроллер
            _spawnController = new CharacterSpawnController();
            _spawnController.StartSpawn();

            _isGameActive = true;
            IsGamePaused = false;
        }

        private void Update()
        {
            if (!_isGameActive || IsGamePaused)
                return;
            
            _gameTimeSec += Time.deltaTime;

            // Спавн теперь живёт здесь
            _spawnController.OnUpdate(Time.deltaTime);

            if (_gameTimeSec >= _gameData.GameTimeSecondsMax)
            {
                GameVictory();
            }
        }

        // ВАЖНО: сюда будет обращаться SpawnController после создания врага
        public void RegisterCharacter(Character character)
        {
            if (character == null || character.LiveComponent == null)
                return;

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
                    ScoreManager.AddScore(deathCharacter.CharacterData.ScoreCost);
                    Debug.Log("Score = " + ScoreManager.GameScore);
                    ExperienceManager.AddExperience(deathCharacter.CharacterData.ExperienceReward);
                    Debug.Log("Exp = " + ExperienceManager.CurrentExperience);
                    break;

            }
        
            CharacterFactory.ReturnCharacterToPool(deathCharacter);
            deathCharacter.gameObject.SetActive(false);

            // отписка
            deathCharacter.LiveComponent.OnCharacterDeath -= OnCharacterDeathHandler;
        }

        public void ClearSession()
        {
            // UI/анимации и кнопки должны работать
            Time.timeScale = 1f;

            // Останавливаем матч
            _isGameActive = false;
            IsGamePaused = true;

            // Остановить спавн 
            if (_spawnController != null)
                _spawnController.StopSpawn();

            // Сброс таймера
            _gameTimeSec = 0f;

            // Сброс очков текущей сессии (не глобальных)
            ScoreManager.StartGame();

            ExperienceManager.StartGame();

            // Убрать всех персонажей
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
            Debug.Log("Score = " + ScoreManager.GameScore);
            Debug.Log("ScoreMax = " + ScoreManager.ScoreMax);
            ScoreManager.CompleteMatch();
            _isGameActive = false;
            IsGamePaused = true;

            // останавливаем спавн
            _spawnController.StopSpawn();
            WindowsService.ShowWindow<DefeatWindow>(false);
        }

        private void GameVictory()
        {
            Debug.Log("Game Over! Time's up!");
            ScoreManager.CompleteMatch();
            _isGameActive = false;
            IsGamePaused = true;

            // останавливаем спавн
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