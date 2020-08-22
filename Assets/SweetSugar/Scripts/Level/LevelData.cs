using System;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEngine;

namespace SweetSugar.Scripts.Level
{
    /// <summary>
    /// Level data for level editor
    /// </summary>
    [Serializable]
    public class LevelData
    {
        public string Name;
        public static LevelData THIS;
        /// level number
        public int levelNum;
        private int hashCode;
        /// fields data
        public List<FieldData> fields = new List<FieldData>();
        /// target container keeps the object should be collected, its count, sprite, color
        [SerializeField] public TargetContainer target;
        /// target manager reference
        [SerializeField] public Target targetObject;
        public int targetIndex;

        // public static TargetContainer targetContainer;
        /// moves or time
        public LIMIT limitType;

        public int[] ingrCountTarget = new int[2];
        /// moves amount or seconds 
        public int limit = 25;
        /// color amount
        public int colorLimit = 5;
        /// score amount for reach 1 star
        public int star1 = 100;
        /// score amount for reach 2 stars
        public int star2 = 300;
        /// score amount for reach 3 stars
        public int star3 = 500;
        /// pre generate marmalade
        public bool enableMarmalade;
        public int maxRows { get { return GetField().maxRows; } set { GetField().maxRows = value; } }
        public int maxCols { get { return GetField().maxCols; } set { GetField().maxCols = value; } }
        public int selectedTutorial;

        public int currentSublevelIndex;
        private TargetEditorScriptable targetEditorObject;
        private List<TargetContainer> targetEditorArray;
        /// target container keeps the object should be collected, its count, sprite, color
        public List<SubTargetContainer> subTargetsContainers = new List<SubTargetContainer>();


        public FieldData GetField()
        {
            return fields[currentSublevelIndex];
        }

        public FieldData GetField(int index)
        {
            currentSublevelIndex = index;
            return fields[index];
        }

        public LevelData(bool isPlaying, int currentLevel)
        {
            hashCode = GetHashCode();
            levelNum = currentLevel;
            Name = "Level " + levelNum;
            LoadTargetObject();
            // if (isPlaying)
            //     targetEditorObject = LevelManager.This.targetEditorScriptable;
            // else
            //     targetEditorObject = AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Scriptable/TargetEditorScriptable.asset", typeof(TargetEditorScriptable)) as TargetEditorScriptable;

        }

        public void LoadTargetObject()
        {
            targetEditorObject = Resources.Load("Levels/TargetEditorScriptable") as TargetEditorScriptable;
            targetEditorArray = targetEditorObject.targets;

        }

        public Target GetTargetObject()
        {
            return targetObject;
        }
        public SquareBlocks GetBlock(int row, int col)
        {
            return GetField().levelSquares[row * GetField().maxCols + col];
        }

        public SquareBlocks GetBlock(Vector2Int vec)
        {
            return GetBlock(vec.y, vec.x);
        }
        public FieldData AddNewField()
        {
            var fieldData = new FieldData();
            fields.Add(fieldData);
            return fieldData;
        }

        public void RemoveField()
        {
            FieldData field = fields.Last();
            fields.Remove(field);
        }
        public Sprite[] GetTargetSprites()
        {
            return subTargetsContainers.Where(i => i.targetPrefab.GetComponent<SpriteRenderer>() != null)
                .Select(i => i.targetPrefab.GetComponent<SpriteRenderer>().sprite).ToArray()
                .Union(subTargetsContainers.Where(i => i.targetPrefab.GetComponent<LayeredBlock>() != null)
                    .SelectMany(i => i.targetPrefab.GetComponent<LayeredBlock>().layers)
                    .Select(x => x.GetComponent<SpriteRenderer>().sprite))
                .Union(subTargetsContainers.Where(i=>i.extraObject && i.extraObject.GetType() == typeof(Sprite)).Select(i=>(Sprite)i.extraObject))
                .ToArray();
        }

        public string[] GetTargetsNames()
        {
            return targetEditorArray?.Select(i => i.name).ToArray();
        }

        public string[] GetSubTargetNames()
        {
            return target.prefabs.Select(i => i.name).ToArray();
        }

