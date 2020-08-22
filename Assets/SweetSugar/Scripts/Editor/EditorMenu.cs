using SweetSugar.Scripts.MapScripts.StaticMap.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SweetSugar.Scripts.Editor
{
    [InitializeOnLoad]
    public static class EditorMenu
    {
        private const string MenuMapStatic = "Sweet Sugar/Scenes/Map switcher/Static map";
        private const string MenuMapDinamic = "Sweet Sugar/Scenes/Map switcher/Dinamic map";

    [MenuItem("Sweet Sugar/Scenes/Main scene")]
    public static void MainScene()
    {
        EditorSceneManager.OpenScene("Assets/SweetSugar/Scenes/main.unity");
    }
    
    [MenuItem(MenuMapStatic)]
    public static void MapSceneStatic()
    {
        SetStaticMap( true);
        GameScene();
    }
    
    [MenuItem(MenuMapDinamic)]
    public static void MapSceneDinamic()
    {
        SetStaticMap( false);
        GameScene();
    }
    
    public static void SetStaticMap(bool enabled) {
 
        Menu.SetChecked(MenuMapStatic, enabled);
        Menu.SetChecked(MenuMapDinamic, !enabled);
        var sc = Resources.Load<MapSwitcher>("Scriptable/MapSwitcher");
        sc.staticMap = enabled;
        EditorUtility.SetDirty(sc);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Sweet Sugar/Scenes/Game scene")]
    public static void GameScene()
    {
        EditorSceneManager.OpenScene("Assets/SweetSugar/Scenes/"+Resources.Load<MapSwitcher>("Scriptable/MapSwitcher").GetSceneName()+".unity");
    }

    [MenuItem("Sweet Sugar/Debug settings")]
    public static void DebugSettings()
    {
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/SweetSugar/Resources/Scriptable/DebugSettings.asset");
    }
    [MenuItem("Sweet Sugar/Pool settings")]
    public static void PoolSettings()
    {
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/SweetSugar/Resources/Scriptable/PoolSettings.asset");
    }
    
    [MenuItem("Sweet Sugar/Documentation/Main")]
    public static void MainDoc()
    {
        Application.OpenURL("https://www.candy-smith.com/sweet-sugar-documentation");
    }    
    
    [MenuItem("Sweet Sugar/Documentation/HOWTos")]
    public static void HowDoc()
    {
        Application.OpenURL("https://docs.google.com/document/d/1TYfts2Xm9va0BAQBiGLQOC6RH37gTlQk8XzKelnV6bI/edit?usp=sharing");
    }  
    [MenuItem("Sweet Sugar/Documentation/ADS/Unity ads")]
    public static void UnityadsDoc()
    {
        Application.OpenURL("https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0/edit");
    }  
    [MenuItem("Sweet Sugar/Documentation/ADS/Google mobile ads(admob)")]
    public static void AdmobDoc()
    {
        Application.OpenURL("https://docs.google.com/document/d/1I69mo9yLzkg35wtbHpsQd3Ke1knC5pf7G1Wag8MdO-M/edit");
    }   
    [MenuItem("Sweet Sugar/Documentation/ADS/Chartboost")]
    public static void ChartboostDoc()
    {
        Application.OpenURL("https://docs.google.com/document/d/1ibnQbuxFgI4izzyUtT45WH5m1ab3R5d1E3ke3Wrb10Y/edit");
    } 
    [MenuItem("Sweet Sugar/Documentation/ADS/Appodeal")]
    public static void AppodealDoc()
    {
        Application.OpenURL("https://docs.google.com/document/d/11W_OZnaCM--q1TAF8ICrvaqaXWanvP05z6GFSmMqmhE/edit?usp=sharing");
    }      
    [MenuItem("Sweet Sugar/Documentation/Unity IAP (in-apps)")]
    public static void Inapp()
    {
        Application.OpenURL("https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0/edit#heading=h.60xg5ccbex9m");
    }   
    
    [MenuItem("Sweet Sugar/Documentation/Leadboard/Facebook (step 1)")]
    public static void FBDoc()
    {
        Application.OpenURL("https://docs.google.com/document/d/1bTNdM3VSg8qu9nWwO7o7WeywMPhVLVl8E_O0gMIVIw0/edit?usp=sharing");
    }  
    
    [MenuItem("Sweet Sugar/Documentation/Leadboard/Gamesparks (step 2)")]
    public static void GSDoc()
    {
        Application.OpenURL("https://docs.google.com/document/d/1JcQfiiD2ALz6v_i9UIcG93INWZKC7z6FHXH_u6w9A8E/edit");
    }  
    [MenuItem("Sweet Sugar/Documentation/Invite and share")]
    public static void GetSocialDoc()
    {
        Application.OpenURL("https://docs.google.com/document/d/1gkIBo9Ouov7bAYOW9wBofMVwprqmOe6cl_jRKMvBed0/edit?usp=sharing");
    }      
    [MenuItem("Sweet Sugar/Documentation/Localization")]
    public static void LocalizationDoc()
    {
        Application.OpenURL("https://docs.google.com/document/d/1YX3MKcQnvf0POdZ6c8tD_smmNCUWePtm9NWGvx-H9rE/edit?usp=sharing");
    }  
    [MenuItem("Sweet Sugar/Documentation/API reference")]
    public static void APIDoc()
    {
        Application.OpenURL("https://candy-smith.info/sweetdocs/namespaces.html");
    }  
    //     [MenuItem("Sweet Sugar/procc")]
//     public static void Start()
//     {
//         var targets = AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Resources/Levels/TargetEditorScriptable.asset", typeof(TargetEditorScriptable)) as TargetEditorScriptable;
//         var target = targets.targets.Where(i => i.name == "Ingredients").First();
//         var levelData = AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Resources/Levels/LevelScriptable.asset", typeof(LevelScriptable)) as LevelScriptable;
//         foreach (var level in levelData.levels)
//         {
//             if (level.target.name == "Ingredients")
//             {
//                 level.target = target.DeepCopy();
//                 Debug.Log(level.levelNum);
//             }
//         }
//     }   
    }
}