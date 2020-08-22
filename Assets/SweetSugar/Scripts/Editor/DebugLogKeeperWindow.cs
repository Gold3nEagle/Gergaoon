using UnityEditor;

namespace SweetSugar.Scripts.Editor
{
    public class DebugLogKeeperWindow : EditorWindow
    {
        private static DebugLogKeeperWindow window;

        public static void ShowWindow()
        {
            GetWindow(typeof(DebugLogKeeperWindow));
        }
        
        public void OnFocus()
        {
            // Get existing open window or if none, make a new one:
            window = (DebugLogKeeperWindow)GetWindow(typeof(DebugLogKeeperWindow));
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                
            }
            EditorGUILayout.EndVertical();
        }
    }
}