        public Sprite[] GetTargetSpritePrefab()
        {
            return subTargetsContainers.Where(i => i.targetPrefab.GetComponent<SpriteRenderer>() != null)
                .Select(i => i.targetPrefab.GetComponent<SpriteRenderer>().sprite).ToArray()
                .Union(subTargetsContainers.Where(i => i.targetPrefab.GetComponent<LayeredBlock>() != null)
                    .SelectMany(i => i.targetPrefab.GetComponent<LayeredBlock>().layers).Take(1)
                    .Select(x => x.GetComponent<SpriteRenderer>().sprite)).ToArray()
                .Union(subTargetsContainers.Where(i => i.extraObject != null).Select(i=>(Sprite)i.extraObject)).ToArray();
        }

        public GameObject[] GetSubTargetPrefabs()
        {

            var layerBlockPrefabs = target.prefabs.Where(i => i.GetComponent<LayeredBlock>() != null).SelectMany(i => i.GetComponent<LayeredBlock>().layers).Select(i => i.gameObject).ToArray();
            var mergedList = layerBlockPrefabs.Concat(target.prefabs.Where(i => i.GetComponent<LayeredBlock>() == null)).ToArray();
            return mergedList;
        }
        public int GetTargetIndex()
        {
            var v = targetEditorArray.FindIndex(i => i.name == target.name);
            if (v < 0) { SetTarget(0); v = 0; }
            return v;
        }

        public void SetTargetFromArray()
        {
            SetTarget(targetEditorArray.FindIndex(x => x.name == target.name));
        }

        public void SetTarget(int index)
        {
            if (targetEditorObject == null || targetEditorArray == null) LoadTargetObject();
            subTargetsContainers.Clear();
            if (index < 0) index = 0;
            target = targetEditorArray[index];
            targetIndex = index;
            try
            {
                targetObject = (Target)Activator.CreateInstance(Type.GetType("SweetSugar.Scripts.TargetScripts."+target.name));
                Debug.Log("create target " + targetObject);
                var subTargetPrefabs = GetSubTargetPrefabs();
                if(subTargetPrefabs.Length>1)
                {
                    foreach (var _target in subTargetPrefabs)
                    {
                        var component = _target.GetComponent<Item>();
                        Sprite extraObject = null;
                        if(component)
                        {
                            extraObject = component.sprRenderer.FirstOrDefault().sprite;
                        }
                        subTargetsContainers.Add(new SubTargetContainer(_target, 0, extraObject));
                    }
                }
                else if (subTargetPrefabs.Length > 0 && subTargetPrefabs[0].GetComponent<IColorableComponent>())
                {
                    foreach (var item in subTargetPrefabs[0].GetComponent<IColorableComponent>().GetSprites(levelNum))
                    {
                        subTargetsContainers.Add(new SubTargetContainer(subTargetPrefabs[0], 0, item));
                    }
                }
                else if (subTargetPrefabs.Length > 0)
                {
                    foreach (var _target in subTargetPrefabs)
                    {
                        var component = _target.GetComponent<Item>();
                        Sprite extraObject = null;
                        if(component)
                        {
                            extraObject = component.sprRenderer.FirstOrDefault().sprite;
                        }
                        subTargetsContainers.Add(new SubTargetContainer(_target, 0, extraObject));
                    }
                }
            }
            catch (Exception)
            {
                Debug.LogError("Check the target name or create class " + target.name);
            }

        }

        public void InitTargetObjects()
        {

            targetObject.subTargetContainers = subTargetsContainers.ToArray();
            if (targetObject.subTargetContainers.Length > 0)
                targetObject.InitTarget();
            else Debug.LogError( "set " + target.name + " more than 0" );
        }

        public void SetItemTarget(Item item)
        {
            foreach (var _subTarget in subTargetsContainers)
            {
                if (item.CompareTag(_subTarget.targetPrefab.tag) && _subTarget.GetCount() > 0 && item.gameObject.GetComponent<TargetComponent>() == null)
                {
                    item.gameObject.AddComponent<TargetComponent>();
                }
            }
        }

        public void SetSquareTarget(GameObject gameObject, SquareTypes _sqType, GameObject prefabLink)
        {
            if (_sqType.ToString().Contains(target.name))
            {
                var subTargetContainer = subTargetsContainers.FirstOrDefault(i => _sqType.ToString().Contains(i.targetPrefab.name));
                if (subTargetContainer == null) Debug.LogError("Check Target Editor for " + _sqType);
                subTargetContainer.changeCount(1);
                gameObject.AddComponent<TargetComponent>();
            }
        }

