using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.System;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEngine;

namespace SweetSugar.Scripts.TargetScripts
{
    /// <summary>
    /// collect items target
    /// </summary>
    public class CollectItems : Target
    {
        public override int CountTarget()
        {
            return amount;
        }

        public override int CountTargetSublevel()
        {
            return amount;
        }

        public override void InitTarget()
        {
            foreach (var item in subTargetContainers)
            {
                amount += item.GetCount();
            }

        }

        public override void DestroyEvent(GameObject obj)
        {


        }

        public override void FulfillTarget<T>(T[] _items)
        {
            if (_items.Length>0 && _items[0].GetType().BaseType != typeof(Item)) return;
            var items = _items as Item[];
            foreach (var item in subTargetContainers)
            {
                foreach (var obj in items)
                {
                    if (obj == null) continue;
                    var color = obj.GetComponent<IColorableComponent>().color;
                    if (item.color == color && item.preCount > 0)
                    {
                        amount--;
                        item.preCount--;
                        var pos = TargetGUI.GetTargetGUIPosition(color);
                        var itemAnim = new GameObject();
                        var animComp = itemAnim.AddComponent<AnimateItems>();
                        animComp.InitAnimation(obj.gameObject, pos, obj.transform.localScale, () => { item.changeCount(-1); });
                    }
                }
            }
        }

        public override int GetDestinationCount()
        {
            return destAmount;
        }

        public override int GetDestinationCountSublevel()
        {
            return destAmount;
        }

        public override bool IsTargetReachedSublevel()
        {
            return amount <= 0;
        }

        public override bool IsTotalTargetReached()
        {
            return amount <= 0;
        }

        public override int GetCount(string spriteName)
        {
            for (var index = 0; index < subTargetContainers.Length; index++)
            {
                var item = subTargetContainers[index];
                if (item.extraObject.name == spriteName)
                    return item.GetCount();
            }

            return CountTarget();
        }
    }
}