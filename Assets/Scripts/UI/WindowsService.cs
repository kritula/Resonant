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
        private bool _isInitialized;

        private void Start()
        {
            Initialize();

            if (_startWindow != null)
                _startWindow.Show(true);
        }

        public void Initialize()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
            _windowsDictionary = new Dictionary<Type, Window>();

            RegisterWindows(_windows);
            RegisterWindows(GetComponentsInChildren<Window>(true));
        }

        public bool HasWindow<T>() where T : Window
        {
            return _windowsDictionary != null &&
                   _windowsDictionary.ContainsKey(typeof(T));
        }

        public void RegisterWindow(Window window)
        {
            RegisterWindowInternal(window);
        }

        private void RegisterWindows(Window[] windows)
        {
            if (windows == null)
                return;

            foreach (Window window in windows)
            {
                RegisterWindowInternal(window);
            }
        }

        private void RegisterWindowInternal(Window window)
        {
            if (window == null)
                return;

            if (_windowsDictionary == null)
                _windowsDictionary = new Dictionary<Type, Window>();

            if (_windowsDictionary.ContainsKey(window.GetType()))
                return;

            _windowsDictionary.Add(window.GetType(), window);
            window.Initialize();
            window.Hide(true);
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
