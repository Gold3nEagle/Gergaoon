using UnityEngine;
using UnityEngine.EventSystems;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Setting button
    /// </summary>
    public class DonutSettingsHandler : MonoBehaviour
    {
        private void Update()
        {
            if(Input.GetMouseButton(0))
            {
                if (EventSystem.current.IsPointerOverGameObject(-1))
                {
                    if(EventSystem.current.currentSelectedGameObject == null) gameObject.SetActive(false);
                }
            }

        }
    }
}