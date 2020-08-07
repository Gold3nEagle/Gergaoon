using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Level")]
public class Level : ScriptableObject
{
    [Header("Board dimensions")]
    public int width, height;

    [Header("Starting Tiles")]
    public TileType[] boardLayout;
    public int breakableHitPoints;

    [Header("Available Candy")]
    public GameObject[] dots;

    [Header("Score Goals")]
    public int[] scoreGoals;

    [Header("End Game Requirements")]
    public EndGameRequirements endGameRequirements;
    public BlankGoal[] levelGoals;
    
}
