using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class AbilityButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text abilityNameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Image iconImage;
        [SerializeField] private Button selectButton;

        private AbilityData _abilityData;

        public void Setup(AbilityData abilityData)
        {
            _abilityData = abilityData;

            abilityNameText.text = abilityData.AbilityName;
            descriptionText.text = abilityData.Description;
            iconImage.sprite = abilityData.Icon;

            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelect);
        }

        private void OnSelect()
        {
            PlayerCharacter player = GameManager.Instance.CharacterFactory.Player as PlayerCharacter;

            if (player == null)
                return;

            player.AbilityManager.AddAbility(_abilityData);

            GameManager.Instance.IsGamePaused = false;
            Time.timeScale = 1f;

            GameManager.Instance.WindowsService.HideWindow<SkillsWindow>(false);
        }
    }
}