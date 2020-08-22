using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SweetSugar.Scripts.Editor
{
    [InitializeOnLoad]
    public class Autorun
    {
        static Autorun()
        {
            EditorApplication.update += InitProject;

        }

        static void InitProject()
        {
            EditorApplication.update -= InitProject;
            if (EditorApplication.timeSinceStartup < 10 || !EditorPrefs.GetBool(Application.dataPath+"AlreadyOpened"))
            {
                if (EditorSceneManager.GetActiveScene().name != "game" && Directory.Exists("Assets/SweetSugar/Scenes"))
                {
                    EditorSceneManager.OpenScene("Assets/SweetSugar/Scenes/game.unity");

                }
                EditorPrefs.SetBool(Application.dataPath+"AlreadyOpened", true);

            }

        }
    }
}