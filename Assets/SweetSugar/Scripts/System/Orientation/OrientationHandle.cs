using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SweetSugar.Scripts.System.Orientation
{
    /// <summary>
    /// Activates Popups depending from orientation
    /// </summary>
    [ExecuteInEditMode]
    public class OrientationHandle : MonoBehaviour
    {
        public List<OrientationObject> list = new List<OrientationObject>();


        void OnEnable()
        {
            OrientationListener.OnOrientationChanged += OnOrientationChanged;
        }

        void OnDisable()
        {
            OrientationListener.OnOrientationChanged -= OnOrientationChanged;
        }
        void OnOrientationChanged(ScreenOrientation orientation)
        {
            foreach (var orientationObject in list)
            {
                var positionList = orientationObject.list.Where(i => i.orientation == orientation).ToArray();
                {
                    foreach (var obj in orientationObject.obj)
                    {
                        foreach (var item in positionList)
                        {
                            obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(item.rectTransform.x, item.rectTransform.y);
                            obj.GetComponent<RectTransform>().sizeDelta = new Vector2(item.rectTransform.width, item.rectTransform.height);
                        }
                    }
                }
            }

        }

        [Serializable]
        public class OrientationPosition
        {
            public ScreenOrientation orientation;
            public Rect rectTransform;

        }

        [Serializable]
        public class OrientationObject
        {
            public GameObject[] obj;
            public List<OrientationPosition> list = new List<OrientationPosition>();

        }
    }
}
