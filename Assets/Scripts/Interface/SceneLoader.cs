using UnityEngine;
using UnityEngine.SceneManagement;

namespace OmniumLessons
{
    public static class SceneLoader
    {
        public const int MainMenuSceneIndex = 0;
        public const int LoadingSceneIndex = 1;
        public const int GameLevelSceneIndex = 2;

        private static int _targetSceneIndex;

        public static int TargetSceneIndex => _targetSceneIndex;

        public static void LoadScene(int sceneIndex)
        {
            _targetSceneIndex = sceneIndex;
            SceneManager.LoadScene(LoadingSceneIndex);
        }

        public static void LoadMainMenu()
        {
            LoadScene(MainMenuSceneIndex);
        }

        public static void LoadGameLevel()
        {
            LoadScene(GameLevelSceneIndex);
        }
    }
}