using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Effects;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.System;
using SweetSugar.Scripts.System.Pool;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEngine;

namespace SweetSugar.Scripts.Blocks
{
    public enum FindSeparating
    {
        NONE = 0,
        HORIZONTAL,
        VERTICAL
    }

//TODO: Add new target

    public enum SquareTypes
    {
        NONE = 0,
        EmptySquare,
        SugarSquare,
        WireBlock,
        SolidBlock,
        ThrivingBlock,
        JellyBlock,
        SpiralBlock,
        UndestrBlock
    }
    /// <summary>
    /// Handles square behaviour like destroying, generating item if the square empty, determines items directions, and obstacles behaviour
    /// </summary>
    public class Square : MonoBehaviour, ISpriteRenderer, IField, IMarmaladeTargetable
    {
        [Header("Score for destroying")]
        public int score;
        /// Item occupies this square
        public Item item;
        public Item Item
        {
            get { return item; }
            set
            {
                if (value == null)
                {
                    if (LevelManager.THIS.DebugSettings.FallingLog) DebugLogKeeper.Log(item + " set square " + name + " empty ", DebugLogKeeper.LogType.Falling);
                }
                else
                {
                    if (LevelManager.THIS.DebugSettings.FallingLog) DebugLogKeeper.Log(value + " set square " + name, DebugLogKeeper.LogType.Falling);
                }
                item = value;

                if (item == null && isEnterPoint && LevelManager.GetGameStatus() != GameState.RegenLevel) StartCoroutine(CheckNullItem());
            }
        }
        /// position on the field
        public int row;
        public int col;
        /// sqaure type
        public SquareTypes type;
        /// sprite of border
        private Sprite borderSprite;

        [Header("can item move inside the square")]
        public bool canMoveIn;
        [Header("can item fall out of the square")]
        public bool canMoveOut;
        [Header("can be the square destroyed")]
        public bool undestroyable;
        /// EDITOR: direction of items
        [HideInInspector] public Vector2 direction;
        /// EDITOR: true - square is begging of the items flow
        [HideInInspector] public bool isEnterPoint;
        ///EDITOR: enter square for current sequence of the squares, Sequence is array of squares along with direction (all squares by column)
        [HideInInspector] public Square enterSquare;
        /// Next square by direction flow
        [HideInInspector] public Square nextSquare;
        /// teleport destination position
        [HideInInspector] public Vector2Int teleportDestinationCoord;
        /// teleport destionation square
        [HideInInspector] public Square teleportDestination;
        /// teleport started square
        [HideInInspector] public Square teleportOrigin;
        /// current field
        [HideInInspector] public FieldBoard field;
        /// mask for the top squares
        public GameObject mask;
        /// teleport effect gameObject
        public GameObject teleportEffect;
        /// teleport square object
        public TeleportDirector teleport;

        /// subsquares of the current square, obstacle or jelly
        [HideInInspector] public List<Square> subSquares = new List<Square>();
        /// if true - this square has enter square upper by items flow
        [HideInInspector] public bool linkedEnterSquare;
        private void Awake()//init some objects
        {
            GetInstanceID();
            gameObject.AddComponent<ItemDebugInspector>();
            border = Resources.Load<GameObject>("Border");
        }

