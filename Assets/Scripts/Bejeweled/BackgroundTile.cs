using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;
    private SpriteRenderer spriteRenderer;
    private GoalManager goalManager;

    private void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage; 
        if(hitPoints <= 0)
        {
            goalManager.CompareGoal(gameObject.tag);
            goalManager.UpdateGoals();
            Destroy(gameObject);
        }
        MakeLighter();
    }

    void MakeLighter()
    {
        Color color = spriteRenderer.color;
        //Get alpha value and divide in half
        float newAlpha = color.a * .5f;
        spriteRenderer.color = new Color(color.r, color.g, color.b, newAlpha);

    }

}
