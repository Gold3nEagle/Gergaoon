using System;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.Level;
using UnityEngine;

namespace SweetSugar.Scripts.System.Combiner
{
    /// <summary>
    /// Bonus item combiner
    /// </summary>
    public class BonusItemCombiner
    {
        static int maxCols = 5;
        static int maxRows = 5;
        // public static ItemTemplate[,] strippedCombine = new ItemTemplate[maxCols, maxRows];
        public static ItemTemplate[] strippedCombine = new ItemTemplate[maxCols * maxRows];
        List<TypeMatrix> bonusCombinesPatterns = new List<TypeMatrix>();
        private DebugSettings _debugSettings;
        private int bonusCombineListCount;

        public BonusItemCombiner()
        {
            _debugSettings = Resources.Load("Scriptable/DebugSettings") as DebugSettings;
            FillPatterns();
        }

        private void FillPatterns()
        {
            bonusCombinesPatterns.Clear();
            var bonusCombineList = Resources.FindObjectsOfTypeAll(typeof(ItemCombineBehaviour)) as ItemCombineBehaviour[];
            bonusCombineList = bonusCombineList.Where(i => i.matrix.All(x => x.items.Any(y => y.item))).ToArray();
            bonusCombineListCount = bonusCombineList.Length;
            var groupCombine = bonusCombineList.GroupBy(i => new { i.priority, i.itemType }).OrderBy(i => i.Key.priority).Select(i => new { ItemType = i.Key, Matrix = i.Select(x => x.matrix).First() }).ToArray();
            foreach (var item in groupCombine)
            {
                foreach (var matrix in item.Matrix)
                {
                    var typeMatrix = new TypeMatrix();
                    var matricesPatterns = RotatePatternMatrices(matrix.items, item.ItemType.itemType);

                    for (var i = 0; i < matricesPatterns.Count(); i++)
                    {
                        var matrixPattern = matricesPatterns[i];
                        matrixPattern = SimplifyArray(matrixPattern.matrix.ToArray());
                        matrixPattern.nodeItem = GetBaseNode(matrixPattern.matrix.ToArray());
                        matricesPatterns[i] = matrixPattern;
                        if(item.ItemType.ToString().Contains("MARMALADE"))
                        {
                            Show2DArray(matrixPattern.matrix.ToArray(), "rotated");
                        
                        }
                    }
                    typeMatrix.itemType = item.ItemType.itemType;
                    typeMatrix.matrices = matricesPatterns;
                    bonusCombinesPatterns.Add(typeMatrix);
                }

            }

            bonusCombineListCount = bonusCombinesPatterns.Count;
        }

        int GetPositionArray(int col, int row)
        {
            return row * maxCols + col;
        }

        public ItemsTypes GetBonusCombine(Combine combine, out ItemTemplate[] foundItems, ItemsTypes prioritiseItem = ItemsTypes.NONE)
        {
            if (bonusCombinesPatterns.Count < bonusCombineListCount) FillPatterns();
            foundItems = null;
            var matrix = ConvertCombine(combine);
            var compare = SimplifyArray(matrix).matrix.ToArray();
            Show2DArray(compare.ToArray(), "compare origin");

            var baseNodePosition = GetBaseNode(compare);

            var types = new List<ItemsTypes>();

            foreach (var typePattern in bonusCombinesPatterns)
            {
                foreach (var m in typePattern.matrices)
                {
                    var pattern = m.DeepCopy().matrix;
                    if (pattern.Any(i => i.position.x > 0 && i.position.y > 0))
                    {
                        var offset = baseNodePosition - m.nodeItem;
                        pattern = pattern.ForEachY(i => i.position += offset).ToList();
                    }
                    Show2DArray(pattern.ToArray(), "pattern ");
                    if (!IsCombineFound(compare, pattern.ToArray(), typePattern, ref types, out foundItems))
                        continue;
                    Show2DArray(compare.ToArray(), "compare found");
                    Show2DArray(pattern.ToArray(), "pattern found");

                    ItemsTypes itemsTypes = GetCombineType(prioritiseItem, types);
                    if (_debugSettings.BonusCombinesShowLog) DebugLogKeeper.Log(itemsTypes + " detected", DebugLogKeeper.LogType.BonusAppearance);
                    return itemsTypes;
                }
            }
            var type = GetCombineType(prioritiseItem, types);

            // Debug.Log(type + " detected");
            return type;
        }

