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
        [SerializeField] private Button quitButton; // кнопка "назад" в паузу

        public override void Initialize()
        {
            quitButton.onClick.AddListener(OnQuitClicked);

            // пока просто слушатели (логику подключим позже)
            soundToggle.onValueChanged.AddListener(OnSoundChanged);
            musicToggle.onValueChanged.AddListener(OnMusicChanged);
            vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);
            notificationsToggle.onValueChanged.AddListener(OnNotificationsChanged);
        }

        private void OnQuitClicked()
        {
            // закрываем настройки
            Hide(true);

            // возвращаемся в паузу
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

