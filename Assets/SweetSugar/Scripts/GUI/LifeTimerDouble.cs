using SweetSugar.Scripts.Localization;
using TMPro;
using UnityEngine;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Time message in the Lifeshop
    /// </summary>
    public class LifeTimerDouble : MonoBehaviour
    {
        public TextMeshProUGUI textSource;
        public TextMeshProUGUI textDest;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            textDest.text = "+1" + LocalizationManager.GetText(0, "life after") + textSource.text;
        }
    }
}