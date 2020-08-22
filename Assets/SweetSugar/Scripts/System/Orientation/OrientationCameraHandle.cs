using System;
using UnityEngine;

namespace SweetSugar.Scripts.System.Orientation
{
    /// <summary>
    /// Changes camera size depending from orientation and aspect ratio
    /// </summary>
    [ExecuteInEditMode]
    public class OrientationCameraHandle : MonoBehaviour
    {
        public Camera mainCamera;
        public Transform objectTransform;
//    public List<OrientationRatio> list = new List<OrientationRatio>();
        public Vector2 currentAspectRatio;
        public Rect currentCameraRect;
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
            currentAspectRatio = GetAspectRatio(Screen.width, Screen.height);

//        var orientationRatio = list.Where(i => i.ratio.x == currentAspectRatio.x && i.ratio.y == currentAspectRatio.y).FirstOrDefault();
//        var size = orientationRatio?.cameraSize ?? 0;
            if (mainCamera != null)
            {
                currentCameraRect = mainCamera.rect;
                mainCamera.orthographicSize = 5.3f;
                mainCamera.orthographicSize =15f / Screen.width * Screen.height / 2f;
//            if (size > 0) mainCamera.orthographicSize = size;
            }
//        if (objectTransform != null && orientationRatio != null) objectTransform.position = orientationRatio.cameraPosition;
        }
        public Vector2 GetAspectRatio(int x, int y)
        {
            var f = x / (float)y;
            var i = 0;
            while (true)
            {
                i++;
                if (Math.Round(f * i, 2) == Mathf.RoundToInt(f * i))
                    break;
            }
            return new Vector2((float)Math.Round(f * i, 2), i);
        }
        [Serializable]
        public class OrientationRatio
        {
            public Vector2 ratio;
            public float cameraSize;
            public Vector2 cameraPosition;

        }
    }
}



