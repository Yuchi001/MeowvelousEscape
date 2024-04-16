using System;
using System.Collections.Generic;
using System.Linq;
using CatPackage;
using Managers;
using Managers.SavingProgress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UiCatInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI descriptionField;
        [SerializeField] private TextMeshProUGUI nameField;
        [SerializeField] private TextMeshProUGUI tierField;
        [SerializeField] private List<StatDisplay> statDisplays;
        [SerializeField] private Image tierImage;

        private SaveFile _saveFile;
        
        private void Awake()
        {
            _saveFile = GameManager.Instance.SaveFile;
            UiSidebar.OnStartGame += DisableDisplay;
        }

        private void OnDisable()
        {
            UiSidebar.OnStartGame -= DisableDisplay;
        }

        private void DisableDisplay()
        {
            gameObject.SetActive(false);
        }

        public void SetCatInfo(SOCat cat)
        {
            var displayInfo = cat.GetDisplayInfo();
            var savedCatData = _saveFile.UnlockedCats.List.FirstOrDefault(c => c.CatName == displayInfo.CatName);
            if (savedCatData == default) return;
            
            var detailedInfo = cat.GetSpecificInfo(savedCatData.Level);

            tierField.text = GetTierInArabic((int)displayInfo.CatTier);
            nameField.text = $"{displayInfo.CatName} ({displayInfo.CatBreed})";
            descriptionField.text = displayInfo.AbilityDescription;

            List<(EStatType statType, float value)> valueTuples = new()
            {
                (EStatType.cooldown, detailedInfo.Cooldown),
                (EStatType.health, detailedInfo.MaxHealth),
                (EStatType.level, savedCatData.Level),
                (EStatType.attackDamage, detailedInfo.Damage),
                (EStatType.attackSpeed, detailedInfo.AttacksPerSecond),
            };

            foreach (var statDisplay in statDisplays)
            {
                var statTuple = valueTuples.FirstOrDefault(s => s.statType == statDisplay.statType);
                if (statTuple == default) continue;
                statDisplay.statTextField.text = $"<sprite name=\"{statTuple.statType}\">: {statTuple.value} {statDisplay.postFix}";
            }
        }

        private static string GetTierInArabic(int tier)
        {
            return tier < 4 ? new string('I', tier) : "IV";
        }
    }

    [System.Serializable]
    public class StatDisplay
    {
        public TextMeshProUGUI statTextField;
        public EStatType statType;
        public string postFix;
    }
}