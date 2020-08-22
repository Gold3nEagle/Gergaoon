using UnityEditor;

namespace SweetSugar.Scripts.System.Orientation.Editor
{
    [CustomEditor(typeof(OrientationCameraHandle))]

    public class OrientationCameraHandleEditor : UnityEditor.Editor
    {
        OrientationCameraHandle myTarget;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
//        myTarget = (OrientationCameraHandle)target;
//        var objectsList = myTarget.list;
//        for (int j = 0; j < objectsList.Count; j++)
//        {
//            var orientationObject = objectsList[j];
//            GUILayout.BeginHorizontal();
//            {
//                orientationObject.ratio = EditorGUILayout.Vector2Field("", orientationObject.ratio, GUILayout.Width(150));
//                orientationObject.cameraSize = EditorGUILayout.FloatField(orientationObject.cameraSize, GUILayout.Width(50));
//                orientationObject.cameraPosition = EditorGUILayout.Vector2Field("", orientationObject.cameraPosition, GUILayout.Width(150));
//
//            }
//            GUILayout.EndHorizontal();

//        }
//        if (GUILayout.Button("+"))
//        {
//            objectsList.Add(new OrientationCameraHandle.OrientationRatio());
//        }
//
//        if (GUILayout.Button("-"))
//        {
//            objectsList.Remove(objectsList.LastOrDefault());
//        }
        }

    }
}
