using SweetSugar.Scripts.Core;
using UnityEngine;

namespace SweetSugar.Scripts.GUI.Purchasing
{
    public class PurchaseFulfillment : MonoBehaviour
    {
        public void GrandCoins(int amount)
        {
            InitScript.Instance.AddGems(amount);
        }
    }
}