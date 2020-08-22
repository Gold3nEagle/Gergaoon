using UnityEngine;

namespace SweetSugar.Scripts.System
{
    public static class PrefabJsonUtils
    {
#if UNITY_EDITOR
        public static string GetFullPath(GameObject targetPrefab)
        {
            string prefabAssetPathOfNearestInstanceRoot = UnityEditor.PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(targetPrefab);
            string prefabStructure = targetPrefab.transform.parent != null ? "@" + targetPrefab.name : "";
            var prefabPath = prefabAssetPathOfNearestInstanceRoot.Replace("/Resources/", "").Replace(".prefab", "").Substring(prefabAssetPathOfNearestInstanceRoot.IndexOf("/Resources/"));
            return prefabPath + prefabStructure;
        }
#endif
    
        public static GameObject LoadPrefab(string prefabPath)
        {
            var indexOf = prefabPath.IndexOf("@");
            string subPrefab = "";
            if (indexOf > 0)
            {
                subPrefab = prefabPath.Substring(indexOf + 1);
                prefabPath = prefabPath.Substring(0, indexOf);
            }

            var gameObject = Resources.Load<GameObject>(prefabPath);
            if (subPrefab != "") gameObject = gameObject.transform.Find(subPrefab).gameObject;
            var targetPrefab = gameObject;
            return targetPrefab;
        }
    }
}