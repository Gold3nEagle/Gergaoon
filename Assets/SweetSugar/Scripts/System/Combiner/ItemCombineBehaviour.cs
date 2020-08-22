using System;
using System.Collections.Generic;
using SweetSugar.Scripts.Items;
using UnityEngine;

namespace SweetSugar.Scripts.System.Combiner
{
    /// <summary>
    /// Item combine editor component
    /// </summary>
    public class ItemCombineBehaviour : ItemMonoBehaviour
    {
        public ItemsTypes itemType;
        public int priority;

        public static int maxCols = 5;
        public static int maxRows = 5;
        [HideInInspector]
        [SerializeField]
        public List<ItemTemplates> matrix = new List<ItemTemplates>();
        public void Init()
        {
            if (matrix.Count == 0)
            {
                Debug.Log("init");
                AddItem();
                for (var col = 0; col < maxCols; col++)
                {
                    for (var row = 0; row < maxRows; row++)
                    {
                        matrix[0].items[row * maxCols + col] = GetItemTemplate(col, row, matrix[0].items);
                    }
                }
            }
        }

        public void AddItem()
        {
            // ItemTemplate[] items = new ItemTemplate[maxCols * maxRows];
            // items = FillMatrix(items);
            matrix.Add(new ItemTemplates());
        }

        public void RemoveItem()
        {
            matrix.RemoveAt(matrix.Count - 1);
        }

        int GetPositionArray(int col, int row)
        {
            return row * maxCols + col;
        }

        public ItemTemplate GetItemTemplate(int col, int row, ItemTemplate[] currentMatrix)
        {
            var itemTemplate = currentMatrix[GetPositionArray(col, row)];
            if (itemTemplate != null)
                itemTemplate.position = new Vector2(col, row);
            else itemTemplate = new ItemTemplate(new Vector2(col, row), false);
            return itemTemplate;
        }

        [Serializable]
        public class ItemTemplates
        {
            public ItemTemplate[] items = new ItemTemplate[maxCols * maxRows];
        }

    }
}