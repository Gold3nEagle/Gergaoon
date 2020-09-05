using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{

    [Header("Active Stuff")]
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;
    private Image buttonImage;
    private Button myButton;
    private int starsActive;

    [Header("Level UI")]
    public Image[] stars;
    public Text levelText;
    public int level;
    public GameObject confirmPanel, underDevPanel;
    int totalLives;

    Overworld overWorld;
    private GameData gameData;

    // Use this for initialization
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        overWorld = FindObjectOfType<Overworld>();
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
        LoadData();
        ActivateStars();
        ShowLevel();
        DecideSprite();
        totalLives = PlayerPrefs.GetInt("totalLives");
    }

    void LoadData()
    {
        //Is GameData present?
        if (gameData != null)
        {
            //Decide if the level is active
            if (gameData.saveData.isActive[level - 1])
            {
                isActive = true;
            }
            else
            {
                isActive = false;
            }
            //Decide how many stars to activate
            starsActive = gameData.saveData.stars[level - 1];
        }
    }

    void ActivateStars()
    {
        for (int i = 0; i < starsActive; i++)
        { 
            stars[i].enabled = true;
        }
    }

    void DecideSprite()
    {
        if (isActive)
        {
            buttonImage.sprite = activeSprite;
            myButton.enabled = true;
            levelText.enabled = true;
        }
        else
        {
            buttonImage.sprite = lockedSprite;
            myButton.enabled = false;
            levelText.enabled = false;
        }
    }

    void ShowLevel()
    {
        levelText.text = "" + level;
    } 

    public void ConfirmPanel(int level)
    { 
        totalLives = PlayerPrefs.GetInt("totalLives");
        if (level < 131)
        {
            if (totalLives <= 0)
            {
                overWorld.ShowPointer(); 
            }
            else
            {
                LoadChosenLevel(level);
            }
        } else
        { 
            underDevPanel.SetActive(true);
        }
    }

    public void CancelUnderDev()
    {
        underDevPanel.SetActive(false);  
    }


    void LoadChosenLevel(int chosenLevel)
    {

        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
        PlayerPrefs.SetInt("currentLevel", chosenLevel);

        GameObject mainMusicGO = GameObject.FindGameObjectWithTag("Music");
        Destroy(mainMusicGO);

        levelLoader.LoadLevel(3);
    }

}
