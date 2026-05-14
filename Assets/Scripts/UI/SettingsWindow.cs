using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class SettingsWindow : Window
    {
        [Header("Toggles")]
        [SerializeField] private Toggle soundToggle;
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle vibrationToggle;
        [SerializeField] private Toggle notificationsToggle;

        [Header("Buttons")]
        [SerializeField] private Button quitButton;

        public override void Initialize()
        {
            quitButton?.onClick.AddListener(OnQuitClicked);

            soundToggle?.onValueChanged.AddListener(OnSoundChanged);
            musicToggle?.onValueChanged.AddListener(OnMusicChanged);
            vibrationToggle?.onValueChanged.AddListener(OnVibrationChanged);
            notificationsToggle?.onValueChanged.AddListener(OnNotificationsChanged);
        }

        public void BindControls(
            Toggle sound,
            Toggle music,
            Toggle vibration,
            Toggle notifications,
            Button quit)
        {
            soundToggle = sound;
            musicToggle = music;
            vibrationToggle = vibration;
            notificationsToggle = notifications;
            quitButton = quit;
        }

        private void OnQuitClicked()
        {
            Hide(true);

            WindowsService windowsService = GetComponentInParent<WindowsService>();

            if (windowsService != null && windowsService.HasWindow<MainMenuWindow>())
            {
                windowsService.ShowWindow<MainMenuWindow>(false);
                return;
            }

            if (GameManager.Instance != null && GameManager.Instance.WindowsService != null)
                GameManager.Instance.WindowsService.ShowWindow<PauseMenuWindow>(false);
        }

        private void OnSoundChanged(bool value)
        {
            Debug.Log("Sound: " + value);
        }

        private void OnMusicChanged(bool value)
        {
            Debug.Log("Music: " + value);
        }

        private void OnVibrationChanged(bool value)
        {
            Debug.Log("Vibration: " + value);
        }

        private void OnNotificationsChanged(bool value)
        {
            Debug.Log("Notifications: " + value);
        }
    }
}
