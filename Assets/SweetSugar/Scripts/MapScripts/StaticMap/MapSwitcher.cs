using UnityEngine;

namespace SweetSugar.Scripts.MapScripts.StaticMap.Editor
{
    // [CreateAssetMenu(fileName = "MapSwitcher", menuName = "MapSwitcher", order = 1)]
    public class MapSwitcher : ScriptableObject
    {
        public bool staticMap;

        public string GetSceneName()
        {
            if (!staticMap) return "game";
            return "gameStatic";
        }
    }
}