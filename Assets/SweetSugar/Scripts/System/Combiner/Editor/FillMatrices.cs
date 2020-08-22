using UnityEditor;

namespace SweetSugar.Scripts.System.Combiner.Editor
{
    public class FillMatrices : EditorWindow
    {
        string myString = "Hello World";
        bool groupEnabled;
        bool myBool = true;
        float myFloat = 1.23f;

        // Add menu named "My Window" to the Window menu
        // [MenuItem("Sweet Sugar/Fill Matrices")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            FillMatrices window = (FillMatrices)GetWindow(typeof(FillMatrices));
            window.Show();
        }

        // void OnGUI()
        // {
        //     if (GUILayout.Button("Fill"))
        //     {
        //         ConsoleProDebug.Clear();
        //         var bonusCombineList = Resources.FindObjectsOfTypeAll(typeof(ItemCombineBehaviour)) as ItemCombineBehaviour[];
        //         List<ItemMarmalade> list = new List<ItemMarmalade>();
        //         foreach (var item in bonusCombineList)
        //         {
        //             item.itemType = ItemsTypes.MARMALADE;
        //             if (item.GetType() == typeof(ItemMarmalade))
        //                 list.Add((ItemMarmalade)item);
        //         }

        //         foreach (ItemMarmalade item in list)
        //         {
        //             item.matrix.Clear();
        //             item.Init();
        //             var matrix = item.matrix[0].items;
        //             matrix.GetElement(1, 1).item = true;
        //             matrix.GetElement(2, 1).item = true;
        //             matrix.GetElement(1, 2).item = true;
        //             matrix.GetElement(2, 2).item = true;
        //             var prefab = PrefabUtility.FindPrefabRoot(item.gameObject);
        //             // DebugM.WatchInstance(prefab);
        //             // ItemCombineBehaviour instance = prefab.GetComponent<ItemCombineBehaviour>();
        //             // Debug.Log(instance + " " + prefab);

        //             // prefab.GetComponent<ItemMarmalade>().matrix = item.matrix;
        //             EditorUtility.SetDirty(prefab);
        //             AssetDatabase.SaveAssets();
        //         }
        //         DebugM.WatchInstance(list);
        //     }
        // }
    }
}