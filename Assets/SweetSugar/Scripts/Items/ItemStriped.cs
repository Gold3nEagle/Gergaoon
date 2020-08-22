using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.System;
using UnityEngine;

namespace SweetSugar.Scripts.Items
{
    /// <summary>
    /// Striped item
    /// </summary>
    public class ItemStriped : Item, IItemInterface
    {
        private Item itemMain;
        // public bool Combinable;
        public bool ActivateByExplosion;
        public bool StaticOnStart;

        private Item GetItem
        {
            get { return GetComponentInParent<Item>(); }
        }

        public void Destroy(Item item1, Item item2)
        {
            //        if (GetItem.dontDestroyOnThisMove ) return;
            //        GetItem.SetDontDestroyOnMove();
            //        Debug.Log(" pre destroy " + " type " + GetItem.currentType + GetItem.GetInstanceID() + " " + item1?.GetInstanceID() + " : " + item2?.GetInstanceID());
            GetItem.square.DestroyBlock();
            if (GetItem.square.type == SquareTypes.WireBlock)
            {
                GetItem.StopDestroy();
                return;
            }
            var list = new[] { item1, item2 };
            list = list.OrderBy(i => i != GetItem).ToArray();
            item1 = list.First();
            item2 = list.Last();

            itemMain = item1;
            var square = itemMain.square;
            SoundBase.Instance.PlayLimitSound(SoundBase.Instance.strippedExplosion);
            LevelManager.THIS.StripedShow(gameObject, item1.currentType == ItemsTypes.HORIZONTAL_STRIPED);
            var itemsList = GetList(square);
            foreach (var item in itemsList)
            {
                if (item != null)
                {
                    item.DestroyItem(true, GetItem, this);
                }
            }

            var sqL = GetSquaresInRow(square, itemMain.currentType);
            LevelManager.THIS.levelData.GetTargetObject().CheckSquares(sqL.ToArray());
            sqL.Where(i => i.item == null).ToList().ForEach(i => i.DestroyBlock());

            DestroyBehaviour();

        }

        private List<Square> GetSquaresInRow(Square square, ItemsTypes type)
        {
            if (type == ItemsTypes.HORIZONTAL_STRIPED)
                return LevelManager.THIS.GetRowSquare(square.row);
            return LevelManager.THIS.GetColumnSquare(square.col);
        }

        public Item GetParentItem()
        {
            return transform.GetComponentInParent<Item>();
        }

        private List<Item> GetList(Square square)
        {
            if (itemMain.currentType == ItemsTypes.HORIZONTAL_STRIPED)
                return LevelManager.THIS.GetRow(square.row);
            return LevelManager.THIS.GetColumn(square.col);
        }

        public override void Check(Item item1, Item item2)
        {
            CheckStripes(item1, item2);
            if(gameObject.activeSelf)
                StartCoroutine(CheckStripePackage(item1, item2));

            if (item2.currentType == ItemsTypes.MARMALADE || item2.currentType == ItemsTypes.MULTICOLOR)
            {
                item2.Check(item2, item1);
            }
        }

        /// <summary>
        /// Check if package and striped switched
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <returns></returns>
        private IEnumerator CheckStripePackage(Item item1, Item item2)
        {
            Item[] list = { item1, item2 };
            var striped = list.Where(i =>
                    i.currentType == ItemsTypes.HORIZONTAL_STRIPED || i.currentType == ItemsTypes.VERTICAL_STRIPED)
                ?.FirstOrDefault();
            var package = list.Where(i => i.currentType == ItemsTypes.PACKAGE)?.FirstOrDefault();

            if (striped != null && package != null)
            {
                var itemsList = LevelManager.THIS.GetSquaresAroundSquare(package.square);
                var direction = 1;
                itemsList.Add(item1.square);
                List<Square> squares = new List<Square>();
                foreach (var _square in itemsList)
                {
                    if (_square != null)
                    {
                        var list1 = GetSquaresInRow(_square, ItemsTypes.HORIZONTAL_STRIPED);
                        var list2 = GetSquaresInRow(_square, ItemsTypes.VERTICAL_STRIPED);
                        squares = squares.Union(list1.Union(list2).ToList()).ToList();
                        if (direction > 0)
                            LevelManager.THIS.StripedShow(_square.gameObject, true);
                        else
                            LevelManager.THIS.StripedShow(_square.gameObject, false);
                        direction *= -1;
                    }
                }
                LevelManager.THIS.levelData.GetTargetObject().CheckSquares(squares.ToArray());
                var squaresToDestroy = squares.Distinct();
                var destroyingItems = squaresToDestroy.Where(i => i.Item != null ).Select(i => i.Item);
                yield return new WaitForSeconds(0.1f);
                foreach (var item in destroyingItems)
                {
                    item.destroyNext = true;
                    item.DestroyItem(true, this, this);
                }
                squaresToDestroy.Where(i=>i.Item == null /*&& i.IsObstacle()*/).ForEachY(i => i.DestroyBlock());
                SoundBase.Instance.PlayLimitSound( SoundBase.Instance.explosion2 );
                striped.DestroyBehaviour();
                package.DestroyBehaviour();
                LevelManager.THIS.FindMatches();
            }
        }


        /// <summary>
        /// is both striped items switched
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        private void CheckStripes(Item item1, Item item2)
        {
            var list = new[] { item1, item2 };
            var stripes = list.Where(i =>
                i.currentType == ItemsTypes.HORIZONTAL_STRIPED || i.currentType == ItemsTypes.VERTICAL_STRIPED);
            stripes = stripes.OrderBy(i => i == GetItem).ToList();
            if (stripes.Count() < 2) return;
            item1.destroying = true;
            item2.destroying = true;
            item2?.DestroyBehaviour();
            SoundBase.Instance.PlayLimitSound(SoundBase.Instance.strippedExplosion);
            LevelManager.THIS.StripedShow(gameObject, false);
            LevelManager.THIS.StripedShow(gameObject, true);
            var list1 = GetSquaresInRow(GetItem.square, ItemsTypes.HORIZONTAL_STRIPED);
            var list2 = GetSquaresInRow(GetItem.square, ItemsTypes.VERTICAL_STRIPED);
            var lDistinct = list1.Union(list2).Distinct();
            LevelManager.THIS.levelData.GetTargetObject().CheckSquares(lDistinct.ToArray());

            foreach (var square in lDistinct)
            {
                if (square.Item != null ) square.Item.DestroyItem(true,this,this);
                else /*if(square.IsObstacle())*/ square.DestroyBlock();
            }
            item1.square.DestroyBlock();
            item1?.DestroyBehaviour();
        }

        public GameObject GetGameobject()
        {
            return gameObject;
        }

        private IEnumerator Timer(float sec, Action callback)
        {
            yield return new WaitForSeconds(sec);
            callback();
        }

        public bool IsCombinable()
        {
            return Combinable;
        }

        public bool IsExplodable()
        {
            return ActivateByExplosion;
        }

        public void SetExplodable(bool setExplodable)
        {
            ActivateByExplosion = setExplodable;
        }

        public bool IsStaticOnStart()
        {
            return StaticOnStart;
        }

        public void SetOrder(int i)
        {
            GetComponent<SpriteRenderer>().sortingOrder = i;
        }

        public void SecondPartDestroyAnimation(Action callback)
        {
            throw new NotImplementedException();
        }
    }
}