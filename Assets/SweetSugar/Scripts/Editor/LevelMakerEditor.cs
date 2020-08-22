using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SweetSugar.Scripts.AdsEvents;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.GUI;
using SweetSugar.Scripts.GUI.Boost;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.Level.ItemsPerLevel.Editor;
using SweetSugar.Scripts.MapScripts.StaticMap.Editor;
using SweetSugar.Scripts.System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SweetSugar.Scripts.Editor
{
    [InitializeOnLoad]
    public class LevelMakerEditor : EditorWindow
    {
        private bool groupEnabled;
        private bool myBool = true;
        private float myFloat = 1.23f;
        private int levelNumber = 1;
        private int subLevelNumber = 1;

        public int SubLevelNumber
        {
            get { return subLevelNumber; }
            set
            {
                subLevelNumber = value;
                levelData.currentSublevelIndex = subLevelNumber - 1;
            }
        }

        public MenuReference MenuReference
        {
            get
            {
                if (!menuReference) menuReference = GameObject.FindObjectOfType<MenuReference>();
                return menuReference;
            }
            set { menuReference = value; }
        }

        private int subLevelNumberTotal = 1;

        private SquareTypes squareType;
        private ItemForEditor itemBrush;
        private string FileName = "1_1.txt";
        private Vector2 scrollViewVector;
        private bool update;
        private static int selected;
        private string[] toolbarStrings = { "Editor", "Settings", "Shop", "In-apps", "Ads", "GUI", "Rate", "About" };
        private string[] sectionsString = { "Blocks", "Items", "Directions", "Teleports" };

        private static LevelMakerEditor window;
        private bool life_settings_show;
        private bool score_settings;
        private bool boost_show;
        private bool failed_settings_show;
        private bool gems_shop_show;
        private bool target_description_show;
        private IEnumerable<ItemForEditor>[] itemsForEditor;
        private IEnumerable<ItemForEditor> itemsForEditorFull;
        string levelPath = "Assets/SweetSugar/Resources/Levels/";

        private bool enableGoogleAdsProcessing;
        private bool enableChartboostAdsProcessing;
        private List<AdEvents> oldList;
        private LevelData levelData;
        private Texture[] arrows = new Texture[6];
        private Texture[] teleports = new Texture[2];
        private Texture[] arrows_enter = new Texture[4];
        private LevelScriptable levelScriptable;

        [MenuItem("Sweet Sugar/Game editor and settings")]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            window = (LevelMakerEditor)GetWindow(typeof(LevelMakerEditor),false, "Game editor");
            window.Show();
        }
     
        public static void ShowHelp()
        {
            selected = 7;
        }

        public static void ShowWindow()
        {
            GetWindow(typeof(LevelMakerEditor));
        }

        private void OnFocus()
        {
            adsSettings = Resources.Load<AdManagerScriptable>("Scriptable/AdManagerScriptable");
            sceneSwitcher = Resources.Load<MapSwitcher>("Scriptable/MapSwitcher");
            levelScriptable = Resources.Load("Levels/LevelScriptable") as LevelScriptable;
            LoadLevel(levelNumber);

            if (levelData != null)
            {
                if (levelData.GetField(subLevelNumber - 1).maxRows <= 0)
                    levelData.GetField(subLevelNumber - 1).maxRows = 9;
                if (levelData.GetField(subLevelNumber - 1).maxCols <= 0)
                    levelData.GetField(subLevelNumber - 1).maxCols = 9;
                Initialize();
            }
            customSkin = Resources.Load("SweetSkin") as GUISkin;
            arrows[0] = (Texture)AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Textures_png/EditorSprites/arrow.png",typeof(Texture));
            arrows[1] = (Texture)AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Textures_png/EditorSprites/arrow_left.png",typeof(Texture));
            arrows[2] = (Texture)AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Textures_png/EditorSprites/arrow_right.png",typeof(Texture));
            arrows[3] = (Texture)AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Textures_png/EditorSprites/arrow_up.png",typeof(Texture));
            arrows[4] = (Texture)AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Textures_png/EditorSprites/circle arrow.png",typeof(Texture));
            arrows[5] = (Texture)AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Textures_png/EditorSprites/circle arrows.png",typeof(Texture));

            teleports[0] =
                (Texture)AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Textures_png/EditorSprites/teleport_icon1.png",typeof(Texture));
            teleports[1] =(Texture)AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Textures_png/EditorSprites/teleport_icon2.png",typeof(Texture));

            arrows_enter[0] = (Texture)AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Textures_png/EditorSprites/arrow_red.png",typeof(Texture));
            arrows_enter[1] = (Texture)AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Textures_png/EditorSprites/arrow_red_left.png",typeof(Texture));
            arrows_enter[2] = (Texture)AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Textures_png/EditorSprites/arrow_red_right.png",typeof(Texture));
            arrows_enter[3] = (Texture)AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Textures_png/EditorSprites/arrow_red_up.png",typeof(Texture));


            if (EditorSceneManager.GetActiveScene().name == "game")
            {
                MenuReference = FindObjectOfType<MenuReference>();  

                LevelManager lm = Camera.main.GetComponent<LevelManager>();
                InitScript initscript = Camera.main.GetComponent<InitScript>();
                if (oldList == null)
                {
                    oldList = new List<AdEvents>();
                    oldList.Clear();
                    for (int i = 0; i < adsSettings.adsEvents.Count; i++)
                    {
                        oldList.Add(new AdEvents());
                        oldList[i].adType = adsSettings.adsEvents[i].adType;
                        oldList[i].everyLevel = adsSettings.adsEvents[i].everyLevel;
                        oldList[i].gameEvent = adsSettings.adsEvents[i].gameEvent;
                    }
                }
            }

            gotFocus = true;
        }

        private void OnLostFocus()
        {
            dirtyLevel = true;
            SaveLevel(levelNumber);
            gotFocus = false;
        }

        private void Initialize()
        {
            var shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
            subLevelNumberTotal = GetSubLevelsCount();
            if (levelNumber < 1)
                levelNumber = 1;
            life_settings_show = true;
            score_settings = true;
            boost_show = true;
            failed_settings_show = true;
            gems_shop_show = true;
            target_description_show = true;
            var num = 0;
            var simpleItem = Resources.Load<GameObject>("Items/Item").GetComponent<ItemSimple>();
            var simpleitems = simpleItem.GetComponent<IColorableComponent>().GetSprites(levelNumber)
                .Select(i => new ItemForEditor
                {
                    Item = simpleItem.gameObject,
                    ItemType = simpleItem.currentType,
                    Texture = i.texture
                }).ToList().ForEachY(i =>
                {
                    i.Color = num;
                    num++;
                });
            Resources.LoadAll("Items");
            var bonusitems = Resources.FindObjectsOfTypeAll(typeof(EditorIcon))
                .Select(i => (EditorIcon)i)
                .GroupBy(i => i.GetType())
                .Select(i => /* i.Key == typeof(ItemMarmalade) ? i.Where(x => ((ItemMarmalade)x).secondItem == null) : */ i)
                .SelectMany(group => group)
                .Select(i => new ItemForEditor
                {
                    Item = i.gameObject,
                    Color = color,
                    ItemType = i.itemType,
                    colors = i.GetComponent<IColorableComponent>(),
                    Texture = i.GetComponent<Item>().sprRenderer.FirstOrDefault().sprite.texture
                });
            packageTexture = bonusitems.First(i => i.ItemType == ItemsTypes.PACKAGE).Texture;
            var l1 = bonusitems.ToList();
            // for (int i = 0; i < 5; i++)
            // {
            //     var item = bonusitems.First(x => x.ItemType == ItemsTypes.PACKAGE);
            //     l1.Add(item.DeepCopy());
            // }

            var list = l1.GroupBy(i => i.ItemType).Select(group => group.OrderBy(i => i.Item.name).ToArray()).ToArray()
                .ForEachY(group =>
                {
                    num = 0;
                    group.ForEachY(i =>
                    {
                        i.Color = num;
                        num++;
                    });
                }).SelectMany(i => i).ToArray();
            // for (int i = 0; i < list.Count(); i++)
            // {
            //     if (list[i].ItemType == ItemsTypes.PACKAGE)
            //     {
            //         list[i].Texture = list[i].Texture.AlphaBlend(simpleitems.ElementAt(list[i].Color).Texture);
            //     }
            // }
            itemsForEditorFull = simpleitems.Union(list);
            itemsForEditor = simpleitems.Union(bonusitems.GroupBy(i => i.ItemType).Select(i => i.First()))
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(i => i.Index / 6)
                .Select(i => i.Select(x => x.Value))
                .ToArray();
            if (levelData.target.prefabs.All(i => i.GetComponent<IColorableComponent>()))
            {
                for (var index = 0; index < levelData.subTargetsContainers.Count; index++)
                {
                    var levelDataSubTargetsContainer = levelData.subTargetsContainers[index];
                    levelDataSubTargetsContainer.extraObject = levelDataSubTargetsContainer.targetPrefab.GetComponent<IColorableComponent>().GetSprite(levelNumber, index);
                }
            }
        }

        private void InitializeSublevel()
        {
            levelData.GetField(subLevelNumber - 1).levelSquares =
                new SquareBlocks[levelData.GetField(subLevelNumber - 1).maxCols *
                                 levelData.GetField(subLevelNumber - 1).maxRows];
            for (int i = 0; i < levelData.GetField(subLevelNumber - 1).levelSquares.Length; i++)
            {
                SquareBlocks sqBlocks = new SquareBlocks();
                sqBlocks.block = SquareTypes.EmptySquare;
                sqBlocks.obstacle = SquareTypes.NONE;

                levelData.GetField(subLevelNumber - 1).levelSquares[i] = sqBlocks;
            }
            ResetDirection();
        }

        private void OnGUI()
        {
//        GUI.skin = customSkin;
            if (!levelData.fields.Any())
            {
                OnFocus();
                return;
            }

            //		if (!gotFocus) return;
            UnityEngine.GUI.changed = false;

     
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
//        GUILayout.Space(30);
            int oldSelected = selected;
            selected = GUILayout.Toolbar(selected, toolbarStrings, GUILayout.Width(450));
            GUILayout.EndHorizontal();

            scrollViewVector = UnityEngine.GUI.BeginScrollView(new Rect(0, 45, position.width , position.height), scrollViewVector,
                new Rect(0, 0, 500, 1600));
            GUILayout.Space(-30);

            if (oldSelected != selected)
                scrollViewVector = Vector2.zero;

            if (selected == 0 && levelData.fields.Any())
            {
                if (levelData != null)
                {
                    //                if (EditorSceneManager.GetActiveScene().name == "game")
                    {
                        GUILevelSelector();
                        GUILayout.Space(10);


                        GUILimit();
                        GUILayout.Space(10);

                        GUIColorLimit();
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Bomb timer",GUILayout.Width(80));
                        GUILayout.Space(70);
                        levelData.GetField(subLevelNumber - 1).bombTimer = EditorGUILayout.IntField("",
                            levelData.GetField(subLevelNumber - 1).bombTimer, GUILayout.Width(50));
                        GUILayout.EndHorizontal();
                        GUILayout.Space(10);

                        GUIStars();
                        GUILayout.Space(10);

                        GUILayout.BeginHorizontal();
                        {
//                        GUIMamalade();
                            //                        GUILayout.Space(10);

                            GUINoRegen();
                            //                        GUILayout.Space(10);
                        }
                        GUILayout.EndHorizontal();

                        GUITarget();
                        GUILayout.Space(10);

                        GUILevelSize();
                        GUILayout.Space(10);

                        GUISections();
                        GUILayout.Space(10);

                        if (section == 0)
                        {
                            GUIBlocks();
                            GUILayout.Space(10);
                        }

                        if (section == 1)
                        {
                            GUIItems();
                            GUILayout.Space(10);
                        }

                        if (section == 2)
                        {
                            GUIDirections();
                            GUILayout.Space(10);
                        }

                        if (section == 3)
                        {
                            GUITeleport();
                            GUILayout.Space(10);
                        }

                        GUIGameField();
                    }
                    //                else
                    //                    GUIShowWarning();
                }
            }
            else if (selected == 1)
            {
                if (EditorSceneManager.GetActiveScene().name == "game" || EditorSceneManager.GetActiveScene().name == "gameStatic")
                    GUISettings();
                else
                    GUIShowWarning();
            
                GUILayout.Space(10);
                // CheckSeparateLevels();
            }
            else if (selected == 2)
            {
                if (EditorSceneManager.GetActiveScene().name == "game" || EditorSceneManager.GetActiveScene().name == "gameStatic")
                    GUIShops();
                else
                    GUIShowWarning();
            }
            else if (selected == 3)
            {
                if (EditorSceneManager.GetActiveScene().name == "game" || EditorSceneManager.GetActiveScene().name == "gameStatic")
                    GUIInappSettings();
                else
                    GUIShowWarning();
            }
            else if (selected == 4)
            {
                GUIAds();
            }
            else if (selected == 5)
            {
                if (EditorSceneManager.GetActiveScene().name == "game" || EditorSceneManager.GetActiveScene().name == "gameStatic")
                    GUIDialogs();
                else
                    GUIShowWarning();
            }
            else if (selected == 6)
            {
                if (EditorSceneManager.GetActiveScene().name == "game" || EditorSceneManager.GetActiveScene().name == "gameStatic")
                    GUIRate();
                else
                    GUIShowWarning();
            }
            else if (selected == 7)
            {
                GUIHelp();
            }


            UnityEngine.GUI.EndScrollView();
            if (UnityEngine.GUI.changed && !EditorApplication.isPlaying)
                EditorSceneManager.MarkAllScenesDirty();

            if (enableGoogleAdsProcessing)
                RunOnceGoogle();

            if (enableChartboostAdsProcessing)
                RunOnceChartboost();
        }
        private void CheckSeparateLevels()
        {
            if (GUILayout.Button("Re-generate separate levels", GUILayout.Width(300)))
            {
                var ls = Resources.Load("Levels/LevelScriptable") as LevelScriptable;
                for (int i = 0; i < ls.levels.Count(); i++)
                {
                    ScriptableLevelManager.CreateFileLevel( i+1, ls.levels[i]);
                }
            }
        }

  


        private void GUIShowWarning()
        {
            GUILayout.Space(100);
            GUILayout.Label("CAUTION!", EditorStyles.boldLabel, GUILayout.Width(600));
            GUILayout.Label("Please open scene - game ( Assets/SweetSugar/Scenes/game.unity )", EditorStyles.boldLabel,
                GUILayout.Width(600));
        }


        #region GUIRate

        private void GUIRate()
        {
            InitScript initscript = Camera.main.GetComponent<InitScript>();

            GUILayout.Label("Rate settings:", EditorStyles.boldLabel, GUILayout.Width(150));
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            initscript.ShowRateEvery = EditorGUILayout.IntField("Show Rate every ", initscript.ShowRateEvery,
                GUILayout.Width(220), GUILayout.MaxWidth(220));
            GUILayout.Label(" level (0 = disable)", EditorStyles.label, GUILayout.Width(150));
            GUILayout.EndHorizontal();
            initscript.RateURL =
                EditorGUILayout.TextField("URL", initscript.RateURL, GUILayout.Width(220), GUILayout.MaxWidth(220));
            initscript.RateURLIOS = EditorGUILayout.TextField("URL iOS", initscript.RateURLIOS, GUILayout.Width(220),
                GUILayout.MaxWidth(220));
        }

        #endregion

        #region GUIDialogs

        private void GUIDialogs()
        {
            GUILayout.Label("GUI elements:", EditorStyles.boldLabel, GUILayout.Width(150));
            GUILayout.Space(10);
            ShowMenuButton("Menu Play", "MenuPlay");
            ShowMenuButton("Menu Complete", "MenuComplete");
            ShowMenuButton("Menu Failed", "MenuFailed");
            ShowMenuButton("PreFailed", "PreFailed");
//        ShowMenuButton("Pause", "MenuPause");
            ShowMenuButton("Boost Shop", "BoostShop");
            ShowMenuButton("Live Shop", "LiveShop");
            ShowMenuButton("Gems Shop", "GemsShop");
//        ShowMenuButton("Boost Info", "BoostInfo");
            ShowMenuButton("Settings", "Settings");
            ShowMenuButton("Reward", "Reward");
//        ShowMenuButton("Tutorial", "Tutorial");
        }

        private void ShowMenuButton(string label, string name)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, EditorStyles.label, GUILayout.Width(100));
            GameObject obj = MenuReference.transform.Find(name).gameObject;
            if (GUILayout.Button(obj.activeSelf ? "hide" : "show", GUILayout.Width(100)))
            {
                EditorGUIUtility.PingObject(obj);
                Selection.activeGameObject = obj;
                obj.SetActive(!obj.activeSelf);
            }

            GUILayout.EndHorizontal();
        }

        public static void SetSearchFilter(string filter, int filterMode)
        {
            SearchableEditorWindow[] windows =
                (SearchableEditorWindow[])Resources.FindObjectsOfTypeAll(typeof(SearchableEditorWindow));
            SearchableEditorWindow hierarchy = null;
            foreach (SearchableEditorWindow window in windows)
            {
                if (window.GetType().ToString() == "UnityEditor.SceneHierarchyWindow")
                {
                    hierarchy = window;
                    break;
                }
            }

            if (hierarchy == null)
                return;

            MethodInfo setSearchType =
                typeof(SearchableEditorWindow).GetMethod("SetSearchFilter", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { filter, filterMode, false };

            setSearchType.Invoke(hierarchy, parameters);
        }

        #endregion

        #region ads_settings

        private void RunOnceGoogle()
        {
            if (Directory.Exists("Assets/PlayServicesResolver"))
            {
                Debug.Log("assets try reimport");
#if GOOGLE_MOBILE_ADS && UNITY_ANDROID
//            GooglePlayServices.PlayServicesResolver.MenuResolve();//2.1.2
            Debug.Log("assets reimorted");
            enableGoogleAdsProcessing = false;
#endif
            }
        }

        private void RunOnceChartboost()
        {
            enableChartboostAdsProcessing = false;
        }


        private void GUIAds()
        {
            LevelManager lm = Camera.main.GetComponent<LevelManager>();
            InitScript initscript = Camera.main.GetComponent<InitScript>();

            GUILayout.Label("Ads settings:", EditorStyles.boldLabel, GUILayout.Width(150));
            GUILayout.BeginHorizontal();

            GUILayout.Label("Unity ads", EditorStyles.boldLabel, GUILayout.Width(150)); //1.6.1
            GUILayout.Label("Install: Windows->\n Services->Ads - ON", GUILayout.Width(130));
            if (GUILayout.Button("Help", GUILayout.Width(80)))
            {
                Application.OpenURL("https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0");
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            initscript.rewardedGems = EditorGUILayout.IntField("Rewarded gems count", initscript.rewardedGems,
                GUILayout.Width(200), GUILayout.MaxWidth(200));
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

#if GOOGLE_MOBILE_ADS
            GUILayout.BeginHorizontal();

            GUILayout.Label("Google mobile ads", EditorStyles.boldLabel, GUILayout.Width(150)); //1.6.1
            if (GUILayout.Button("Install", GUILayout.Width(100)))
            {
                Application.OpenURL("https://github.com/googleads/googleads-mobile-unity/releases"); //1.6.1
            }

            if (GUILayout.Button("Help", GUILayout.Width(80)))
            {
                Application.OpenURL(
                    "https://docs.google.com/document/d/1I69mo9yLzkg35wtbHpsQd3Ke1knC5pf7G1Wag8MdO-M/edit?usp=sharing");
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            adsSettings.admobUIDAndroid = EditorGUILayout.TextField("Admob Interstitial ID Android ",
                adsSettings.admobUIDAndroid, GUILayout.Width(220), GUILayout.MaxWidth(220));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            adsSettings.admobUIDIOS = EditorGUILayout.TextField("Admob Interstitial ID iOS", adsSettings.admobUIDIOS,
                GUILayout.Width(220), GUILayout.MaxWidth(220));
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
#endif

            //CHARTBOOST ADS
//        GUILayout.BeginHorizontal();
//        bool oldenableChartboostAds = initscript.enableChartboostAds;
//        //		initscript.enableChartboostAds = EditorGUILayout.Toggle ("Enable Chartboost Ads", initscript.enableChartboostAds, new GUILayoutOption[] {//1.6.1
//        //			GUILayout.Width (50),
//        //			GUILayout.MaxWidth (200)
//        //		});
//        GUILayout.Label("Chartboost ads", EditorStyles.boldLabel, GUILayout.Width(150)); //1.6.1
//        if (GUILayout.Button("Install", GUILayout.Width(100)))
//        {
//            Application.OpenURL("http://www.chartboo.st/sdk/unity"); //1.6.1
//        }
//
//        if (GUILayout.Button("Help", GUILayout.Width(80)))
//        {
//            Application.OpenURL("https://docs.google.com/document/d/1ibnQbuxFgI4izzyUtT45WH5m1ab3R5d1E3ke3Wrb10Y");
//        }
//
//        GUILayout.EndHorizontal();

//        GUILayout.Space(10);
//        //		if (oldenableChartboostAds != initscript.enableChartboostAds) {//1.6.1
//        //			SetScriptingDefineSymbols ();
//        //			if (initscript.enableChartboostAds) {
//        //				enableChartboostAdsProcessing = true;
//        //			}
//        //
//        //		}
//        if (initscript.enableChartboostAds)
//        {
//            GUILayout.BeginHorizontal();
//            GUILayout.Space(20);
//            EditorGUILayout.LabelField("menu Chartboost->Edit settings", GUILayout.Width(50), GUILayout.MaxWidth(200));
//            GUILayout.EndHorizontal();
//            GUILayout.BeginHorizontal();
//            GUILayout.Space(20);
//            EditorGUILayout.LabelField("Put ad ID to appropriate platform to prevent crashing!", EditorStyles.boldLabel,
//                GUILayout.Width(100), GUILayout.MaxWidth(400));
//            GUILayout.EndHorizontal();
//
//            GUILayout.Space(10);
//        }


            GUILayout.Space(10);

            GUILayout.Label("Ads controller:", EditorStyles.boldLabel, GUILayout.Width(150));

            EditorGUILayout.Space();

            GUILayout.Label("Event:               Status:                            Show every:", GUILayout.Width(350));

            foreach (AdEvents item in adsSettings.adsEvents)
            {
                EditorGUILayout.BeginHorizontal();
                item.gameEvent = (GameState)EditorGUILayout.EnumPopup(item.gameEvent, GUILayout.Width(100));
                item.adType = (AdType)EditorGUILayout.EnumPopup(item.adType, GUILayout.Width(150));
                item.everyLevel = EditorGUILayout.IntPopup(item.everyLevel, new[]
                {
                    "1",
                    "2",
                    "3",
                    "4",
                    "5",
                    "6",
                    "7",
                    "8",
                    "9",
                    "10"
                }, new[]
                {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10
                }, GUILayout.Width(100));

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                AdEvents adevent = new AdEvents();
                adevent.everyLevel = 1;
                adsSettings.adsEvents.Add(adevent);
            }

            if (GUILayout.Button("Delete"))
            {
                if (adsSettings.adsEvents.Count > 0)
                    adsSettings.adsEvents.Remove(adsSettings.adsEvents[adsSettings.adsEvents.Count - 1]);
            }


            GUILayout.Space(10);

            if (GUILayout.Button("Save"))
            {   
                EditorUtility.SetDirty(adsSettings);
                AssetDatabase.SaveAssets();
            }

        }
    
        private string PendingItemDrawer(Rect position, string itemValue) {
            // Text fields do not like null values!
            if (itemValue == null)
                itemValue = "";
		
            position.width -= 50;
            itemValue = EditorGUI.TextField(position, itemValue);
		
            position.x = position.xMax + 5;
            position.width = 45;
            if (UnityEngine.GUI.Button(position, "Info")) {
            }
		
            return itemValue;
        }
        private void DrawEmpty() {
            GUILayout.Label("No items in list.", EditorStyles.miniLabel);
        }
        #endregion

        #region inapps_settings

        private void GUIInappSettings()
        {
            LevelManager lm = Camera.main.GetComponent<LevelManager>();

            GUILayout.Label("In-apps settings:", EditorStyles.boldLabel, GUILayout.Width(150));

            if (GUILayout.Button("Reset to default", GUILayout.Width(150)))
            {
                ResetInAppsSettings();
            }


            GUILayout.Space(10);
            bool oldenableInApps = lm.enableInApps;

            GUILayout.BeginHorizontal();
            //		lm.enableInApps = EditorGUILayout.Toggle ("Enable In-apps", lm.enableInApps, new GUILayoutOption[] {//1.6.1
            //			GUILayout.Width (180)
            //		});
            if (GUILayout.Button("How to setup", GUILayout.Width(120)))
            {
                Application.OpenURL(
                    "https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0#bookmark=id.b1efplsspes5");
            }

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Install: Windows->Services->\n In-app Purchasing - ON->Import", GUILayout.Width(400));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            //		if (oldenableInApps != lm.enableInApps) {1.6.1
            //			SetScriptingDefineSymbols ();
            //		}


            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.BeginVertical();
            for (int i = 0; i < lm.InAppIDs.Length; i++)
            {
                lm.InAppIDs[i] = EditorGUILayout.TextField("Product id " + (i + 1), lm.InAppIDs[i], GUILayout.Width(300),
                    GUILayout.MaxWidth(300));
            }

            GUILayout.Space(10);

            GUILayout.Label("Android:", EditorStyles.boldLabel, GUILayout.Width(150));

            GUILayout.BeginHorizontal();
            GUILayout.Space(30);

            GUILayout.BeginVertical();
            GUILayout.Space(10);
            //GUILayout.Label(" Put Google license key into the field \n from the google play account ", EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(300) });
            //GUILayout.Space(10);

            //lm.GoogleLicenseKey = EditorGUILayout.TextField("Google license key", lm.GoogleLicenseKey, new GUILayoutOption[] {
            //    GUILayout.Width (300),
            //    GUILayout.MaxWidth (300)
            //});

            GUILayout.Space(10);
            if (GUILayout.Button("Android account help", GUILayout.Width(400)))
            {
                Application.OpenURL("http://developer.android.com/google/play/billing/billing_admin.html");
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.BeginVertical();

            GUILayout.Space(10);
            GUILayout.Label("iOS:", EditorStyles.boldLabel, GUILayout.Width(150));
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);

            GUILayout.BeginVertical();

            //GUILayout.Label(" StoreKit library must be added \n to the XCode project, generated by Unity ", EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(300) });
            GUILayout.Space(10);
            if (GUILayout.Button("iOS account help", GUILayout.Width(400)))
            {
                Application.OpenURL("https://developer.apple.com/library/ios/qa/qa1329/_index.html");
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void ResetInAppsSettings()
        {
            LevelManager lm = Camera.main.GetComponent<LevelManager>();
            lm.InAppIDs[0] = "gems10";
            lm.InAppIDs[1] = "gems50";
            lm.InAppIDs[2] = "gems100";
            lm.InAppIDs[3] = "gems150";
        }

        #endregion

        private void GUIHelp()
        {
            GUILayout.Label("Sweet Sugar Match 3 Complete Project + EDITOR v 1.3.3", EditorStyles.boldLabel,
                GUILayout.Width(400));
            GUILayout.Label("Please read our documentation:", EditorStyles.boldLabel, GUILayout.Width(200));
            if (GUILayout.Button("DOCUMENTATION", GUILayout.Width(150)))
            {
                Application.OpenURL("https://docs.google.com/document/d/17QwNYwZysylZUvRcjLWZU-IaJPNynaAJ3Ds-JafhtMA/edit");
            }

            GUILayout.Space(10);
            GUILayout.Label(
                "To get support you should provide \n ORDER NUMBER (asset store) \n or NICKNAME and DATE of purchase (other stores):",
                EditorStyles.boldLabel, GUILayout.Width(350));
            GUILayout.Space(10);
            GUILayout.TextArea("info@candy-smith.com", EditorStyles.boldLabel, GUILayout.Width(350));
        }

        #region settings

        private bool share_settings;
        private bool target_settings;

        private void GUISettings()
        {
            LevelManager lm = Camera.main.GetComponent<LevelManager>();//TODO: move all game settings to scriptabble
            InitScript initscript = Camera.main.GetComponent<InitScript>();
            GUILayout.Label("Game settings:", EditorStyles.boldLabel, GUILayout.Width(150));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset to default", GUILayout.Width(150)))
            {
                ResetSettings();
            }

            if (GUILayout.Button("Clear player prefs", GUILayout.Width(150)))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                Debug.Log("Player prefs cleared");
            }
            if (GUILayout.Button("Open all levels", new GUILayoutOption[] { GUILayout.Width(150) }))
            {
                for (int i = 1; i < 1000; i++)
                {
                    SaveLevelStarsCount(i, 3);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            bool oldFacebookEnable = lm.FacebookEnable;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Facebook", EditorStyles.boldLabel, GUILayout.Width(150)); //1.6.1
            if (GUILayout.Button("Install", GUILayout.Width(70)))
            {
                Application.OpenURL("https://developers.facebook.com/docs/unity/downloads");
            }

            if (GUILayout.Button("Account", GUILayout.Width(70)))
            {
                Application.OpenURL("https://developers.facebook.com");
            }

            if (GUILayout.Button("How to setup", GUILayout.Width(120)))
            {
                Application.OpenURL(
                    "https://docs.google.com/document/d/1bTNdM3VSg8qu9nWwO7o7WeywMPhVLVl8E_O0gMIVIw0/edit?usp=sharing");
            }

            GUILayout.EndHorizontal();

#if GAMESPARKS
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(150);
//                if (GUILayout.Button("Create game", GUILayout.Width(100)))
//                {
//                    GamesparksConfiguration window = CreateInstance<GamesparksConfiguration>();
//                    window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 200);
//                    window.ShowPopup();
//                }

            }
            GUILayout.EndHorizontal();
#endif
//#if FACEBOOK
//        share_settings = EditorGUILayout.Foldout(share_settings, "Share settings:");
//        if (share_settings)
//        {
//            GUILayout.BeginHorizontal();
//            GUILayout.Space(30);
//            GUILayout.BeginVertical();
//            {
//                lm.androidSharingPath =
// EditorGUILayout.TextField("Android path", lm.androidSharingPath, GUILayout.MaxWidth(500));
//                lm.iosSharingPath =
// EditorGUILayout.TextField("iOS path", lm.iosSharingPath, GUILayout.MaxWidth(500));
//            }
//            GUILayout.EndVertical();
//            GUILayout.EndHorizontal();
//
//            GUILayout.Space(10);
//        }
//#endif
            GUILayout.BeginHorizontal();
            GUILayout.Label("Leadboard Gamesparks", EditorStyles.boldLabel, GUILayout.Width(150)); //1.6.1
            if (GUILayout.Button("Install", GUILayout.Width(70)))
            {
                Application.OpenURL("https://docs.gamesparks.com/sdk-center/unity.html");
            }

            if (GUILayout.Button("Account", GUILayout.Width(70)))
            {
                Application.OpenURL("https://portal.gamesparks.net");
            }

            if (GUILayout.Button("How to setup", GUILayout.Width(120)))
            {
                Application.OpenURL("https://docs.google.com/document/d/1JcQfiiD2ALz6v_i9UIcG93INWZKC7z6FHXH_u6w9A8E");
            }

            GUILayout.EndHorizontal();


            //		if (oldFacebookEnable != lm.FacebookEnable) {//1.6.1
            //			SetScriptingDefineSymbols ();
            //		}
            if (lm.FacebookEnable)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("menu Facebook-> Edit settings", GUILayout.Width(300));
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(30);
            var lastRect = GUILayoutUtility.GetLastRect();
            lm.fallingCurve = EditorGUI.CurveField(new Rect(lastRect.x + 3, lastRect.y + 30, position.width - 50, 25), "Falling curve", lm.fallingCurve);
            GUILayout.Space(30);

            lm.waitAfterFall = EditorGUILayout.FloatField("Delay after fall", lm.waitAfterFall, GUILayout.Width(50),
                GUILayout.MaxWidth(200));
            GUILayout.Space(10);

//        score_settings = EditorGUILayout.Foldout(score_settings, "Score settings:");
//        if (score_settings)
//        {
//            GUILayout.Space(10);
//            GUILayout.BeginHorizontal();
//            GUILayout.Space(30);
//            GUILayout.BeginVertical();
//            lm.scoreForItem = EditorGUILayout.IntField("Score for item", lm.scoreForItem, GUILayout.Width(50),
//                GUILayout.MaxWidth(200));
//            lm.scoreForBlock = EditorGUILayout.IntField("Score for block", lm.scoreForBlock, GUILayout.Width(50),
//                GUILayout.MaxWidth(200));
//            lm.scoreForWireBlock = EditorGUILayout.IntField("Score for wire block", lm.scoreForWireBlock,
//                GUILayout.Width(50), GUILayout.MaxWidth(200));
//            lm.scoreForSolidBlock = EditorGUILayout.IntField("Score for solid block", lm.scoreForSolidBlock,
//                GUILayout.Width(50), GUILayout.MaxWidth(200));
//            lm.scoreForThrivingBlock = EditorGUILayout.IntField("Score for thriving block", lm.scoreForThrivingBlock,
//                GUILayout.Width(50), GUILayout.MaxWidth(200));
//            GUILayout.Space(10);
//
//            lm.showPopupScores = EditorGUILayout.Toggle("Show popup scores", lm.showPopupScores, GUILayout.Width(50),
//                GUILayout.MaxWidth(200));
//            GUILayout.Space(10);
//
//            lm.scoresColors[0] = EditorGUILayout.ColorField("Score color item 1", lm.scoresColors[0],
//                GUILayout.Width(200), GUILayout.MaxWidth(200));
//            lm.scoresColors[1] = EditorGUILayout.ColorField("Score color item 2", lm.scoresColors[1],
//                GUILayout.Width(200), GUILayout.MaxWidth(200));
//            lm.scoresColors[2] = EditorGUILayout.ColorField("Score color item 3", lm.scoresColors[2],
//                GUILayout.Width(200), GUILayout.MaxWidth(200));
//            lm.scoresColors[3] = EditorGUILayout.ColorField("Score color item 4", lm.scoresColors[3],
//                GUILayout.Width(200), GUILayout.MaxWidth(200));
//            lm.scoresColors[4] = EditorGUILayout.ColorField("Score color item 5", lm.scoresColors[4],
//                GUILayout.Width(200), GUILayout.MaxWidth(200));
//            lm.scoresColors[5] = EditorGUILayout.ColorField("Score color item 6", lm.scoresColors[5],
//                GUILayout.Width(200), GUILayout.MaxWidth(200));
//            GUILayout.Space(10);
//
//            lm.scoresColorsOutline[0] = EditorGUILayout.ColorField("Score color outline item 1",
//                lm.scoresColorsOutline[0], GUILayout.Width(200), GUILayout.MaxWidth(200));
//            lm.scoresColorsOutline[1] = EditorGUILayout.ColorField("Score color outline item 2",
//                lm.scoresColorsOutline[1], GUILayout.Width(200), GUILayout.MaxWidth(200));
//            lm.scoresColorsOutline[2] = EditorGUILayout.ColorField("Score color outline item 3",
//                lm.scoresColorsOutline[2], GUILayout.Width(200), GUILayout.MaxWidth(200));
//            lm.scoresColorsOutline[3] = EditorGUILayout.ColorField("Score color outline item 4",
//                lm.scoresColorsOutline[3], GUILayout.Width(200), GUILayout.MaxWidth(200));
//            lm.scoresColorsOutline[4] = EditorGUILayout.ColorField("Score color outline item 5",
//                lm.scoresColorsOutline[4], GUILayout.Width(200), GUILayout.MaxWidth(200));
//            lm.scoresColorsOutline[5] = EditorGUILayout.ColorField("Score color outline item 6",
//                lm.scoresColorsOutline[5], GUILayout.Width(200), GUILayout.MaxWidth(200));
//            GUILayout.EndVertical();
//            GUILayout.EndHorizontal();
//        }
//
//        GUILayout.Space(20);

            life_settings_show = EditorGUILayout.Foldout(life_settings_show, "Lifes settings:");
            if (life_settings_show)
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                GUILayout.BeginVertical();


                initscript.CapOfLife = EditorGUILayout.IntField("Max of lifes", initscript.CapOfLife, GUILayout.Width(200),
                    GUILayout.MaxWidth(200));
                GUILayout.Space(10);

                GUILayout.Label("Total time for refill lifes:", EditorStyles.label);
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                GUILayout.Label("Hour", EditorStyles.label, GUILayout.Width(50));
                GUILayout.Label("Min", EditorStyles.label, GUILayout.Width(50));
                GUILayout.Label("Sec", EditorStyles.label, GUILayout.Width(50));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                initscript.TotalTimeForRestLifeHours =
                    EditorGUILayout.FloatField("", initscript.TotalTimeForRestLifeHours, GUILayout.Width(50));
                initscript.TotalTimeForRestLifeMin =
                    EditorGUILayout.FloatField("", initscript.TotalTimeForRestLifeMin, GUILayout.Width(50));
                initscript.TotalTimeForRestLifeSec =
                    EditorGUILayout.FloatField("", initscript.TotalTimeForRestLifeSec, GUILayout.Width(50));
                GUILayout.EndHorizontal();
                GUILayout.Space(10);


                lm.lifeShop.CostIfRefill = EditorGUILayout.IntField("Cost of refilling lifes", lm.lifeShop.CostIfRefill,
                    GUILayout.Width(200), GUILayout.MaxWidth(200));
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(20);

            initscript.FirstGems = EditorGUILayout.IntField("Start gems", initscript.FirstGems, GUILayout.Width(200),
                GUILayout.MaxWidth(200));
            GUILayout.Space(20);

            initscript.losingLifeEveryGame = EditorGUILayout.Toggle("Losing a life every game",
                initscript.losingLifeEveryGame, GUILayout.Width(200), GUILayout.MaxWidth(200));
            GUILayout.Space(20);


            failed_settings_show = EditorGUILayout.Foldout(failed_settings_show, "Failed settings:");
            if (failed_settings_show)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                GUILayout.BeginVertical();

                lm.FailedCost =
                    EditorGUILayout.IntField(new GUIContent("Cost of continue", "Cost of continue after failed"),
                        lm.FailedCost, GUILayout.Width(200), GUILayout.MaxWidth(200));
                lm.ExtraFailedMoves = EditorGUILayout.IntField(new GUIContent("Extra moves", "Extra moves after continue"),
                    lm.ExtraFailedMoves, GUILayout.Width(200), GUILayout.MaxWidth(200));
                lm.ExtraFailedSecs =
                    EditorGUILayout.IntField(new GUIContent("Extra seconds", "Extra seconds after continue"),
                        lm.ExtraFailedSecs, GUILayout.Width(200), GUILayout.MaxWidth(200));
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(20);

            //  EditorUtility.SetDirty(lm);
        }

        private void ResetSettings()
        {
            LevelManager lm = Camera.main.GetComponent<LevelManager>();
            lm.showPopupScores = false;
            lm.scoresColors[0] = new Color(251f / 255f, 80 / 255f, 1 / 255f);
            lm.scoresColors[1] = new Color(255 / 255f, 193 / 255f, 22 / 255f);
            lm.scoresColors[2] = new Color(237 / 255f, 13 / 255f, 233 / 255f);
            lm.scoresColors[3] = new Color(41 / 255f, 157 / 255f, 255 / 255f);
            lm.scoresColors[4] = new Color(255 / 255f, 255 / 255f, 38 / 255f);
            lm.scoresColors[5] = new Color(37 / 255f, 219 / 255f, 0 / 255f);

            lm.scoresColorsOutline[0] = new Color(138 / 255f, 1 / 255f, 0 / 255f);
            lm.scoresColorsOutline[1] = new Color(184 / 255f, 77 / 255f, 1 / 255f);
            lm.scoresColorsOutline[2] = new Color(128 / 255f, 0 / 255f, 128 / 255f);
            lm.scoresColorsOutline[3] = new Color(0 / 255f, 64 / 255f, 182 / 255f);
            lm.scoresColorsOutline[4] = new Color(174 / 255f, 104 / 255f, 0 / 255f);
            lm.scoresColorsOutline[5] = new Color(19 / 255f, 111 / 255f, 0 / 255f);

            InitScript initscript = Camera.main.GetComponent<InitScript>();
            initscript.CapOfLife = 5;
            initscript.TotalTimeForRestLifeHours = 0;
            initscript.TotalTimeForRestLifeMin = 15;
            initscript.TotalTimeForRestLifeSec = 0;
            lm.lifeShop.CostIfRefill = 12;
            lm.FailedCost = 12;
            lm.ExtraFailedMoves = 5;
            lm.ExtraFailedSecs = 30;
            EditorUtility.SetDirty(lm);
        }

        #endregion
        public void SaveLevelStarsCount(int level, int starsCount)
        {
            Debug.Log(string.Format("Stars count {0} of level {1} saved.", starsCount, level));
            PlayerPrefs.SetInt(GetLevelKey(level), starsCount);
        }
    
        private string GetLevelKey(int number)
        {
            return string.Format("Level.{0:000}.StarsCount", number);
        }
        #region shop

        private void GUIShops()
        {
            LevelManager lm = Camera.main.GetComponent<LevelManager>();

            GUILayout.Label("Shop settings:", EditorStyles.boldLabel, GUILayout.Width(150));

            if (GUILayout.Button("Reset to default", GUILayout.Width(150)))
            {
                ResetShops();
            }

            GUILayout.Space(10);
            gems_shop_show = EditorGUILayout.Foldout(gems_shop_show, "Gems shop settings:");
            if (gems_shop_show)
            {
                int i = 1;
                foreach (GemProduct item in lm.gemsProducts)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(30);
                    GUILayout.Label("Gems count", GUILayout.Width(100));
                    GUILayout.Label("Price $", GUILayout.Width(100));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(30);
                    item.count = EditorGUILayout.IntField("", item.count, GUILayout.Width(100), GUILayout.MaxWidth(100));
                    item.price = EditorGUILayout.FloatField("", item.price, GUILayout.Width(100), GUILayout.MaxWidth(100));
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    i++;
                }
            }

            GUILayout.Space(10);
            boost_show = EditorGUILayout.Foldout(boost_show, "Boosts shop settings:");
            if (boost_show)
            {
                BoostShop bs = MenuReference.BoostShop.GetComponent<BoostShop>();
                List<BoostProduct> bp = bs.boostProducts;
                foreach (BoostProduct item in bp)
                {
                    GUILayout.BeginVertical();
                    {
                        item.boostType = (BoostType) EditorGUILayout.EnumPopup(item.boostType, GUILayout.Width(93));

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Description");
                        item.description =
                            EditorGUILayout.TextField("", item.description, GUILayout.Width(400), GUILayout.MaxWidth(400));
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Localization ref",GUILayout.Width(110));
                        item.descriptionLocalizationRefrence =
                            EditorGUILayout.IntField("", item.descriptionLocalizationRefrence, GUILayout.Width(30), GUILayout.MaxWidth(30));
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(item.icon.texture, GUILayout.Width(50), GUILayout.Height(50));
                        GUILayout.BeginVertical();
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label("Count", EditorStyles.label, GUILayout.Width(80));
                                GUILayout.Label("Price(gem)", EditorStyles.label, GUILayout.Width(80));

                                }
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                {
                                    item.count = EditorGUILayout.IntField("", item.count, GUILayout.Width(80), GUILayout.MaxWidth(80));
                                    item.GemPrices =
                                        EditorGUILayout.IntField("", item.GemPrices, GUILayout.Width(80), GUILayout.MaxWidth(80));
                                }
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.EndVertical();
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
            }
        }

        private void ResetShops()
        {
            LevelManager lm = Camera.main.GetComponent<LevelManager>();

            lm.gemsProducts[0].count = 10;
            lm.gemsProducts[0].price = 0.99f;
            lm.gemsProducts[1].count = 50;
            lm.gemsProducts[1].price = 4.99f;
            lm.gemsProducts[2].count = 100;
            lm.gemsProducts[2].price = 9.99f;
            lm.gemsProducts[3].count = 150;
            lm.gemsProducts[3].price = 14.99f;

            BoostShop bs = MenuReference.BoostShop.GetComponent<BoostShop>();
            bs.boostProducts[0].description = "Gives you the 5 extra moves";
            bs.boostProducts[1].description = "Place this special item in game";
            bs.boostProducts[2].description = "Place this special item in game";
            bs.boostProducts[3].description = "Gives you the 30 extra seconds";
            bs.boostProducts[4].description = "Destroy the item";
            bs.boostProducts[5].description = "Place this special item in game";
            bs.boostProducts[6].description = "Switch to item that don't match";
            bs.boostProducts[7].description = "Replace the near items color";

            for (int i = 0; i < 8; i++)
            {
                bs.boostProducts[i].count = 3;

                bs.boostProducts[i].GemPrices = 5;
            }

            EditorUtility.SetDirty(lm);
            EditorUtility.SetDirty(bs);
        }

        #endregion

        #region leveleditor

        private void TestLevel(bool playNow = true, bool testByPlay = true)
        {
            dirtyLevel = true;
            SaveLevel(levelNumber);
            if (EditorSceneManager.GetActiveScene().name != sceneSwitcher.GetSceneName()) EditorSceneManager.OpenScene("Assets/SweetSugar/Scenes/"+sceneSwitcher.GetSceneName()+".unity");
            LevelManager lm = Camera.main.GetComponent<LevelManager>();
            PlayerPrefs.SetInt("OpenLevelTest", levelNumber);
            PlayerPrefs.SetInt("OpenLevel", levelNumber);
            PlayerPrefs.Save();

            if (playNow)
            {
                if (EditorApplication.isPlaying)
                    EditorApplication.isPlaying = false;
                else
                    EditorApplication.isPlaying = true;
            }
        }


        private void GUILevelSelector()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Level editor", GUILayout.Width(150));
                if (GUILayout.Button("Test level", GUILayout.Width(158)))
                {
                    TestLevel();
                }
                if (GUILayout.Button("Save", GUILayout.Width(50)))
                {
                    dirtyLevel = true;
                    SaveLevel(levelNumber);
                }

            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Level", GUILayout.Width(50));
                GUILayout.Space(100);
                if (GUILayout.Button("<<", GUILayout.Width(50)))
                {
                    PreviousLevel();
                }

                string changeLvl = GUILayout.TextField(" " + levelNumber, GUILayout.Width(50));
                if (int.Parse(changeLvl) != levelNumber)
                {
                    subLevelNumber = 1;
                    if (LoadLevel(int.Parse(changeLvl)))
                        levelNumber = int.Parse(changeLvl);

                }

                if (GUILayout.Button(">>", GUILayout.Width(50)))
                {
                    NextLevel();
                }

                if (GUILayout.Button(new GUIContent("+", "add level"), GUILayout.Width(20)))
                {
                    AddLevel();
                }

                if (GUILayout.Button(new GUIContent("- ", "remove current level"), GUILayout.Width(20)))
                {
                    RemoveLevel();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            //Sub Level

            GUILayout.BeginHorizontal();
            {
//            GUILayout.Space(60);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Sub Level", GUILayout.Width(80));
                GUILayout.Space(70);
                if (GUILayout.Button("<<", GUILayout.Width(50)))
                {
                    PreviousSubLevel();
                }

                GUILayout.Label(" " + SubLevelNumber + " / " + subLevelNumberTotal, GUILayout.Width(50));

                if (GUILayout.Button(">>", GUILayout.Width(50)))
                {
                    NextSubLevel();
                }

                if (GUILayout.Button(new GUIContent("+", "add sublevel"), GUILayout.Width(20)))
                {
                    AddSubLevel();
                }

                if (GUILayout.Button(new GUIContent("- ", "remove current sublevel"), GUILayout.Width(20)))
                {
                    RemoveSubLevel();
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();
        }

        private void GUILevelSize()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            int oldValue = levelData.GetField(subLevelNumber - 1).maxRows + levelData.GetField(subLevelNumber - 1).maxCols;
            GUILayout.Label("Columns", GUILayout.Width(50));
            GUILayout.Space(100);
            levelData.GetField(subLevelNumber - 1).maxCols = EditorGUILayout.IntField("",
                levelData.GetField(subLevelNumber - 1).maxCols, GUILayout.Width(50));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Rows", GUILayout.Width(50));
            GUILayout.Space(100);
            levelData.GetField(subLevelNumber - 1).maxRows = EditorGUILayout.IntField("",
                levelData.GetField(subLevelNumber - 1).maxRows, GUILayout.Width(50));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (levelData.GetField(subLevelNumber - 1).maxRows < 3)
                levelData.GetField(subLevelNumber - 1).maxRows = 3;
            if (levelData.GetField(subLevelNumber - 1).maxCols < 3)
                levelData.GetField(subLevelNumber - 1).maxCols = 3;
            if (levelData.GetField(subLevelNumber - 1).maxRows > 11)
                levelData.GetField(subLevelNumber - 1).maxRows = 11;
            if (levelData.GetField(subLevelNumber - 1).maxCols > 11)
                levelData.GetField(subLevelNumber - 1).maxCols = 11;
            if (oldValue != levelData.GetField(subLevelNumber - 1).maxRows + levelData.GetField(subLevelNumber - 1).maxCols)
            {
                Initialize();
                InitializeSublevel();
                dirtyLevel = true;
                // SaveLevel();
            }

        }

        private void GUILimit()
        {
            GUILayout.BeginHorizontal();
            {

                GUILayout.Label("Limit", EditorStyles.label, GUILayout.Width(50));
                GUILayout.Space(100);
                LIMIT limitTypeSave = levelData.limitType;
                int oldLimit = levelData.limit;
                levelData.limitType = (LIMIT)EditorGUILayout.EnumPopup(levelData.limitType, GUILayout.Width(93));
                if (levelData.limitType == LIMIT.MOVES)
                    levelData.limit = EditorGUILayout.IntField(levelData.limit, GUILayout.Width(50));
                else
                {
                    GUILayout.BeginHorizontal();
                    int limitMin = EditorGUILayout.IntField(levelData.limit / 60, GUILayout.Width(30));
                    GUILayout.Label(":", GUILayout.Width(10));
                    int limitSec =
                        EditorGUILayout.IntField(levelData.limit - (levelData.limit / 60) * 60, GUILayout.Width(30));
                    levelData.limit = limitMin * 60 + limitSec;
                    GUILayout.EndHorizontal();
                }

                if (levelData.limit <= 0)
                    levelData.limit = 1;
                if (limitTypeSave != levelData.limitType || oldLimit != levelData.limit)
                    dirtyLevel = true;
                // 	SaveLevel();
            }
            GUILayout.EndHorizontal();
        }

        private void GUIColorLimit()
        {
            GUILayout.BeginHorizontal();

            int saveInt = levelData.colorLimit;
            GUILayout.Label("Color limit", EditorStyles.label, GUILayout.Width(100));
            GUILayout.Space(50);
            levelData.colorLimit = (int)GUILayout.HorizontalSlider(levelData.colorLimit, 3, 6, GUILayout.Width(100));
            levelData.colorLimit = EditorGUILayout.IntField("", levelData.colorLimit, GUILayout.Width(50));
            if (levelData.colorLimit < 3)
                levelData.colorLimit = 3;
            if (levelData.colorLimit > 6)
                levelData.colorLimit = 6;

            GUILayout.EndHorizontal();

            if (saveInt != levelData.colorLimit)
            {
                dirtyLevel = true;
                // SaveLevel();
            }
        }


        private void GUIStars()
        {
            GUILayout.BeginHorizontal();
//        GUILayout.Space(35);

            //GUILayout.BeginVertical();

            GUILayout.Label("Stars", GUILayout.Width(30));

            GUILayout.Space(120);
            GUILayout.BeginHorizontal();
            int s = 0;
            s = EditorGUILayout.IntField("", levelData.star1, GUILayout.Width(50));
            if (s != levelData.star1)
            {
                levelData.star1 = s;
                dirtyLevel = true;
                // SaveLevel();
            }

            if (levelData.star1 <= 0)
                levelData.star1 = 100;
            s = EditorGUILayout.IntField("", levelData.star2, GUILayout.Width(50));
            if (s != levelData.star2)
            {
                levelData.star2 = s;
                dirtyLevel = true;
                // SaveLevel();
            }

            if (levelData.star2 < levelData.star1)
                levelData.star2 = levelData.star1 + 10;
            s = EditorGUILayout.IntField("", levelData.star3, GUILayout.Width(50));
            if (s != levelData.star3)
            {
                levelData.star3 = s;
                dirtyLevel = true;
                // SaveLevel();
            }

            if (levelData.star3 < levelData.star2)
            {
                levelData.star3 = levelData.star2 + 10;
                dirtyLevel = true;
                // SaveLevel();
            }

            GUILayout.EndHorizontal();
            //GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void GUIMamalade()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(30);

                GUILayout.Label("Marmalade",  GUILayout.Width(70));
                bool s = false;
                s = EditorGUILayout.Toggle(levelData.enableMarmalade, GUILayout.Width(50));
                if (s != levelData.enableMarmalade)
                {
                    levelData.enableMarmalade = s;
                    dirtyLevel = true;
                    // SaveLevel();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void GUINoRegen()
        {
            GUILayout.BeginHorizontal();
            {

                GUILayout.Label(new GUIContent("Don't Regen", "Don't regenerate if no matches possible"),
                    GUILayout.Width(80));
                bool s = false;
                GUILayout.Space(70);
                s = EditorGUILayout.Toggle(levelData.GetField().noRegenLevel, GUILayout.Width(50));
                if (s != levelData.GetField().noRegenLevel)
                {
                    levelData.GetField().noRegenLevel = s;
                    dirtyLevel = true;
                    // SaveLevel();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void GUITarget()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Target",GUILayout.ExpandWidth (false));
                GUILayout.Space(110);
//                target_settings = EditorGUILayout.Foldout(target_settings, "Target:");
//                if (target_settings)
                GUILayout.BeginVertical();
                {
                        int selectedTarget = levelData.GetTargetIndex();
                        int newselectedTarget = EditorGUILayout.Popup(selectedTarget, levelData.GetTargetsNames(),
                            GUILayout.Width(100),GUILayout.ExpandWidth (false));
                        GUILayout.BeginVertical(GUILayout.ExpandWidth (false));
                        {
                            var subTargets = levelData.subTargetsContainers;
                            for (int i = 0; i < subTargets.Count; i++)
                            {
                                var item = subTargets[i];
                                if (item?.IsTargetSquare() ?? true) continue;
                                int oldCount = item.GetCount();
                                item.SetCount(EditorGUILayout.IntField(item.extraObject.name, item.GetCount(), GUILayout.ExpandWidth (false),GUILayout.MinWidth(30)));
                                if (oldCount != item.GetCount()) dirtyLevel = true;
                            }

                            if (newselectedTarget != selectedTarget)
                            {
                                levelData.SetTarget(newselectedTarget);
                                dirtyLevel = true;
                            }
                        }
                        GUILayout.EndVertical(); 
                }
                GUILayout.EndVertical(); 

            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Tutorial",GUILayout.ExpandWidth (false));
                GUILayout.Space(100);
                int newselectedTutorial = EditorGUILayout.Popup(levelData.selectedTutorial, GetTutorialNames(),GUILayout.Width(100),GUILayout.ExpandWidth (false));
                if(levelData.selectedTutorial != newselectedTutorial)
                {
                    levelData.selectedTutorial = newselectedTutorial;
                    dirtyLevel = true;
                }
            }
            GUILayout.EndHorizontal();
        }

        string[] GetTutorialNames() => new[] {"Disabled","SIMPLE", ItemsTypes.HORIZONTAL_STRIPED.ToString(), ItemsTypes.PACKAGE.ToString(), ItemsTypes.TimeBomb.ToString()};

        private bool HideAnotherTargetBlock(string blockName)
        {
            var targets = levelData.GetTargetsNames();
            var currentTarget = levelData.target.name;
            if (IsBlockTarget(blockName, targets) && !blockName.Contains(currentTarget) &&
                (blockName.Contains("Jelly") || blockName.Contains("Sugar")))
                return true;
            return false;
        }

        private bool IsBlockTarget(string blockName, string[] targets)
        {
            var list = targets.Where(i => blockName.Contains(i));
            return list.Count() > 0;
        }

        private void GUISections()
        {
            GUILayout.BeginHorizontal();
            {
                var saveSetion = section;
                section = GUILayout.Toolbar(section, sectionsString, GUILayout.Width(450));
                if (section != saveSetion && section == 3) squareBlockSelected = null;
            }
            GUILayout.EndHorizontal();
        }

        private void GUITeleport()
        {
            GUILayout.BeginHorizontal();
            {
                //GUILayout.Space(30);
                GUILayout.BeginVertical();
                {
                    if (GUILayout.Button(new GUIContent("Clear", "clear all teleports"), GUILayout.Width(50),
                        GUILayout.Height(50)))
                    {
                        squareBlockSelected = null;
                        for (int i = 0; i < levelData.GetField(subLevelNumber - 1).levelSquares.Length; i++)
                        {
                            levelData.GetField(subLevelNumber - 1).levelSquares[i].isEnterTeleport = false;
                            levelData.GetField(subLevelNumber - 1).levelSquares[i].teleportCoordinatesLinkedBack =
                                Vector2Int.one * -1;
                            levelData.GetField(subLevelNumber - 1).levelSquares[i].teleportCoordinatesLinked =
                                Vector2Int.one * -1;
                        }

                        dirtyLevel = true;
                        // SaveLevel();
                    }

                    GUILayout.Space(10);

                    GUILayout.Label("Click to the field, once for enter then for exit.", EditorStyles.boldLabel);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private int arrow_index;
        private int selectDirectionType;

        private void GUIDirections()
        {
            GUILayout.BeginHorizontal();
            {
                //GUILayout.Space(30);
                GUILayout.BeginVertical();
                {
                    GUILayout.Label("Tools:", EditorStyles.boldLabel);
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(new GUIContent(arrows_enter[0], "enter point on/off"), GUILayout.Width(50),
                            GUILayout.Height(50)))
                        {
                            selectDirectionType = 1;
                        }
                    
                        if (GUILayout.Button(new GUIContent(arrows[4], "rotate one arrow"), GUILayout.Width(50),
                            GUILayout.Height(50)))
                        {
                            selectDirectionType = 0;
                        }

                        if (GUILayout.Button(new GUIContent(arrows[5], "rotate all"), GUILayout.Width(50),
                            GUILayout.Height(50)))
                        {
                            arrow_index = (int)Mathf.Repeat(arrow_index + 1, 4);
                            var squares = levelData.GetField(subLevelNumber - 1).levelSquares;
                            foreach (var item in squares)
                            {
                                item.direction = GetDirectionByIndex(arrow_index);
                            }

                            SetEnterPoint(GetDirectionByIndex(arrow_index));
                        }

                        if (GUILayout.Button(new GUIContent("Default", "default all direction"), GUILayout.Width(50),
                            GUILayout.Height(50)))
                        {
                            arrow_index = 0;
                            ResetDirection();
                            // SaveLevel();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private void ResetDirection()
        {
            var squares = levelData.GetField(subLevelNumber - 1).levelSquares;
            foreach (var item in squares)
            {
                item.direction = GetDirectionByIndex(arrow_index);
            }

            SetEnterPoint(GetDirectionByIndex(arrow_index));
            dirtyLevel = true;
        }

        private void SetEnterPoint(Vector2 direction)
        {
            levelData.GetField(subLevelNumber - 1).levelSquares.ToList().ForEach(i => i.enterSquare = false);
            if (direction == Vector2.down)
            {
                for (int col = 0; col < levelData.GetField(subLevelNumber - 1).maxCols; col++)
                {
                    var squareBlocks = levelData.GetBlock(0, col);
                    if (squareBlocks.teleportCoordinatesLinkedBack == Vector2Int.one * -1)
                        squareBlocks.enterSquare = true;
                }
            }

            if (direction == Vector2.up)
            {
                for (int col = 0; col < levelData.GetField(subLevelNumber - 1).maxCols; col++)
                {
                    var squareBlocks = levelData.GetBlock(levelData.GetField(subLevelNumber - 1).maxRows - 1, col);
                    if (squareBlocks.teleportCoordinatesLinkedBack == Vector2Int.one * -1)
                        squareBlocks.enterSquare = true;
                }
            }

            if (direction == Vector2.left)
            {
                for (int row = 0; row < levelData.GetField(subLevelNumber - 1).maxRows; row++)
                {
                    var squareBlocks = levelData.GetBlock(row, levelData.GetField(subLevelNumber - 1).maxCols - 1);
                    if (squareBlocks.teleportCoordinatesLinkedBack == Vector2Int.one * -1)
                        squareBlocks.enterSquare = true;
                }
            }

            if (direction == Vector2.right)
            {
                for (int row = 0; row < levelData.GetField(subLevelNumber - 1).maxRows; row++)
                {
                    var squareBlocks = levelData.GetBlock(row, 0);
                    if (squareBlocks.teleportCoordinatesLinkedBack == Vector2Int.one * -1)
                        squareBlocks.enterSquare = true;
                }
            }
        }

        private int GetIndexByDirection(Vector2 direction)
        {
            if (direction == Vector2.right)
                return 3;
            if (direction == Vector2.left)
                return 1;
            if (direction == Vector2.up)
                return 2;
            return 0;
        }

        private Vector2 GetDirectionByIndex(int index)
        {
            if (index == 0) return Vector2.down;
            if (index == 1) return Vector2.left;
            if (index == 2) return Vector2.up;
            if (index == 3) return Vector2.right;
            return Vector2.down;
        }

        private Texture GetArrowByAngle(float angle)
        {
            return arrows[(int)(angle % 90)];
        }

        private Texture GetArrowByVector(Vector2 direction, bool enterPoint)
        {
            var arr = arrows;
            if (enterPoint) arr = arrows_enter;
            if (direction == Vector2.up)
                return arr[3];
            if (direction == Vector2.left)
                return arr[1];
            if (direction == Vector2.right)
                return arr[2];
            return arr[0];
        }

        private void GUIItems()
        {
            GUILayout.BeginHorizontal();
            {
                //GUILayout.Space(30);
                GUILayout.BeginVertical();
                {
                    GUILayout.Label("Tip: right click to change sprite for level", EditorStyles.boldLabel);
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(new GUIContent("Clear", "clear all field"), GUILayout.Width(50),
                            GUILayout.Height(50)))
                        {
                            ClearItems();
                            // SaveLevel();
                        }

                        GUILayout.BeginVertical();
                        {
                            foreach (var val in itemsForEditor)
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    foreach (var item in val)
                                    {
                                        GameObject item1 = item.Item;
                                        if(!item1) continue;
                                        if (item.ItemType != ItemsTypes.NONE)
                                            item.SetColor(color, levelNumber);
                                        var texture = item.Texture;
                                        if (item.ItemType == ItemsTypes.PACKAGE)
                                        {
                                            try
                                            {
                                                texture = packageTexture.AlphaBlend(item.Texture);
                                            }
                                            catch (Exception e)
                                            {
                                            }
                                        }
                                        if (GUILayout.Button(new GUIContent(texture, item1.name),
                                            GUILayout.Width(50), GUILayout.Height(50)))
                                        {
                                            Event current = Event.current;
                                            if(current.button == 1)
                                            {
                                                dirtyLevel = true;
                                                SaveLevel(levelNumber);
                                                ItemsPerLevelEditor.ShowWindow(item1, levelNumber);
                                            }
                                            else
                                            {
                                                itemBrush = item.DeepCopy();
                                                if (item.ItemType == ItemsTypes.NONE) color = itemBrush.Color;
                                            }
                                        }
                                    }
                                }
                                if (val == itemsForEditor.Last())
                                {
                                    if (GUILayout.Button(new GUIContent("X", "Clear block"), GUILayout.Width(50),
                                        GUILayout.Height(50)))
                                    {
                                        itemBrush = null;
                                    }
                                }

                                GUILayout.EndHorizontal();
                            }
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private void ClearItems()
        {
            for (int i = 0; i < levelData.GetField(subLevelNumber - 1).levelSquares.Length; i++)
            {
                levelData.GetField(subLevelNumber - 1).levelSquares[i].item = null;
            }

            dirtyLevel = true;
        }

        private void GUIBlocks()
        {
            GUILayout.BeginHorizontal();
            {
                //GUILayout.Space(30);
                GUILayout.BeginVertical();
                {
                    GUILayout.Label("Tools:", EditorStyles.boldLabel);
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(new GUIContent("Clear", "clear all field"), GUILayout.Width(50),
                            GUILayout.Height(50)))
                        {
                            ClearLevel();
                            // SaveLevel();
                        }

                        GUILayout.BeginHorizontal();
                        {
                            //GUILayout.Space(30);
                            UnityEngine.GUI.color = new Color(1, 1, 1, 1f);
                            foreach (SquareTypes squareTypeItem in EnumUtil.GetValues<SquareTypes>())
                            {
                                if (HideAnotherTargetBlock(squareTypeItem.ToString())) continue;
                                if (squareTypeItem == SquareTypes.NONE) continue;
                                if (GUILayout.Button(
                                    new GUIContent(Square.GetSquareTexture(squareTypeItem), squareTypeItem.ToString()),
                                    GUILayout.Width(50), GUILayout.Height(50)))
                                    squareType = squareTypeItem;
                            }

                            UnityEngine.GUI.color = new Color(1, 1, 1, 1f);

                            if (GUILayout.Button(new GUIContent("X", "Clear block"), GUILayout.Width(50),
                                GUILayout.Height(50)))
                            {
                                squareType = SquareTypes.NONE;
                            }

                            if (GUILayout.Button(new GUIContent("Fill+", "Fill with selected block, second click change or clear filling"), GUILayout.Width(50),
                                GUILayout.Height(50)))
                            {
                                FillLevel();
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private void ClearLevel()
        {
            for (int i = 0; i < levelData.GetField(subLevelNumber - 1).levelSquares.Length; i++)
            {
                levelData.GetField(subLevelNumber - 1).levelSquares[i].block = SquareTypes.EmptySquare;
                levelData.GetField(subLevelNumber - 1).levelSquares[i].blockLayer = 1;
                levelData.GetField(subLevelNumber - 1).levelSquares[i].obstacle = SquareTypes.NONE;
                levelData.GetField(subLevelNumber - 1).levelSquares[i].obstacleLayer = 1;
            }

            dirtyLevel = true;
        }

        private void FillLevel()
        {
            for (int i = 0; i < levelData.GetField(subLevelNumber - 1).levelSquares.Length; i++)
            {
                var squareBlocks = levelData.GetField(subLevelNumber - 1).levelSquares[i];
                var tempType = squareType;
                if ((squareBlocks.block == squareType && Square.GetLayersCount(squareType) == squareBlocks.blockLayer)
                    || (squareBlocks.obstacle == squareType && Square.GetLayersCount(squareType) == squareBlocks.obstacleLayer))
                {
                    var sqPos = squareBlocks.position;
                    squareType = SquareTypes.EmptySquare;
                    SetSquareType(sqPos.x, sqPos.y);
                    squareType = tempType;

                }
                else if (squareBlocks.block == SquareTypes.EmptySquare || (squareBlocks.block == squareType || squareBlocks.obstacle == squareType))
                {
                    var sqPos = squareBlocks.position;
                    SetSquareType(sqPos.x, sqPos.y);
                }
            }

            dirtyLevel = true;
        }

        private SquareBlocks squareBlockSelected; //for teleport linking

        private void GUIGameField()
        {
            GUILayout.BeginVertical();
            for (int row = 0; row < levelData.GetField(subLevelNumber - 1).maxRows; row++)
            {
                GUILayout.BeginHorizontal();

                for (int col = 0; col < levelData.GetField(subLevelNumber - 1).maxCols; col++)
                {
                    Color squareColor = new Color(0.8f, 0.8f, 0.8f);
                    var imageButton = new object();
                    SquareBlocks squareBlock = levelData.GetBlock(row, col);
                    squareBlock.position = new Vector2Int(col, row);
                    var blockTextures = Square.GetSquareTextures(squareBlock);
                    var blockTexturesArray = new List<Texture2D>();
                    var itemTextures = new Texture2D[2];
                    if (squareBlock.block == SquareTypes.NONE)
                    {
                        imageButton = "X";
                        squareColor = new Color(0.8f, 0.8f, 0.8f);
                    }
                    else
                    {
                        imageButton = blockTextures?[0];
                        squareColor = Color.white;
                    }

                    blockTexturesArray = blockTextures.Take(squareBlock.blockLayer + squareBlock.obstacleLayer).ToList();
                    if (section == 0 || section == 1)
                    {
                        if (squareBlock.block != SquareTypes.NONE )
                        {
                            //                        imageButton = squareBlock.item?.Texture;
                            if(squareBlock.item != null && squareBlock.item.Item != null)
                                blockTexturesArray.Add(squareBlock.item?.Item.GetComponent<IColorableComponent>().GetSprite(levelNumber, squareBlock.item.Color).texture);
                            else
                                blockTexturesArray.Add(squareBlock.item?.Texture);
                        }
                        if (squareBlock.item?.ItemType == ItemsTypes.PACKAGE)
                        {
                            // if (squareBlock.item?.Texture == null)
                            blockTexturesArray.Add(itemsForEditorFull.First(i => i.ItemType == ItemsTypes.PACKAGE).Texture
                                ?.AlphaBlend(squareBlock.item?.Item.GetComponent<IColorableComponent>().GetSprite(levelNumber, squareBlock.item.Color).texture));
                        }
                    }

                    if (section == 2)
                    {
                        imageButton = GetArrowByVector(squareBlock.direction, squareBlock.enterSquare);
                    }

                    if (section == 3)
                    {
                        if (squareBlock.teleportCoordinatesLinkedBack != Vector2Int.one * -1)
                        {
                            imageButton = teleports[1];
                        }

                        if (squareBlock.isEnterTeleport)
                            imageButton = teleports[0];
                    }

                    UnityEngine.GUI.color = squareColor;
                    if (GUILayout.Button(imageButton as Texture, GUILayout.Width(50), GUILayout.Height(50)))
                    {
                        if (section == 0)
                            SetSquareType(col, row);
                        if (section == 1)
                            SetItem(col, row);
                        else if (section == 2)
                        {
                            if (selectDirectionType == 0)
                                SetArrow(col, row);
                            else if (selectDirectionType == 1)
                                levelData.GetBlock(row, col).enterSquare = !levelData.GetBlock(row, col).enterSquare;
                        }
                        else if (section == 3)
                        {
                            if (squareBlock.isEnterTeleport ||
                                squareBlock.teleportCoordinatesLinkedBack != Vector2Int.one * -1)
                            {
                                squareBlockSelected = null;
                                squareBlock.isEnterTeleport = false;
                                squareBlock.teleportCoordinatesLinkedBack = Vector2Int.one * -1;
                                squareBlock.teleportCoordinatesLinked = Vector2Int.one * -1;
                            }
                            else if (squareBlockSelected == null)
                            {
                                squareBlockSelected = squareBlock;
                                squareBlockSelected.isEnterTeleport = true;
                            }
                            else
                            {
                                levelData.GetBlock(row, col).enterSquare = false;
                                squareBlock.teleportCoordinatesLinkedBack = squareBlockSelected.position;
                                squareBlockSelected.teleportCoordinatesLinked = squareBlock.position;
                                squareBlockSelected = null;
                            }
                        }
                    }


                    var lastRect = GUILayoutUtility.GetLastRect();
                    if (lastRect != Rect.zero && lastRect.position != Vector2.zero)
                        squareBlock.guiRect = lastRect;
                    if (section != 2 && section != 3)
                        DrawLayeredTextures(blockTexturesArray.WhereNotNull().ToArray(), lastRect);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            SetupArrows();
            DrawLines();

        }

        private static void DrawLayeredTextures(Texture2D[] textures, Rect lastRect)
        {
            if (textures.Length > 1)
            {
                for (int i = 1; i < textures.Length; i++)
                {
                    if (i < textures.Length)
                        UnityEngine.GUI.DrawTexture(new Rect(lastRect.position.x + 4, lastRect.position.y + 4, 40, 40),
                            textures[i]);
                }
            }
        }

        private void DrawLines()
        {
            if (section != 1) return;
            var marmalades = levelData.GetField().levelSquares.Where(i => i.item != null && i.item.EnableMarmaladeTargets);
            Handles.color = Color.red;
            foreach (var marmalade in marmalades)
            {
                //            Handles.BeginGUI();
                foreach (var targetPosition in marmalade.item.TargetMarmaladePositions)
                {
                    Handles.DrawLine(marmalade.guiRect.center, levelData.GetBlock(targetPosition).guiRect.center);
                    RotateGizmo(marmalade.guiRect.center, levelData.GetBlock(targetPosition).guiRect.center, 20f);
                    RotateGizmo(marmalade.guiRect.center, levelData.GetBlock(targetPosition).guiRect.center, -20f);
                }
                //            Handles.EndGUI();
            }
        }

        private void RotateGizmo(Vector3 square, Vector3 pivot, float angle)
        {
            var rightPoint = Vector3.zero;
            var dir = (square - pivot) * 0.1f;
            dir = Quaternion.AngleAxis(angle, Vector3.back) * dir;
            rightPoint = dir + pivot;
            Handles.DrawLine(pivot, rightPoint);
        }

        private void SetItem(int col, int row)
        {
            SquareBlocks squareBlock = levelData.GetBlock(row, col);
            ItemForEditor itemTemp = squareBlock.item;
            if (itemTemp == null)
                itemTemp = itemBrush.DeepCopy();
            else if (itemBrush?.ItemType != ItemsTypes.NONE ||
                     (itemTemp.ItemType == ItemsTypes.NONE && itemBrush?.ItemType == ItemsTypes.NONE))
                itemTemp = itemBrush?.DeepCopy();
            else if (itemBrush.ItemType == ItemsTypes.NONE && itemTemp.ItemType != ItemsTypes.NONE)
                itemTemp.Color = itemBrush.Color;
            //        if (itemTemp.ItemType != ItemsTypes.NONE)
            if (itemTemp != null && itemBrush != null && itemsForEditorFull.First(i => i.ItemType == itemTemp.ItemType).Item.GetComponent<IColorableComponent>().GetSprites(levelNumber).Count() > 1)
                itemTemp.Texture = itemsForEditorFull.FirstOrDefault(i => i.ItemType == itemTemp.ItemType).Item.GetComponent<IColorableComponent>().GetSprites(levelNumber)[color].texture;
            squareBlock.item = itemTemp;
        }

        private void SetupArrows()
        {
            for (int row = 0; row < levelData.GetField(subLevelNumber - 1).maxRows; row++)
            {
                for (int col = 0; col < levelData.GetField(subLevelNumber - 1).maxCols; col++)
                {
                    SquareBlocks squareBlock = levelData.GetBlock(row, col);
                    if (section == 2 || section == 3)
                    {
                        if (squareBlock.teleportCoordinatesLinkedBack != Vector2Int.one * -1)
                        {
                            Handles.BeginGUI();
                            Handles.color = Color.red;
                            Vector2 position1 = squareBlock.guiRect.center;
                            Vector2 position2 = levelData.GetBlock(squareBlock.teleportCoordinatesLinkedBack).guiRect
                                .center;
                            Handles.DrawLine(position1, position2);
                            float angle = AngleInDeg(position1, position2);
                            Handles.ArrowHandleCap(0, position2,
                                Quaternion.AngleAxis(angle, Vector3.forward) * Quaternion.Euler(new Vector3(0, -95, 0)), 50,
                                EventType.Repaint);
                            Handles.EndGUI();
                        }
                    }
                }
            }
        }

        //This returns the angle in radians
        public static float AngleInRad(Vector3 vec1, Vector3 vec2)
        {
            return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);
        }

        //This returns the angle in degress
        public static float AngleInDeg(Vector3 vec1, Vector3 vec2)
        {
            return AngleInRad(vec1, vec2) * 180 / Mathf.PI;
        }


        private void SetArrow(int col, int row)
        {
            SquareBlocks squareBlock = levelData.GetBlock(row, col);
            int index = GetIndexByDirection(squareBlock.direction);
            index = (int)Mathf.Repeat(index + 1, 4);
            // if (squareBlock.block != SquareTypes.NONE)
            // {
            squareBlock.direction = GetDirectionByIndex(index);
            // }
        }


        private void SaveLevel(int _levelNumber)
        {
            squareBlockSelected = null;
            if (dirtyLevel)
            {
                if (!FileName.Contains(".txt"))
                    FileName += ".txt";
                SaveToScriptable(_levelNumber);
                // SaveCommonValues();
                // SaveField();
                dirtyLevel = false;
            }
        }

        private void AddLevel()
        {
            squareBlockSelected = null;
            SaveLevel(levelNumber);
            levelNumber = GetLastLevel()+1;
            SubLevelNumber = 1;
            ScriptableLevelManager.CreateFileLevel(levelNumber, levelData);
            levelData = ScriptableLevelManager.LoadLevel(levelNumber);
            Initialize();
            ClearLevel();
            ClearItems();
            SaveLevel(levelNumber);
            subLevelNumberTotal = GetSubLevelsCount();
        }

        private int GetLastLevel()
        {
            int lastLevel = LoadingManager.GetLastLevelNum();
            if(lastLevel == 0) lastLevel = levelScriptable.levels.Count;
            return lastLevel;
        }

        private void AddSubLevel()
        {
            squareBlockSelected = null;
            SaveLevel(levelNumber);
            levelData.AddNewField();
            SubLevelNumber = GetSubLevelsCount();
            Initialize();
            InitializeSublevel();
            SaveLevel(levelNumber);
            subLevelNumberTotal = GetSubLevelsCount();
        }

        private void RemoveSubLevel()
        {
            if (GetSubLevelsCount() > 1)
            {
                SaveLevel(levelNumber);
                levelData.RemoveField();
                SubLevelNumber = GetSubLevelsCount();
                Initialize();
                // InitializeSublevel();
                SaveLevel(levelNumber);
                subLevelNumberTotal = GetSubLevelsCount();
            }
        }

        private int GetSubLevelsCount()
        {
            return levelData.fields.Count; //GetSublevelsScriptable();
        }

        private int GetSublevelsScriptable()
        {
            return levelScriptable.levels[levelNumber - 1].fields.Count;
        }

        private void RemoveLevel()
        {
            File.Delete(levelPath+"Level_"+levelNumber+".asset");
            levelNumber--; 
            LoadLevel(levelNumber);
        }

        private void NextLevel()
        {
            SaveLevel(levelNumber);
            if (levelNumber + 1 <= GetLastLevel())
            {
                levelNumber++;
                SubLevelNumber = 1;
                LoadLevel(levelNumber);
            }
        }

        private void PreviousLevel()
        {
            SaveLevel(levelNumber);
            levelNumber--;
            SubLevelNumber = 1;
            if (levelNumber < 1)
                levelNumber = 1;
            if (!LoadLevel(levelNumber))
            {
                levelNumber++;
                LoadLevel(levelNumber);
            }
        }

        private void NextSubLevel()
        {
            if (SubLevelNumber + 1 <= GetSubLevelsCount())
            {
                SaveLevel(levelNumber);
                SubLevelNumber++;
            }

            // if (!LoadLevel(levelNumber, SubLevelNumber))
            // 	SubLevelNumber--;
        }

        private void PreviousSubLevel()
        {
            SaveLevel(levelNumber);
            SubLevelNumber--;
            if (SubLevelNumber < 1)
                SubLevelNumber = 1;
            // if (!LoadLevel(levelNumber, SubLevelNumber))
            // SubLevelNumber++;
        }

        private bool dirtyLevel;
        private int section;
        private bool gotFocus;
        private static Material mat;
        private int color;
        private Texture2D packageTexture;
        private GUISkin customSkin;
        private MenuReference menuReference;
        private AdManagerScriptable adsSettings;
        private int selectedTutorial;
        private MapSwitcher sceneSwitcher;

        private void SetSquareType(int col, int row)
        {
            dirtyLevel = true;
            SquareBlocks squareBlock = levelData.GetBlock(row, col);
            if (squareType == SquareTypes.EmptySquare || squareType == SquareTypes.NONE)
            {
                squareBlock.block = squareType;
                squareBlock.blockLayer = 1;
                squareBlock.obstacle = SquareTypes.NONE;
                squareBlock.obstacleLayer = 1;
            }
            else
            {
                Square squarePrototype = Square.GetBlockPrefab(squareType).Last().GetComponent<Square>();
                if (!squarePrototype.IsObstacle() && squareType != SquareTypes.EmptySquare &&
                    squareType != SquareTypes.NONE)
                {
                    if (squareBlock.block == squareType)
                        squareBlock.blockLayer =
                            (int)Mathf.Repeat(squareBlock.blockLayer, Square.GetLayersCount(squareType)) + 1;
                    else
                        squareBlock.blockLayer = 1;
                    squareBlock.block = squareType;
                }
                else if (squarePrototype.IsObstacle())
                {
                    if (squareBlock.obstacle == squareType)
                        squareBlock.obstacleLayer =
                            (int)Mathf.Repeat(squareBlock.obstacleLayer, Square.GetLayersCount(squareType)) + 1;
                    else
                        squareBlock.obstacleLayer = 1;
                    squareBlock.obstacle = squareType;
                }
            }

            update = true;
            // SaveLevel();
            // GetSquare(col, row).type = (int) squareType;
        }

        private void SaveToScriptable(int _levelNumber)
        {
            levelData.InitTargetObjects();
            SquareBlocks[] levelSquares = levelData.GetField(subLevelNumber - 1).levelSquares;
            for (int i = 0; i < levelSquares.Length; i++)
            {
                var item = levelSquares[i];
                var dir = item.direction;

                if (dir == Vector2.zero) item.direction = Vector2.down;
            }

            bool enterSquaresExist = levelSquares.Any(i => i.enterSquare);
            if (!enterSquaresExist)
            {
                for (int col = 0; col < levelData.GetField(subLevelNumber - 1).maxCols; col++)
                {
                    levelData.GetBlock(0, col).enterSquare = true;
                }
            }

            if (levelScriptable.levels.Count() < _levelNumber)
                levelScriptable.levels.Add(levelData);
            else
                levelScriptable.levels[_levelNumber - 1] = levelData.DeepCopy(_levelNumber);
            ScriptableLevelManager.SaveLevel(levelPath, _levelNumber, levelData);

//        EditorUtility.SetDirty(levelScriptable);
//        AssetDatabase.SaveAssets();
        }

        private bool LoadLevel(int currentLevel)
        {
            levelData = LoadingManager.LoadlLevel(currentLevel, levelData);
            squareBlockSelected = null;
            // PlayerPrefs.SetInt("OpenLevelTest", currentLevel);
            // PlayerPrefs.Save();

            if (levelData != null)
            {
                Initialize();
                return true;
            }

            return false;
        }

        #endregion
    }
}