        public string GetSaveString()
        {
            var str = "";
            foreach (var item in subTargetsContainers)
            {
                str += item.GetCount() + "/";
            }
            return str;
        }

        public LevelData DeepCopy(int level)
        {
            LoadTargetObject();
            var other = (LevelData)MemberwiseClone();
            other.hashCode = other.GetHashCode();
            other.levelNum = level;
            other.Name = "Level " + other.levelNum;
            other.fields = new List<FieldData>();
            for (var i = 0; i < fields.Count; i++)
            {
                other.fields.Add(fields[i].DeepCopy());
            }
            if (targetEditorArray.Count > 0)
                other.target = targetEditorArray.First(x => x.name == target.name);//target.DeepCopy();
            else
                other.target = target.DeepCopy();
            if (targetObject != null)
                other.targetObject = targetObject.DeepCopy();
            other.subTargetsContainers = new List<SubTargetContainer>();
            for (var i = 0; i < subTargetsContainers.Count; i++)
            {
                other.subTargetsContainers.Add(subTargetsContainers[i].DeepCopy());
            }

            other.targetObject = (Target)Activator.CreateInstance(Type.GetType("SweetSugar.Scripts.TargetScripts."+target.name));

            return other;
        }

        public LevelData DeepCopyForPlay(int level)
        {
            LevelData data = DeepCopy(level);
            data.subTargetsContainers = new List<SubTargetContainer>();
            for (var i = 0; i < subTargetsContainers.Count; i++)
            {
                subTargetsContainers[i].color = i;
                if (subTargetsContainers[i].GetCount() > 0)
                {
                    var subTargetContainer = subTargetsContainers[i].DeepCopy();
                    subTargetContainer.color = i;
                    data.subTargetsContainers.Add(subTargetContainer);
                }
            }

            return data;
        }





        // public static GameObject[] GetTargetObjects()
        // {
        //     return targetEditorObject.GetComponent<TargetMono>().targets.Find(i => i.target == target).prefabs.ToArray();
        // }
    }

    /// <summary>
    /// Field data contains field size and square array
    /// </summary>
    [Serializable]
    public class FieldData
    {
        private int hashCode;
        public int subLevel;
        public int maxRows;
        public int maxCols;
        public bool noRegenLevel; //no regenerate level if no matches possible
        public SquareBlocks[] levelSquares = new SquareBlocks[81];
        internal int row;
        public int bombTimer = 15;

        public FieldData()
        {
            hashCode = GetHashCode();

        }

        public FieldData DeepCopy()
        {
            var other = (FieldData)MemberwiseClone();
            other.levelSquares = new SquareBlocks[levelSquares.Length];
            for (var i = 0; i < levelSquares.Length; i++)
            {
                other.levelSquares[i] = levelSquares[i].DeepCopy();
            }

            other.hashCode = other.GetHashCode();

            return other;
        }
    }

    /// <summary>
    /// Square blocks uses in editor
    /// </summary>
    [Serializable]
    public class SquareBlocks
    {
        public SquareTypes block;
        public int blockLayer = 1;
        public SquareTypes obstacle;
        public int obstacleLayer = 1;
        public Vector2Int position;
        public Vector2 direction;
        public bool enterSquare;
        public bool isEnterTeleport;
        public Vector2Int teleportCoordinatesLinked = new Vector2Int(-1, -1);
        public Vector2Int teleportCoordinatesLinkedBack = new Vector2Int(-1, -1);
        public Rect guiRect;
        public ItemForEditor item;

        public SquareBlocks DeepCopy()
        {
            var other = (SquareBlocks)MemberwiseClone();
            return other;
        }
    }

    /// <summary>
    /// Item for editor uses in editor
    /// </summary>
    [Serializable]
    public class ItemForEditor
    {
        public int Color;
        public ItemsTypes ItemType;
        public Texture2D Texture;
        public IColorableComponent colors;
        public IItemInterface ItemInterface;
        public GameObject Item;
        public bool EnableMarmaladeTargets;
        public Vector2Int[] TargetMarmaladePositions;

        public ItemForEditor DeepCopy()
        {
            var other = (ItemForEditor)MemberwiseClone();
            return other;
        }

        public void SetColor(int color, int currentLevel)
        {
            Color = color;
            if(colors && colors.GetSprites(currentLevel).Count() > color)
                Texture = colors.GetSprites(currentLevel)[color].texture;
        }
    }
}