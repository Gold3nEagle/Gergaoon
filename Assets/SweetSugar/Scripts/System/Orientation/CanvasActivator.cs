using UnityEngine;
using UnityEngine.UI;

namespace SweetSugar.Scripts.System.Orientation
{
    /// <summary>
    /// Canvas scaler depending from orientation
    /// </summary>
    public class CanvasActivator : OrientationActivator
    {
        public override void OnOrientationChanged(ScreenOrientation orientation)
        {
            if (orientation == ScreenOrientation.Portrait)
                GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
            else if (orientation == ScreenOrientation.Landscape)
                GetComponent<CanvasScaler>().matchWidthOrHeight = 0;

        }
    }
}