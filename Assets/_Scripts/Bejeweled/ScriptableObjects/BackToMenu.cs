﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Analytics;

public class BackToMenu : MonoBehaviour
{ 
    private GameData gameData; 
    public LevelLoader levelLoader;
    public AdsScript adScript;
    public TMP_Text candyScoreText; 
    int currentLevel;
    private int adsNum;
    private bool adsEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = PlayerPrefs.GetInt("currentLevel");
        gameData = FindObjectOfType<GameData>(); 
        adsNum = PlayerPrefs.GetInt("IAPAds");
        if(adsNum == 1)
        {
            adsEnabled = true;
        }

    }

    public void WinOK()
    {
        int currentLevel = PlayerPrefs.GetInt("currentLevel");
        AnalyticsResult analyticsResult = AnalyticsEvent.LevelComplete(currentLevel);
        Debug.Log("Analytics Result: " + analyticsResult);

        //Enable the next level
        if (gameData != null)
        { 
            gameData.saveData.isActive[currentLevel] = true;
            gameData.Save(); 
        }
         
            StartCoroutine(GoToMenu(1)); 


    }

    public void LoseOK()
    { 
        int totalLives = PlayerPrefs.GetInt("totalLives");
        AnalyticsResult analyticsResult = AnalyticsEvent.LevelFail(currentLevel);
        Debug.Log("Analytics Result: " + analyticsResult);


        if (totalLives > 1)
        {
            totalLives--;
        } else if(totalLives <= 1)
        {
            totalLives = 0;
        } 

        PlayerPrefs.SetInt("totalLives", totalLives);

        if (adsEnabled == false)
        {
            if (currentLevel % 3 == 0)
            { 
                adScript.ShowInterstitialAd();
                StartCoroutine(GoToMenu(3));
            } else
            {
                StartCoroutine(GoToMenu(1));
            }
        } else
        { 
            StartCoroutine(GoToMenu(1));
        }
    }

    IEnumerator GoToMenu(int waitTime)
    {
        GameObject musicGameObject = GameObject.Find("Music");
        Destroy(musicGameObject);


        yield return new WaitForSeconds(waitTime); 
        levelLoader.LoadLevel(1);

    }  

    public void SetStars(int level, int stars)
    {
        gameData.saveData.stars[level - 1 ] = stars;

        int latestLevel = gameData.GetLatestUnlockedLevel();
        if (currentLevel <= latestLevel && currentLevel >= latestLevel - 2)
        {
            //Giving Candy to the player
            int totalCandy = PlayerPrefs.GetInt("totalCandy");

            if(stars == 3)
            {
                stars = 4;
            }

            totalCandy += (50 * stars);
            PlayerPrefs.SetInt("DoubleReward", (stars * 50));
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            SetCandyScoreText(50 * stars);
        }

    }

    public void SetCandyScoreText(int score)
    {
        candyScoreText.text = "+ " + score.ToString();
    }

}
