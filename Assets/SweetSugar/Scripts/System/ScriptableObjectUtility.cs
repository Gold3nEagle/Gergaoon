using UnityEditor;
using UnityEngine;

namespace SweetSugar.Scripts.System
{
    public static class ScriptableObjectUtility
    {
        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
#if UNITY_EDITOR
        public static T CreateAsset<T>(string path, string name) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T> ();
 
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + name + ".asset");
 
            AssetDatabase.CreateAsset (asset, assetPathAndName);
 
            AssetDatabase.SaveAssets ();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow ();
            Selection.activeObject = asset;
            return asset;
        }
#endif
    }
}