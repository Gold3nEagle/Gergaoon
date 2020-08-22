using System.Linq;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.System;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEngine;

namespace SweetSugar.Scripts.TargetScripts
{
    /// <summary>
    /// Jelly block target
    /// </summary>
    public class JellyBlock : Target
    {
        /// <summary>
        /// get jelly blocks
        /// </summary>
        /// <returns></returns>
        public override int CountTarget()
        {
//        return LevelManager.THIS.fieldBoards.SelectMany(i => i.squaresArray).WhereNotNull().Count(i => i.type == SquareTypes.JellyBlock);
            return LevelManager.THIS.fieldBoards.SelectMany(i => i.squaresArray).WhereNotNull().SelectMany(i => i.subSquares).Count(i => i.type == SquareTypes.JellyBlock);
        }

        public override void InitTarget()
        {
            description = "Spread the jelly everywhere";
            subTargetContainers[0].count = LevelData.THIS.fields.Sum(x => x.levelSquares.WhereNotNull().Count(i => i.block == SquareTypes.JellyBlock));

            // Debug.Log(description);
        }

        public override int CountTargetSublevel()
        {
            var count = 0;
            var field = LevelManager.THIS.field;
            count += field.CountSquaresByType(GetType().Name.ToString());
            return count;
        }

        public override int GetDestinationCountSublevel()
        {
            return LevelManager.THIS.field.squaresArray.WhereNotNull().Count(i => i.type != SquareTypes.NONE && !i.undestroyable);
        }

        /// <summary>
        /// Total squares
        /// </summary>
        /// <returns></returns>
        public override int GetDestinationCount()
        {
            return LevelManager.THIS.fieldBoards.SelectMany(i => i.squaresArray).WhereNotNull().Count(i => i.type != SquareTypes.NONE && !i.undestroyable);
        }

        public override void FulfillTargets<T>(T[] _items)
        {
            if (_items.TryGetElement(0)?.GetType() != typeof(Square)) return;
            if (!(_items is Square[] squares)) return;
            Square[] squares1 = squares.Where(i =>i!=null && i.type != SquareTypes.NONE && !i.undestroyable).ToArray();
            if (squares1.Select(i => i.type).Any(i => i.ToString() == GetType().Name))
            {
                foreach (var item in squares1)
                {
                    if (item != null) item.SetType(SquareTypes.JellyBlock, 1, SquareTypes.NONE, 1);
                }
            }
        }

        public override void FulfillTarget<T>(T[] _items)
        {
            if (_items.TryGetElement(0)?.GetType() != typeof(Square)) return;
            var items = _items as Square[];
            if (items?.Select(i => i.type)?.Where(i => i.ToString() == GetType().Name)?.Count() > 0)
            {
                foreach (var item in items)
                {
                    item.SetType(SquareTypes.JellyBlock, 1, SquareTypes.NONE, 1);
                }
            }
        }

        public override void DestroyEvent(GameObject obj)
        {
            // throw new NotImplementedException();
        }

        public override int GetCount(string spriteName)
        {
            return GetDestinationCount() - CountTarget();
        }

        public override bool IsTotalTargetReached()
        {
            return CountTarget() >= GetDestinationCount();
        }

        public override bool IsTargetReachedSublevel()
        {
            return CountTargetSublevel() >= GetDestinationCountSublevel();
        }
    }
}