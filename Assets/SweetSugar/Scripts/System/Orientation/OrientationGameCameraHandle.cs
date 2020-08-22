using System.Collections;
using System.Linq;
using SweetSugar.Scripts.Core;
using UnityEngine;

namespace SweetSugar.Scripts.System.Orientation
{
    /// <summary>
    /// Changes camera size depending from field size and orientation
    /// </summary>
    [ExecuteInEditMode]
    public class OrientationGameCameraHandle : MonoBehaviour
    {
        public Camera mainCamera;
        public RectTransform topPanel;
        public RectTransform bottomPanel;

        public RectTransform[] panelsVertical;
        public RectTransform[] panelsHorizontal;
        public RectTransform[] panelsHorizontalHD;

        public OrientationActivator orientationActivator;
        private Rect layoutRect;
        private Vector3 screenCenter;
        private bool levelLoaded;
        private float latestSize;
        private ScreenOrientation previousOrientation;
        private ScreenOrientation currentOrientation;

        void OnEnable()
        {
            OrientationListener.OnOrientationChanged += OnOrientationChanged;
            LevelManager.OnWaitForTutorial += OnSublevelChanged;
            LevelManager.OnLevelLoaded += OnLevelLoaded;
            LevelManager.OnSublevelChanged += OnSublevelChanged;

        }

        private void OnSublevelChanged()
        {
            previousOrientation = ScreenOrientation.Unknown;
            // StartCoroutine(OrientationChangeDelay());

        }

        private void OnLevelLoaded()
        {
            levelLoaded = true;
        }

        void OnDisable()
        {
            OrientationListener.OnOrientationChanged -= OnOrientationChanged;
            LevelManager.OnWaitForTutorial -= OnSublevelChanged;
            LevelManager.OnLevelLoaded -= OnLevelLoaded;
            LevelManager.OnSublevelChanged -= OnSublevelChanged;

        }

        public Vector3 GetCenter(bool vertical = false)
        {
            previousOrientation = currentOrientation;
            var transform1 = orientationActivator.GetCurrentPanels().First();
            var transform2 = orientationActivator.GetCurrentPanels()?.Last();
            var leftRect = RectTransformToWorldSpace(transform1);
            var rightRect = RectTransformToWorldSpace(transform2);
            var rect1 = leftRect;
            var rect2 = rightRect;
            var position = Vector2.zero;
            if (rightRect.size == Vector2.zero)
            {
                position = new Vector2(Screen.width, Screen.height);
                rect2 = new Rect(mainCamera.ScreenToWorldPoint(position).x, 0, 0, 0);
            }
            if (!vertical)
            {
                var v = mainCamera.WorldToScreenPoint(new Vector3(rect1.xMax, 0, 0));
                var v2 = mainCamera.WorldToScreenPoint(new Vector3(rect2.xMin, mainCamera.orthographicSize * 2, 0));
                layoutRect = new Rect(v.x, 0, v2.x - v.x, v2.y - v.y);
            }
            else
            {
                //Vector3 v = mainCamera.WorldToScreenPoint(new Vector3(0, rect1.yMax, 0));
                //Vector3 v2 = mainCamera.WorldToScreenPoint(new Vector3(0, rect2.xMax, 0));
                layoutRect = new Rect(rect1.xMax, rect1.yMax, rect2.yMin - rect1.xMax, rect1.width);
            }

            return mainCamera.ScreenToWorldPoint(layoutRect.center);
        }

        private bool f;
        void OnOrientationChanged(ScreenOrientation orientation)
        {
            currentOrientation = orientation;
            // DefineLayoutRect();
            if (!Application.isPlaying) return;
            if (!levelLoaded) return;
            if ((int)LevelManager.THIS.gameStatus < 2) return;
            if (orientation != previousOrientation)
            {
                f = true;
                StartCoroutine(OrientationChangeDelay());
            }
        }

        IEnumerator OrientationChangeDelay()
        {
            yield return new WaitForSeconds(0.3f);
            var cameraParameters = GetCameraParameters();
            StartCoroutine(SmoothZooming(cameraParameters.size));

            if (LevelManager.THIS.gameStatus != GameState.ChangeSubLevel && LevelManager.THIS.gameStatus != GameState.WaitForPopup)
                mainCamera.transform.position = cameraParameters.position;
            if(f)
            {
                yield return new WaitForEndOfFrame();
                f = false;
                StartCoroutine(OrientationChangeDelay());
            }
        }

        public struct CameraParameters
        {
            public float size;
            public Vector2 position;
        }

