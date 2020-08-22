using System;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Items;
using UnityEngine;

namespace SweetSugar.Scripts.TargetScripts.TargetSystem
{
    /// <summary>
    /// Target object
    /// </summary>
    [Serializable]
    public abstract class Target
    {
        public int amount;
        public int destAmount;
        public SubTargetContainer[] subTargetContainers;

        public string description;

        public abstract int GetDestinationCount();
        public abstract int GetDestinationCountSublevel();

        public abstract void InitTarget();

        public abstract int CountTarget();
        public abstract int GetCount(string spriteName);

        public abstract int CountTargetSublevel();

        public abstract void FulfillTarget<T>(T[] items);

        public abstract void DestroyEvent(GameObject obj);

        public Target()
        {
            TargetComponent.OnDestroyEvent += DestroyEvent;
        }

        public abstract bool IsTargetReachedSublevel();
        public abstract bool IsTotalTargetReached();

        public void CheckItems(Item[] items)
        {
            FulfillTarget(items);
        }

        public void CheckSquare(Square[] squares)
        {
            FulfillTarget(squares);
        }

        public void CheckSquares(Square[] squares)
        {
            FulfillTargets(squares);
        }

        public virtual void FulfillTargets<T>(T[] items)
        {

        }

        public virtual void CheckTargetItemsAfterDestroy() { }

        public virtual bool IsIngredientRequire() { return false; }

        public Target DeepCopy()
        {
            var other = (Target)MemberwiseClone();

            return other;
        }

    }
}