        private static Vector2 GetBaseNode(ItemTemplate[] compare)
        {
            var list1 = compare.GroupBy(i => i.position.y);
            var compareRows = new List<IEnumerable<ItemTemplate>>();
            foreach (var group in list1)
            {
                var emptySquares = group.Where(i => !i.item);
                var itemTemplates = group.Where(i => i.item);
                if(itemTemplates.Count()==0) continue;
                var max = itemTemplates.Max(i => i.position.x);
                var min = itemTemplates.Min(i => i.position.x);
                if (!emptySquares.Any(i => i.position.x < max && i.position.x > min) || !emptySquares.Any())
                    compareRows.Add(itemTemplates);
            }
            var list2 = compare.GroupBy(i => i.position.x);
            var compareCols = new List<IEnumerable<ItemTemplate>>();
            foreach (var group in list2)
            {
                var emptySquares = group.Where(i => !i.item);
                if(group.Any(i => i.item))
                {
                    var max = group.Where(i => i.item).Max(i => i.position.y);
                    var min = group.Where(i => i.item).Min(i => i.position.y);
                    if (!emptySquares.Any(i => i.position.y < max && i.position.y > min) || !emptySquares.Any())
                        compareCols.Add(group.Where(i => i.item));
                }
            }

            return compareRows.Concat(compareCols)
                .Select(gr => gr.Select(i => new { Item = i, Count = gr.Count() }))
                .SelectMany(i => i)
                .GroupBy(i => i.Item).Select(i => new { Item = i.Key, Sum = i.Sum(x => x.Count) })
                .OrderByDescending(i => i.Sum).First().Item.position;
        }

        private static ItemsTypes GetCombineType(ItemsTypes prioritiseItem, List<ItemsTypes> types)
        {
            var type = ItemsTypes.NONE;

            if (!types.Any()) return type;
            if (prioritiseItem == ItemsTypes.NONE)
                type = types.Select(i => new { Type = i, Num = (int)i }).OrderByDescending(i => i.Num).First()?.Type ??
                       ItemsTypes.NONE; //ordering types
            else if (types.Any(i => i == prioritiseItem))
                type = types.First(i => i == prioritiseItem);
            return type;
        }

        private bool IsCombineFound(ItemTemplate[] compare, ItemTemplate[] pattern, TypeMatrix typePattern, ref List<ItemsTypes> types, out ItemTemplate[] foundItems)
        {
            var found = CompareMatrix(compare, pattern, out foundItems);

            if (!found) return false;
            types.Add(typePattern.itemType);

            return true;
        }

        List<Matrix> RotatePatternMatrices(ItemTemplate[] matrix, ItemsTypes type)
        {
            var list = new List<Matrix>();
            var pivot = new Vector2Int(maxCols / 2, maxRows / 2);
            if (type != ItemsTypes.HORIZONTAL_STRIPED && type != ItemsTypes.VERTICAL_STRIPED)
            {
                for (var angle = 0; angle < 360; angle += 90)
                {
                    list.Add(RotateMatrix(matrix, pivot, angle));
                }
            }
            else
            {
                list.Add(RotateMatrix(matrix, pivot, 0));
                list.Add(RotateMatrix(matrix, pivot, 180));
            }
            return list;
        }

        private Matrix RotateMatrix(ItemTemplate[] matrix, Vector2 pivot, float angle)
        {
            var m = new Matrix();
            var l2 = new List<ItemTemplate>();
            foreach (var itemTemplate in matrix)
            {
                var item = itemTemplate.DeepCopy();
                var point = item.position;
                var dir = pivot - point;
                dir = Quaternion.Euler(0, 0, angle) * dir;
                point = pivot + dir;
                item.position = point;
                l2.Add(item);
            }

            m.matrix = l2;
            return m;
        }

        public class MyStruct
        {
            public List<ItemTemplate> items = new List<ItemTemplate>();
        }

