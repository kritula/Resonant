using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class LoadingWindow : MonoBehaviour
    {
        [SerializeField] private Slider _loadingSlider;
        [SerializeField] private TMP_Text _progressText;

        private void Start()
        {
            StartCoroutine(LoadTargetScene());
        }

        private IEnumerator LoadTargetScene()
        {
            int targetSceneIndex = SceneLoader.TargetSceneIndex;

            // fallback если вдруг ничего не задано
            if (targetSceneIndex < 0)
                targetSceneIndex = SceneLoader.MainMenuSceneIndex;

            AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneIndex);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);

                if (_loadingSlider != null)
                    _loadingSlider.value = progress;

                if (_progressText != null)
                    _progressText.text = $"{Mathf.RoundToInt(progress * 100f)}%";

                if (operation.progress >= 0.9f)
                {
                    yield return new WaitForSeconds(0.3f);
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}