using System;
using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class WindowsService : MonoBehaviour
    {
        [SerializeField] private Window[] _windows;
        [SerializeField] private Window _startWindow;

        private Dictionary<Type, Window> _windowsDictionary;

        private void Start()
        {
            Initialize();

            if (_startWindow != null)
                _startWindow.Show(true);
        }

        public void Initialize()
        {
            _windowsDictionary = new Dictionary<Type, Window>();

            foreach (Window window in _windows)
            {
                _windowsDictionary.Add(window.GetType(), window);
                window.Initialize();
                window.Hide(true);
            }
        }

        public T GetWindow<T>() where T : Window
        {
            return _windowsDictionary[typeof(T)] as T;
        }

        public void ShowWindow<T>(bool isImmediately) where T : Window
        {
            if (_windowsDictionary.TryGetValue(typeof(T), out Window window) == false)
            {
                Debug.LogError($"Window {typeof(T).Name} not found");
                return;
            }

            window.Show(isImmediately);
        }

        public void HideWindow<T>(bool isImmediately) where T : Window
        {
            if (_windowsDictionary.TryGetValue(typeof(T), out Window window) == false)
            {
                Debug.LogError($"Window {typeof(T).Name} not found");
                return;
            }

            window.Hide(isImmediately);
        }
    }
}