        bool CompareMatrix(ItemTemplate[] compare, ItemTemplate[] pattern, out ItemTemplate[] foundItems)
        {
            foundItems = null;
            var found = false;
            if (!pattern.Any(i => i.position.x > 0 && i.position.y > 0))
            {
                var compareXmatches = GetLinearMatrix(compare, true);
                var compareYmatches = GetLinearMatrix(compare, false);
                if (compareXmatches.Length >= pattern.GroupBy(i => i.position.x).Count() && pattern.Length == pattern.GroupBy(i => i.position.x).Count())
                {
                    found = true;
                    foundItems = compareXmatches;
                }
                if (compareYmatches.Length >= pattern.GroupBy(i => i.position.y).Count() && pattern.Length == pattern.GroupBy(i => i.position.y).Count())
                {
                    found = true;
                    foundItems = compareYmatches;
                }
            }
            else
            {
                found = pattern.Where(i => i.item).All(i => compare.Contains(i, new ElementComparer()));
                if (found)
                    foundItems = compare.Where(i => i.item).Where(i => pattern.Contains(i, new ElementComparer())).ToArray();
                else
                {
                    foreach (var itemTemplate in compare)
                    {
                        var offset = itemTemplate.position - pattern.First().position;
                        var newpattern = pattern.ForEachY(i => i.position += offset).ToList();
                        found = newpattern.Where(i => i.item).All(i => compare.Contains(i, new ElementComparer()));
                        if (!found) continue;
                        foundItems = compare.Where(i => i.item).Where(i => pattern.Contains(i, new ElementComparer())).ToArray();
                        break;
                    }
                }
            }

            return found;
        }

        private static ItemTemplate[] GetLinearMatrix(ItemTemplate[] pattern, bool axisX)
        {
            var xBombines = new List<MyStruct>();
            var list1 = pattern.Where(i => i.item).GroupBy(i => GetAxisValue(i.position, !axisX));
            foreach (var group in list1)
            {
                var gr = group.ToArray();
                var comb = xBombines.Addd(new MyStruct());

                for (int i = 0; i < gr.Length; i++)
                {
                    var item = gr[i];
                    if (GetAxisValue(item.position, axisX) -
                        GetAxisValue(gr.ElementAtOrDefault(i - 1, new ItemTemplate(item.position, true)).position, axisX) <= 1) comb.items.Add(item);
                    else
                    {
                        comb = xBombines.Addd(new MyStruct());
                        comb.items.Add(item);
                    }
                }
            }
            return xBombines.OrderByDescending(i => i.items.Count()).First().items.ToArray();
        }

        static float GetAxisValue(Vector2 pos, bool axisX)
        {
            if (axisX)
                return pos.x;
            return pos.y;
        }


        public class ElementComparer : IEqualityComparer<ItemTemplate>
        {
            public bool Equals(ItemTemplate x, ItemTemplate y)
            {
                return x.item == y.item && x.position == y.position;
            }

            public int GetHashCode(ItemTemplate product)
            {
                return product.position.GetHashCode();
            }
        }

        Matrix SimplifyArray(ItemTemplate[] array)
        {
            var matrix = new Matrix();
            var leftTopPos = GetMinPosition(array);
            var rightBottomPos = GetMaxPosition(array) + Vector2Int.one;
            var items = new ItemTemplate[(rightBottomPos.x - (leftTopPos.x)) * (rightBottomPos.y - (leftTopPos.y))];
            for (var col = leftTopPos.x; col < rightBottomPos.x; col++)
            {
                for (var row = leftTopPos.y; row < rightBottomPos.y; row++)
                {
                    var y = row - leftTopPos.y;
                    var x = col - leftTopPos.x;
                    var maxX = rightBottomPos.x - leftTopPos.x;
                    var item = array.First(i => i.position == new Vector2(col, row)).DeepCopy();
                    item.position = new Vector2(x, y);
                    items[y * maxX + x] = item;
                }
            }

            matrix.matrix = items.ToList();
            return matrix;
        }

        private Vector2Int GetMaxPosition(ItemTemplate[] array)
        {
            return new Vector2Int(Convert.ToInt32(array.Where(i => i.item).Max(i => i.position.x)), Convert.ToInt32(array.Where(i => i.item).Max(i => i.position.y)));
        }

        private Vector2Int GetMinPosition(ItemTemplate[] array)
        {
            return new Vector2Int(Convert.ToInt32(array.Where(i => i.item).Min(i => i.position.x)), Convert.ToInt32(array.Where(i => i.item).Min(i => i.position.y)));
        }

        ItemTemplate[] ConvertCombine(Combine combine)
        {
            var leftTopPos = new Vector2(combine.items.Min(i => i.square.col), combine.items.Min(i => i.square.row));

            var matrix = new ItemTemplate[maxCols * maxRows];

            for (var col = 0; col < maxCols; col++)
            {
                for (var row = 0; row < maxRows; row++)
                {
                    matrix[GetPositionArray(col, row)] = GetNewItemTemplate(col, row, false);
                    var item = combine.items.Find(i => (i.square.GetPosition() - leftTopPos) == new Vector2(col, row));
                    if (item != null)
                    {
                        matrix[GetPositionArray(col, row)] = GetNewItemTemplate(col, row, true);
                        matrix[GetPositionArray(col, row)].itemRef = item;
                    }
                }
            }

            return matrix;
        }

