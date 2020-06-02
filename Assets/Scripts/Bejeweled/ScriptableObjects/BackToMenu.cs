using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class BackToMenu : MonoBehaviour
{ 
    private GameData gameData;
    private Board board;
    public LevelLoader levelLoader;
    public AdsScript adScript;

    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        board = FindObjectOfType<Board>();
    }

    public void WinOK()
    {
        if(gameData != null)
        {
            gameData.saveData.isActive[board.level + 1] = true;
            gameData.Save();
        }
        StartCoroutine(GoToMenu());
    }

    public void LoseOK()
    {
        int totalLives = PlayerPrefs.GetInt("totalLives");
        totalLives--;
        PlayerPrefs.SetInt("totalLives", totalLives);
        StartCoroutine(GoToMenu());
         
    }

    IEnumerator GoToMenu()
    {
        if (board.level == 4 || board.level == 8 || board.level == 12)
        {
            adScript.ShowInterstitialAd();
        }
        yield return new WaitForSeconds(2f); 
        levelLoader.LoadLevel(2);

    } 
}
