using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Overworld : MonoBehaviour
{

    public Text totalCandyDisplay, totalLivesDisplay;
    public GameObject pointerAnim, guidePanel;
    public GPlayServices gPlay;
    GameData gameData;
    Camera mainCamera;


    private void Awake()
    {
        gameData = FindObjectOfType<GameData>();
         
        if (SceneManager.GetActiveScene().name == "OverWorld")
        {
            if (!PlayerPrefs.HasKey("firstMatch3"))
            {
                PlayerPrefs.SetInt("firstMatch3", 1);
                PlayerPrefs.SetInt("totalLives", 5);
                PlayerPrefs.SetInt("ExtraMoves", 1);
                PlayerPrefs.SetInt("FreeMove", 1);
                PlayerPrefs.SetInt("ExplodeArea", 1);
                PlayerPrefs.SetInt("Bomb", 1);
                gameData.CheckLevels();

                PlayerPrefs.SetInt("BeenRated", 0);
            } 
        }
        gameData.CheckLevels(); 

        DisplayTotalCandy(); 
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "OverWorld")
        {
            CheckUI();
            mainCamera = Camera.main;
            int latestLevel = gameData.GetLatestUnlockedLevel();


            if (latestLevel >= 10)
                gPlay.UnlockAchievement(6);

            if (latestLevel >= 100)
                gPlay.UnlockAchievement(7);

            //Precaution so camera is not out of bounds.
            if (latestLevel > 150)
                latestLevel -= 3;

            //Don't jump to next map when the last level of a map is unlocked.
            if (latestLevel == 18 || latestLevel == 131)
                latestLevel--;

            GameObject neededPosition = GameObject.Find("Level Prefab " + "(" + latestLevel.ToString() + ")");
            mainCamera.transform.position = new Vector3(neededPosition.transform.position.x, neededPosition.transform.position.y, -10);
        }
    }


    public void DisplayTotalCandy()
    {
        int totalCandy = PlayerPrefs.GetInt("totalCandy");
        totalCandyDisplay.text = totalCandy.ToString();
    }

    public int GetTotalCandy()
    {
        int totalCandy = PlayerPrefs.GetInt("totalCandy");

        return totalCandy;
    }
     
    public void ShowPointer()
    { 
        pointerAnim.SetActive(true);
        StartCoroutine(PointerOff());
    }

    IEnumerator PointerOff()
    {
        yield return new WaitForSeconds(3f);
        pointerAnim.SetActive(false);
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.G))
        {
            Debug.Log("Cheating eh?");
            int totalCandy = PlayerPrefs.GetInt("totalCandy");
            totalCandy += 10000;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            DisplayTotalCandy();

            RegenerateLives lifeRegan = FindObjectOfType<RegenerateLives>();
            lifeRegan.AddLife(10); 

        }
    }

    void CheckUI()
    {
        if (!PlayerPrefs.HasKey("firstMatch3"))
        {
            PlayerPrefs.SetInt("firstMatch3", 1);
        }
        int timesPlayed = PlayerPrefs.GetInt("firstMatch3");

        if(timesPlayed <= 2)
        {
            guidePanel.SetActive(true);
            PlayerPrefs.SetInt("firstMatch3", 4);
        }
    }

}
