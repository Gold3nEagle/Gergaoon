using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SweetSugar.Scripts.Editor
{
    public class PostImporting : AssetPostprocessor
    {
        static bool imported = false;

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {


            SetScriptingDefineSymbols();



        }

        private static BuildTargetGroup[] GetBuildTargets()
        {
            ArrayList _targetGroupList = new ArrayList();  
            _targetGroupList.Add(BuildTargetGroup.Standalone);
            _targetGroupList.Add(BuildTargetGroup.Android);
            _targetGroupList.Add(BuildTargetGroup.iOS);
            _targetGroupList.Add(BuildTargetGroup.WSA);
            return (BuildTargetGroup[])_targetGroupList.ToArray(typeof(BuildTargetGroup));
        }

        static void SetScriptingDefineSymbols()
        {
            BuildTargetGroup[] _buildTargets = GetBuildTargets();
            if (!EditorPrefs.GetBool(Application.dataPath+"Project_opened"))
            {
                foreach (BuildTargetGroup _target in _buildTargets)
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(_target, "");
                }
                EditorPrefs.SetBool(Application.dataPath+"Project_opened",true);
            }
            foreach (BuildTargetGroup _target in _buildTargets)
            {
                string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(_target);
                CheckDefines(ref defines,"Assets/GoogleMobileAds", "GOOGLE_MOBILE_ADS");
                CheckDefines(ref defines,"Assets/Chartboost", "CHARTBOOST_ADS");
                CheckDefines(ref defines, "Assets/FacebookSDK", "FACEBOOK");
                CheckDefines(ref defines,"Assets/PlayFabSDK", "PLAYFAB");
                CheckDefines(ref defines,"Assets/GameSparks", "GAMESPARKS");
                CheckDefines(ref defines,"Assets/Appodeal", "APPODEAL");
                CheckDefines(ref defines,"Assets/GetSocial", "USE_GETSOCIAL_UI");
                CheckDefines(ref defines,"Assets/Plugins/UnityPurchasing/Bin", "UNITY_INAPPS");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(_target, defines);
            }
        }

        static bool CheckDefines(ref string defines, string path, string symbols)
        {
            if (Directory.Exists(path))
            {
                if (!defines.Contains(symbols))
                {
                    defines = defines + "; " + symbols;
                }
                return true;
            }

            var replace = defines.Replace(symbols, "");
            defines = replace;
            return false;
        }



    }
}