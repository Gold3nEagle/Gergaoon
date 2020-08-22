using UnityEditor;
using UnityEngine;

namespace SweetSugar.Scripts.System.Orientation.Editor
{
    [CustomEditor(typeof(OrientationHandle))]
    public class OrientationHandleEditor : UnityEditor.Editor
    {
        OrientationHandle myTarget;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            myTarget = (OrientationHandle)target;
            var objectsList = myTarget.list;
            for (int j = 0; j < objectsList.Count; j++)
            {
                var orientationObject = objectsList[j];
                for (int k = 0; k < orientationObject.obj.Length; k++)
                    orientationObject.obj[k] = (GameObject)EditorGUILayout.ObjectField(orientationObject.obj[k], typeof(GameObject), true);
                for (int i = 0; i < orientationObject.list.Count; i++)
                {
                    var list = orientationObject.list;
                    var objectPosition = list[i];
                    objectPosition.orientation = (ScreenOrientation)EditorGUILayout.EnumPopup(objectPosition.orientation);
                    list[i].rectTransform = EditorGUILayout.RectField(list[i].rectTransform);
                    if (GUILayout.Button("Get transform"))
                    {
                        objectPosition.rectTransform = orientationObject.obj[0].GetComponent<RectTransform>().rect;
                        objectPosition.rectTransform.x = orientationObject.obj[0].GetComponent<RectTransform>().anchoredPosition.x;
                        objectPosition.rectTransform.y = orientationObject.obj[0].GetComponent<RectTransform>().anchoredPosition.y;

                    }
                }
                GUILayout.Space(30);
            }

        }
    }
}