using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class SkillsWindow : Window
    {
        //[SerializeField] private Button backButton;

        // ймнойх яонянамняреи
        [SerializeField] private AbilityButton[] abilityButtons;

        public override void Initialize()
        {
            //backButton.onClick.AddListener(OnBackClicked);
        }

        //private void OnBackClicked()
        //{
        //    Hide(false);
        //    GameManager.Instance.WindowsService.ShowWindow<PauseMenuWindow>(true);
        //}

        public void ShowAbilities(List<AbilityData> abilities)
        {
            for (int i = 0; i < abilities.Count && i < abilityButtons.Length; i++)
            {
                abilityButtons[i].Setup(abilities[i]);
            }
        }
    }
}