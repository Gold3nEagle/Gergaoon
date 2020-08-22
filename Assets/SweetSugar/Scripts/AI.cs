using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.System;
using SweetSugar.Scripts.System.Combiner;
using SweetSugar.Scripts.System.Pool;
using UnityEngine;

namespace SweetSugar.Scripts
{
    public enum CombineType
    {
        LShape,
        VShape
    }

    /// <summary>
    /// Searches tips and automatic player for debugger
    /// </summary>
    public class AI : MonoBehaviour
    {
        /// <summary>
        /// The reference to this object
        /// </summary>
        public static AI THIS;
        private DebugSettings _debugSettings;

        /// <summary>
        /// have got a tip
        /// </summary>
        public bool gotTip;
        /// <summary>
        /// The allow show tip
        /// </summary>
        public bool allowShowTip;
        /// <summary>
        /// The tip identifier
        /// </summary>
        int tipID;
        /// <summary>
        /// The count of coroutines
        /// </summary>
        public int corCount;
        /// <summary>
        /// The tip items
        /// </summary>
        private List<Item> currentPreCombine = new List<Item>();
        // Use this for initialization

        private void Awake()
        {
            THIS = this;
            _debugSettings = Resources.Load("Scriptable/DebugSettings") as DebugSettings;
            InitSprites();
        }

        private void InitSprites()
        {
            if (ObjectPooler.Instance != null) itemSprites = ObjectPooler.Instance.GetPooledObject("Item", false).GetComponent<IColorableComponent>();
        }

        public Vector3 vDirection;
        public CombineType currentCombineType;
        private Item tipItem;
        private bool changeTipAI;
        private IColorableComponent itemSprites;
        private int maxRow;
        private int maxCol;

        public Item TipItem
        {
            get
            {
                return tipItem;
            }

            set
            {
                tipItem = value;
            }
        }

        /// <summary>
        /// Gets the square. Return square by row and column
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The column.</param>
        /// <param name="currentSquare"></param>
        /// <returns></returns>
        Square GetSquare(int row, int col, Square currentSquare)
        {
            var v1 = currentSquare.GetPosition() - new Vector2(col, row);
            if (v1.magnitude > 1) currentSquare = LevelManager.THIS.field.GetSquare(currentSquare.GetPosition() + new Vector2(v1.x, v1.y * -1).normalized);
            if (currentSquare != null && currentSquare.IsDirectionRestricted(v1.normalized)) return null;
            return LevelManager.THIS.GetSquare(col, row);
        }