        // Use this for initialization
        void Start()//init some objects
        {
            name = "Square_" + col + "_" + row;
            if ((LevelData.THIS.target.name == "Ingredients" || direction != Vector2.down) && enterSquare && type != SquareTypes.NONE)
            {
                if (orderInSequence == 0)
                    CreateArrow(isEnterPoint);
                else if (direction != Vector2.down && isEnterPoint)
                    CreateArrow(isEnterPoint);
            }
        }
        /// <summary>
        /// Create animated arrow for bottom row
        /// </summary>
        /// <param name="enterPoint"></param>
        void CreateArrow(bool enterPoint)
        {
            var obj = Instantiate(Resources.Load("Prefabs/Arrow")) as GameObject;
            obj.transform.SetParent(transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero + Vector3.down * 0.5f;
            if(enterPoint)
                obj.transform.localPosition = Vector3.zero - Vector3.down * 2f;
            var angle = Vector3.Angle(Vector2.down, direction);
            angle = Mathf.Sign(Vector3.Cross(Vector2.down, direction).z) < 0 ? (360 - angle) % 360 : angle;
            Vector2 pos = obj.transform.localPosition;
            pos = Quaternion.Euler(0, 0, angle) * pos;
            obj.transform.localPosition = pos;
            obj.transform.rotation = Quaternion.Euler(0, 0, angle);
            ParticleSystem.MainModule mainModule = obj.GetComponent<ParticleSystem>().main;
            mainModule.startRotation = -angle * Mathf.Deg2Rad;
        }
        /// <summary>
        /// Check is the square is empty
        /// </summary>
        /// <returns></returns>
        IEnumerator CheckNullItem()
        {
            while (item == null)
            {
                if (!IsHaveSolidAbove() && !IsHaveItemsAbove() && isEnterPoint && item == null)
                    GenItem();

                else if (IsHaveDestroybleObstacle())
                    break;
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// set mask for top squares
        /// </summary>
        public void SetMask()
        {
            if (isEnterPoint || teleportOrigin)
            {
                var m = Instantiate(mask, transform.position + Vector3.up * (field.squareHeight + 3.05f), Quaternion.identity, transform);
                var angle = Vector3.Angle(Vector2.down, direction);
                angle = Mathf.Sign(Vector3.Cross(Vector2.down, direction).z) < 0 ? (360 - angle) % 360 : angle;
                Vector2 pos = m.transform.localPosition;
                pos = Quaternion.Euler(0, 0, angle) * pos;
                m.transform.localPosition = pos;
                m.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

        }

        /// <summary>
        /// Generate new item
        /// </summary>
        /// <returns>The item.</returns>
        /// <param name="falling">If set to <c>true</c> prepare for falling animation.</param>
        public Item GenItem(bool falling = true, ItemsTypes itemType = ItemsTypes.NONE, int color = -1)
        {
            if (IsNone() && !CanGoInto())
                return null;
            GameObject item = null;
            if (itemType == ItemsTypes.NONE)
            {
                if (LevelManager.THIS.collectIngredients && !IsObstacle())
                    item = GetIngredientItem();
                if(!item)
                    item = ObjectPooler.Instance.GetPooledObject("Item");
            }
            else
            {
                item = ObjectPooler.Instance.GetPooledObject(itemType.ToString());
                item?.GetComponent<Item>().anim?.SetTrigger("bonus_appear");
            }
            if (item == null)
            {
                Debug.LogError("there is no " + itemType + " in pool ");
            }
            item.transform.localScale = Vector2.one * 0.42f;
            Item itemComponent = item.GetComponent<Item>();
            itemComponent.square = this;
            // item.GetComponent<IColorableComponent>().RandomizeColor();
            IColorableComponent colorableComponent = item.GetComponent<IColorableComponent>();
            if (color == -1)
                itemComponent.GenColor();
            else if (colorableComponent != null) colorableComponent.SetColor(color); 
            itemComponent.field = field;
            itemComponent.needFall = falling;
            if(!falling)
            {
                item.transform.position = (Vector3) ((Vector2) transform.position) + Vector3.back * 0.2f;

            }
            if (LevelManager.THIS.gameStatus != GameState.Playing && LevelManager.THIS.gameStatus != GameState.Tutorial)
                Item = itemComponent;
            else if (!IsHaveFallingItemsAbove())
                Item = itemComponent;
            //		this.item.falling = falling;
            if (falling)
            {
                Vector3 startPos = GetReverseDirection();
                item.transform.position = transform.position + startPos * field.squareHeight + Vector3.back * 0.2f;
                itemComponent.JustCreatedItem = true;

            }
            if (LevelManager.THIS.DebugSettings.DestroyLog) DebugLogKeeper.Log(name + " gen item " + item.name + " pos " + item.transform.position, DebugLogKeeper.LogType.Destroying);

            itemComponent.needFall = falling;
//        if ((!nextSquare?.IsFree() ?? false) || Item == itemComponent)
            itemComponent.StartFallingTo(itemComponent.GenerateWaypoints(this));
//            itemComponent.StartFalling();
            return itemComponent;
        }
        /// <summary>
        /// Generates ingredient
        /// </summary>
        /// <returns></returns>
        private GameObject GetIngredientItem()
        {
            GameObject item = null;
            var restCount = LevelManager.THIS.levelData.GetTargetObject().CountTargetSublevel();
            var fieldCount = LevelManager.THIS.field.GetItems()?.Where(i => i.currentType == ItemsTypes.INGREDIENT)?.Count() ?? 0;
            if (fieldCount < 10)
            {
                if (LevelManager.THIS.levelData.target.prefabs.FirstOrDefault()?.GetComponent<ItemIngredient>() != null && !this.IsBottom())
                {
                    if (restCount - fieldCount > 0)
                    {
                        if (Random.Range(0, LevelManager.THIS.levelData.limit / 3) == 0)
                        {
                            string ingredientColor = field.GetIngredientName();
                            if (ingredientColor != "")
                            {
                                item = ObjectPooler.Instance.GetPooledObject(ingredientColor);
                            }
                        }
                    }
                }
            }

            return item;
        }


        /// <summary>
        /// set square with teleport
        /// </summary>
        public void SetTeleports()
        {
            if (teleportDestinationCoord != Vector2Int.one * -1)
            {
                var sq = field.GetSquare(teleportDestinationCoord);
                teleportDestination = sq;
                sq.teleportOrigin = this;
            }
        }
        /// set square with teleport
        public void CreateTeleports()
        {
            if (teleportDestination != null || teleportOrigin != null)
            {
                teleport = Instantiate(teleportEffect).GetComponent<TeleportDirector>();
                teleport.transform.SetParent(transform);
                teleport.SetTeleport(teleportDestination != null && teleportOrigin == null);
                var pos = new Vector2(0, -0.92f);
                var angle = Vector3.Angle(Vector2.down, direction);
                angle = Mathf.Sign(Vector3.Cross(Vector2.down, direction).z) < 0 ? (360 - angle) % 360 : angle;
                if (teleportOrigin != null) angle += 180;
                pos = Quaternion.Euler(0, 0, angle) * pos;
                teleport.transform.localPosition = pos;
                teleport.transform.rotation = Quaternion.Euler(0, 0, angle);
                CreateArrow(teleportOrigin);
            }
        }
        /// <summary>
        /// set square direction on field creation
        /// </summary>
        public void SetDirection()
        {
            Square nextSq = null;
            if (teleportDestination != null) nextSq = teleportDestination;
            else if (direction == Vector2.down) nextSq = GetNeighborBottom(true);
            else if (direction == Vector2.up) nextSq = GetNeighborTop(true);
            else if (direction == Vector2.left) nextSq = GetNeighborLeft(true);
            else if (direction == Vector2.right) nextSq = GetNeighborRight(true);
//        if (!nextSq?.IsNone() ?? false)
            {
                nextSquare = nextSq;
            }
            CreateTeleports();
        }
    
        /// <summary>
        /// change square type
        /// </summary>
        /// <param name="sqBlock"></param>
        public void SetType(SquareBlocks sqBlock)
        {
            SetType(sqBlock.block, sqBlock.blockLayer, sqBlock.obstacle, sqBlock.obstacleLayer);
        }

        public void SetType(SquareTypes _type, int sqLayer, SquareTypes obstacleType, int obLayer)
        {
            var prefabs = GetBlockPrefab(_type);
            if (type != _type && !IsTypeExist(_type))
            {
                for (var i = 0; i < sqLayer; i++)
                {
                    var prefab = prefabs[i];
                    if (prefab != null && _type != SquareTypes.EmptySquare && _type != SquareTypes.NONE)
                    {
                        var b = Instantiate(prefab);
                        LevelManager.THIS.levelData.SetSquareTarget(b, _type, prefab);
                        b.transform.SetParent(transform);
                        b.GetComponent<Square>().field = field;
                        b.transform.localScale = Vector2.one;
                        b.transform.localPosition = new Vector3(0, 0, -0.01f);
                        b.GetComponent<SpriteRenderer>().sortingOrder = 0 + i + GetComponent<SpriteRenderer>().sortingOrder;
                        if (_type != SquareTypes.JellyBlock)
                            subSquares.Add(b.GetComponent<Square>());
                        else
                            subSquares.Insert(0, b.GetComponent<Square>());
                        type = GetSubSquare().type;
                    }
                }
            }

            if (obstacleType != SquareTypes.NONE)
                CreateObstacle(obstacleType, obLayer);
        }

        /// <summary>
        /// Is type exist among sub-squares
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        private bool IsTypeExist(SquareTypes _type)
        {
            return subSquares.Count(i => i.type == _type) > 0;
        }
        /// <summary>
        /// create obstacle on the square
        /// </summary>
        /// <param name="obstacleType"></param>
        /// <param name="obLayer"></param>
        public void CreateObstacle(SquareTypes obstacleType, int obLayer)
        {
            if (obstacleType == SquareTypes.SpiralBlock)
            {
                GenSpiral();
                return;
            }
            var prefabs = GetBlockPrefab(obstacleType);
            for (var i = 0; i < obLayer; i++)
            {
                var prefab = prefabs[i];
                var b = Instantiate(prefab);
                b.transform.SetParent(transform);
                Square square = b.GetComponent<Square>();
                square.field = field;
                square.mainSquqre = this;
                b.transform.localPosition = new Vector3(0, 0, -0.5f);
                b.transform.localScale = Vector2.one;
                LevelManager.THIS.levelData.SetSquareTarget(b, obstacleType, prefab);
                // if (prefab != null && obstacleType == SquareTypes.ThrivingBlock)
                //     Destroy(prefab.gameObject);
                if (obstacleType != SquareTypes.JellyBlock)
                    subSquares.Add(b.GetComponent<Square>());
                else
                    subSquares.Insert(0, b.GetComponent<Square>());
                type = GetSubSquare().type;
                undestroyable = square.undestroyable;
            }
        }
        /// <summary>
        /// generate spiral item
        /// </summary>
        void GenSpiral()
        {
            LevelManager.OnLevelLoaded += () =>
            {
                if (Item != null) Item.transform.position = transform.position;
            };
            GenItem(false, ItemsTypes.SPIRAL);
        }
        /// <summary>
        /// check is which direction is restricted
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public bool IsDirectionRestricted(Vector2 dir)
        {
            foreach (var restriction in directionRestriction)
            {
                if (restriction == dir) return true;
            }
            return false;
        }
        /// <summary>
        /// Get neighbor methods
        /// </summary>
        /// <param name="considerRestrictions"></param>
        /// <param name="safe"></param>
        /// <returns></returns>
        public Square GetNeighborLeft(bool considerRestrictions = true, bool safe = false)
        {
            if (considerRestrictions && (IsDirectionRestricted(Vector2.left))) return null;
            if (col == 0 && !safe)
                return null;
            var square = field.GetSquare(col - 1, row, safe);
            // if (considerRestrictions && (square?.IsNone() ?? false)) return null;
            return square;
        }

        public Square GetNeighborRight(bool considerRestrictions = true, bool safe = false)
        {
            if (considerRestrictions && (IsDirectionRestricted(Vector2.right))) return null;
            if (col >= field.fieldData.maxCols && !safe)
                return null;
            var square = field.GetSquare(col + 1, row, safe);
            // if (considerRestrictions && (square?.IsNone() ?? false)) return null;
            return square;
        }

        public Square GetNeighborTop(bool considerRestrictions = true, bool safe = false)
        {
            if (considerRestrictions && (IsDirectionRestricted(Vector2.up))) return null;
            if (row == 0 && !safe)
                return null;
            var square = field.GetSquare(col, row - 1, safe);
            // if (considerRestrictions && (square?.IsNone() ?? false)) return null;
            return square;
        }

        public Square GetNeighborBottom(bool considerRestrictions = true, bool safe = false)
        {
            if (considerRestrictions && (IsDirectionRestricted(Vector2.down))) return null;
            if (row >= field.fieldData.maxRows && !safe)
                return null;
            var square = field.GetSquare(col, row + 1, safe);
            // if (considerRestrictions && (square?.IsNone() ?? false)) return null;
            return square;
        }

        /// <summary>
        /// Get next square along with direction
        /// </summary>
        /// <param name="safe"></param>
        /// <returns></returns>
        public Square GetNextSquare(bool safe = false)
        {
            return nextSquare;
        }
        /// <summary>
        /// Get previous square along with direction
        /// </summary>
        /// <param name="safe"></param>
        /// <returns></returns>
        public Square GetPreviousSquare(bool safe = false)
        {
            if (teleportOrigin != null) return teleportOrigin;
            if (GetNeighborBottom(true, safe)?.direction == Vector2.up) return GetNeighborBottom(true, safe);
            if (GetNeighborTop(true, safe)?.direction == Vector2.down) return GetNeighborTop(true, safe);
            if (GetNeighborLeft(true, safe)?.direction == Vector2.right) return GetNeighborLeft(true, safe);
            if (GetNeighborRight(true, safe)?.direction == Vector2.left) return GetNeighborRight(true, safe);

            return null;
        }
        /// <summary>
        /// Get squares sequence before this square
        /// </summary>
        /// <returns></returns>
        public Square[] GetSeqBeforeFromThis()
        {
            var sq = sequence;//field.GetCurrentSequence(this);
            return sq.Where(i => i.orderInSequence > orderInSequence).OrderBy(i => i.orderInSequence).ToArray();
        }
        /// <summary>
        /// Check is there any falling items in this sequence
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool AnyFallingItemsInSeq(int color = -1)
        {
            var sq = sequence;//field.GetCurrentSequence(this);
            if (color > -1)
            {
                var anyFallingItemsInSeq = sq.Any(i => i.Item != null && i.Item?.color == color && i.Item.falling);
                return anyFallingItemsInSeq;
            }

            return sq.Any(i => i.Item?.falling ?? false);
        }
        /// <summary>
        /// Check any falling items around the square
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool AnyFallingItemsAround(int color = -1)
        {
            return GetAllNeghborsCross().Any(i => i.AnyFallingItemsInSeq(color));
        }

        /// <summary>
        /// Get item above the square by flow
        /// </summary>
        /// <returns></returns>
        public Item GetItemAbove()
        {
            if (isEnterPoint) return null;
            var sq = sequence;//field.GetCurrentSequence(this);
            var list = sq.Where(i => i.orderInSequence > orderInSequence).OrderBy(i => i.orderInSequence);
            //		var list = GetSeqBeforeFromThis();
            foreach (var square in list)
            {
                if (square.IsFree() && square.Item)
                    return square.Item;
                if (!square.IsFree())
                    return null;
            }
            if (list.Count() == 0) return GetPreviousSquare().Item;
            return null;
        }
        /// <summary>
        /// Get reverse direction
        /// </summary>
        /// <returns></returns>
        public Vector2 GetReverseDirection()
        {
            Vector3 pos = Vector2.up;
            if (direction == Vector2.down) pos = Vector2.up;
            if (direction == Vector2.up) pos = Vector2.down;
            if (direction == Vector2.left) pos = Vector2.right;
            if (direction == Vector2.right) pos = Vector2.left;
            return pos;
        }

        /// <summary>
        /// Gets the match color around to set right color for Generated item
        /// </summary>
        /// <returns>The count match color around.</returns>
        public int GetMatchColorAround(int col)
        {
            var matches = 0;
            if ((GetNeighborBottom()?.Item?.color ?? -1) == col)
                matches=1;
            if (((GetNeighborBottom()?.GetNeighborBottom())?.Item?.color ?? -1) == col)
                matches++;
            if ((GetNeighborTop()?.Item?.color ?? -1) == col)
                matches=1;
            if (((GetNeighborTop()?.GetNeighborTop())?.Item?.color ?? -1) == col)
                matches++;
            if ((GetNeighborRight()?.Item?.color ?? -1) == col)
                matches=1;
            if (((GetNeighborRight()?.GetNeighborRight())?.Item?.color ?? -1) == col)
                matches++;
            if ((GetNeighborLeft()?.Item?.color ?? -1) == col)
                matches=1;
            if (((GetNeighborLeft()?.GetNeighborLeft())?.Item?.color ?? -1) == col)
                matches++;
            return matches;
        }
        /// <summary>
        /// Match 3 search methods
        /// </summary>
        /// <param name="spr_COLOR"></param>
        /// <param name="countedSquares"></param>
        /// <param name="separating"></param>
        /// <param name="countedSquaresGlobal"></param>
        /// <returns></returns>
        Hashtable FindMoreMatches(int spr_COLOR, Hashtable countedSquares, FindSeparating separating, Hashtable countedSquaresGlobal = null)
        {
            var globalCounter = true;
            if (countedSquaresGlobal == null)
            {
                globalCounter = false;
                countedSquaresGlobal = new Hashtable();
            }

            if (Item == null || Item.destroying || Item.falling)
                return countedSquares;
            //    if (LevelManager.This.countedSquares.ContainsValue(this.item) && globalCounter) return countedSquares;
            if (Item.color == spr_COLOR && !countedSquares.ContainsValue(Item) && Item.currentType != ItemsTypes.INGREDIENT && Item.currentType != ItemsTypes.MULTICOLOR && !Item
                    .falling && Item.Combinable)
            {
                if (LevelManager.THIS.onlyFalling && Item.JustCreatedItem)
                    countedSquares.Add(countedSquares.Count - 1, Item);
                else if (!LevelManager.THIS.onlyFalling)
                    countedSquares.Add(countedSquares.Count - 1, Item);
                else
                    return countedSquares;

                //			if (separating == FindSeparating.VERTICAL)
                {
                    if (GetNeighborTop() != null)
                        countedSquares = GetNeighborTop().FindMoreMatches(spr_COLOR, countedSquares, FindSeparating.VERTICAL);
                    if (GetNeighborBottom() != null)
                        countedSquares = GetNeighborBottom().FindMoreMatches(spr_COLOR, countedSquares, FindSeparating.VERTICAL);
                }
                //			else if (separating == FindSeparating.HORIZONTAL)
                {
                    if (GetNeighborLeft() != null)
                        countedSquares = GetNeighborLeft().FindMoreMatches(spr_COLOR, countedSquares, FindSeparating.HORIZONTAL);
                    if (GetNeighborRight() != null)
                        countedSquares = GetNeighborRight().FindMoreMatches(spr_COLOR, countedSquares, FindSeparating.HORIZONTAL);
                }
            }
            return countedSquares;
        }

        public List<Item> FindMatchesAround(FindSeparating separating = FindSeparating.NONE, int matches = 3, Hashtable countedSquaresGlobal = null)
        {
            var globalCounter = true;
            var newList = new List<Item>();
            if (countedSquaresGlobal == null)
            {
                globalCounter = false;
                countedSquaresGlobal = new Hashtable();
            }
            var countedSquares = new Hashtable();
            countedSquares.Clear();
            if (Item == null)
                return newList;
            if(!Item.Combinable)
                return newList;
            //		if (separating != FindSeparating.HORIZONTAL)
            //		{
            countedSquares = FindMoreMatches(Item.color, countedSquares, FindSeparating.VERTICAL, countedSquaresGlobal);
            //		}

            foreach (DictionaryEntry de in countedSquares)
            {
                field.countedSquares.Add(field.countedSquares.Count - 1, de.Value);
            }

            foreach (DictionaryEntry de in countedSquares)
            {
                field.countedSquares.Add(field.countedSquares.Count - 1, de.Value);
            }

            if (countedSquares.Count < matches)
                countedSquares.Clear();

            foreach (DictionaryEntry de in countedSquares)
            {
                newList.Add((Item)de.Value);
            }
            // print(countedSquares.Count);
            return newList.Distinct().ToList();
        }
        /// <summary>
        /// Check should Item fall down
        /// </summary>
        public void CheckFallOut()
        {
            if (LevelManager.THIS.StopFall) return;
            if (Item != null && CanGoOut())
            {
                var nxtSq = Vector2.Distance(this.transform.position,Item.transform.position)>0.3f ? this: GetNextSquareRecursively(this);//GetNextSquareRecursively();
                if (nxtSq)
                {
                    if (nxtSq.CanGoInto())
                    {
                        if (nxtSq.Item == null)
                        {
                            Item.ReplaceCurrentSquareToFalling(nxtSq);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Get next square with Item 
        /// </summary>
        /// <returns></returns>
        public Square GetNextSquareRecursively(Square originSquare)
        {
            var sq = GetNextSquare();
            if (sq != null)
            {
                if (sq.IsNone())
                    return this;
                if (sq.Item != null || !sq.CanGoInto() || (Vector2.Distance(sq.GetPosition(), GetPosition())>=2 && originSquare.orderInSequence-sq.orderInSequence>1 ))
                    return this;
                sq = sq.GetNextSquareRecursively(originSquare);
            }
            else
                sq = this;
            return sq;
        }
        /// <summary>
        /// true if square disabled in editor
        /// </summary>
        /// <returns></returns>
        public bool IsNone()
        {
            return type == SquareTypes.NONE;
        }
        /// <summary>
        /// true if square available and non Undestroyable
        /// </summary>
        /// <returns></returns>
        public bool IsAvailable()
        {
            return !IsNone() && !IsUndestroyable();
        }
        /// <summary>
        /// true if has destroyable obstacle
        /// </summary>
        /// <returns></returns>
        public bool IsHaveDestroybleObstacle()
        {
            return !IsUndestroyable() && (!GetSubSquare()?.canMoveIn ?? false) ;
        }
        /// <summary>
        /// true if the square has obstacle
        /// </summary>
        /// <returns></returns>
        public bool IsObstacle()
        {
            return (!GetSubSquare().CanGoInto() || !GetSubSquare().CanGoOut() || IsUndestroyable());
        }

        public bool IsUndestroyable()
        {
            return GetSubSquare()?.undestroyable?? false;
        }

        /// <summary>
        /// true if the square available for Item
        /// </summary>
        /// <returns></returns>
        public bool IsFree()
        {
            return (!IsNone() && !IsObstacle());
        }
        /// <summary>
        /// Can item fall out of the square
        /// </summary>
        /// <returns></returns>
        public bool CanGoOut()
        {
            return (GetSubSquare()?.canMoveOut ?? false) && (!IsUndestroyable()) ;
        }
        /// <summary>
        /// can item fall to the square
        /// </summary>
        /// <returns></returns>
        public bool CanGoInto()
        {
            return (GetSubSquare()?.canMoveIn ?? false) && (!IsUndestroyable()) ; //TODO: none square falling through
        }
        /// <summary>
        /// Next move event
        /// </summary>
        void OnNextMove()
        {
            dontDestroyOnThisMove = false;
            LevelManager.OnTurnEnd -= OnNextMove;
        }

        public void SetDontDestroyOnMove()
        {
            dontDestroyOnThisMove = true;
            LevelManager.OnTurnEnd += OnNextMove;
        }
        /// <summary>
        /// destroy block (obstacle or jelly)
        /// </summary>
        public void DestroyBlock()
        {
            if (destroyIteration == 0)
            {
                destroyIteration = LevelManager.THIS.destLoopIterations;
                LeanTween.delayedCall(1, () => { destroyIteration = 0; });
            }
            else return;
            if (IsUndestroyable())
                return;
            if (LevelManager.THIS.DebugSettings.DestroyLog)
                DebugLogKeeper.Log("DestroyBlock " + " " + type + " " + GetInstanceID(), DebugLogKeeper.LogType.Destroying);
            if (GetSubSquare().CanGoInto())
            {
                var sqList = GetAllNeghborsCross();
                foreach (var sq in sqList)
                {
                    if (!sq.GetSubSquare().CanGoInto() || (sq.Item?.currentType == ItemsTypes.SPIRAL && Item?.currentType != ItemsTypes.SPIRAL))
                        sq.DestroyBlock();
                }
            }

            if (Item?.currentType == ItemsTypes.SPIRAL)
            {
                Item.DestroyItem();
            }

            if (subSquares.Count > 0)
            {
                var subSquare = GetSubSquare();
                if (type == SquareTypes.JellyBlock) return;
                if (subSquare.GetComponent<TargetComponent>() == null)
                {
                    SoundBase.Instance.PlayOneShot(SoundBase.Instance.block_destroy);
                    var itemAnim = new GameObject();
                    var animComp = itemAnim.AddComponent<AnimateItems>();
                    animComp.InitAnimation(subSquare.gameObject, new Vector2(transform.position.x, -5), subSquare.transform.localScale, null);
                }
                LevelManager.THIS.levelData.GetTargetObject().CheckSquare(new[] { this });
                LevelManager.THIS.ShowPopupScore(subSquare.score, transform.position, 0);

                subSquares.Remove(subSquare);
                Destroy(subSquare.gameObject);
                subSquare = GetSubSquare();
                if (subSquares.Count > 0)
                    type = subSquare.type;

                if (subSquares.Count == 0)
                    type = SquareTypes.EmptySquare;

            }

            if (IsFree() && Item == null) Item = null;
        }
        /// <summary>
        /// Get sub-square (i.e. layered obstacles)
        /// </summary>
        /// <returns></returns>
        public Square GetSubSquare()
        {
            if (subSquares.Count == 0)
                return this;

            return subSquares?.LastOrDefault();
        }
        /// <summary>
        /// Get group of squares for cloud animation on different direction levels
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="group"></param>
        /// <param name="forPair"></param>
        /// <returns></returns>
        public List<List<Square>> GetGroupsSquare(List<List<Square>> groups, List<Square> group = null, bool forPair = true)
        {
            var list = GetAllNeghborsCross();
            if (forPair)
            {
                list = list.Where(i => i.direction == direction).ToList();
                if (direction.y == 0)
                    list = list.Where(i => i.col == col).ToList();
                else
                    list = list.Where(i => i.row == row).ToList();
            }
            else
            {
                list = list.Where(i => i.direction == direction).ToList();
            }

            if (group == null)
            {
                foreach (var sq in list)
                {
                    group = groups.Find(i => i.Contains(sq));
                }
                if (group == null) { group = new List<Square>(); groups.Add(group); }
            }
            if (!group.Contains(this))
                group.Add(this);
            list.RemoveAll(i => group.Any(x => x.Equals(i)));
            foreach (var sq in list)
            {
                groups = sq.GetGroupsSquare(groups, group);
            }
            return groups;
        }


        // [HideInInspector]
        public List<Square> squaresGroup;

        /// <summary>
        /// Get square position on the field
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPosition()
        {
            return new Vector2(col, row);
        }

        /// <summary>
        /// check square is bottom 
        /// </summary>
        /// <returns></returns>
        public bool IsBottom()
        {
            return row == LevelManager.THIS.field.fieldData.maxRows - 1;
        }
    
        public List<Square> GetVerticalNeghbors()
        {
            var sqList = new List<Square>();
            Square nextSquare = null;
            nextSquare = GetNeighborBottom();
            if (nextSquare != null)
                sqList.Add(nextSquare);
            nextSquare = GetNeighborTop();
            if (nextSquare != null)
                sqList.Add(nextSquare);
            return sqList;
        }

        public List<Square> GetAllNeghborsCross()
        {
            var sqList = new List<Square>();
            Square nextSquare = null;
            nextSquare = GetNeighborBottom();
            if (nextSquare != null && !nextSquare.IsNone())
                sqList.Add(nextSquare);
            nextSquare = GetNeighborTop();
            if (nextSquare != null && !nextSquare.IsNone())
                sqList.Add(nextSquare);
            nextSquare = GetNeighborLeft();
            if (nextSquare != null && !nextSquare.IsNone())
                sqList.Add(nextSquare);
            nextSquare = GetNeighborRight();
            if (nextSquare != null && !nextSquare.IsNone())
                sqList.Add(nextSquare);
            return sqList;
        }
    
        /// <summary>
        /// Have square solid obstacle above, used for diagonally falling items animation
        /// </summary>
        /// <returns></returns>
        public bool IsHaveSolidAbove()
        {
            if (isEnterPoint) return false;
            var seq = sequenceBeforeThisSquare;
            return seq.Any(square => square != this && (square.GetSubSquare().CanGoOut() == false || square.GetSubSquare().CanGoInto() == false ||
                                                        IsUndestroyable() || square.IsNone()));
        }
        /// <summary>
        /// Have square item above
        /// </summary>
        /// <returns></returns>
        public bool IsHaveItemsAbove()
        {
            if (isEnterPoint) return false;
            var seq = sequenceBeforeThisSquare;
            foreach (var square in seq)
            {
                if (square.Item ?? false)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Have square falling item above
        /// </summary>
        /// <returns></returns>
        public bool IsHaveFallingItemsAbove()
        {
            if (isEnterPoint) return false;
            var seq = sequenceBeforeThisSquare;
            foreach (var square in seq)
            {
                if (square.Item && !square.Item.falling && !square.Item.needFall)
                    return true;
            }
            return false;
        }
        /// Have square falling item above
        public bool IsItemAbove()
        {
            if (isEnterPoint) return false;
            var seq = sequenceBeforeThisSquare;
            foreach (var square in seq)
            {
                if (!square.IsFree())
                    return false;
                if (square.Item)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Methods for the editor
        /// </summary>
        /// <param name="sqType"></param>
        /// <returns></returns>
        public static GameObject[] GetBlockPrefab(SquareTypes sqType)
        {
            var list = new List<GameObject>();
            var item1 = Resources.Load("Blocks/" + sqType) as GameObject;
            if (item1?.GetComponent<LayeredBlock>() != null)
                list.AddRange(item1.GetComponent<LayeredBlock>().layers.Select(i => i.gameObject));

            if (list.Count() == 0 && item1 != null) list.Add(item1);
            return list.ToArray();
        }

        public static int GetLayersCount(SquareTypes sqType)
        {
            var layers = 1;
            var item1 = Resources.Load("Blocks/" + sqType) as GameObject;
            layers = item1?.GetComponent<LayeredBlock>()?.layers?.Length ?? 0;
            if (layers == 0) layers = 1;
            return layers;
        }

        public static Texture2D GetSquareTexture(SquareTypes sqType)
        {
            return GetBlockPrefab(sqType)?.First().GetComponent<SpriteRenderer>()?.sprite?.texture;
        }

        public static List<Texture2D> GetSquareTextures(SquareBlocks sqBlock)
        {
            var listBlocks = GetBlockPrefab(sqBlock.block);
            var listObstacles = GetBlockPrefab(sqBlock.obstacle);
            var textureBlocks = listBlocks.Select(i => i.GetComponent<SpriteRenderer>().sprite.texture).Take(sqBlock.blockLayer);
            var textureObstacles = listObstacles.Select(i => i.GetComponent<SpriteRenderer>().sprite.texture).Take(sqBlock.obstacleLayer);
            var resultList = new List<Texture2D>();
            resultList.AddRange(textureBlocks);
            if (sqBlock.obstacle != SquareTypes.NONE)
                resultList.AddRange(textureObstacles);

            return resultList;
        }

        public Sprite GetSprite()
        {
            if (subSquares.Count > 0)
                return subSquares.Last().GetComponent<SpriteRenderer>().sprite;
            return GetComponent<SpriteRenderer>().sprite;
        }

        public SpriteRenderer GetSpriteRenderer()
        {
            if (subSquares.Count > 0)
                return subSquares.Last().GetComponent<SpriteRenderer>();
            return GetComponent<SpriteRenderer>();
        }

        public void SetBorderDirection()
        {
            var square = GetNeighborRight();
            if(IsNone()) return;
            if (direction + (square?.direction ?? direction) == Vector2.zero && (!square?.IsNone() ?? false))
            {
                SetBorderDirection(Vector2.right);
            }
            square = GetNeighborBottom();
            if (direction + (square?.direction ?? direction) == Vector2.zero && (!square?.IsNone() ?? false))
            {
                SetBorderDirection(Vector2.down);
            }

        }

        public List<Vector2> directionRestriction = new List<Vector2>();
        public int orderInSequence; //latest square has 0, enter square has last number
        public List<Square> sequence = new List<Square>();
        private int destroyIteration;
        private bool dontDestroyOnThisMove;
        public Square[] sequenceBeforeThisSquare;
        public GameObject border;
        protected Square mainSquqre;
        private GameObject marmaladeTarget;

        private void SetBorderDirection(Vector2 dir)
        {
            var border = new GameObject();
            var spr = border.AddComponent<SpriteRenderer>();
            spr.sortingOrder = 1;
            spr.sprite = borderSprite;
            border.transform.SetParent(transform);
            border.transform.localScale = Vector3.one;
            if (dir != Vector2.down && dir != Vector2.up) border.transform.rotation = Quaternion.Euler(0, 0, 90);
            border.transform.localPosition = Vector2.zero + dir * (LevelManager.THIS.squareWidth + 0.1f);
            SetSquareRestriction(dir);
        }

        public void SetSquareRestriction(Vector2 dir)
        {
            directionRestriction.Add(dir);
            if (dir == Vector2.right)
                GetNeighborRight(false)?.SetSquareRestriction(Vector2.left);
            if (dir == Vector2.down)
                GetNeighborBottom(false)?.SetSquareRestriction(Vector2.up);
        }

        public FieldBoard GetField()
        {
            return field;
        }

        void OnDrawGizmos()
        {
            if (nextSquare != null)
            {
                if (teleportDestination == null)
                    SetNextArrow();
                else
                    SetNextArrowTeleport();
            }
        }

        private void SetNextArrowTeleport()
        {
            Gizmos.color = Color.green;
            var pivot = nextSquare.transform.position + Vector3.left * .1f;
            Gizmos.DrawLine(transform.position + Vector3.left * .1f, pivot);
            RotateGizmo(pivot, 20);
            RotateGizmo(pivot, -20);
        }

        private void SetNextArrow()
        {
            Gizmos.color = Color.blue;
            var pivot = nextSquare.transform.position;
            Gizmos.DrawLine(transform.position, pivot);
            RotateGizmo(pivot, 20);
            RotateGizmo(pivot, -20);
        }

        private void RotateGizmo(Vector3 pivot, float angle)
        {
            var rightPoint = Vector3.zero;
            var dir = (transform.position - pivot) * 0.2f;
            dir = Quaternion.AngleAxis(angle, Vector3.back) * dir;
            rightPoint = dir + pivot;
            Gizmos.DrawLine(pivot, rightPoint);
        }

        public void SetOutline()
        {
            if (GetNeighborBottom()?.IsNone() ?? true) Instantiate(border, transform.position, Quaternion.Euler(0, 0, 90), transform);
            if (GetNeighborTop()?.IsNone() ?? true) Instantiate(border, transform.position, Quaternion.Euler(0, 0, -90), transform);
            if (GetNeighborLeft()?.IsNone() ?? true) Instantiate(border, transform.position, Quaternion.Euler(0, 0, 0), transform);
            if (GetNeighborRight()?.IsNone() ?? true) Instantiate(border, transform.position, Quaternion.Euler(0, 0, 180), transform);
        }

        public Square DeepCopy()
        {
            var other = (Square)MemberwiseClone();
            other.Item = Item.DeepCopy();
            return other;
        }
        
        public GameObject GetMarmaladeTarget
        {
            get => marmaladeTarget;
            set => marmaladeTarget = value;
        }
        public GameObject GetGameObject => gameObject;
        public Item GetItem => Item;
    }
}