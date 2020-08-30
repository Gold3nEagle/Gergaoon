using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Overworld : MonoBehaviour
{

    public Text totalCandyDisplay, totalLivesDisplay;
    public GameObject pointerAnim;
    GameData gameData;
    Camera mainCamera;


    private void Awake()
    {
        gameData = FindObjectOfType<GameData>();
         
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (!PlayerPrefs.HasKey("firstMatch3"))
            {
                PlayerPrefs.SetInt("firstMatch3", 1);
                PlayerPrefs.SetInt("totalLives", 3);
                PlayerPrefs.SetInt("ExtraMoves", 1);
                PlayerPrefs.SetInt("FreeMove", 1);
                PlayerPrefs.SetInt("ExplodeArea", 1);
                PlayerPrefs.SetInt("Bomb", 1);
                gameData.CheckLevels();

                PlayerPrefs.SetInt("BeenRated", 0);
            } 
        }
        gameData.CheckLevels();

        //PlayerPrefs.SetInt("totalCandy", 100000);
        //PlayerPrefs.SetInt("totalLives", 3);

        DisplayTotalCandy(); 
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            mainCamera = Camera.main;
            int latestLevel = gameData.GetLatestUnlockedLevel();

            if(latestLevel >= 17)
                //Unlock Basket Game and show celebration!

            if (latestLevel > 90)
                latestLevel -= 2;
            

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


}
