using UnityEngine;

namespace SweetSugar.Scripts
{
    public class PrivacyButton : UnityEngine.MonoBehaviour
    {
        public void OpenPrivacy()
        {
            Application.OpenURL("https://docs.google.com/document/d/1f9yEo_CSsVvb5vYVtyKtIfdxgaY3aTmUjwPyiKqUS0A/edit?usp=sharing");
        }
    }
}