        /// <summary>
        /// Checks the square. Is the color of item of this square is equal to desired color. If so we add the item to nextMoveItems array.
        /// </summary>
        /// <param name="square">The square.</param>
        /// <param name="COLOR">The color.</param>
        /// <param name="moveThis">is the item should be movable?</param>
        bool CheckSquare(Square square, int COLOR, bool moveThis = false)
        {
            if (square == null)
                return false;
            if(currentPreCombine==null) currentPreCombine= new List<Item>();
            if (square.Item != null)
            {
                if (CheckColorCondition(square, COLOR))
                {
                    if (moveThis && square.GetSubSquare().CanGoOut())
                    {
                        currentPreCombine.Add(square.Item);
                        return true;
                    }
                    else if (!moveThis)
                    {
                        currentPreCombine.Add(square.Item);
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool CheckColorCondition(Square square, int COLOR)
        {
            if(LevelManager.THIS.gameStatus != GameState.Tutorial)
                return square.Item.color == COLOR && square.Item.Combinable;
            else
                return square.Item.color == COLOR && square.Item.Combinable && square.Item.tutorialItem;

        }

        public List<Item> GetCombine()
        {
            return currentPreCombine;
        }



        bool IsVerticalCombine()
        {
            return (currentPreCombine[0].square.row != currentPreCombine[1].square.row && currentPreCombine[0].square.row != currentPreCombine[2].square.row);
        }

        /// <summary>
        /// Loop of searching possible combines
        /// </summary>
        /// <returns></returns>
        public IEnumerator CheckPossibleCombines()
        {
            if(!itemSprites)InitSprites();
            //waiting for 1 second just in case to be sure that field was built
            yield return new WaitForSeconds(1);

            //allow to show tips
            allowShowTip = true;

            //get max positions of squares
            maxRow = LevelManager.THIS.levelData.maxRows;
            maxCol = LevelManager.THIS.levelData.maxCols;

            //variable to check: are we got tip or not
            gotTip = false;

            //break, if the main scripts have not ready yet
            while (LevelManager.THIS == null)
            {
                yield return new WaitForEndOfFrame();
            }
            //if game is not in Playing status - wait
            while (LevelManager.THIS.gameStatus != GameState.Playing && LevelManager.THIS.gameStatus != GameState.Tutorial)
            {
                yield return new WaitForEndOfFrame();
            }

            //if drag have not blocked and game status Playing - continue
            if (!LevelManager.THIS.DragBlocked && (LevelManager.THIS.gameStatus == GameState.Playing || LevelManager.THIS.gameStatus == GameState.Tutorial))
            {
                currentPreCombine = new List<Item>();

                if (LevelManager.THIS.gameStatus != GameState.Playing && LevelManager.THIS.gameStatus != GameState.Tutorial)
                    yield break;


                //Iteration for search possible combination 
                if (itemSprites != null)
                    for (var COLOR = 0; COLOR < itemSprites.GetSprites(LevelManager.THIS.currentLevel).Length; COLOR++)
                    {
                        if (changeTipAI && Random.Range(0, 2) == 1) continue;
                        for (var col = 0; col < LevelManager.THIS.levelData.maxCols; col++)
                        {
                            for (var row = 0; row < LevelManager.THIS.levelData.maxRows; row++)
                            {
                                var square = LevelManager.THIS.GetSquare(col, row);
                                if (square?.GetSubSquare().CanGoOut() == false || square.Item == null)
                                    continue;
                                //current square called x
                                //o-o-x
                                //	  o
                                vDirection = Vector3.zero;
                                currentCombineType = CombineType.LShape;
                                if (col > 1 && row < maxRow - 1)
                                {
                                    CheckSquare(GetSquare(row + 1, col, square), COLOR, true);
                                    CheckSquare(GetSquare(row, col - 1, square), COLOR);
                                    CheckSquare(GetSquare(row, col - 2, square), COLOR);
                                }
                                
                                if (currentPreCombine.Count == 3 && GetSquare(row, col, square).CanGoInto())
                                {
                                    // StartCoroutine(showTip(nextMoveItems[0], Vector3.up));
                                    showTip(currentPreCombine);
                                    TipItem = currentPreCombine[0];
                                    vDirection = Vector3.up;
                                    yield break;
                                }
                                
                                currentPreCombine.Clear();
                                
                                //    o
                                //o-o x
                                if (col > 1 && row > 0)
                                {
                                    CheckSquare(GetSquare(row - 1, col, square), COLOR, true);
                                    CheckSquare(GetSquare(row, col - 1, square), COLOR);
                                    CheckSquare(GetSquare(row, col - 2, square), COLOR);
                                }
                                
                                if (currentPreCombine.Count == 3 && GetSquare(row, col, square).CanGoInto())
                                {
                                    // StartCoroutine(showTip(nextMoveItems[0], Vector3.down));
                                    vDirection = Vector3.down;
                                    TipItem = currentPreCombine[0];
                                    showTip(currentPreCombine);
                                    yield break;
                                }
                                
                                currentPreCombine.Clear();
                                
                                //x o o
                                //o
                                if (col < maxCol - 2 && row < maxRow - 1)
                                {
                                    CheckSquare(GetSquare(row + 1, col, square), COLOR, true);
                                    CheckSquare(GetSquare(row, col + 1, square), COLOR);
                                    CheckSquare(GetSquare(row, col + 2, square), COLOR);
                                }
                                
                                if (currentPreCombine.Count == 3 && GetSquare(row, col, square).CanGoInto())
                                {
                                    // StartCoroutine(showTip(nextMoveItems[0], Vector3.up));
                                    vDirection = Vector3.up;
                                    TipItem = currentPreCombine[0];
                                    showTip(currentPreCombine);
                                    yield break;
                                }
                                
                                currentPreCombine.Clear();
                                
                                //o
                                //x o o
                                if (col < maxCol - 2 && row > 0)
                                {
                                    CheckSquare(GetSquare(row - 1, col, square), COLOR, true);
                                    CheckSquare(GetSquare(row, col + 1, square), COLOR);
                                    CheckSquare(GetSquare(row, col + 2, square), COLOR);
                                }
                                
                                if (currentPreCombine.Count == 3 && GetSquare(row, col, square).CanGoInto())
                                {
                                    //  StartCoroutine(showTip(nextMoveItems[0], Vector3.down));
                                    vDirection = Vector3.down;
                                    TipItem = currentPreCombine[0];
                                    showTip(currentPreCombine);
                                    yield break;
                                }
                                
                                currentPreCombine.Clear();
                                
                                //o
                                //o
                                //x o
                                if (col < maxCol - 1 && row > 1)
                                {
                                    CheckSquare(GetSquare(row, col + 1, square), COLOR, true);
                                    CheckSquare(GetSquare(row - 1, col, square), COLOR);
                                    CheckSquare(GetSquare(row - 2, col, square), COLOR);
                                }
                                
                                if (currentPreCombine.Count == 3 && GetSquare(row, col, square).CanGoInto())
                                {
                                    // StartCoroutine(showTip(nextMoveItems[0], Vector3.left));
                                    vDirection = Vector3.left;
                                    TipItem = currentPreCombine[0];
                                    showTip(currentPreCombine);
                                    yield break;
                                }
                                
                                currentPreCombine.Clear();
                                
                                //x o
                                //o
                                //o
                                if (col < maxCol - 1 && row < maxRow - 2)
                                {
                                    CheckSquare(GetSquare(row, col + 1, square), COLOR, true);
                                    CheckSquare(GetSquare(row + 1, col, square), COLOR);
                                    CheckSquare(GetSquare(row + 2, col, square), COLOR);
                                }
                                
                                if (currentPreCombine.Count == 3 && GetSquare(row, col, square).CanGoInto())
                                {
                                    //  StartCoroutine(showTip(nextMoveItems[0], Vector3.left));
                                    vDirection = Vector3.left;
                                    TipItem = currentPreCombine[0];
                                    showTip(currentPreCombine);
                                    yield break;
                                }
                                
                                currentPreCombine.Clear();
                                
                                //	o
                                //  o
                                //o x
                                if (col > 0 && row > 1)
                                {
                                    CheckSquare(GetSquare(row, col - 1, square), COLOR, true);
                                    CheckSquare(GetSquare(row - 1, col, square), COLOR);
                                    CheckSquare(GetSquare(row - 2, col, square), COLOR);
                                }
                                
                                if (currentPreCombine.Count == 3 && GetSquare(row, col, square).CanGoInto())
                                {
                                    //  StartCoroutine(showTip(nextMoveItems[0], Vector3.right));
                                    vDirection = Vector3.right;
                                    TipItem = currentPreCombine[0];
                                    showTip(currentPreCombine);
                                    yield break;
                                }
                                
                                currentPreCombine.Clear();
                                
                                //o x
                                //  o
                                //  o
                                if (col > 0 && row < maxRow - 2)
                                {
                                    CheckSquare(GetSquare(row, col - 1, square), COLOR, true);
                                    CheckSquare(GetSquare(row + 1, col, square), COLOR);
                                    CheckSquare(GetSquare(row + 2, col, square), COLOR);
                                }
                                
                                if (currentPreCombine.Count == 3 && GetSquare(row, col, square).CanGoInto())
                                {
                                    //  StartCoroutine(showTip(nextMoveItems[0], Vector3.right));
                                    vDirection = Vector3.right;
                                    TipItem = currentPreCombine[0];
                                    showTip(currentPreCombine);
                                    yield break;
                                }
                                
                                currentPreCombine.Clear();
                                
                                //o-x-o-o
                                if (col < maxCol - 2 && col > 0)
                                {
                                    CheckSquare(GetSquare(row, col - 1, square), COLOR, true);
                                    CheckSquare(GetSquare(row, col + 1, square), COLOR);
                                    CheckSquare(GetSquare(row, col + 2, square), COLOR);
                                }
                                
                                if (currentPreCombine.Count == 3 && GetSquare(row, col, square).CanGoInto())
                                {
                                    //   StartCoroutine(showTip(nextMoveItems[0], Vector3.right));
                                    vDirection = Vector3.right;
                                    TipItem = currentPreCombine[0];
                                    showTip(currentPreCombine);
                                    yield break;
                                }
                                
                                currentPreCombine.Clear();
                                //o-o-x-o
                                if (col < maxCol - 1 && col > 1)
                                {
                                    CheckSquare(GetSquare(row, col + 1, square), COLOR, true);
                                    CheckSquare(GetSquare(row, col - 1, square), COLOR);
                                    CheckSquare(GetSquare(row, col - 2, square), COLOR);
                                }
                                
                                if (currentPreCombine.Count == 3 && GetSquare(row, col, square).CanGoInto())
                                {
                                    //   StartCoroutine(showTip(nextMoveItems[0], Vector3.left));
                                    vDirection = Vector3.left;
                                    TipItem = currentPreCombine[0];
                                    showTip(currentPreCombine);
                                    yield break;
                                }
                                
                                currentPreCombine.Clear();
                                //o
                                //x
                                //o
                                //o
                                if (row < maxRow - 2 && row > 0)
                                {
                                    CheckSquare(GetSquare(row - 1, col, square), COLOR, true);
                                    CheckSquare(GetSquare(row + 1, col, square), COLOR);
                                    CheckSquare(GetSquare(row + 2, col, square), COLOR);
                                }
                                
                                if (currentPreCombine.Count == 3 && GetSquare(row, col, square).CanGoInto())
                                {
                                    //  StartCoroutine(showTip(nextMoveItems[0], Vector3.down));
                                    vDirection = Vector3.down;
                                    TipItem = currentPreCombine[0];
                                    showTip(currentPreCombine);
                                    yield break;
                                }
                                
                                currentPreCombine.Clear();
                                
                                //o
                                //o
                                //x
                                //o
                                if (row < maxRow - 2 && row > 1)
                                {
                                    CheckSquare(GetSquare(row + 1, col, square), COLOR, true);
                                    CheckSquare(GetSquare(row - 1, col, square), COLOR);
                                    CheckSquare(GetSquare(row - 2, col, square), COLOR);
                                }
                                
                                if (currentPreCombine.Count == 3 && GetSquare(row, col, square).CanGoInto())
                                {
                                    //   StartCoroutine(showTip(nextMoveItems[0], Vector3.up));
                                    vDirection = Vector3.up;
                                    TipItem = currentPreCombine[0];
                                    showTip(currentPreCombine);
                                    yield break;
                                }
                                currentPreCombine.Clear();

                                //Get marmalade tip
                                int[] array;
                                array = new[]
                                {
                                    1, 1, 0,
                                    1, 0, 0,
                                    0, 2, 0
                                };
                                if (CheckMatrix(square, array, COLOR, 7)) yield break;
                                currentPreCombine.Clear();
                                array = new[]
                                {
                                    1, 1, 0,
                                    1, 0, 2,
                                    0, 0, 0
                                };
                                if (CheckMatrix(square, array, COLOR, 5)) yield break;
                                currentPreCombine.Clear();
                                array = new[]
                                {
                                    0, 1, 1,
                                    0, 0, 1,
                                    0, 2, 0
                                };
                                if (CheckMatrix(square, array, COLOR, 7)) yield break;
                                currentPreCombine.Clear();
                                array = new[]
                                {
                                    0, 1, 1,
                                    2, 0, 1,
                                    0, 0, 0
                                };
                                if (CheckMatrix(square, array, COLOR, 3)) yield break;
                                currentPreCombine.Clear();
                                array = new[]
                                {
                                    0, 0, 0,
                                    2, 0, 1,
                                    0, 1, 1
                                };
                                if (CheckMatrix(square, array, COLOR, 3)) yield break;
                                currentPreCombine.Clear();
                                array = new[]
                                {
                                    0, 2, 0,
                                    0, 0, 1,
                                    0, 1, 1
                                };
                                if (CheckMatrix(square, array, COLOR, 1)) yield break;
                                currentPreCombine.Clear();
                                array = new[]
                                {
                                    0, 2, 0,
                                    1, 0, 0,
                                    1, 1, 0
                                };
                                if (CheckMatrix(square, array, COLOR, 1)) yield break;
                                currentPreCombine.Clear();
                                array = new[]
                                {
                                    0, 0, 0,
                                    1, 0, 2,
                                    1, 1, 0
                                };
                                if (CheckMatrix(square, array, COLOR, 5)) yield break;
                                currentPreCombine.Clear();
                                //  o
                                //o x o
                                //  o
                                var h = 0;
                                var v = 0;
                                v = GetCrossCombine(row, col, square, COLOR, v, ref h);
                                
                                //if we found 3or more items and they not lock show tip
                                if (currentPreCombine.Count == 3 && square.CanGoInto() && square.GetSubSquare().CanGoOut())
                                {
                                    if (v > h && currentPreCombine[2].square.GetSubSquare().CanGoOut())
                                    {
                                        //StartCoroutine(showTip(nextMoveItems[2], new Vector3(Random.Range(-1f, 1f), 0, 0)));
                                        TipItem = currentPreCombine[2];
                                        if (TipItem.transform.position.x > currentPreCombine[0].transform.position.x)
                                            vDirection = Vector3.left;
                                        else
                                            vDirection = Vector3.right;
                                        showTip(currentPreCombine);
                                        yield break;
                                    }
                                
                                    if (v < h && currentPreCombine[0].square.GetSubSquare().CanGoOut())
                                    {
                                        // StartCoroutine(showTip(nextMoveItems[0], new Vector3(0, Random.Range(-1f, 1f), 0)));
                                        TipItem = currentPreCombine[0];
                                        var k = currentPreCombine.GroupBy(i => i.transform.position.y).OrderBy(i => i.Key).Select(i => i.Count()).ToArray();
                                        if (k[0]>k[1])
                                            vDirection = Vector3.down;
                                        else
                                            vDirection = Vector3.up;
                                
                                        showTip(currentPreCombine);
                                        yield break;
                                    }
                                
                                    currentPreCombine.Clear();
                                }
                                else if (square.item && square.item.currentType != ItemsTypes.NONE && square.item.currentType!=ItemsTypes.TimeBomb && square.CanGoInto() && square.GetSubSquare()
                                .CanGoOut() &&
                                         square.Item.Combinable &&
                                         square.Item.CombinableWithBonus && currentPreCombine.Count < 3)
                                {
                                    currentPreCombine.Clear();
                                    List<Square> allNeghborsCross = square.GetAllNeghborsCross();
                                    var neibSquare = allNeghborsCross.FirstOrDefault(i => i.Item && i.Item.currentType != ItemsTypes.NONE);
                                    if (neibSquare && neibSquare.Item && neibSquare.Item.Combinable && neibSquare.CanGoOut())
                                    {
                                        TipItem = square.Item;
                                        vDirection = (neibSquare.Item.transform.position - square.Item.transform.position).normalized;
                                        currentPreCombine.Add(square.Item);
                                        currentPreCombine.Add(neibSquare.Item);
                                        showTip(currentPreCombine);
                                        yield break;
                                    }
                                
                                    currentPreCombine.Clear();
                                }
                                else
                                    currentPreCombine.Clear();
                            }
                        }
                    }

                if (!gotTip)
                {
                    var mult = LevelManager.THIS.field.GetItems().FirstOrDefault(i => i.currentType == ItemsTypes.MULTICOLOR);
                    if (mult && mult.square.CanGoOut())
                    {
                        var item = mult.square.GetAllNeghborsCross().FirstOrDefault(i => i.item != null);
                        if(item && item.CanGoOut())
                        {
                            currentPreCombine.Clear();
                            currentPreCombine.Add(item.item);
                            currentPreCombine.Add(mult);
                            showTip(currentPreCombine);
                        }
                    }
                }

                //if we don't get any tip.  call nomatches to regenerate level
                if (!LevelManager.THIS.DragBlocked && !LevelManager.THIS.field.GetItems().Any(i => i.falling || i.destroying))
                {
                    if (!gotTip)
                        LevelManager.THIS.NoMatches();
                }

                //        }
                yield return new WaitForEndOfFrame();
                //find possible combination again 
                if (!LevelManager.THIS.DragBlocked)
                    StartCoroutine(CheckPossibleCombines());
                else
                {
                    yield return new WaitForSeconds(2);
                    StartCoroutine(CheckPossibleCombines());
                }

            }
        }

        private bool CheckMatrix(Square square, int[] array, int COLOR, int findIndex)
        {
            if(square==null) return false;
            
            vDirection = GetDirection(4) - GetDirection(findIndex);
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == 0) continue;
                var arraySq = square.GetPosition() + GetDirection(i);
                if (vDirection.x > 0 && arraySq.x < 0 ) return false; 
                if (vDirection.x < 0 && arraySq.x > maxCol-1 ) return false; 
                if (vDirection.y > 0 && arraySq.y < 0 ) return false; 
                if (vDirection.y < 0 && arraySq.y > maxRow-1 ) return false; 
            }

            int c = 0;
            var moveSq = GetRelativeSquare(square, GetDirection(findIndex));
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == 2 && CheckSquare(moveSq, COLOR, true)) c++;
                else if (array[i] == 1 && CheckSquare(GetRelativeSquare(square, GetDirection(i)), COLOR)) c++;
            }

            if (c == 4 && moveSq.CanGoInto())
            {
                showTip(currentPreCombine);
                TipItem = currentPreCombine[0];
                return true;
            }

            return false;
        }

