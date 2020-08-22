using System;
using System.Collections.Generic;
using SweetSugar.Scripts.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SweetSugar.Scripts.System.Orientation
{
    /// <summary>
    /// Activates object from the array depending from orientation
    /// </summary>
    [ExecuteInEditMode]
    public class OrientationActivator : MonoBehaviour
    {

        public List<Object> horrizontalObjects;
        public List<Object> horrizontalObjectsHD;
        public List<Object> verticalObjects;
        private RectTransform[] currentPanels;

        public RectTransform[] GetCurrentPanels()
        {
            return currentPanels;
        }

        void OnEnable()
        {
            OnOrientationChanged(OrientationListener.previousOrientation);
            OrientationListener.OnOrientationChanged += OnOrientationChanged;
        }

        void OnDisable()
        {
            OrientationListener.OnOrientationChanged -= OnOrientationChanged;
        }
        public virtual void OnOrientationChanged(ScreenOrientation orientation)
        {
            if (orientation == ScreenOrientation.Portrait)
            {
                var aspect = Screen.height / (float)Screen.width;
                aspect = (float)Math.Round(aspect, 2);

                SetActiveList(horrizontalObjects, false);
                SetActiveList(verticalObjects, true);
                SetActiveList(horrizontalObjectsHD, false);

            }

            else if (orientation == ScreenOrientation.Landscape)
            {
                var aspect = Screen.width / (float)Screen.height;
                aspect = (float)Math.Round(aspect, 2);
                if (aspect >= 1.6f)
                {
                    SetActiveList(horrizontalObjects, false);
                    SetActiveList(horrizontalObjectsHD, true);
                }
                if (aspect < 1.6f)
                {
                    SetActiveList(horrizontalObjectsHD, false);
                    SetActiveList(horrizontalObjects, true);
                }
                SetActiveList(verticalObjects, false);
            }
        }

        private void SetActiveList(List<Object> list, bool activate)
        {
            foreach (var item in list)
            {
                if (item.GetType() == typeof(GameObject))
                {
                    var gameObj = (GameObject)item;
                    gameObj.SetActive(activate);
                    if (activate)
                    {
                        OrientationPanels orientationPanels = gameObj.GetComponent<OrientationPanels>();
                        currentPanels = orientationPanels?.panels;
                        if (orientationPanels)
                            LevelManager.THIS.movesTransform = orientationPanels.movesTransform;
                    }
                }
                else
                {
                    var gameObj = (MonoBehaviour)item;
                    gameObj.enabled = activate;
                }
            }
        }
    }
}
