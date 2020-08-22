using SweetSugar.Scripts.Core;
using TMPro;
using UnityEngine;

//1.1
namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Localized prices handler
    /// </summary>
    public class PriceLocalisation : MonoBehaviour
    {
        public TextMeshProUGUI[] prices;


        // Update is called once per frame
        void Update()
        {
#if UNITY_PURCHASING && UNITY_INAPPS
            if (UnityInAppsIntegration.m_StoreController == null) return;
            for (int i = 0; i < prices.Length; i++)
            {
                if (UnityInAppsIntegration.m_StoreController.products.WithID(LevelManager.THIS.InAppIDs[i]).metadata.localizedPrice > new decimal(0.01))
                    prices[i].text = UnityInAppsIntegration.m_StoreController.products.WithID(LevelManager.THIS.InAppIDs[i]).metadata.localizedPriceString;
            }
#endif
        }
    }
}