        private int GetCrossCombine(int row, int col, Square square, int COLOR, int v, ref int h)
        {
            currentCombineType = CombineType.VShape;

            if (row < maxRow - 1)
            {
                var square1 = GetSquare(row + 1, col, square);
                if (square1)
                {
                    //1.6
                    if (square1.Item != null)
                    {
                        if (CheckColorCondition(square1, COLOR))
                        {
                            vDirection = Vector3.up;
                            currentPreCombine.Add(square1.Item);
                            v++;
                        }
                    }
                }
            }

            if (row > 0)
            {
                var square1 = GetSquare(row - 1, col, square);
                if (square1)
                {
                    //1.6
                    if (square1.Item != null)
                    {
                        if (CheckColorCondition(square1, COLOR))
                        {
                            vDirection = Vector3.down;
                            currentPreCombine.Add(square1.Item);
                            v++;
                        }
                    }
                }
            }

            if (col > 0)
            {
                var square1 = GetSquare(row, col - 1, square);
                if (square1)
                {
                    //1.6
                    if (square1.Item != null)
                    {
                        if (CheckColorCondition(square1, COLOR))
                        {
                            vDirection = Vector3.right;
                            currentPreCombine.Add(square1.Item);
                            h++;
                        }
                    }
                }
            }

            if (col < maxCol - 1)
            {
                var square1 = GetSquare(row, col + 1, square);
                if (square1)
                {
                    //1.6
                    if (square1.Item != null)
                    {
                        if (CheckColorCondition(square1, COLOR))
                        {
                            vDirection = Vector3.left;
                            currentPreCombine.Add(square1.Item);
                            h++;
                        }
                    }
                }
            }

            return v;
        }

