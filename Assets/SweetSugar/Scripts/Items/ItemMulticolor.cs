using System;
using System.Collections;
using System.Linq;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Effects;
using SweetSugar.Scripts.System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SweetSugar.Scripts.Items
{
    /// <summary>
    /// Item multicolor
    /// </summary>
    public class ItemMulticolor : Item, IItemInterface
    {
        // public bool Combinable;
        public bool ActivateByExplosion;
        public bool StaticOnStart;

        public GameObject LightningPrefab;
        private bool jellySpread;

        public override void Check(Item item1, Item item2)
        {
            if (item1?.square?.type == SquareTypes.JellyBlock || item2?.square?.type == SquareTypes.JellyBlock)
                jellySpread = true;
            if (item2 != null && ((!item2.Combinable && item2.currentType != ItemsTypes.MULTICOLOR)))
                return;
            GetParentItem().destroying = true;
            if (item2.currentType == ItemsTypes.NONE)
            {
                DestroyColor(item2.color);
                item2.DestroyItem();
            }
            else if (item2.currentType == ItemsTypes.HORIZONTAL_STRIPED || item2.currentType == ItemsTypes.VERTICAL_STRIPED)
                LevelManager.THIS.StartCoroutine(SetTypeByColor(item2));
            else if (item2.currentType == ItemsTypes.PACKAGE)
                LevelManager.THIS.StartCoroutine(SetTypeByColor(item2));
            else if (item2.currentType == ItemsTypes.MARMALADE)
                LevelManager.THIS.StartCoroutine(SetTypeByColor(item2));
            else if (item2.currentType == ItemsTypes.MULTICOLOR)
            {
                item2.SmoothDestroy();
                DestroyDoubleMulticolor(item1.square.col, () =>
                {
                    var list = new[] { item1, item2 };
                    list.First(i => i != GetParentItem()).SmoothDestroy();
                    list.First(i => i == GetParentItem()).SmoothDestroy();
//                LevelManager.THIS.DragBlocked = false;
                });
//            item1.HideSprites(true);
//            item2.HideSprites(true);
            }
        }

        public void Destroy(Item item1, Item item2)
        {
            if (item2 == null)
            {
                if (GetParentItem().square.type == SquareTypes.WireBlock)
                {
                    GetParentItem().square.DestroyBlock();
                }

                if (LevelManager.GetGameStatus() == GameState.PreWinAnimations)
                {
                    item2 = LevelManager.THIS.field.squaresArray.First(i => i.item != null && i.item.currentType == ItemsTypes.NONE).item;
                    Check(item1, item2);
                }
                // GetParentItem().StopDestroy();
                if(explodedItem) DestroyColor(explodedItem.color);
                else DestroyColor(Random.Range(0,LevelManager.THIS.levelData.colorLimit-1));

                return;
            }
            item1?.SmoothDestroy();
        }



        #region ChangeItemTypes

        private IEnumerator SetTypeByColor(Item item2)
        {
            //		SetTypeByColor(item2.color, item2.currentType);
            var items = LevelManager.THIS.field.GetItemsByColor(item2.color).Where(i => !i.Equals(GetParentItem()) && i.currentType == ItemsTypes.NONE).ToArray();
            var nextType = item2.currentType;
            bool loopFinished = false;
            GameObject itemMarmaladeTarget = null;
            itemMarmaladeTarget = new GameObject();
            item2.DestroyItem();
            StartCoroutine(IterateItems(items, item =>
            {
                if (nextType == ItemsTypes.HORIZONTAL_STRIPED || nextType == ItemsTypes.VERTICAL_STRIPED)
                    item.GetComponent<Item>().NextType = (ItemsTypes)Random.Range(4, 6);
                else
                    item.GetComponent<Item>().NextType = nextType;
                item.marmaladeTarget = itemMarmaladeTarget;
                item.GetComponent<Item>().ChangeType(null, false);
//            item.Explodable = false;
                CreateLightning(transform.position, item.transform.position);
            }, () => { loopFinished = true;  Destroy(itemMarmaladeTarget);}));
            yield return new WaitUntil(() => loopFinished);
            if(item2.currentType != ItemsTypes.MARMALADE)
                DestroyColor(item2.color);
            else
            {
                LevelManager.THIS.FindMatches();
                SmoothDestroy();
            }
            //		var list = LevelManager.This.field.GetItems().Where(i => i.currentType == nextType).ToList();

            //		yield return new WaitWhileDestroyPipeline(list, new Delays() { afterevery = new WaitForSecCustom() { s = 0.1f } });
        }

        #endregion

        private void DestroyColor(int p)
        {
            SoundBase.Instance.PlayOneShot(SoundBase.Instance.colorBombExpl);

            var items = LevelManager.THIS.field.GetItemsByColor(p).Where(i => !i.Equals(GetParentItem())).ToArray();
            StartCoroutine(IterateItems(items, item =>
            {
                CreateLightning(transform.position, item.transform.position);
                item.DestroyItem(true, true, this, true);
            }, () =>
            {
                LevelManager.THIS.FindMatches();
                SmoothDestroy();

            }));
//        HideSprites(true);

        }

        private IEnumerator IterateItems(Item[] items, Action<Item> iterateItem, Action onFinished = null)
        {
            var i = 0;
            foreach (var item in items)
            {
                if (item != null && item.gameObject.activeSelf)
                {
                    i++;
                    if (jellySpread)
                        item.square?.SetType(SquareTypes.JellyBlock, 1, SquareTypes.NONE, 1);
                    iterateItem(item); 
                    if (i % (Mathf.Clamp(items.Length,items.Length,2)) == 0)
                        yield return new WaitForSeconds(0.2f);
                }
            }
            if (onFinished != null)
                yield return new WaitForSeconds(0.2f);
            onFinished();

        }

        private void CreateLightning(Vector3 pos1, Vector3 pos2)
        {
            var go = Instantiate(LightningPrefab, Vector3.zero, Quaternion.identity);
            var lightning = go.GetComponent<Lightning>();
            lightning.SetLight(pos1, pos2);
        }

        #region DoubleMulitcolor
        public void DestroyDoubleMulticolor(int col, Action callback)
        {
            LevelManager.THIS.field.GetItems();
            StartCoroutine(DestroyDoubleBombCor(col, () => { callback(); }));
        }

        private IEnumerator DestroyDoubleBombCor(int col, Action callback)
        {
            for (var i = 0; i < LevelManager.THIS.field.fieldData.maxCols; i++)
            {
            
                var list = LevelManager.THIS.GetColumn(i).Where(a => !a.destroying ).ToList();
                foreach (var a in list)
                {
                    a.globalExplEffect = true;
                    if (a.currentType == ItemsTypes.MARMALADE) a.GetComponent<ItemMarmalade>().noMarmaladeLaunch = true;
                    CreateLightning(transform.position, a.transform.position);
                    if(jellySpread)
                        a.square?.SetType(SquareTypes.JellyBlock, 1, SquareTypes.NONE, 1);
                }

                LevelManager.THIS.levelData.GetTargetObject().CheckItems(list.ToArray());
                yield return new WaitWhileDestroyPipeline(list, new Delays());
                yield return new WaitForSeconds(0.1f);
            }
            callback();
        }

        #endregion

        public Item GetParentItem()
        {
            return transform.GetComponentInParent<Item>();
        }

        public GameObject GetGameobject()
        {
            return gameObject;
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


    }
}
