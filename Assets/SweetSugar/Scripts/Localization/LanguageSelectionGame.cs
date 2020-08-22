using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.Localization;
using SweetSugar.Scripts.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class LanguageSelectionGame : UnityEngine.MonoBehaviour
    {
        private IEnumerable<CultureInfo> cultures;
        private TMP_Dropdown Dropdown;
        private void Start()
        {
            Dropdown = GetComponent<TMP_Dropdown>();
            var txt = Resources.LoadAll<TextAsset>("Localization/");
            cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(i => txt.Any(x => x.name == i.DisplayName));
            Dropdown.options = cultures.Select(i => new TMP_Dropdown.OptionData(i.Name.ToUpper(), null)).ToList();
            var _debugSettings = Resources.Load("Scriptable/DebugSettings") as DebugSettings;
            Dropdown.captionText.text = cultures.First(i => i.EnglishName == LocalizationManager.GetSystemLanguage(_debugSettings).ToString()).Name.ToUpper();
        }

        public void OnChangeLanguage()
        {
            LocalizationManager.LoadLanguage((SystemLanguage)Enum.Parse(typeof(SystemLanguage),GetSelectedLanguage().EnglishName));
            CrosssceneData.selectedLanguage = (SystemLanguage) Enum.Parse(typeof(SystemLanguage), GetSelectedLanguage().EnglishName);
        }

        private CultureInfo GetSelectedLanguage()
        {
            return cultures.ToArray()[Dropdown.value];
        }
    }
}