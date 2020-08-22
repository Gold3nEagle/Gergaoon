using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.Level;
using UnityEngine;

namespace SweetSugar.Scripts.System.Combiner
{
    /// <summary>
    /// Bonus combine prediction for tutorial
    /// </summary>
    public class BonusItemPrediction
    {

        public static ItemTemplate[] strippedCombine = new ItemTemplate[25];

        public static List<Item> IsItemPredicted(ItemsTypes itemType)
        {
            Debug.Log("prediction bonus..");
            List<Item> predicted;
            var field = LevelManager.THIS.field;
            predicted = PridictCombines(field, true, itemType);
            if (!predicted.Any())
                predicted = PridictCombines(field, false, itemType);

            return predicted;
        }

        private static List<Item> PridictCombines(FieldBoard field, bool right, ItemsTypes itemType)
        {
            var combineManager = LevelManager.THIS.CombineManager;

            for (var i = 0; i < field.squaresArray.Count(); i++)
            {
                var item = field.squaresArray[i].Item;
                Item item1 = null;
                if (right)
                    item1 = (item?.square?.GetNeighborRight())?.Item;
                else
                    item1 = (item?.square?.GetNeighborBottom())?.Item;

                if (item1 != null && !item.destroying && !item1.destroying)
                {
                    var color = item.color;
                    var color1 = item1.color;

                    item.color = color1;
                    item1.color = color;

                    var combines = combineManager.GetCombines(field, itemType);
                    item.color = color;
                    item1.color = color1;

                    var combine = combines.Find(x => GetConditionByType(x, itemType));
                    if (combine != null)
                    {
                        if (item.color == combine.color)
                        {
                            AI.THIS.TipItem = item;
                            AI.THIS.vDirection = (item1.transform.position - item.transform.position).normalized;
                        }
                        else
                        {
                            AI.THIS.TipItem = item1;
                            AI.THIS.vDirection = (item.transform.position - item1.transform.position).normalized;
                        }
                        combine.items.Add(item);
                        combine.items.Add(item1);
                        return combine.items;
                    }

                }
            }

            return new List<Item>();

        }

        private static bool GetConditionByType(Combine x, ItemsTypes itemType)
        {
            if (itemType == ItemsTypes.HORIZONTAL_STRIPED || itemType == ItemsTypes.VERTICAL_STRIPED)
                return x.nextType == ItemsTypes.HORIZONTAL_STRIPED || x.nextType == ItemsTypes.VERTICAL_STRIPED;
            return x.nextType == itemType;
        }

    }
}