using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEditor;
using UnityEngine;

namespace SweetSugar.Scripts.Items.Editor
{
    [CustomEditor(typeof(BonusItem))]
    public class BonusItemEditor : UnityEditor.Editor
    {
        BonusItem myTarget;
        private GameObject targetEditorObject;
        Texture2D bonusItem;
        Texture2D item;
        private GameObject prefab;
        int[] array;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            myTarget = (BonusItem) target;
            array = myTarget.list[0].array;

            if (targetEditorObject == null)
            {
                targetEditorObject = Resources.Load("TargetEditor", typeof(GameObject)) as GameObject;
                item = targetEditorObject.GetComponent<TargetMono>().targets[0].prefabs[0].GetComponent<SpriteRenderer>().sprite.texture;
            }

            if (bonusItem == null)
                bonusItem = myTarget.gameObject.GetComponent<SpriteRenderer>().sprite.texture;
            EditorGUILayout.LabelField("Combination editor");
            GUILayout.BeginVertical();
            {
                for (int i = 0; i < 5; i++)
                {
                    GUILayout.BeginHorizontal();
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            if (GUILayout.Button(GetValue(i, j), GUILayout.Height(25), GUILayout.Width(25)))
                            {
                                ChangeItem(i, j);
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        void ChangeItem(int i, int j)
        {
            array[i * 5 + j]++;
            array[i * 5 + j] %= 3;
            prefab = PrefabUtility.FindPrefabRoot(myTarget.gameObject);
            prefab.GetComponent<BonusItem>().list[0].array = array;
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
        }

        GUIContent GetValue(int i, int j)
        {
            var value = array[i * 5 + j];
            if (value == 0)
                return new GUIContent("", "unused");
            if (value == 1)
                return new GUIContent(item, "item");
            return new GUIContent(bonusItem, "bonus item");
        }
    }
}