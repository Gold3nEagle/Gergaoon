using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class BackToMenu : MonoBehaviour
{ 
    private GameData gameData;
    private Board board;
    public LevelLoader levelLoader;
    public AdsScript adScript;
    private int adsNum;
    private bool adsEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        board = FindObjectOfType<Board>();
        adsNum = PlayerPrefs.GetInt("IAPAds");
        if(adsNum == 1)
        {
            adsEnabled = true;
        }

    }

    public void WinOK()
    { 
        if (gameData != null)
        {
            gameData.saveData.isActive[board.level + 1] = true;
            gameData.Save();
        }

        if (adsEnabled == false)
        {
            if (board.level % 3 == 0)
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

    public void LoseOK()
    { 
        int totalLives = PlayerPrefs.GetInt("totalLives");
 
        if(totalLives > 1)
        {
            totalLives--;
        } else if(totalLives <= 1)
        {
            totalLives = 0;
        } 

        PlayerPrefs.SetInt("totalLives", totalLives);

        if (adsEnabled == false)
        {
            if (board.level % 3 == 0)
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
        yield return new WaitForSeconds(waitTime); 
        levelLoader.LoadLevel(2);

    }  
}