        private ItemTemplate GetNewItemTemplate(int col, int row, bool empty)
        {
            return new ItemTemplate(new Vector2(col, row), empty);
        }

        void Show2DArray(ItemTemplate[] array, string name = "", bool showNow = false)
        {
            if (!_debugSettings.BonusCombinesShowLog && !showNow) return;
            var r = "array " + name + "\n";
            var maxR = array.Max(i => Mathf.RoundToInt(i.position.y)) + 1;
            var maxC = array.Max(i => Mathf.RoundToInt(i.position.x)) + 1;

            for (var row = 0; row < maxR; row++)
            {
                for (var col = 0; col < maxC; col++)
                {
                    r += GetItemBool(array, row, col) + ",";
                }
                r += "\n";
            }
            if(!showNow)
                DebugLogKeeper.Log(r, DebugLogKeeper.LogType.BonusAppearance);
            else
                Debug.Log(r);
        }

        private string GetItemBool(ItemTemplate[] array, int row, int col)
        {
            return array.Any(i => i.position == new Vector2(col, row)) && array.First(i => i.position == new Vector2(col, row)).item ? "x" : "0";
        }

        [Serializable]
        class TypeMatrix
        {
            public ItemsTypes itemType;
            public List<Matrix> matrices;
        }

        class Matrix
        {
            public List<ItemTemplate> matrix;
            public Vector2 nodeItem;

            public Matrix DeepCopy()
            {
                var other = (Matrix)MemberwiseClone();
                other.matrix = new List<ItemTemplate>();
                matrix.ForEachY(i => other.matrix.Add(i.DeepCopy()));
                return other;
            }

        }
    
        public List<Combine> FindBonusCombine(FieldBoard field, ItemsTypes itemsTypes=ItemsTypes.NONE)
        {
            List<Combine> combines = new List<Combine>();
            var combinesPatterns = bonusCombinesPatterns;
            if (itemsTypes != ItemsTypes.NONE) combinesPatterns = bonusCombinesPatterns.Where(i => i.itemType == itemsTypes).ToList();
            foreach (var typePattern in combinesPatterns)
            {
                foreach (var pattern in typePattern.matrices)
                {
                    var rightBottomPosPattern = GetMaxPosition(pattern.matrix.ToArray()) + Vector2Int.one;
                    var segments = SplitFieldSegments(field, rightBottomPosPattern);
                    foreach (var segment in segments)
                    {
                        var matrix = ConvertCombine(segment);
                        var compare = SimplifyArray(matrix);
                        var types = new List<ItemsTypes>();
                        ItemTemplate[] foundItems;
                        if (!IsCombineFound(compare.matrix.ToArray(),pattern.matrix.ToArray(), typePattern, ref types, out foundItems))
                            continue;
                        else
                        {
                            segment.nextType = types.FirstOrDefault();
                            combines.Add(segment);
                            return combines;
                        }
                    
//                    if(GetBonusCombine(segment)!= ItemsTypes.NONE)
//                        combines.Add(segment);
                    }
                }
            }
            return combines;
        }

        /// <summary>
        /// Split field to appropriate rect fragments
        /// </summary>
        /// <param name="field"></param>
        /// <param name="rightBottomPosPattern"></param>
        /// <returns></returns>
        private static List<Combine> SplitFieldSegments(FieldBoard field, Vector2Int rightBottomPosPattern)
        {
            List<Combine> segments = new List<Combine>();
            for (var row = 0; row < field.fieldData.maxRows; row++)
            {
                for (var col = 0; col < field.fieldData.maxCols; col++)
                {
                    var items = field
                        .GetFieldSeqment(new RectInt(col, row, col + rightBottomPosPattern.x, row + rightBottomPosPattern.y))
                        .WhereNotNull().Select(i => i.Item).WhereNotNull().ToList();
                    var splitByColors = items.GroupBy(i => i.color);
                    foreach (var color in splitByColors)
                    {
                        var combine = new Combine {items = color.ToList(), color = color.Key};
                        segments.Add(combine);
                    }
                    col += rightBottomPosPattern.x;
                }

                row += rightBottomPosPattern.y;
            }
            return segments;
        }
    }
}