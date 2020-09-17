using Malee;
using TMPro;
using UnityEngine;

namespace SweetSugar.Scripts.Localization
{
    public class LocalizeText : MonoBehaviour
    {
        private TextMeshProUGUI textObject;
        public int instanceID;
        [SerializeField, Reorderable( elementNameProperty = "language"),HideInInspector]

        private string _originalText;
        private string _currentText; 

        FixArabicTMProUGUI fixArabic;

        private void Awake()
        {
            textObject = GetComponent<TextMeshProUGUI>();
            fixArabic = GetComponent<FixArabicTMProUGUI>(); 
        }

        private void OnEnable()
        { 
            _originalText = textObject.text;
            _currentText = LocalizationManager.GetText(instanceID, _originalText);

            if(fixArabic != null)
            {
                fixArabic.UpdateText(_currentText);
            }
            else
            {
                textObject.text = _currentText;
            }


        }
    }
}