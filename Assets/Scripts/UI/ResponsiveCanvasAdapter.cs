using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OmniumLessons
{
    [DefaultExecutionOrder(-10000)]
    public static class ResponsiveCanvasAdapter
    {
        private static readonly Vector2 ReferenceResolution = new Vector2(1920f, 1080f);
        private static readonly HashSet<int> NormalizedCanvasRects = new HashSet<int>();

        private static bool _isInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
            Canvas.willRenderCanvases += ApplyToLoadedCanvases;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            NormalizedCanvasRects.Clear();
            ApplyToLoadedCanvases();
        }

        private static void ApplyToLoadedCanvases()
        {
            Canvas[] canvases = Object.FindObjectsByType<Canvas>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            for (int i = 0; i < canvases.Length; i++)
            {
                ApplyToCanvas(canvases[i]);
            }
        }

        private static void ApplyToCanvas(Canvas canvas)
        {
            if (canvas == null || canvas.renderMode == RenderMode.WorldSpace)
                return;

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();

            if (scaler == null)
                scaler = canvas.gameObject.AddComponent<CanvasScaler>();

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            NormalizeCanvasRect(canvas);
        }

        private static void NormalizeCanvasRect(Canvas canvas)
        {
            int instanceId = canvas.GetInstanceID();

            if (NormalizedCanvasRects.Contains(instanceId))
                return;

            if (!(canvas.transform is RectTransform rectTransform))
                return;

            NormalizedCanvasRects.Add(instanceId);

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.localScale = Vector3.one;
        }
    }
}
