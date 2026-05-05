using UnityEngine;

namespace OmniumLessons
{
    public class GameLevelEntryPoint : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Instance.StartNewSession();

            GameManager.Instance.WindowsService.ShowWindow<GameplayWindow>(false);
        }
    }
}