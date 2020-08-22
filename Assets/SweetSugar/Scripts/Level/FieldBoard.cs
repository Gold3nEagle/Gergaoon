using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Effects;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.System;
using SweetSugar.Scripts.System.Combiner;
using SweetSugar.Scripts.System.Pool;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEngine;
using Random = UnityEngine.Random;

 namespace SweetSugar.Scripts.Level
 {
     /// <summary>
     /// Field object
     /// </summary>
     public class FieldBoard : MonoBehaviour
     {
         // public SquareBlocks[] fieldData.levelSquares = new SquareBlocks[81];
         public GameObject squarePrefab;
         public Sprite squareSprite1;
         public GameObject outline1;
         public GameObject outline2;
         public GameObject outline3;

         // public int fieldData.maxRows = 9;
         // public int fieldData.maxCols = 9;
         public float squareWidth = 1.2f;
         public float squareHeight = 1.2f;
         public Vector2 firstSquarePosition;
         public Square[] squaresArray;
         public Transform GameField;
         public Hashtable countedSquares = new Hashtable();
         public FieldData fieldData;
         private GameObject pivot;

         public int enterPoints;

         // Use this for initialization
         private void OnEnable()
         {
             GameField = transform;
         }

         /// <summary>
         /// Initialize field
         /// </summary>
         public void CreateField()
         {
             var chessColor = false;
             //squaresArray = new Square[maxCols * maxRows];

             for (var row = 0; row < fieldData.maxRows; row++)
             {
                 if (fieldData.maxCols % 2 == 0)
                     chessColor = !chessColor;
                 for (var col = 0; col < fieldData.maxCols; col++)
                 {
                     CreateSquare(col, row, chessColor);
                     chessColor = !chessColor;
                 }
             }
             foreach (var i in squaresArray.ToList())
             { 
                 i.SetBorderDirection();
                 i.SetTeleports();  
                 if(!i.IsNone())
                     i.SetOutline();
             }
             foreach (var i in squaresArray.ToList())
                 i.enterSquare = GetEnterPoint(i);
             enterPoints = squaresArray.Count(i => i.isEnterPoint);
             foreach (var i in squaresArray.ToList())
             {
                 i.SetDirection(); 
                 // i.SetMask(); 
             }
             SetOrderInSequence();
             foreach (var i in squaresArray.ToList())
             {
                 i.sequenceBeforeThisSquare = i.GetSeqBeforeFromThis();
                 if (i.sequence.All(x => !x.IsNone() && !x.undestroyable) && i.sequence.Any() && i.sequence.Any(x=>x.isEnterPoint))
                     i.linkedEnterSquare = true;
             }

             DirectionCloudEffect.SetGroupSquares(squaresArray);
             SetPivot();
             SetPosY(0);
             GenerateNewItems(false);

             // CreateTestItems();

             StartCoroutine(DeleteMatches());

             if (LevelManager.THIS.enableMarmalade)
                 CreateMarmalade();
//        new OutlineBorder(fieldData.maxRows, fieldData.maxCols, this);
             // LevelManager.This.gameStatus = GameState.WaitForPopup;

             // SetPos(Vector2.zero);
         }

         /// <summary>
         /// Set order for the squares sequience 
         /// </summary>
         private void SetOrderInSequence()
         {
             var list = GetSquareSequence();
             foreach (var seq in list)
             {
                 var order = 0;
                 foreach (var sq in seq)
                 {
                     sq.orderInSequence = order;
                     sq.sequence = seq;
                     order++;
                 }
             }

         }


         private void CreateTestItems()
         {
             // if (GetSquare(5, 5) == null) return;
             GetSquare(3, 3).Item.colorableComponent.SetColor(0);
             GetSquare(3, 4).Item.colorableComponent.SetColor(0);
             GetSquare(4, 3).Item.colorableComponent.SetColor(0);
             GetSquare(4, 4).Item.colorableComponent.SetColor(0);
         }

         /// <summary>
         /// Creates marmalades on the field
         /// </summary>
         private void CreateMarmalade()
         {
             var items = GetItems(true);
             items = items.Where(i => !i.tutorialItem).ToList();
             items[Random.Range(0, items.Count())].SetType(ItemsTypes.MARMALADE);
             var itemsFiltered = items.Where(i => i.currentType != ItemsTypes.MARMALADE && !i.tutorialItem).ToArray();
             itemsFiltered[Random.Range(0, itemsFiltered.Count())].SetType(ItemsTypes.MARMALADE);

         }

         private void SetPosY(int y)
         {
             transform.position = new Vector2(transform.position.x, transform.position.y + (y - GetPosition().y));
         }

         public void SetPosition(Vector2 pos)
         {
             transform.position = new Vector2(transform.position.x + (pos.x - GetPosition().x), transform.position.y + (pos.y - GetPosition().y));
         }

         private void SetPivot()
         {
             // transform.position = GetCenter();
             pivot = new GameObject();
             pivot.name = "Pivot";
             pivot.transform.SetParent(transform);
             pivot.transform.position = GetCenter();
             // foreach (var square in squaresArray)
             // {
             //     square.transform.SetParent(transform);
             //     if (square.item != null)
             //         square.item.transform.SetParent(transform);
             // }
         }

         private Vector2 GetCenter()
         {
             var minX = squaresArray.Min(x => x.transform.position.x);
             var minY = squaresArray.Min(x => x.transform.position.y);
             var maxX = squaresArray.Max(x => x.transform.position.x);
             var maxY = squaresArray.Max(x => x.transform.position.y);
             var pivotPosMin = new Vector2(minX, minY);
             var pivotPosMax = new Vector2(maxX, maxY);
             var pivotPos = pivotPosMin + (pivotPosMax - pivotPosMin) * 0.5f;
             return pivotPos;
         }

         public Vector2 GetPosition()
         {
             return pivot?.transform.position ?? Vector2.zero;
         }

         private IEnumerator DeleteMatches()
         {
             // yield return new WaitForSeconds(0.5f);
             //        var combs = GetMatches();
             List<Combine> combs = new List<Combine>();
             List<Combine> allFoundCombines = new List<Combine>();
             List<Combine> bonusCombines = new List<Combine>();
             do
             {
                 combs = LevelManager.THIS.CombineManager.GetCombines(this, out allFoundCombines);
                 ChangeFoundCombines(combs);
                 ChangeFoundCombines(allFoundCombines);
                 combs = LevelManager.THIS.CombineManager.GetCombines(this, out allFoundCombines);
                 bonusCombines.Clear();
                 bonusCombines.Add( AI.THIS.GetMarmaladeCombines());
                 bonusCombines = bonusCombines.WhereNotNull().ToList();
                 ChangeFoundCombines(bonusCombines);
                 yield return new WaitForEndOfFrame();
                    
//            yield return new WaitForEndOfFrame();

             } while (combs.Count > 0 || allFoundCombines.SelectMany(i=>i.items).Any() || bonusCombines.Any());
             GetItems().ForEach(i => i.Hide(false));
             yield return new WaitWhileFall(false);
             LevelManager.THIS.gameStatus = GameState.WaitForPopup;
         }

         public void ChangeFoundCombines(List<Combine> allFoundCombines)
         {
             ChangeFoundCombines(allFoundCombines.Select(i=>i.items).ToList());
         }
         public void ChangeFoundCombines(List<List<Item>> allFoundCombines)
         {
             foreach (var comb in allFoundCombines)
             {
                 if(comb==null) continue;
                 var colorOffset = 0;
                 foreach (var item in comb)
                 {
                     if (item.tutorialItem) continue;
                     item.GetComponent<IColorableComponent>().RandomizeColor();
                     colorOffset++;
                 }
             }
         }

         public void GenerateNewItems(bool falling = true)
         {
             var prepareLevel = !falling;
             var squares = squaresArray;
             foreach (var square in squares)
             {
                 if (square.IsNone() || !square.CanGoInto()) continue;
                 if (!square.IsHaveSolidAbove() || prepareLevel)
                 {
                     if (square.Item == null) //|| !falling && square.item?.currentType != ItemsTypes.SPIRAL)
                     {
                         var squareBlock = fieldData.levelSquares[square.row * fieldData.maxCols + square.col];
                         if (!falling && (squareBlock.item.Texture != null || squareBlock.item.ItemType != ItemsTypes.NONE))
                         {
                             var item = square.GenItem(false, squareBlock.item.ItemType, squareBlock.item.Color);
                             if (square.IsNone()) return;
                             item.tutorialItem = true; // not destroy this on regen
                             item.itemForEditor = squareBlock.item;
                             square.Item = item;
                         }
                         if (square.Item == null) 
                             GenSimpleItem(falling, square);
                     }
                 }
             }
         }
    
         public void RegenItems(bool falling = true)
         {
             var squares = squaresArray;
             foreach (var square in squares)
             {
                 if (square.IsNone() || !square.CanGoInto()) continue;
                 if (!square.IsHaveSolidAbove())
                 {
                     if (square.Item != null && square.Item.currentType == ItemsTypes.NONE) //|| !falling && square.item?.currentType != ItemsTypes.SPIRAL)
                     {
                         square.Item.colorableComponent.RandomizeColor();
                     }
                 }
             }
         }

         private static void GenSimpleItem(bool falling, Square square)
         {
             square.GenItem(falling);
         }
    
         public string GetIngredientName()
         {
             var targetPrefabs = LevelManager.THIS.levelData.subTargetsContainers;
             var IngredientsCountOnField = GetItems().Where(i => i.currentType == ItemsTypes.INGREDIENT).GroupBy(i => i.GetComponent<PoolBehaviour>().name).Select(i =>
                 new {name = i.Key, Count = i.Count()});

             for (var index = 0; index < targetPrefabs.Count; index++)
             {
                 var targetPrefab = targetPrefabs[index];
                 var firstOrDefault = IngredientsCountOnField.FirstOrDefault(i => i.name == targetPrefab.targetPrefab.name);
                 int targetCount = LevelManager.THIS.levelData.targetObject.GetCount(targetPrefab.extraObject.name);
                 if (firstOrDefault ==null && targetCount > 0 || firstOrDefault != null && targetCount > firstOrDefault.Count)
                 {
                     return targetPrefab.targetPrefab.name;
                 }
             }

             return "";
         }

         public List<Item.Waypoint> GetWaypoints(Square startSquare, Square destSquare, List<Square> list = null)
         {
             if (destSquare.Equals(startSquare) || startSquare == null)
                 return list.Select(i => new Item.Waypoint(i.transform.position, i)).ToList();
             var nextSquare = startSquare.GetNextSquare();
             list = list ?? new List<Square>();
             if (!list.Any()) list.Add(startSquare);
             if (nextSquare?.IsFree() ?? false)
             {
                 list.Add(nextSquare);
                 GetWaypoints(nextSquare, destSquare, list);
             }

             return list.Select(i => new Item.Waypoint(i.transform.position, i)).ToList();

         }

         public Square GetEnterPoint(Square square)
         {
             var enterSquare = square.GetPreviousSquare();
             if (enterSquare == null) return square;
             if (!enterSquare.isEnterPoint)
                 enterSquare = GetEnterPoint(enterSquare);
             if (enterSquare.isEnterPoint && enterSquare.IsNone())
                 enterSquare = GetEnterPoint(enterSquare);
             return enterSquare;
         }


         /// <summary>
         /// Get squares sequence from first to end
         /// </summary>
         /// <returns></returns>
         public List<List<Square>> GetSquareSequence()
         {
             var enterSquares = squaresArray.Where(i => i.isEnterPoint);
             var listofSequences = ListofSequences(enterSquares);
             var l = listofSequences.SelectMany(i => i);
             var squaresNotJoinEnter = squaresArray.Where(i => !l.Contains(i));
             var topSquares = new List<Square>();
             foreach (Square square in squaresNotJoinEnter)
             {
                 var sq = square;
                 Square prevSquare;
                 do
                 {
                     prevSquare = sq.GetPreviousSquare();
                     if (prevSquare != null && prevSquare.IsNone()) prevSquare = null;
                     if(prevSquare == null) topSquares.Add(sq);
                     sq = prevSquare;
                 } while (prevSquare != null);
             }
        
             listofSequences.AddRange( ListofSequences(topSquares));
             return listofSequences;
         }

         /// <summary>
         /// Get list of all squares sequences
         /// </summary>
         /// <param name="enterSquares"></param>
         /// <returns></returns>
         private List<List<Square>> ListofSequences(IEnumerable<Square> enterSquares)
         {
             var listofSequences = new List<List<Square>>();
             foreach (var enterSquare in enterSquares)
             {
                 var sequence = new List<Square>();
                 sequence.Add(enterSquare);
                 sequence = GetSquareSequenceStep(sequence);
                 sequence.Reverse();
                 listofSequences.Add(sequence);
             }

             return listofSequences;
         }

         /// <summary>
         /// Get square sequece
         /// </summary>
         /// <param name="sequence"></param>
         /// <returns></returns>
         private List<Square> GetSquareSequenceStep(List<Square> sequence)
         {
             var nextSquare = sequence.LastOrDefault().GetNextSquare();
             if (nextSquare != null && !nextSquare.IsNone())
             {
                 sequence.Add(nextSquare);
                 sequence = GetSquareSequenceStep(sequence);
             }
             return sequence;
         }

         /// <summary>
         /// Get sequence of the square
         /// </summary>
         /// <param name="square"></param>
         /// <returns></returns>
         public List<Square> GetCurrentSequence(Square square)
         {
             return GetSquareSequence().Where(i => i.Any(x => x == square)).SelectMany(i => i).ToList();
         }

         /// <summary>
         /// Create a square
         /// </summary>
         /// <param name="col">column</param>
         /// <param name="row">row</param>
         /// <param name="chessColor">color switch</param>
         private void CreateSquare(int col, int row, bool chessColor = false)
         {
             GameObject squareObject = null;
             var squareBlock = fieldData.levelSquares[row * fieldData.maxCols + col];
             squareObject = Instantiate(squarePrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity);
             if (chessColor)
             {
                 squareObject.GetComponent<SpriteRenderer>().sprite = squareSprite1;
             }
             squareObject.transform.SetParent(GameField);//set parent later
             squareObject.transform.localPosition = firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight);
             var square = squareObject.GetComponent<Square>();
             squaresArray[row * fieldData.maxCols + col] = square;
             square.field = this;
             square.row = row;
             square.col = col;
             square.type = SquareTypes.EmptySquare;
             square.direction = squareBlock.direction;
             if (square.teleportOrigin == null)
                 square.isEnterPoint = squareBlock.enterSquare;
             square.teleportDestinationCoord = squareBlock.teleportCoordinatesLinked;
             // square.AddComponent(Type.GetType(LevelData.target.ToString()));
             if (squareBlock.block == SquareTypes.EmptySquare)
             {
                 square.SetType(squareBlock);
             }
             else if (squareBlock.block == SquareTypes.NONE)
             {
                 squareObject.GetComponent<SpriteRenderer>().enabled = false;
                 square.type = SquareTypes.NONE;

             }
             else
             {
                 square.SetType(squareBlock);

             }


         }

         /// <summary>
         /// Get bottom row
         /// </summary>
         /// <returns>returns list of squares</returns>
         public List<Square> GetBottomRow()
         {
             var itemsList = GetSquareSequence().Select(i => i.FirstOrDefault()).Where(i => i.type != SquareTypes.NONE).ToList();
             return itemsList;
         }

         /// <summary>
         /// Get field rect
         /// </summary>
         /// <returns>rect</returns>
         public Rect GetFieldRect()
         {
             var square = GetSquare(0, 0);
             var squareRightBottom = GetSquare(fieldData.maxCols - 1, fieldData.maxRows - 1);
             return new Rect(square.transform.position.x, square.transform.position.y, squareRightBottom.transform.position.x - square.transform.position.x, square.transform.position.y - squareRightBottom.transform.position.y);
         }

         /// <summary>
         /// Get squares from a rect
         /// </summary>
         /// <param name="rect">rect</param>
         public List<Square> GetFieldSeqment(RectInt rect)
         {
             List<Square> squares = new List<Square>();
             for (int row = rect.yMin; row <= rect.yMax; row++)
             {
                 for (int col = rect.xMin; col <= rect.xMax; col++)
                 {
                     squares.Add(GetSquare(col, row));
                 }
             }
             return squares;
         }

         /// <summary>
         /// Get top row
         /// </summary>
         /// <returns>list of squares</returns>
         public List<Square> GetTopRow()
         {
             return squaresArray.Where(i => !i.IsNone()).GroupBy(i => i.col).Select(i => new { Sq = i.OrderBy(x => x.row).First() }).Select(i => i.Sq).ToList();
         }

         public List<Square> GetSimpleItemsInRow(int count)
         {

             var list = squaresArray
                 .Where(i => i.GetSubSquare().IsFree())
                 .Where(y => y.GetVerticalNeghbors().Count(z => z.IsFree()) == 2)
                 .Select(x => new { Index = x.row, Value = x })
                 .GroupBy(i => i.Index)
                 .Select(i => i.Select(x => x.Value).Take(count).ToList())
                 .ToArray();

             var v1 = list.GetValue(Random.Range(0, list.Length)) as List<Square>;
             return v1;
         }

         public List<Square> GetSquares(bool withUndestroyble = false)
         {
             var list = new List<Square>();
             foreach (var item in squaresArray)
             {
                 if (withUndestroyble && item.GetSubSquare().IsObstacle() && item.GetSubSquare().undestroyable)
                     list.Add(item);
                 else
                     list.Add(item);

             }
             return list;
         }

         public Square GetSquare(Vector2 vector)
         {
             return GetSquare(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
         }

         public Square GetSquare(int col, int row, bool safe = false)
         {
             if (!safe)
             {
                 if (row >= fieldData.maxRows || col >= fieldData.maxCols || row < 0 || col < 0)
                     return null;
                 return squaresArray[row * fieldData.maxCols + col];
             }

             row = Mathf.Clamp(row, 0, fieldData.maxRows - 1);
             col = Mathf.Clamp(col, 0, fieldData.maxCols - 1);
             return squaresArray[row * fieldData.maxCols + col];
         }

         /// <summary>
         /// Destroy items around the square
         /// </summary>
         /// <param name="square"></param>
         /// <param name="item1"></param>
         public void DestroyItemsAround(Square square, Item item1)
         {
             var itemsList = LevelManager.THIS.GetItemsAround9(square);

             foreach (var item in itemsList)
             {
                 if (item != null)
                 {
                     if (item.Combinable || item.Explodable)
                         item.DestroyItem(true, true, item1);
                 }
             }
             LevelManager.THIS.GetSquaresAroundSquare(square).ForEach(i => i.DestroyBlock());

         }

         /// <summary>
         /// Check if any items should be destroy
         /// </summary>
         /// <returns></returns>
         public bool DestroyingItemsExist()
         {
             return GetDestroyingItems()?.Length > 0;
         }

         /// <summary>
         /// Get destroying items
         /// </summary>
         /// <returns></returns>
         public ILongDestroyable[] GetDestroyingItems()
         {
             var longDestroyable = FindObjectsOfType<MonoBehaviour>().OfType<ILongDestroyable>().Where(i => !i.IsAnimationFinished() && i.CanBeStarted()).OrderBy(i => i.GetPriority()).ToArray();
             return longDestroyable;
         }

         public List<List<Item>> GetMatches(FindSeparating separating = FindSeparating.NONE, int matches = 3)
         {
             var newCombines = new List<List<Item>>();
             countedSquares = new Hashtable();
             countedSquares.Clear();
             for (var col = 0; col < fieldData.maxCols; col++)
             {
                 for (var row = 0; row < fieldData.maxRows; row++)
                 {
                     if (GetSquare(col, row) != null)
                     {
                         if (!countedSquares.ContainsValue(GetSquare(col, row).Item))
                         {
                             var newCombine = GetSquare(col, row).FindMatchesAround(separating, matches, countedSquares);
                             if (newCombine.Count >= matches)
                                 newCombines.Add(newCombine);
                         }
                     }
                 }
             }
             return newCombines;
         }

         /// <summary>
         /// Get random items for win animation and boosts
         /// </summary>
         /// <param name="count">count of items</param>
         /// <returns>list of items</returns>
         public List<Item> GetRandomItems(int count)
         {
             var list = GetItems(true);

             var list2 = new List<Item>();
             while (list2.Count < Mathf.Clamp(count, 0, GetItems(true).Count()))
             {
                 // try
                 // {
                 if (!list.Any()) return list;
                 var newItem = list[Random.Range(0, list.Count)];
                 if (list2.IndexOf(newItem) < 0)
                 {
                     list2.Add(newItem);
                 }
                 // }
                 // catch (Exception ex)
                 // {
                 //     gameStatus = GameState.Win;//TODO: check win conditions
                 // }
             }
             return list2;
         }

         /// <summary>
         /// Get items by parameters
         /// </summary>
         /// <param name="onlySimple">only simple items</param>
         /// <param name="exceptItems">except items</param>
         /// <param name="nonDestroying">only items currently shouldn't be destroy</param>
         /// <returns></returns>
         public List<Item> GetItems(bool onlySimple = false, Item[] exceptItems = null, bool nonDestroying = true)
         {
             if (exceptItems == null) exceptItems = new Item[] { };

             var items = squaresArray.Where(i => i?.Item != null).Select(i => i.Item).Except(exceptItems);
             if (onlySimple)
                 items = items?.Where(i => i.currentType == ItemsTypes.NONE && i.NextType == ItemsTypes.NONE);
             if (nonDestroying)
                 items = items?.Where(i => !i.destroying);
             return items.ToList();
         }

         /// <summary>
         /// Get new created items
         /// </summary>
         /// <returns></returns>
         public List<Item> GetJustCreatedItems()
         {
             return FindObjectsOfType<Item>().Where(i => i.JustCreatedItem && i.needFall && !i.falling && !i.GetItemInterfaces()[0].IsStaticOnStart()).ToList();
         }

         /// <summary>
         /// Get items from bottom order
         /// </summary>
         /// <returns></returns>
         public List<Item> GetItemsFromBottomOrder()
         {
             // return GetItems(false, null, false).OrderByDescending(i => i.square.row).ToList();
             var list = GetSquareSequence();
             var items = new List<Item>();
             foreach (var seq in list)
             {
                 var order = 0;
                 foreach (var sq in seq)
                 {
                     if (sq.Item != null)
                     {
                         sq.Item.orderInSequence = order;
                         items.Add(sq.Item);
                     }
                     order++;
                 }
             }

             var excludedItems = GetItems().Except(items);
             foreach (var item in excludedItems)
             {
                 items.Add(item);
             }
             return items.ToList();
         }

         /// <summary>
         /// Get target objects to check destination count
         /// </summary>
         /// <returns></returns>
         public TargetComponent[] GetTargetObjects()
         {
             var list = FindObjectsOfType(typeof(TargetComponent)) as TargetComponent[];
             list = list.Where(i => i.GetComponent<IField>().GetField() == this).Select(i => i.GetComponent<TargetComponent>()).ToArray();
             return list;
         }

         /// <summary>
         /// Get all bonus items like striped, package
         /// </summary>
         /// <returns></returns>
         public List<Item> GetAllExtaItems()
         {
             var list = new List<Item>();
             foreach (var square in squaresArray)
             {
                 if (square.Item != null && square.Item.currentType != ItemsTypes.NONE && square.Item.Combinable)
                 {
                     list.Add(square.Item);
                 }
             }

             return list;
         }

         /// <summary>
         /// Get squares of particular type
         /// </summary>
         /// <param name="type"></param>
         /// <returns></returns>
         public int CountSquaresByType(string type)
         {
             var squareType = (SquareTypes)Enum.Parse(typeof(SquareTypes), type.Replace("SweetSugar.Scripts.TargetScripts.",""));

             return /*squaresArray.Count(item => item.type == squareType) + */squaresArray.WhereNotNull().SelectMany(i => i.subSquares).Count(item => item.type == squareType);
         }

         /// <summary>
         /// Get squares without items
         /// </summary>
         /// <returns></returns>
         public Square[] GetEmptySquares()
         {
             return squaresArray.Where(i => i.IsFree() && i.Item == null && !i.IsHaveSolidAbove() && i.linkedEnterSquare && (i.enterSquare.isEnterPoint)).ToArray();
         }

         /// <summary>
         /// Get top square in a column
         /// </summary>
         /// <param name="col">column</param>
         /// <returns></returns>
         public Square GetTopSquareInCol(int col)
         {
             return squaresArray.Where(i => i.IsFree() && i.col == col).OrderBy(i => i.row).FirstOrDefault();
         }

         /// <summary>
         /// Get items by color
         /// </summary>
         /// <param name="color"></param>
         /// <returns></returns>
         public Item[] GetItemsByColor(int color)
         {
             //Debug.Log(GetItems()?.Count);
             //Debug.Log(GetItems()?.Where(i => i.color == color).Count());
             return GetItems()?.Where(i => i.color == color).ToArray();
         }


         /// <summary>
         /// Get items without a neighbour
         /// </summary>
         /// <returns></returns>
         public Item[] GetLonelyItemsOrCage()
         {
             return squaresArray.Where(i => i.Item != null).Where(i => !i.GetAllNeghborsCross().Any() || !i.CanGoOut()).Select(i => i.Item).ToArray();
         }

         public FieldBoard DeepCopy()
         {
             var other = (FieldBoard)MemberwiseClone();
             other.squaresArray = new Square[fieldData.maxCols * fieldData.maxRows];
             for (var i = 0; i < squaresArray.Count(); i++)
             {
                 var square = squaresArray[i];
                 other.squaresArray[i] = square.DeepCopy();
             }
             return other;
         }

     }
 }

