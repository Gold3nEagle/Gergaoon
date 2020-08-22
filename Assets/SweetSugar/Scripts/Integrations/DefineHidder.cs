using UnityEngine;

namespace SweetSugar.Scripts.Integrations
{
    /// <summary>
    /// Shows Invite button if GetSocial installed
    /// </summary>
    public class DefineHidder : MonoBehaviour
    {

        private void Start()
        {
#if !USE_GETSOCIAL_UI
            gameObject.SetActive(false);
#endif
        }
    }
}