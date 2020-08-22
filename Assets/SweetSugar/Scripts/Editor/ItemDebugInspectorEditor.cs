using SweetSugar.Scripts.Items;
using UnityEditor;
using UnityEngine;

namespace SweetSugar.Scripts.Editor
{
    [CustomEditor(typeof(ItemDebugInspector))]
    public class ItemDebugInspectorEditor : UnityEditor.Editor
    {
        private string log;
        private string logDesrt;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Print falling log"))
            {
                DebugLogKeeper.GetLog(((ItemDebugInspector)target).GetComponent<Item>().instanceID.ToString(), DebugLogKeeper.LogType.Falling);
            }
            if (GUILayout.Button("Print destroying log"))
            {
                DebugLogKeeper.GetLog(((ItemDebugInspector)target).GetComponent<Item>().instanceID.ToString(), DebugLogKeeper.LogType.Destroying);
            }
            if (GUILayout.Button("Print bonus log"))
            {
                DebugLogKeeper.GetLog(((ItemDebugInspector)target).GetComponent<Item>().instanceID.ToString(), DebugLogKeeper.LogType.BonusAppearance);
            }
            base.OnInspectorGUI();

        }

    }
}