        public CameraParameters GetCameraParameters()
        {
            var cameraParameters = new CameraParameters();
            if (currentOrientation == ScreenOrientation.Landscape)
            {
                var fieldRect = LevelManager.THIS.field.GetFieldRect();
                var h = fieldRect.height / 2 + 0.5f;
                // cameraParameters.size = Mathf.Clamp(v, 3, v);
                var topRectTransform = orientationActivator.GetCurrentPanels().FirstOrDefault();
                var topRect = GetWorldRect(topRectTransform);
                topRect = RectTransformToWorldSpace(topRectTransform);

                var w = fieldRect.width * Screen.height / Screen.width / 2 + 2f;
                // var w = (fieldRect.height + topRect.height * 2) / 2 + 0.5f;
                var maxLength = Mathf.Max(h, w);
                cameraParameters.size = Mathf.Clamp(maxLength, 3, maxLength);

                Vector2 offsetPosition = GetCenterOffset();
                cameraParameters.position = LevelManager.THIS.field.GetPosition() + offsetPosition;
            }

            if (currentOrientation == ScreenOrientation.Portrait)
            {
                var topRectTransform = orientationActivator.GetCurrentPanels().FirstOrDefault();
                var transform2 = orientationActivator.GetCurrentPanels()?.Last();
                previousOrientation = currentOrientation;

                var topRect = GetWorldRect(topRectTransform);
                var bottomRect = GetWorldRect(transform2);

                var fieldRect = LevelManager.THIS.field.GetFieldRect();
                var centerRect = new Rect(topRect.xMin, topRect.yMax, topRect.xMax, bottomRect.yMin - topRect.yMax);
                topRect = RectTransformToWorldSpace(topRectTransform);

                var h = fieldRect.width * Screen.height / Screen.width / 2 + 1.5f;
                var w = (fieldRect.height + topRect.height * 2) / 2 + 1.5f;
                var maxLength = Mathf.Max(h, w);
                cameraParameters.size = Mathf.Clamp(maxLength, 3, maxLength);

                var offsetPosition = (Vector2)mainCamera.ScreenToWorldPoint(centerRect.center) -
                                     (Vector2)mainCamera.ScreenToWorldPoint(new Vector2(Screen.width / 2f,
                                         Screen.height / 2f));
                cameraParameters.position = LevelManager.THIS.field.GetPosition() + offsetPosition;
            }
            return cameraParameters;
        }

        public Vector2 GetCenterOffset()
        {
            return mainCamera.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f)) -
                   GetCenter();
        }

        IEnumerator SmoothZooming(float size)
        {
            if (latestSize == size) yield break;
            var firstSize = mainCamera.orthographicSize;
            var destSize = size;
            latestSize = size;
            var time = 0f;
            while (mainCamera.orthographicSize != destSize)
            {
                mainCamera.orthographicSize = Mathf.Lerp(firstSize, latestSize, time);
                time += 10f * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            var cameraParameters = GetCameraParameters();
            mainCamera.transform.position = cameraParameters.position;
        }

        public static Rect RectTransformToWorldSpace(RectTransform transform)
        {
            if (transform != null)
            {
                var size = Vector2.Scale(transform.rect.size, transform.lossyScale);
                var rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
                rect.x -= (transform.pivot.x * size.x);
                rect.y -= ((1.0f - transform.pivot.y) * size.y);
                return rect;
            }

            return new Rect();
        }


        //public Vector2 GetScreenPosition(RectTransform rect)
        //{
        //	Vector3[] corners = new Vector3[4];
        //	rect.GetWorldCorners(corners);
        //	Debug.Log("world " + corners[0]);
        //	Vector2 vector2 = RectTransformUtility.WorldToScreenPoint(mainCamera, corners[0]);
        //	Debug.Log("screen " + vector2);
        //	//Debug.Log("Screen point1: " + new Vector3(rect.rect.xMax, rect.rect.yMin, 0) + rect.position);
        //	//foreach (Vector3 corner in corners)
        //	//{
        //	//	Debug.Log("World point: " + corner);
        //	//	vector2 = RectTransformUtility.WorldToScreenPoint(null, corner);
        //	//	Debug.Log("Screen point: " + vector2);
        //	//	Debug.Log("Viewport: " + Camera.main.ScreenToViewportPoint(corner));
        //	//}
        //	return vector2;
        //}


        public Rect GetWorldRect(RectTransform uiElement)
        {
            var worldCorners = new Vector3[4];
            uiElement.GetWorldCorners(worldCorners);
            //Debug.Log("World Corners");
            for (var i = 0; i < 4; i++)
            {
                //Debug.Log("World Corner " + i + " : " + worldCorners[i]);
                worldCorners[i] =
                    RectTransformUtility.WorldToScreenPoint(mainCamera,
                        worldCorners[i]); //mainCamera.WorldToViewportPoint(worldCorners[i]);
                worldCorners[i] = new Vector3(worldCorners[i].x, Screen.height - worldCorners[i].y, worldCorners[i].z);
                //Debug.Log("Screen Corner " + i + " : " + worldCorners[i]);
            }

            var result = new Rect(
                worldCorners[0].x,
                worldCorners[1].y,
                worldCorners[2].x - worldCorners[0].x,
                Mathf.Abs(worldCorners[3].y - worldCorners[1].y));
            return result;
        }
    }
}