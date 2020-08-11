using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSetup : MonoBehaviour
{

    public Board board;

    public void SetUp()
    {
        GenerateBlankSpaces();
        GenerateBreakableTiles();
        GenerateLockTiles();
        GenerateConcreteTiles();
        GenerateSlimeTiles();
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (!board.blankSpaces[i, j]) // && !concreteTiles[i,j] && !slimeTiles[i,j])
                {
                    Vector2 tempPosition = new Vector2(i, j + board.offSet);
                    Vector2 tilePosition = new Vector2(i, j);
                    GameObject backgroundTile = Instantiate(board.tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "( " + i + ", " + j + " )";


                    if (!board.blankSpaces[i, j] && !board.concreteTiles[i, j] && !board.slimeTiles[i, j])
                    {
                        //Initializaing
                        int dotToUse = UnityEngine.Random.Range(0, board.dots.Length);
                        int maxIterations = 0;

                        while (board.MatchesAt(i, j, board.dots[dotToUse]) && maxIterations < 100)
                        {
                            dotToUse = UnityEngine.Random.Range(0, board.dots.Length);
                            maxIterations++;
                        }
                        maxIterations = 0;

                        GameObject dot = Instantiate(board.dots[dotToUse], tempPosition, Quaternion.identity);
                        dot.GetComponent<Dot>().row = j;
                        dot.GetComponent<Dot>().column = i;
                        dot.transform.parent = this.transform;
                        dot.name = "( " + i + ", " + j + " )";
                        board.allDots[i, j] = dot;
                    }
                }
            }
        }
    }

    public void GenerateBlankSpaces()
    {
        for (int i = 0; i < board.boardLayout.Length; i++)
        {
            if (board.boardLayout[i].tileKind == TileKind.Blank)
            {
                board.blankSpaces[board.boardLayout[i].x, board.boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakableTiles()
    {
        //Check all tiles in the layout
        for (int i = 0; i < board.boardLayout.Length; i++)
        {
            //if a tile is a jelly tile
            if (board.boardLayout[i].tileKind == TileKind.Breakable)
            {
                //create a jelly tile at that position
                Vector2 tempPosition = new Vector2(board.boardLayout[i].x, board.boardLayout[i].y);
                GameObject tile = Instantiate(board.breakableTilePrefab, tempPosition, Quaternion.identity);
                tile.GetComponent<BackgroundTile>().hitPoints = board.world.levels[board.level].breakableHitPoints;
                board.breakableTiles[board.boardLayout[i].x, board.boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }

        }
    }

    private void GenerateLockTiles()
    {
        //Check all tiles in the layout
        for (int i = 0; i < board.boardLayout.Length; i++)
        {
            //if a tile is a lock tile
            if (board.boardLayout[i].tileKind == TileKind.Lock)
            {
                //create a lock tile at that position
                Vector2 tempPosition = new Vector2(board.boardLayout[i].x, board.boardLayout[i].y);
                GameObject tile = Instantiate(board.lockTilePrefab, tempPosition, Quaternion.identity);
                board.lockTiles[board.boardLayout[i].x, board.boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }

        }
    }

    private void GenerateConcreteTiles()
    {
        //Check all tiles in the layout
        for (int i = 0; i < board.boardLayout.Length; i++)
        {
            //if a tile is a jelly tile
            if (board.boardLayout[i].tileKind == TileKind.Concrete)
            {
                //create a lock tile at that position
                Vector2 tempPosition = new Vector2(board.boardLayout[i].x, board.boardLayout[i].y);
                GameObject tile = Instantiate(board.concreteTilePrefab, tempPosition, Quaternion.identity);
                board.concreteTiles[board.boardLayout[i].x, board.boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }

        }
    }

    private void GenerateSlimeTiles()
    {
        //Look at all the tiles in the layout
        for (int i = 0; i < board.boardLayout.Length; i++)
        {
            //if a tile is a "Slime" tile
            if (board.boardLayout[i].tileKind == TileKind.Slime)
            {
                //Create a "Slime" tile at that position;
                Vector2 tempPosition = new Vector2(board.boardLayout[i].x, board.boardLayout[i].y);
                GameObject tile = Instantiate(board.slimePiecePrefab, tempPosition, Quaternion.identity);
                board.slimeTiles[board.boardLayout[i].x, board.boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }


}