        //show tip function calls coroutine for
        void showTip(List<Item> nextMoveItems)
        {
            StopCoroutine(showTipCor(nextMoveItems));
            StartCoroutine(showTipCor(nextMoveItems));
        }

        //show tip coroutine
        IEnumerator showTipCor(List<Item> nextMoveItems)
        {
            changeTipAI = false;
            gotTip = true;
            corCount++;

            if (corCount > 1)
            {
                corCount--;
                yield break;
            }
            if (LevelManager.THIS.DragBlocked && !allowShowTip)
            {
                corCount--;
                yield break;
            }
            tipID = LevelManager.THIS.moveID;
            //while (!LevelManager.This.DragBlocked && allowShowTip)
            //{
            yield return new WaitForSeconds(1);
            if (LevelManager.THIS.DragBlocked && !allowShowTip && tipID != LevelManager.THIS.moveID)
            {
                corCount--;
                yield break;
            }
            foreach (var item in nextMoveItems)
            {
                if (item == null)
                {
                    corCount--;
                    yield break;
                }

            }
            //call animation trigger for every found item to show tip
            foreach (var item in nextMoveItems)
            {
                if (item != null)
                {
                    item.anim.SetBool("package_idle", false);
                    item.anim.SetTrigger("tip");
                }
            }
            yield return new WaitForSeconds(0);
            corCount--;
            StartCoroutine(CheckPossibleCombines());
#if UNITY_EDITOR
            if (LevelManager.THIS?.gameStatus == GameState.Playing && (_debugSettings?.AI ?? false)&& currentPreCombine?.Count() > 0)
            {
                if (TipItem && !TipItem.destroying && TipItem.gameObject.activeSelf && !LevelManager.THIS.findMatchesStarted) TipItem.SwitchDirection(-THIS.vDirection);
                yield return new WaitForSeconds(1);
                if (THIS.allowShowTip) changeTipAI = true;
                else changeTipAI = false;
            }
#endif
        
            // }
        }
        private Vector2 GetDirection(int num)
        {
            if (num == 0) return new Vector2(-1,-1); 
            if (num == 1) return new Vector2(0,-1); 
            if (num == 2) return new Vector2(1,-1); 
            if (num == 3) return new Vector2(-1,0); 
            if (num == 4) return new Vector2(0,0); 
            if (num == 5) return new Vector2(1,0); 
            if (num == 6) return new Vector2(-1,1); 
            if (num == 7) return new Vector2(0,1); 
            if (num == 8) return new Vector2(1,1); 
            return Vector2.zero;
        }
        
        private Square GetRelativeSquare(Square sq, Vector2 vector2)
        {
            return sq.field.GetSquare(sq.GetPosition() + vector2 );
        }
        
        public Combine GetMarmaladeCombines()
        {
            for (var COLOR = 0; COLOR < 6; COLOR++)
            {
                for (var col = 0; col < LevelManager.THIS.levelData.maxCols; col++)
                {
                    for (var row = 0; row < LevelManager.THIS.levelData.maxRows; row++)
                    {
                        var square = LevelManager.THIS.GetSquare(col, row);
                        //Get marmalade tip
                        int[] array;
                        array = new[]
                        {
                            1, 1, 0,
                            1, 1, 0,
                            0, 0, 0
                        };
                        if (CheckMatrix(square, array, COLOR, 0))
                        {
                            var l = new Item[currentPreCombine.Count];
                            for (var index = 0; index < currentPreCombine.Count; index++)
                            {
                                var item = currentPreCombine[index];
                                l[index]=item;
                            }

                            return new Combine {items = l.ToList()};
                        }
                        currentPreCombine.Clear();
                    }
                }
            }
            return null;
        }
    }
}