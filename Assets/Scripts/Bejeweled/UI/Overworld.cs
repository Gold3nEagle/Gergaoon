using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Overworld : MonoBehaviour
{

    public Text totalCandyDisplay, totalLivesDisplay;
    public GameObject pointerAnim;

    // Start is called before the first frame update
    void Start()
    { 

        DisplayTotalCandy();
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (!PlayerPrefs.HasKey("firstMatch3"))
            {
                PlayerPrefs.SetInt("firstMatch3", 1);
                PlayerPrefs.SetInt("totalLives", 3);
            }
            DisplayTotalLives();
        }
    }

    private void OnEnable()
    {
        DisplayTotalCandy();
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (!PlayerPrefs.HasKey("firstMatch3"))
            {
                PlayerPrefs.SetInt("firstMatch3", 1);
                PlayerPrefs.SetInt("totalLives", 3);
            }
            DisplayTotalLives();
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

    public void DisplayTotalLives()
    {
        int totalLives = PlayerPrefs.GetInt("totalLives");
        totalLivesDisplay.text = totalLives.ToString();
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

}
